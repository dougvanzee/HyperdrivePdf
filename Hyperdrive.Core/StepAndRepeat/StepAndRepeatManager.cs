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
using Windows.Media.SpeechRecognition;

namespace Hyperdrive.Core.StepAndRepeat
{
    public static class StepAndRepeatManager
    {
        public static bool BusinessCard8Up(string SRC, string dest)
        {
            try
            {
                PdfDocument pdf = new PdfDocument(new PdfWriter(dest));

                PdfDocument sourcePdf = new PdfDocument(new PdfReader(SRC));

                PageSize nUpPageSize = PageSize.LETTER; // Letter Page Size

                for (int i = 1; i <= sourcePdf.GetNumberOfPages(); i++)
                {
                    //Original page

                    PdfPage origPage = sourcePdf.GetPage(i);

                    // Crop original page

                    float x = origPage.GetCropBox().GetX();

                    float width = origPage.GetCropBox().GetWidth();

                    float y = origPage.GetCropBox().GetY();

                    float height = origPage.GetCropBox().GetHeight();

                    float deltaX = (288 - (width - x)) / 2;

                    float deltaY = (180 - (height - y)) / 2;

                    origPage.SetCropBox(new Rectangle(x - deltaX, y - deltaY, width + (2 * deltaX), height + (2 * deltaY)));

                    // Setup Page

                    PdfFormXObject pageCopy = origPage.CopyAsFormXObject(pdf);

                    Rectangle orig = origPage.GetPageSize();

                    //N-up page

                    PdfPage page = pdf.AddNewPage(nUpPageSize);

                    PdfCanvas canvas = new PdfCanvas(page);

                    //Scale page
                    /*
                    AffineTransform transformationMatrix = AffineTransform.GetScaleInstance(

                        nUpPageSize.GetWidth() / orig.GetWidth() / 2f,

                        nUpPageSize.GetHeight() / orig.GetHeight() / 2f);

                    canvas.ConcatMatrix(transformationMatrix);
                    */
                    //Add pages to N-up page

                    float offsetX = -144;

                    float offsetY = -90;

                    canvas.AddXObject(pageCopy, 160 + offsetX + deltaX, 126 + offsetY + deltaY);

                    canvas.AddXObject(pageCopy, 450 + offsetX + deltaX, 126 + offsetY + deltaY);

                    canvas.AddXObject(pageCopy, 160 + offsetX + deltaX, 306 + offsetY + deltaY);

                    canvas.AddXObject(pageCopy, 450 + offsetX + deltaX, 306 + offsetY + deltaY);

                    canvas.AddXObject(pageCopy, 160 + offsetX + deltaX, 486 + offsetY + deltaY);

                    canvas.AddXObject(pageCopy, 450 + offsetX + deltaX, 486 + offsetY + deltaY);

                    canvas.AddXObject(pageCopy, 160 + offsetX + deltaX, 666 + offsetY + deltaY);

                    canvas.AddXObject(pageCopy, 450 + offsetX + deltaX, 666 + offsetY + deltaY);
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
