using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonPdfLib.MuPdf;
using System.Drawing;
using System.Drawing.Imaging;
using iText.Kernel.Pdf;
using System.Threading.Tasks;
using static Hyperdrive.Core.Stats.PageSizeCount.CounterDirectory;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Hyperdrive.Core.Stats.ColorStats;
using System.Web;

namespace Hyperdrive.Core.Stats
{
    public class ColorStats
    {
        public class ColorStatsObject
        {
            private float _width;
            private float _height;
            private bool _hasColor;

            public float width
            {
                get { return _width; }
            }

            public float height
            {
                get { return _height; }
            }

            public bool hasColor
            {
                get
                {
                    return _hasColor;
                }
            }

            public ColorStatsObject(float width, float height, bool hasColor)
            {
                _width = width;
                _height = height;
                _hasColor = hasColor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdfPath"></param>
        /// <param name="pageNumber">Page number of PDF starting at zero</param>
        /// <returns></returns>
        public static bool PdfPageContainsColor(string pdfPath, int pageNumber)
        {
            FileSource fileSource = new FileSource(pdfPath);
            using (Bitmap bitmap = MuPdfWrapper.ExtractPage(fileSource, 0, 0.2f))
            {
                bool hasColor = false;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        // Get the pixel
                        Color pixel = bitmap.GetPixel(x, y);

                        // Check if the pixel is not fully transparent
                        if (pixel.A != 0)
                        {
                            // Check if the pixel is not grayscale
                            if (Math.Abs(pixel.R - pixel.G) > 2 || Math.Abs(pixel.R - pixel.B) > 2)
                            {
                                // The pixel is colored
                                hasColor = true;
                                Console.WriteLine("R: " + pixel.R + "   G: " + pixel.G + "   B: " + pixel.B);
                                break;
                            }
                        }
                    }
                    if (hasColor)
                        break;
                }
                return hasColor;
            }

            // bitmap.Save("C:/Users/Doug/Downloads/Flyer_loveless-1wesdfsdfgdfdre.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);   
        }

        public static bool PdfPageContainsColorLockedBits(string pdfPath, int pageNumber)
        {
            FileSource fileSource = new FileSource(pdfPath);
            using (Bitmap bmp = MuPdfWrapper.ExtractPage(fileSource, pageNumber, 0.2f))
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    int stride = bmpData.Stride;
                    IntPtr ptr = bmpData.Scan0;
                    int bytes = stride * bmp.Height;
                    byte[] rgbaValues = new byte[bytes];
                    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbaValues, 0, bytes);

                    // Iterate over the pixels
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            int index = y * stride + x * 4;
                            byte r = rgbaValues[index + 2];
                            byte g = rgbaValues[index + 1];
                            byte b = rgbaValues[index + 0];
                            byte a = rgbaValues[index + 3];

                            // Check for colored pixels
                            if (Math.Abs(r - g) > 8 || Math.Abs(r - b) > 8 || Math.Abs(g - b) > 8)
                            {
                                return true;
                            }
                        }
                    }
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }

            return false;
        }

        /*
        private async Task<List<Bitmap>> RenderPagesAsync(int startPage, int endPage)
        {
            List<Task<Bitmap>> tasks = new List<Task<Bitmap>>();
            for (int i = startPage; i <= endPage; i++)
            {
                var task = RenderPageAsync(i);
                tasks.Add(task);
            }

            return await Task.WhenAll(tasks.ToArray());
        }

        private async Task<Bitmap> RenderPageAsync(int pageNumber)
        {
            // Open the document and get the page
            var doc = new MuPdfDocument();
            doc.Open("path/to/document.pdf", "");
            var page = doc.GetPage(pageNumber);

            // Render the page to a bitmap
            var bitmap = new Bitmap(page.Width, page.Height);
            var gfx = Graphics.FromImage(bitmap);
            page.Render(gfx);

            return bitmap;
        }
        */

        public async void PrintColorStats(string pdfPath)
        {
           GetColorStats(pdfPath);

        }

        private async Task<List<ColorStatsObject>> GetColorStats(string pdfPath)
        {
            var tasks= new List<Task<ColorStatsObject>>();
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfPath));
            int totalPages = pdfDoc.GetNumberOfPages();

            for (int i = 1; i <= totalPages; i++)
            {
                var task = GetColorStat(pdfPath, i);
                //task.Start();
                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());

            var tasks2 = new List<ColorStatsObject>();
            foreach (var task in tasks)
            {
                tasks2.Add(task.Result);
            }

            foreach (var statsObject in tasks2)
            {
                Console.WriteLine(statsObject.height + " x " + statsObject.width + ": " + statsObject.hasColor);
            }
            return tasks2;
        }

        private async Task<ColorStatsObject> GetColorStat(string pdfPath, int pageNumber)
        {
            FileSource fileSource = new FileSource(pdfPath);
            bool hasColor = false;

            PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfPath));

            PdfPage currentPage;
            iText.Kernel.Geom.Rectangle cropRect;
            currentPage = pdfDoc.GetPage(pageNumber);
            cropRect = currentPage.GetCropBox();
            float width;
            float height;

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

            using (Bitmap bmp = MuPdfWrapper.ExtractPage(fileSource, pageNumber, 0.2f))
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        // Get the pixel
                        Color pixel = bmp.GetPixel(x, y);

                        // Check if the pixel is not fully transparent
                        if (pixel.A != 0)
                        {
                            // Check if the pixel is not grayscale
                            if (Math.Abs(pixel.R - pixel.G) > 2 || Math.Abs(pixel.R - pixel.B) > 2)
                            {
                                // The pixel is colored
                                hasColor = true;
                                // Console.WriteLine("R: " + pixel.R + "   G: " + pixel.G + "   B: " + pixel.B);
                                break;
                            }
                        }
                    }
                    if (hasColor)
                        break;
                }
            }

            return new ColorStatsObject(width, height, hasColor);
        }

        private async Task<ColorStatsObject> GetColorStat2(string pdfPath, int pageNumber)
        {
            FileSource fileSource = new FileSource(pdfPath);
            bool hasColor = false;

            PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfPath));

            PdfPage currentPage;
            iText.Kernel.Geom.Rectangle cropRect;
            currentPage = pdfDoc.GetPage(pageNumber);
            cropRect = currentPage.GetCropBox();
            float width;
            float height;

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

            using (Bitmap bmp = MuPdfWrapper.ExtractPage(fileSource, pageNumber, 0.2f))
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    int stride = bmpData.Stride;
                    IntPtr ptr = bmpData.Scan0;
                    int bytes = stride * bmp.Height;
                    byte[] rgbaValues = new byte[bytes];
                    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbaValues, 0, bytes);

                    // Iterate over the pixels
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            int index = y * stride + x * 4;
                            byte r = rgbaValues[index + 2];
                            byte g = rgbaValues[index + 1];
                            byte b = rgbaValues[index + 0];
                            byte a = rgbaValues[index + 3];

                            // Check for colored pixels
                            if (Math.Abs(r - g) > 2 || Math.Abs(b - r) > 2)
                            {
                                hasColor = true;
                                break;
                            }
                        }
                    }
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }

            return new ColorStatsObject(width, height, hasColor);
        }
    }
}
