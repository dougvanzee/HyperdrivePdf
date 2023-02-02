using Hyperdrive.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iText.Kernel.Pdf;
using Hyperdrive.Core.Stats.PageSizeCount;
using Hyperdrive.Core.Stats;
using System.Threading;
using iText.Kernel.Geom;
//using System.Windows.Forms;

namespace Hyperdrive.UI.Views.SideToolbars
{
    /// <summary>
    /// Interaction logic for PageCountToolbar.xaml
    /// </summary>
    public partial class PageColorCountToolbar : UserControl, INotifyPropertyChanged
    {
        private BackgroundWorker worker;

        public Window parentWindow = Application.Current.MainWindow;

        ItemsControl itemsControl;

        public PageColorCountToolbar()
        {
            InitializeComponent();

            parentWindow = Application.Current.MainWindow;

            var pd = DependencyPropertyDescriptor.FromProperty(UIElement.IsEnabledProperty, typeof(PageColorCountToolbar));
            pd.AddValueChanged(this, OnIsEnabledChanged);
            Console.WriteLine("TEST");
            progressBar = (ProgressBar)this.FindName("CountsProgressBar");
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            OnIsEnabledChanged(this, new PropertyChangedEventArgs(nameof(OnIsEnabledChanged)));
            progressBar = (ProgressBar)this.FindName("CountsProgressBar");
            itemsControl = (ItemsControl)this.FindName("PageColorCountContainer");
        }

        void OnUnload(object sender, RoutedEventArgs e)
        {
            DisableToolbar();
        }

        private ProgressBar progressBar;
        private int totalPageCount { get; set; } = 1;
        private int colorPageCount { get; set; } = 0;
        private int bwPageCount { get; set; } = 0;
        private int currentProgress { get; set; } = 0;

        public int progressBarProgress
        {
            get
            {
                return (currentProgress / totalPageCount) * 100;
            }
        }


        #region Dependencies

        public void OnIsEnabledChanged(object sender, EventArgs e)
        {
            if (((UserControl)sender).IsEnabled)
            {
                EnableToolbar();
            }
            else
            {
                DisableToolbar();
            }
                
        }

        #endregion

        public void EnableToolbar()
        {
            resetValues();
            progressBar.Visibility = Visibility.Visible;

            ItemsControl itemsControl = (ItemsControl)this.FindName("PageColorCountContainer");
            if (itemsControl != null)
                itemsControl.Items.Clear();

            // GetPageSizes();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_ColorTest;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_Completed;

            worker.RunWorkerAsync();

            //Task.Run(() => ColorTest(this));
            //ColorTest();
        }

        public void DisableToolbar()
        {
            resetValues();

            try {
                worker.CancelAsync();
            }
            catch { 
            }

            ItemsControl itemsControl = (ItemsControl)this.FindName("PageColorCountContainer");
            if (itemsControl != null)
                itemsControl.Items.Clear();


        }

        private void resetValues()
        {
            currentProgress = 0;
            totalPageCount= 1;
            bwPageCount = 0;
            colorPageCount= 0;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Maximum = totalPageCount;
            progressBar.Value = e.ProgressPercentage;
        }

        private void worker_CancelAsync()
        {
            ItemsControl itemsControl = (ItemsControl)this.FindName("PageColorCountContainer");
            if (itemsControl != null)
                itemsControl.Items.Clear();
        }

        private void ColorAsyncTest()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string inPath = ((WindowViewModel)parentWindow.DataContext).FilePath;
            ColorStats colorStats = new ColorStats();
            colorStats.PrintColorStats(inPath);

            stopwatch.Stop();
            Console.WriteLine("Elapsed time: {0}", stopwatch.Elapsed);
        }

        private void worker_Completed(object sender, EventArgs e)
        {
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void worker_ColorTest(object sender, DoWorkEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string inPath = "";
            this.Dispatcher.Invoke(() =>
            {
                inPath = ((WindowViewModel)parentWindow.DataContext).FilePath;
            });

            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inPath));
            totalPageCount = pdfDoc.GetNumberOfPages();

            List<PageSizeCount> colorPageSizes = new List<PageSizeCount>();
            colorPageSizes.AddRange(defaultPageSizes());

            List<PageSizeCount> bwPageSizes = new List<PageSizeCount>();
            bwPageSizes.AddRange(defaultPageSizes());

            for (int i = 1; i <= totalPageCount; i++)
            {
                if (worker.CancellationPending == true)
                    break;

                currentProgress = i;
                (sender as BackgroundWorker).ReportProgress(i);

                bool sizeFound = false;
                float width;
                float height;

                bool bHasColor = ColorStats.PdfPageContainsColorLockedBits(inPath, i);

                PdfPage currentPage = pdfDoc.GetPage(i);
                iText.Kernel.Geom.Rectangle cropRect = currentPage.GetCropBox();

                if (cropRect.GetWidth() <= cropRect.GetHeight())
                {
                    width = cropRect.GetWidth() / 72;
                    height = cropRect.GetHeight() / 72;
                }
                else
                {
                    width = cropRect.GetHeight() / 72;
                    height = cropRect.GetWidth() / 72;
                }

                if (bHasColor == true)
                {
                    foreach (PageSizeCount pageSize in colorPageSizes)
                    {
                        float deltaX = Math.Abs(width - pageSize.SizeX);
                        float deltaY = Math.Abs(height - pageSize.SizeY);

                        if (deltaX <= 0.04f && deltaY <= 0.04f)
                        {
                            pageSize.AddToCount();
                            sizeFound = true;
                            break;
                        }
                    }

                    if (!sizeFound)
                    {
                        float rWidth = (float)Decimal.Round((Decimal)width, 2);
                        float rHeight = (float)Decimal.Round((Decimal)height, 2);
                        colorPageSizes.Add(new PageSizeCount(Math.Min(rWidth, rHeight), Math.Max(rWidth, rHeight)));
                        colorPageSizes.Last().AddToCount();
                    }
                }
                else
                {
                    foreach (PageSizeCount pageSize in bwPageSizes)
                    {
                        float deltaX = Math.Abs(width - pageSize.SizeX);
                        float deltaY = Math.Abs(height - pageSize.SizeY);

                        if (deltaX <= 0.04f && deltaY <= 0.04f)
                        {
                            pageSize.AddToCount();
                            sizeFound = true;
                            break;
                        }
                    }

                    if (!sizeFound)
                    {
                        float rWidth = (float)Decimal.Round((Decimal)width, 2);
                        float rHeight = (float)Decimal.Round((Decimal)height, 2);
                        bwPageSizes.Add(new PageSizeCount(Math.Min(rWidth, rHeight), Math.Max(rWidth, rHeight)));
                        bwPageSizes.Last().AddToCount();
                    }
                }
            }

            colorPageSizes.RemoveAll(t => t.NumberOfPages == 0);
            bwPageSizes.RemoveAll(t => t.NumberOfPages == 0);
            this.Dispatcher.Invoke(() =>
            {
                if (itemsControl != null)
                {
                    itemsControl.Items.Add(new PageCountItem("TOTAL PAGES", totalPageCount, true));
                    itemsControl.Items.Add(new PageCountItem("", ""));

                    int colorTotal = 0;
                    foreach (PageSizeCount pageSize in colorPageSizes)
                    {
                        colorTotal += pageSize.NumberOfPages;
                    }
                    itemsControl.Items.Add(new PageCountItem("COLOR PAGES", colorTotal, true));

                    foreach (PageSizeCount pageSize in colorPageSizes)
                    {
                        Debug.WriteLine(pageSize.SizeY + "x" + pageSize.SizeY + ": " + pageSize.NumberOfPages);
                        itemsControl.Items.Add(new PageCountItem(pageSize.SizeX, pageSize.SizeY, pageSize.NumberOfPages));
                    }

                    itemsControl.Items.Add(new PageCountItem("", ""));

                    int bwTotal = 0;
                    foreach (PageSizeCount pageSize in bwPageSizes)
                    {
                        bwTotal += pageSize.NumberOfPages;
                    }
                    itemsControl.Items.Add(new PageCountItem("B/W PAGES", bwTotal, true));

                    foreach (PageSizeCount pageSize in bwPageSizes)
                    {
                        Debug.WriteLine(pageSize.SizeY + "x" + pageSize.SizeY + ": " + pageSize.NumberOfPages);
                        itemsControl.Items.Add(new PageCountItem(pageSize.SizeX, pageSize.SizeY, pageSize.NumberOfPages));
                    }
                }

            });

            


            this.Dispatcher.Invoke(() =>
            {
                if (((WindowViewModel)parentWindow.DataContext).FilePath == "" || worker.CancellationPending == true)
                {
                    if (itemsControl != null)
                        itemsControl.Items.Clear();
                }
            });
              



            stopwatch.Stop();
            Console.WriteLine("Elapsed time: {0}", stopwatch.Elapsed);
        }

        private void GetPageSizes()
        {
            string inPath = "";
            this.Dispatcher.Invoke(() =>
            {
                inPath = ((WindowViewModel)parentWindow.DataContext).FilePath;
            });
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inPath));
            totalPageCount = pdfDoc.GetNumberOfPages();
            OnPropertyChanged(nameof(totalPageCount));

            List<PageSizeCount> pageSizes = new List<PageSizeCount>();
            pageSizes.AddRange(defaultPageSizes());

            

            for (int i = 1; i <= totalPageCount; i++)
            {
                bool sizeFound = false;
                float width;
                float height;

                currentProgress = i;
                OnPropertyChanged(nameof(currentProgress));
                OnPropertyChanged(nameof(progressBarProgress));

                PdfPage currentPage = pdfDoc.GetPage(i);
                iText.Kernel.Geom.Rectangle cropRect = currentPage.GetCropBox();
                
                if (cropRect.GetWidth() <= cropRect.GetHeight())
                {
                    width = cropRect.GetWidth()/72;
                    height = cropRect.GetHeight()/72;
                }
                else
                {
                    width = cropRect.GetHeight()/72;
                    height = cropRect.GetWidth()/72;
                }

                foreach (PageSizeCount pageSize in pageSizes)
                {
                    float deltaX = Math.Abs(width - pageSize.SizeX);
                    float deltaY = Math.Abs(height - pageSize.SizeY);

                    if (deltaX <= 0.04f && deltaY <= 0.04f)
                    {
                        pageSize.AddToCount();
                        sizeFound = true;
                        break;
                    }
                }

                if (!sizeFound)
                {
                    float rWidth = (float)Decimal.Round((Decimal)width, 2);
                    float rHeight = (float)Decimal.Round((Decimal)height, 2);
                    pageSizes.Add(new PageSizeCount(Math.Min(rWidth, rHeight), Math.Max(rWidth, rHeight)));
                    pageSizes.Last().AddToCount();
                }
            }

            pageSizes.RemoveAll(t => t.NumberOfPages == 0);

            ItemsControl itemsControl = (ItemsControl)this.FindName("PageColorCountContainer");

            if (itemsControl != null)
                itemsControl.Items.Add(new PageCountItem("TOTAL PAGES", totalPageCount, true));

            foreach (PageSizeCount pageSize in pageSizes)
            {
                Debug.WriteLine(pageSize.SizeY + "x" + pageSize.SizeY + ": " + pageSize.NumberOfPages);
                if (itemsControl != null)
                    itemsControl.Items.Add(new PageCountItem(pageSize.SizeX, pageSize.SizeY, pageSize.NumberOfPages));
            }
            
        }

        #region Private Helpers

        private List<PageSizeCount> defaultPageSizes()
        {
            List<PageSizeCount> defaultPageSizes = new List<PageSizeCount>();
            defaultPageSizes.Add(new PageSizeCount(8.5f, 11f));
            defaultPageSizes.Add(new PageSizeCount(11f, 17f));
            defaultPageSizes.Add(new PageSizeCount(17f, 22f));
            defaultPageSizes.Add(new PageSizeCount(22f, 34f));
            defaultPageSizes.Add(new PageSizeCount(34f, 44f));
            defaultPageSizes.Add(new PageSizeCount(12f, 18f));
            defaultPageSizes.Add(new PageSizeCount(18f, 24f));
            defaultPageSizes.Add(new PageSizeCount(24f, 36f));
            defaultPageSizes.Add(new PageSizeCount(36f, 48f));

            return defaultPageSizes;
        }

        private bool ComparePageSizes(PageSizeCount pageSize, float sizeX, float sizeY)
        {

            return true;
        }

        #endregion

        #region IPropertyNotify

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
