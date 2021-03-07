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

namespace Hyperdrive.Core.StepAndRepeat
{
    public class StepAndRepeatManager
    {
        public StepAndRepeatManager()
        {

        }

        public void BusinessCard8Up(string src, string dest)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            float marginX;
            float marginY;

            float targetX = 288;
            float targetY = 180;

            int RepeatX = 2;
            int RepeatY = 4;

            // Loop over every page
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                // PdfDictionary pageDict = pdfDoc.GetPage(i).GetPdfObject();
                // PdfArray mediaBox = pageDict.GetAsArray(PdfName.MediaBox);
                
                PdfPage page = pdfDoc.GetPage(i);
                Rectangle cropBox = page.GetCropBox();
                Rectangle mediaBox = page.GetMediaBox();
                // float llx = mediaBox.GetAsNumber(0).FloatValue();
                // float lly = mediaBox.GetAsNumber(1).FloatValue();
                // float urx = mediaBox.GetAsNumber(2).FloatValue();
                // float ury = mediaBox.GetAsNumber(3).FloatValue();

                float mx = mediaBox.GetX();
                float my = mediaBox.GetY();
                float mwidth = mediaBox.GetWidth();
                float mheight = mediaBox.GetHeight();

                marginX = (targetX - Math.Abs(mwidth - mx)) / 2;
                marginY = (targetY - Math.Abs(mheight - my)) / 2;

                page.SetMediaBox(new Rectangle(mx - marginX, my - marginY, 288, 180));
                page.SetCropBox(new Rectangle(mx - marginX, my - marginY, 288, 180));
                // mediaBox.Set(0, new PdfNumber(llx - marginX));
                // mediaBox.Set(1, new PdfNumber(lly - marginY));
                // mediaBox.Set(2, new PdfNumber(urx + marginX));
                // mediaBox.Set(3, new PdfNumber(ury + marginY));
                // PdfCanvas over = new PdfCanvas(pdfDoc.GetPage(i));
                // over.SaveState();
                // over.SetFillColor(new DeviceGray(1.0f));
                // over.Rectangle(llx - 18, lly - 18, urx + 36, ury + 36);
                // over.Fill();
                // over.RestoreState();

                               
                Rectangle orig = page.GetPageSize();
                PdfFormXObject pageCopy = page.CopyAsFormXObject(pdfDoc);
                
                // N-up page       
                PageSize nUpPageSize = PageSize.LETTER;
                PdfPage finalPage = pdfDoc.AddNewPage(nUpPageSize);
                PdfCanvas canvas = new PdfCanvas(finalPage);

                // Offsets
                float LocalOffsetX = -marginX;
                float LocalOffsetY = -marginY;
                float GlobalOffsetX = 18;
                float GlobalOffsetY = 36;
                float PageWidth = orig.GetWidth();
                float PageHeight = orig.GetHeight();

                for(int x = 1; x <= RepeatX; x++)
                {
                    for(int y = 1; y <= RepeatY; y++)
                    {
                        canvas.AddXObject(pageCopy, GlobalOffsetX - LocalOffsetX + PageWidth * (x - 1), GlobalOffsetY - LocalOffsetY + PageHeight * (y - 1));
                    }
                }

                pdfDoc.RemovePage(i);
                pdfDoc.MovePage(pdfDoc.GetLastPage(), i);

                /*
                // Add pages to N-up page       
                canvas.AddXObject(pageCopy, LocalOffsetX, orig.GetHeight() + LocalOffsetY);
                canvas.AddXObject(pageCopy, LocalOffsetX + orig.GetWidth(), LocalOffsetY + orig.GetHeight());
                canvas.AddXObject(pageCopy, LocalOffsetX, LocalOffsetY);
                canvas.AddXObject(pageCopy, LocalOffsetX + orig.GetWidth(), LocalOffsetY);
                */
            }

            pdfDoc.Close();
        }

            public void BusinessCard8Up(string src, string dest, uint repeatX, uint repeatY)
        {
            
            // Creating a PdfWriter object       
            PdfWriter writer = new PdfWriter(dest);

            // Creating a PdfReader       
            PdfReader reader = new PdfReader(src);

            // Creating a PdfDocument objects       
            PdfDocument destpdf = new PdfDocument(writer);
            PdfDocument srcPdf = new PdfDocument(reader);

            // Opening a page from the existing PDF       
             PdfPage origPage = srcPdf.GetPage(1);
            // origPage.SetMediaBox(new Rectangle(0, 0, 288, 180));

            float margin = 18;
            Rectangle mediaBox = origPage.GetMediaBox();
            Rectangle newMediaBox = new Rectangle(
                    mediaBox.GetLeft() - margin * 2, mediaBox.GetBottom() - margin * 2,
                    mediaBox.GetWidth() + margin * 2, mediaBox.GetHeight() + margin * 2);
            origPage.SetMediaBox(newMediaBox);
            

            Rectangle orig = origPage.GetPageSize();
             PdfFormXObject pageCopy = origPage.CopyAsFormXObject(destpdf);

            // N-up page       
            PageSize nUpPageSize = PageSize.LETTER;
            PdfPage page = destpdf.AddNewPage(nUpPageSize);
            PdfCanvas canvas = new PdfCanvas(page);

            /*
            // Scale page
            AffineTransform transformationMatrix = AffineTransform.GetScaleInstance(
               nUpPageSize.GetWidth() / orig.GetWidth() /
               2f, nUpPageSize.GetHeight() / orig.GetHeight() / 2f);
            canvas.ConcatMatrix(transformationMatrix);
            */

            // Add pages to N-up page       
            canvas.AddXObject(pageCopy, 0, orig.GetHeight() - orig.GetBottom());
            canvas.AddXObject(pageCopy, orig.GetWidth() - orig.GetLeft(), orig.GetHeight() - orig.GetBottom());
            canvas.AddXObject(pageCopy, 0, 0);
            canvas.AddXObject(pageCopy, orig.GetWidth() - orig.GetLeft(), 0);

            // closing the documents       
            destpdf.Close();
            srcPdf.Close();
            
            /*
            // Creating a PdfWriter object       
            PdfWriter writer = new PdfWriter(dest);

            // Creating a PdfReader       
            PdfReader reader = new PdfReader(src);

            // Creating a PdfDocument objects       
            PdfDocument destpdf = new PdfDocument(writer);
            PdfDocument srcPdf = new PdfDocument(reader);

            // PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));

            // Loop over every page
            for (int i = 1; i <= srcPdf.GetNumberOfPages(); i++)
            {
                PdfDictionary pageDict = srcPdf.GetPage(i).GetPdfObject();
                PdfArray mediaBox = pageDict.GetAsArray(PdfName.MediaBox);
                float llx = mediaBox.GetAsNumber(0).FloatValue();
                float lly = mediaBox.GetAsNumber(1).FloatValue();
                float urx = mediaBox.GetAsNumber(2).FloatValue();
                float ury = mediaBox.GetAsNumber(3).FloatValue();
                mediaBox.Set(0, new PdfNumber(llx - 18));
                mediaBox.Set(1, new PdfNumber(lly - 18));
                mediaBox.Set(2, new PdfNumber(urx + 18));
                mediaBox.Set(3, new PdfNumber(ury + 18));
                PageSize ThePageSize = srcPdf.GetPage(i).GetPageSize();
                PdfPage page = destpdf.AddNewPage(ThePageSize);
                PdfCanvas over = new PdfCanvas(page);
                // PdfCanvas over = new PdfCanvas(srcPdf.GetPage(i));
                over.SaveState();
                over.SetFillColor(new DeviceGray(1.0f));
                over.Rectangle(llx - 18, lly - 18, urx + 36, ury + 36);
                over.Fill();
                over.RestoreState();
                PdfPage origPage = srcPdf.GetPage(1);
                Rectangle orig = origPage.GetPageSize();
                PdfFormXObject pageCopy = origPage.CopyAsFormXObject(destpdf);
                over.AddXObject(pageCopy, 0, orig.GetHeight());
            }
            
            srcPdf.Close();
            destpdf.Close();
            */
        }

        public void GetPageCount(string src, string dest)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));

            Console.WriteLine("GetPageCount");
        }
    }
}
