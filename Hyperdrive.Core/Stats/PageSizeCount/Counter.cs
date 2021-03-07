using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText;
using iText.Layout;
using iText.Kernel.Pdf;

namespace Hyperdrive.Core.Stats.PageSizeCount
{
    public class Counter
    {
        public int GetPageCount(string filePath)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filePath));
            // Document doc = new Document(pdfDoc);

            // int numberOfPages = pdfDoc.GetNumberOfPages();

            return pdfDoc.GetNumberOfPages();
        }

        /*
        public void GetPageSizes(string filePath)
        {
            if (filePath != null)
            {
                // Right side of equation is location of YOUR pdf file
                //string ppath = "C:\\Users\\thevfxguy13\\Desktop\\18443435_LinuxFoundation.pdf";
                //string ppath = "C:\\Users\\thevfxguy13\\Downloads\\Wesley.pdf";
                PdfReader pdfReader = new PdfReader(filePath);
                int numberOfPages = GetPageCount(pdfReader);

                // float[] PageSize = PdfInfo.GetPdfPageSize(1, pdfReader);

                int PageSizeLetter = 0;
                int PageSizeTabloid = 0;
                int PageSizeOther = 0;

                for (int i = 1; i <= numberOfPages; i++)
                {
                    float[] PageSize = GetPdfPageSize(i, pdfReader);

                    if ((PageSize[0] == 8.5 && PageSize[1] == 11) || (PageSize[0] == 11 && PageSize[1] == 8.5))
                    {
                        PageSizeLetter++;
                    }
                    else if ((PageSize[0] == 17 && PageSize[1] == 11) || (PageSize[0] == 11 && PageSize[1] == 17))
                    {
                        PageSizeTabloid++;
                    }
                    else
                    {
                        PageSizeOther++;
                    }
                }
            }
        }


        public static float[] GetPdfPageSize(int PageNumber, PdfReader Pdf)
        {
            float[] PageSize = new float[] { 0, 0 };

            // Get page width in inches, rounded to two decimals
            float x = (Pdf.GetPageSize(PageNumber).Width) / 72;
            x = (float)Math.Round(x, 2);
            PageSize[0] = x;

            // Get page height in inches, rounded to two decimals
            float y = (Pdf.GetPageSize(PageNumber).Height) / 72;
            y = (float)Math.Round(y, 2);
            PageSize[1] = y;

            return PageSize;
        }

        // Gets the number of pages in the document
        public static int GetPageCount(PdfReader Pdf)
        {
            int numberOfPages = 0;

            return numberOfPages;
        }


        */

    }
}
