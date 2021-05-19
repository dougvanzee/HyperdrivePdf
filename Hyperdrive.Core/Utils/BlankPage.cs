using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperdrive.Core.Utils
{
    public static class BlankPage
    {
        /// <summary>
        /// Returns a PDF byte stream with one blank page at the specified size
        /// </summary>
        /// <param name="width">Width in units</param>
        /// <param name="height">Height in units</param>
        /// <returns></returns>
        public static byte[] GetBlankPage(float width, float height)
        {
            byte[] buffer;
            using (MemoryStream stream = new MemoryStream())
            {

                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf, new PageSize(width, height));
                document.SetMargins(72, 36, 54, 36);
                
                
                // Header
                Paragraph header = new Paragraph("")
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(20);

                // New line
                Paragraph newline = new Paragraph(new Text("\n"));

                // document.Add(newline);
                document.Add(header);

                /*
                // Add sub-header
                Paragraph subheader = new Paragraph(rootFolderPath)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(15);
                document.Add(subheader);

                // Line separator
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                document.Add(newline);
                */


                document.Close();

                buffer = stream.ToArray();
            }

            return buffer;
        }

        public static byte[] GetIntentionallyLeftBlankPage(float width, float height)
        {
            byte[] buffer;
            using (MemoryStream stream = new MemoryStream())
            {

                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf, new PageSize(width, height));
                // document.SetMargins(72, 36, 54, 36);

                
                // Header
                Paragraph header = new Paragraph("THIS PAGE INTENTIONALLY LEFT BLANK")
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(12)
                   .SetFixedPosition(0, height / 2, width)
                   .SetBold();

                // New line
                Paragraph newline = new Paragraph(new Text("\n"));

                // document.Add(newline);
                document.Add(header);

                /*
                // Add sub-header
                Paragraph subheader = new Paragraph(rootFolderPath)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(15);
                document.Add(subheader);

                // Line separator
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                document.Add(newline);
                */

                document.Close();

                buffer = stream.ToArray();
            }

            return buffer;
        }
    }
}
