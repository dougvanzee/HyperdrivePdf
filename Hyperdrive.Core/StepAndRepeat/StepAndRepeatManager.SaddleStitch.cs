using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Hyperdrive.Core.StepAndRepeat
{
    public static partial class StepAndRepeatManager
    {
        public static bool SaddleStitchFullBleed(string SRC, string dest)
        {
            PdfDocument pdf = new PdfDocument(new PdfWriter(dest));

            PdfDocument sourcePdf = new PdfDocument(new PdfReader(SRC));

            PageSize nUpPageSize = new PageSize(new Rectangle(0, 0, 1296, 864)); // 12x18 Page Size

            try
            {
                int numPages = sourcePdf.GetNumberOfPages();

                int numBookPages = (int)(Math.Ceiling((double)numPages / 4) * 2);

                for (int i = 1; i <= numBookPages; i++)
                {
                    // if odd numbered book page
                    if (i % 2 == 1)
                    {
                        // Setup Page

                        PdfPage page = pdf.AddNewPage(nUpPageSize);
                        PdfCanvas canvas = new PdfCanvas(page);

                        // Right side of odd page
                        if (i <= numPages)
                        {
                            //Original page

                            PdfPage origPage = sourcePdf.GetPage(i);

                            Rectangle pageSize = origPage.GetPageSize();

                            float pageX = (pageSize.GetWidth() - pageSize.GetX()) / 72;
                            float pageY = (pageSize.GetHeight() - pageSize.GetY()) / 72;

                            float x = origPage.GetCropBox().GetX();
                            float y = origPage.GetCropBox().GetY();
                            float width = origPage.GetCropBox().GetWidth();
                            float height = origPage.GetCropBox().GetHeight();

                            float deltaX;
                            float deltaY;

                            deltaX = (612 - (width - x)) / 2;
                            deltaY = (792 - (height - y)) / 2;

                            origPage.SetCropBox(new Rectangle(x - deltaX, y, width, height));

                            // Setup Page

                            PdfFormXObject pageCopy = origPage.CopyAsFormXObject(pdf);

                            // PdfPage page = pdf.AddNewPage(nUpPageSize);
                            // PdfCanvas canvas = new PdfCanvas(page);

                            AffineTransform affineTransform = AffineTransform.GetTranslateInstance(deltaX + 648, 36 + deltaY);
                            float[] matrix = new float[6];
                            affineTransform.GetMatrix(matrix);

                            canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                        }

                        // Left side of odd page
                        if (numBookPages * 2 - i + 1 <= numPages)
                        {
                            //Original page

                            PdfPage origPage = sourcePdf.GetPage(numBookPages * 2 - i + 1);

                            Rectangle pageSize = origPage.GetPageSize();

                            float pageX = (pageSize.GetWidth() - pageSize.GetX()) / 72;
                            float pageY = (pageSize.GetHeight() - pageSize.GetY()) / 72;

                            float x = origPage.GetCropBox().GetX();
                            float y = origPage.GetCropBox().GetY();
                            float width = origPage.GetCropBox().GetWidth();
                            float height = origPage.GetCropBox().GetHeight();

                            float deltaX;
                            float deltaY;

                            deltaX = (612 - (width - x)) / 2;
                            deltaY = (792 - (height - y)) / 2;

                            origPage.SetCropBox(new Rectangle(x, y, width + deltaX, height));

                            PdfFormXObject pageCopy = origPage.CopyAsFormXObject(pdf);

                            AffineTransform affineTransform = AffineTransform.GetTranslateInstance(36 + deltaX, 36 + deltaY);
                            float[] matrix = new float[6];
                            affineTransform.GetMatrix(matrix);

                            canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                        }

                    }
                    else // Even pages
                    {
                        // Setup Page

                        PdfPage page = pdf.AddNewPage(nUpPageSize);
                        PdfCanvas canvas = new PdfCanvas(page);

                        // Left side of even page
                        if (i <= numPages)
                        {
                            //Original page

                            PdfPage origPage = sourcePdf.GetPage(i);

                            Rectangle pageSize = origPage.GetPageSize();

                            float pageX = (pageSize.GetWidth() - pageSize.GetX()) / 72;
                            float pageY = (pageSize.GetHeight() - pageSize.GetY()) / 72;

                            float x = origPage.GetCropBox().GetX();
                            float y = origPage.GetCropBox().GetY();
                            float width = origPage.GetCropBox().GetWidth();
                            float height = origPage.GetCropBox().GetHeight();

                            float deltaX;
                            float deltaY;

                            deltaX = (612 - (width - x)) / 2;
                            deltaY = (792 - (height - y)) / 2;

                            // origPage.SetCropBox(new Rectangle(x - deltaX, y, width, height));
                            origPage.SetCropBox(new Rectangle(x, y, width + deltaX, height));

                            // Setup Page

                            PdfFormXObject pageCopy = origPage.CopyAsFormXObject(pdf);

                            // PdfPage page = pdf.AddNewPage(nUpPageSize);
                            // PdfCanvas canvas = new PdfCanvas(page);

                            AffineTransform affineTransform = AffineTransform.GetTranslateInstance(36 + deltaX, 36 + deltaY);
                            float[] matrix = new float[6];
                            affineTransform.GetMatrix(matrix);

                            canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                        }

                        // Right side of even page
                        if (numBookPages * 2 - i + 1 <= numPages)
                        {
                            //Original page

                            PdfPage origPage = sourcePdf.GetPage(numBookPages * 2 - i + 1);

                            Rectangle pageSize = origPage.GetPageSize();

                            float pageX = (pageSize.GetWidth() - pageSize.GetX()) / 72;
                            float pageY = (pageSize.GetHeight() - pageSize.GetY()) / 72;

                            float x = origPage.GetCropBox().GetX();
                            float y = origPage.GetCropBox().GetY();
                            float width = origPage.GetCropBox().GetWidth();
                            float height = origPage.GetCropBox().GetHeight();

                            float deltaX;
                            float deltaY;

                            deltaX = (612 - (width - x)) / 2;
                            deltaY = (792 - (height - y)) / 2;

                            // origPage.SetCropBox(new Rectangle(x, y, width + deltaX, height));
                            origPage.SetCropBox(new Rectangle(x - deltaX, y, width, height));

                            PdfFormXObject pageCopy = origPage.CopyAsFormXObject(pdf);

                            AffineTransform affineTransform = AffineTransform.GetTranslateInstance(deltaX + 648, 36 + deltaY);
                            float[] matrix = new float[6];
                            affineTransform.GetMatrix(matrix);

                            canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                        }
                    }


                }
                // close the documents

                pdf.Close();

                sourcePdf.Close();
            }
            catch
            {
                return false;
            }

            return true;

        }
    }
}
