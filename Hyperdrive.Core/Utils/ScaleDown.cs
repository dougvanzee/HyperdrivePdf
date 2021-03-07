using iText.Kernel.Events;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperdrive.Core.Utils
{
    public class ScaleDown
    {
        private string src;
        private string dest;

        public ScaleDown(string src, string dest)
        {
            this.src = src;
            this.dest = dest;
            // File file = new File(DEST);
            manipulatePdf();
        }

        protected void manipulatePdf()
        {
            PdfDocument srcDoc = new PdfDocument(new PdfReader(src));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(dest));

            float scale = 2f;
            ScaleDownEventHandler eventHandler = new ScaleDownEventHandler(scale);
            pdfDoc.AddEventHandler(PdfDocumentEvent.START_PAGE, eventHandler);

            int numberOfPages = srcDoc.GetNumberOfPages();
            for (int p = 1; p <= numberOfPages; p++)
            {
                eventHandler.SetPageDict(srcDoc.GetPage(p).GetPdfObject());

                // Copy and paste scaled page content as formXObject
                PdfFormXObject page = srcDoc.GetPage(p).CopyAsFormXObject(pdfDoc);
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.AddXObject(page, scale, 0f, 0f, scale, 0f, 0f);
            }

            pdfDoc.Close();
            srcDoc.Close();
        }


        private class ScaleDownEventHandler : IEventHandler
        {
            protected float scale = 1;
            protected PdfDictionary pageDict;

            public ScaleDownEventHandler(float scale)
            {
                this.scale = scale;
            }

            public void SetPageDict(PdfDictionary pageDict)
            {
                this.pageDict = pageDict;
            }

            public void HandleEvent(Event currentEvent)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
                PdfPage page = docEvent.GetPage();

                page.Put(PdfName.Rotate, pageDict.GetAsNumber(PdfName.Rotate));

                // The MediaBox value defines the full size of the page.
                scaleDown(page, pageDict, PdfName.MediaBox, scale);

                // The CropBox value defines the visible size of the page.
                scaleDown(page, pageDict, PdfName.CropBox, scale);
            }

            protected void scaleDown(PdfPage destPage, PdfDictionary pageDictSrc, PdfName box, float scale)
            {
                PdfArray original = pageDictSrc.GetAsArray(box);
                if (original != null)
                {
                    float width = original.GetAsNumber(2).FloatValue() - original.GetAsNumber(0).FloatValue();
                    float height = original.GetAsNumber(3).FloatValue() - original.GetAsNumber(1).FloatValue();

                    PdfArray result = new PdfArray();
                    result.Add(new PdfNumber(0));
                    result.Add(new PdfNumber(0));
                    result.Add(new PdfNumber(1728));
                    result.Add(new PdfNumber(2592));
                    // result.Add(new PdfNumber(width * scale));
                    // result.Add(new PdfNumber(height * scale));
                    destPage.Put(box, result);
                }
            }
        }
    }
}

