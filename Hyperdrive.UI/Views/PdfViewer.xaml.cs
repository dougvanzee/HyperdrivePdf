using System;
using System.Collections.Generic;
using System.IO;
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
using System.Data;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading;
using System.Drawing;

namespace Hyperdrive.UI.Views
{
    /// <summary>
    /// Interaction logic for PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : UserControl
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        #region Bindable Properties

        public string PdfPath
        {
            get { return (string)GetValue(PdfPathProperty); }
            set { SetValue(PdfPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PdfPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PdfPathProperty =
            DependencyProperty.Register("PdfPath", typeof(string), typeof(PdfViewer), new PropertyMetadata(null, propertyChangedCallback: OnPdfPathChanged));

        private static void OnPdfPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pdfDrawer = (PdfViewer)d;

            if (!string.IsNullOrEmpty(pdfDrawer.PdfPath) || pdfDrawer.PdfPath != "")
            {
                //making sure it's an absolute path
                var path = System.IO.Path.GetFullPath(pdfDrawer.PdfPath);

                pdfDrawer.cancellationTokenSource = new CancellationTokenSource();

                /*
                StorageFile.GetFileFromPathAsync(path).AsTask()
                  //load pdf document on background thread
                  .ContinueWith(t => PdfDocument.LoadFromFileAsync(t.Result).AsTask()).Unwrap()
                  //display on UI Thread
                  .ContinueWith(t2 => PdfToImages(pdfDrawer, t2.Result), TaskScheduler.FromCurrentSynchronizationContext());
                  */

                StorageFile.GetFileFromPathAsync(path).AsTask()
                  //load pdf document on background thread
                  .ContinueWith(t => PdfDocument.LoadFromFileAsync(t.Result).AsTask()).Unwrap()
                  //display on UI Thread
                  .ContinueWith(t2 => ExecutePageToBitmapAsync(pdfDrawer, t2.Result), TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                try
                {
                    pdfDrawer.cancellationTokenSource.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine("obj accessed after dispose!");
                }
            }
            pdfDrawer.PagesContainer.Items.Clear();
        }

        #endregion

        public PdfViewer()
        {
            InitializeComponent();
        }

        private static async Task PdfToImages(PdfViewer pdfViewer, PdfDocument pdfDoc)
        {
            var items = pdfViewer.PagesContainer.Items;
            items.Clear();

            System.Windows.Size maxSize = pdfViewer.PagesContainer.RenderSize;

            if (pdfDoc == null) return;

            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                using (var page = pdfDoc.GetPage(i))
                {
                    var bitmap = await PageToBitmapAsync(page);
                    var image = new System.Windows.Controls.Image
                    {
                        Source = bitmap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 4, 0, 4),
                        MaxWidth = maxSize.Width,
                        MaxHeight = maxSize.Height
                    };
                    items.Add(image);
                }
            }

            
        }

        private static async Task<BitmapImage> PageToBitmapAsync(PdfPage page)
        {
            BitmapImage image = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream.AsStream();
                image.EndInit();
            }

            return image;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ItemsControl itemsControl = sender as ItemsControl;
            ScrollViewer scrollViewer = e.OriginalSource as ScrollViewer;
            FrameworkElement lastElement = null;
            foreach (object obj in itemsControl.Items)
            {
                FrameworkElement element = itemsControl.ItemContainerGenerator.ContainerFromItem(obj) as FrameworkElement;
                double offset = element.TransformToAncestor(scrollViewer).Transform(new System.Windows.Point(0, 0)).Y + scrollViewer.VerticalOffset;
                if (offset > e.VerticalOffset)
                {
                    if (lastElement != null)
                        lastElement.BringIntoView();
                    break;
                }
                lastElement = element;
            }
        }














        private static async Task PdfToImages2(PdfViewer pdfViewer, PdfDocument pdfDoc, CancellationToken cancellationToken)
        {
            var items = pdfViewer.PagesContainer.Items;
            items.Clear();

            System.Windows.Size maxSize = pdfViewer.PagesContainer.RenderSize;

            if (pdfDoc == null) return;

            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                using (var page = pdfDoc.GetPage(i))
                {

                    var bitmap = await PageToBitmap2Async(page);

                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();

                    var image = new System.Windows.Controls.Image
                    {
                        Source = bitmap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 4, 0, 4),
                        MaxWidth = maxSize.Width,
                        MaxHeight = maxSize.Height
                    };
                    items.Add(image);
                }
            }
        }

        public static async Task ExecutePageToBitmapAsync(PdfViewer pdfViewer, PdfDocument pdfDoc)
        {
            

            // BitmapImage image = new BitmapImage();

            using (pdfViewer.cancellationTokenSource)
            {
                /*
                // Creating a task to listen to keyboard key press
                var keyBoardTask = Task.Run(() =>
                {
                    Thread.Sleep(500);

                    // Cancel the task
                    pdfViewer.cancellationTokenSource.Cancel();
                });
                */
                try
                {
                    var longRunningTask = PdfToImages2(pdfViewer, pdfDoc, pdfViewer.cancellationTokenSource.Token);

                    await longRunningTask;
                    // image = result;
                    // Console.WriteLine("Result {0}", result);
                    Console.WriteLine("Press enter to continue");
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Task was cancelled");
                }

                // await keyBoardTask;
            }

            pdfViewer.cancellationTokenSource.Dispose();
            // return image;
        }

        private static async Task<BitmapImage> PageToBitmap2Async(PdfPage page)
        {
            BitmapImage image = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream.AsStream();
                image.EndInit();
            }

            return image;
        }
    }
}
