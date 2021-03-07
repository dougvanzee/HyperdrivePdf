using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hyperdrive.Core.Resizer
{
    public enum PagesToScaleType
    {
        ALL,
        CURRENT,
        SELECTED,
        RANGE
    }

    public enum ScaleType
    {
        EXACT,
        FIT,
        SCALE_DOWN,
        HEIGHT_OR_WIDTH
    }

    public enum RotationType
    {
        NONE,
        AUTO_LEFT,
        AUTO_RIGHT,
        PAGE
    }

    public enum PageAlignment
    {
        CENTER,
        TOP_LEFT,
        TOP,
        TOP_RIGHT,
        RIGHT,
        BOTTOM_RIGHT,
        BOTTOM,
        BOTTOM_LEFT,
        LEFT
    }

    public enum PageHeightWidthScaleType
    {
        WIDTH,
        HEIGHT,
        HEIGHT_WIDTH,
        LONG_EDGE,
        SHORT_EDGE
    }

    public class Resizer
    {
        private string src;
        private string dest;

        private float scale;
        private float pageWidth;
        private float pageHeight;

        private ScaleType scaleType;
        private PageAlignment pageAlignment;

        public Resizer(string src, string dest)
        {
            this.src = src;
            this.dest = dest;
        }

        public void ResizeToScale(float scale)
        {
            PdfDocument srcDoc = new PdfDocument(new PdfReader(src));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(dest));
            
            ScalePageEventHandler eventHandler = new ScalePageEventHandler(scale);
            pdfDoc.AddEventHandler(PdfDocumentEvent.START_PAGE, eventHandler);
            
            int numberOfPages = srcDoc.GetNumberOfPages();
            for (int p = 1; p <= numberOfPages; p++)
            {
                eventHandler.SetPageDict(srcDoc.GetPage(p).GetPdfObject());

                // Copy and paste scaled page content as formXObject
                PdfFormXObject page = srcDoc.GetPage(p).CopyAsFormXObject(pdfDoc);
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());

                // canvas.AddXObject(page, scale, 0f, 0f, scale, 0f, 0f);
                canvas.AddXObjectWithTransformationMatrix(page, scale, 0f, 0f, scale, 0f, 0f);
            }
            
            pdfDoc.Close();
            srcDoc.Close();
        }

        public void ResizeToPageSize(float pageWidth, float pageHeight, float preferredScale, ScaleType scaleType = ScaleType.FIT, PageAlignment pageAlignment = PageAlignment.CENTER)
        {
            PdfDocument srcDoc = new PdfDocument(new PdfReader(src));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(dest));

            ScalePageEventHandler eventHandler = new ScalePageEventHandler(pageWidth, pageHeight);
            pdfDoc.AddEventHandler(PdfDocumentEvent.START_PAGE, eventHandler);

            int numberOfPages = srcDoc.GetNumberOfPages();
            for (int p = 1; p <= numberOfPages; p++)
            {
                eventHandler.SetPageDict(srcDoc.GetPage(p).GetPdfObject());

                // Copy and paste scaled page content as formXObject
                PdfFormXObject page = srcDoc.GetPage(p).CopyAsFormXObject(pdfDoc);
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());

                // canvas.AddXObject(page, scale, 0f, 0f, scale, 0f, 0f);
                canvas.AddXObjectWithTransformationMatrix(page, scale, 0f, 0f, scale, 0f, 0f);
            }

            pdfDoc.Close();
            srcDoc.Close();
        }



        private class ScalePageEventHandler : IEventHandler
        {
            protected enum PageType
            {
                EXACT_SIZE,
                SCALED
            };

            protected float scale = 1;
            protected PdfDictionary pageDict;
            protected PageType pageType;
            protected float pageWidth;
            protected float pageHeight;

            public ScalePageEventHandler(float scale)
            {
                this.scale = scale;
                pageType = PageType.SCALED;
            }

            public ScalePageEventHandler(float width, float height)
            {
                this.pageWidth = width;
                this.pageHeight = height;
                pageType = PageType.EXACT_SIZE;
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

                switch (pageType)
                {
                    case PageType.SCALED:
                        scalePage(page, pageDict, PdfName.MediaBox);
                        scalePage(page, pageDict, PdfName.CropBox);
                        break;

                    case PageType.EXACT_SIZE:
                        scalePage(page, pageDict, PdfName.MediaBox, pageWidth, pageHeight);
                        scalePage(page, pageDict, PdfName.CropBox, pageWidth, pageHeight);
                        break;
                }
            }

            // Sizes page based upon scale
            protected void scalePage(PdfPage destPage, PdfDictionary pageDictSrc, PdfName box)
            {
                PdfArray original = pageDictSrc.GetAsArray(box);
                if (original != null)
                {
                    float width = original.GetAsNumber(2).FloatValue() - original.GetAsNumber(0).FloatValue();
                    float height = original.GetAsNumber(3).FloatValue() - original.GetAsNumber(1).FloatValue();

                    PdfArray result = new PdfArray();
                    result.Add(new PdfNumber(0));
                    result.Add(new PdfNumber(0));
                    result.Add(new PdfNumber(width * scale));
                    result.Add(new PdfNumber(height * scale));
                    destPage.Put(box, result);
                }
            }

            // Sizes page to exact size
            protected void scalePage(PdfPage destPage, PdfDictionary pageDictSrc, PdfName box, float width, float height)
            {
                PdfArray original = pageDictSrc.GetAsArray(box);
                if (original != null)
                {
                    PdfArray result = new PdfArray();
                    result.Add(new PdfNumber(0));
                    result.Add(new PdfNumber(0));
                    result.Add(new PdfNumber(width));
                    result.Add(new PdfNumber(height));
                    destPage.Put(box, result);
                }
            }
        }
    }
}

