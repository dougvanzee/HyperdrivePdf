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

namespace Hyperdrive.UI.Views.SideToolbars
{
    /// <summary>
    /// Interaction logic for PageCountToolbar.xaml
    /// </summary>
    public partial class PageCountToolbar : UserControl, INotifyPropertyChanged
    {
        public Window parentWindow = Application.Current.MainWindow;

        public PageCountToolbar()
        {
            InitializeComponent();

            // parentWindow = Window.GetWindow(this);
            parentWindow = Application.Current.MainWindow;
            //OnPropertyChanged(nameof(totalPageCount));
            //OnPropertyChanged(nameof(currentProgress));

            Debug.WriteLine("Initial PageCountToolbar: " + ((WindowViewModel)(parentWindow.DataContext)).bIsFileOpen);

            // UIElement.IsEnabledProperty.AddOwner(typeof(PageCountToolbar), new FrameworkPropertyMetadata(OnIsEnabledChanged));

            //var desc = DependencyPropertyDescriptor.FromProperty(UIElement.IsEnabledProperty, typeof(FrameworkElement));
            //desc.DesignerCoerceValueCallback += OnIsEnabledChanged;

            var pd = DependencyPropertyDescriptor.FromProperty(UIElement.IsEnabledProperty, typeof(PageCountToolbar));
            pd.AddValueChanged(this, OnIsEnabledChanged);

            // OnPropertyChanged(nameof(OnIsEnabledChanged));
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("PageCountToolbar ONLOAD");
            OnIsEnabledChanged(this, new PropertyChangedEventArgs(nameof(OnIsEnabledChanged)));
        }

        public int totalPageCount { get; set; } = 1;
        public int currentProgress { get; set; } = 1;


        #region Dependencies

        public void OnIsEnabledChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("PageCountToolbar OnIsEnabledChanged: Called");
            if (((UserControl)sender).IsEnabled)
            {
                Debug.WriteLine("PageCountToolbar OnIsEnabledChanged: Enabled");
                EnableToolbar();
            }
            else
            {
                Debug.WriteLine("PageCountToolbar OnIsEnabledChanged: Disabled");
                DisableToolbar();
            }
                
        }

        #endregion

        public void EnableToolbar()
        {
            // totalPageCount = GetTotalPageCount();
            currentProgress = 0;
            GetPageSizes();
            
        }

        public void DisableToolbar()
        {
            totalPageCount = 1;
            currentProgress = 0;
            ItemsControl itemsControl = (ItemsControl)this.FindName("PageCountContainer");
            if (itemsControl != null)
                itemsControl.Items.Clear();
        }

        private void GetPageSizes()
        {
            string inPath = ((WindowViewModel)parentWindow.DataContext).FilePath;
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

                    if (deltaX <= 0.02f && deltaY <= 0.02f)
                    {
                        pageSize.AddToCount();
                        sizeFound = true;
                        break;
                    }
                }

                if (!sizeFound)
                {
                    pageSizes.Add(new PageSizeCount(width, height));
                    pageSizes.Last().AddToCount();
                }
            }

            pageSizes.RemoveAll(t => t.NumberOfPages == 0);

            ItemsControl itemsControl = (ItemsControl)this.FindName("PageCountContainer");

            if (itemsControl != null)
                itemsControl.Items.Add(new PageCountItem("TOTAL PAGES", totalPageCount));

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
