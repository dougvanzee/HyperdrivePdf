using Common.Logging;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Source;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Threading;
using iText.Kernel.Pdf.Colorspace;

namespace Hyperdrive.Core.Stats.PageSizeCount
{
    public class CounterDirectory
    {
        public class PageSizeCountError
        {
            private string fileName;
            private string filePath;
            private string errorMessage;

            public PageSizeCountError(string fileName, string filePath, string errorMessage)
            {
                this.fileName = fileName;
                this.filePath = filePath;
                this.errorMessage = errorMessage;
            }

            public string FileName { get { return fileName; } }
            public string FilePath { get { return filePath; } }
            public string ErrorMessage { get { return errorMessage; } }
        }

        private string rootFolderPath;
        private string reportPath;
        private int totalPageCount = 0;

        private List<PageSizeCount> pageSizeCounts;
        private List<PageSizeCountError> pageSizeCountErrors = new List<PageSizeCountError>();

        private List<string> filePaths;

        private int fileListLength = 0;
        private volatile int currentProgress = 0;
        private volatile bool isDone = false;

        private object loadingScreen;

        private CancellationTokenSource cts;
        CancellationToken token;

        public CounterDirectory(string folderPath, string reportPath, bool includeSubdirectories, bool includeNonPdfs)
        {
            rootFolderPath = folderPath;
            this.reportPath = reportPath;

            filePaths = GetFileList(includeSubdirectories, includeNonPdfs).ToList();
            fileListLength = filePaths.Count();
        }

        public int getNumberOfPdfs() { return fileListLength; }

        public int getCurrentProgress() { return currentProgress; }

        public bool getIsDone() { return isDone; }

        public async void PrintPageSizeCounts()
        {
            cts = new CancellationTokenSource();
            token = cts.Token;
            try
            {
                await Task.Run(() => PrintAllPdfFilesInDirectory());
            }
            catch (OperationCanceledException)
            {

            }
        }

        private async Task PrintAllPdfFilesInDirectory()
        {
            pageSizeCounts = GetAllPageSizeCounts();

            isDone = true;

            token.ThrowIfCancellationRequested();

            generatePdfReport();
        }

        private List<PageSizeCount> GetAllPageSizeCounts()
        {
            totalPageCount = 0;

            List<PageSizeCount> pageSizes = new List<PageSizeCount>();
            pageSizes.AddRange(defaultPageSizeList());

            int currentIndex = 0;

            foreach (string filePath in filePaths)
            {
                token.ThrowIfCancellationRequested();

                foreach (PageSizeCount pageSize in pageSizes)
                {
                    pageSize.ClearStagedCounts();
                }

                bool bEndInError = false;
                currentIndex++;
                currentProgress = currentIndex;

                try
                {
                    if (!(System.IO.Path.GetExtension(filePath).Equals(".pdf", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        Console.WriteLine(System.IO.Path.GetExtension(filePath));
                        pageSizeCountErrors.Add(new PageSizeCountError(System.IO.Path.GetFileName(filePath), filePath, "Not a PDF"));
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                PdfDocument pdfDoc;
                int pdfPageCount;

                try
                {
                    pdfDoc = new PdfDocument(new PdfReader(filePath));
                    pdfPageCount = pdfDoc.GetNumberOfPages();
                }
                catch (Exception ex)
                {
                    pageSizeCountErrors.Add(new PageSizeCountError(System.IO.Path.GetFileName(filePath), filePath, "Invalid PDF"));
                    Console.WriteLine(ex);
                    continue;
                }

                for (int i = 1; i <= pdfPageCount; i++)
                {
                    token.ThrowIfCancellationRequested();
                    
                    bool sizeFound = false;
                    float width;
                    float height;

                    PdfPage currentPage;
                    iText.Kernel.Geom.Rectangle cropRect;

                    try
                    {
                        currentPage = pdfDoc.GetPage(i);
                        cropRect = currentPage.GetCropBox();
                    }
                    catch
                    {
                        bEndInError = true;
                        pageSizeCountErrors.Add(new PageSizeCountError(System.IO.Path.GetFileName(filePath), filePath, "PDF Syntax Error"));
                        break;
                    }

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

                    foreach (PageSizeCount pageSize in pageSizes)
                    {
                        float deltaX = Math.Abs(width - pageSize.SizeX);
                        float deltaY = Math.Abs(height - pageSize.SizeY);

                        if (deltaX <= 0.04f && deltaY <= 0.04f)
                        {
                            sizeFound = true;
                            pageSize.AddToStagedCount();
                            break;
                        }
                    }

                    if (!sizeFound)
                    {
                        foreach (PageSizeCount pageSize in pageSizes)
                        {
                            float deltaX = Math.Abs(width - pageSize.SizeX);
                            float deltaY = Math.Abs(height - pageSize.SizeY);

                            if (deltaX <= 0.49f && deltaY <= 0.49f)
                            {
                                sizeFound = true;
                                pageSize.AddToStagedCount();
                                break;
                            }
                        }
                    }

                    if (!sizeFound)
                    {
                        foreach (PageSizeCount pageSize in pageSizes)
                        {
                            float deltaX = Math.Abs(width - pageSize.SizeX);
                            float deltaY = Math.Abs(height - pageSize.SizeY);

                            if (deltaX <= 0.99f && deltaY <= 0.99f)
                            {
                                sizeFound = true;
                                pageSize.AddToStagedCount();
                                break;
                            }
                        }
                    }

                    if (!sizeFound)
                    {
                        pageSizes.Add(new PageSizeCount(width, height));
                        pageSizes.Last().AddToStagedCount();
                    }
                }

                if (!bEndInError)
                {
                    totalPageCount += pdfPageCount;
                    foreach (PageSizeCount pageSizeCount in pageSizes)
                    {
                        pageSizeCount.CommitStagedCounts();
                    }
                }
                else
                {
                    for (int i = pageSizes.Count - 1; i >= 0; i--)
                    {
                        if (pageSizes[i].NumberOfPages == 0 && pageSizes[i].IsDefaultSize == false)
                        {
                            pageSizes.RemoveAt(i);
                        }
                        else
                        {
                            pageSizes[i].ClearStagedCounts();
                        }
                    }
                }
            }

            pageSizes.RemoveAll(t => t.NumberOfPages == 0);

            return pageSizes;
        }

        private List<PageSizeCount> defaultPageSizeList()
        {
            List<PageSizeCount> defaultPageSizes = new List<PageSizeCount>();
            defaultPageSizes.Add(new PageSizeCount(8.5f, 11f, true));
            defaultPageSizes.Add(new PageSizeCount(8.5f, 14f, true));
            defaultPageSizes.Add(new PageSizeCount(9f, 11f, true));
            defaultPageSizes.Add(new PageSizeCount(11f, 17f, true));
            defaultPageSizes.Add(new PageSizeCount(12f, 18f, true));
            defaultPageSizes.Add(new PageSizeCount(15f, 21f, true));
            defaultPageSizes.Add(new PageSizeCount(15f, 22f, true));
            defaultPageSizes.Add(new PageSizeCount(17f, 21f, true));
            defaultPageSizes.Add(new PageSizeCount(17f, 22f, true));
            defaultPageSizes.Add(new PageSizeCount(18f, 24f, true));
            defaultPageSizes.Add(new PageSizeCount(18f, 36f, true));
            defaultPageSizes.Add(new PageSizeCount(22f, 34f, true));
            defaultPageSizes.Add(new PageSizeCount(22f, 36f, true));
            defaultPageSizes.Add(new PageSizeCount(24f, 36f, true));
            defaultPageSizes.Add(new PageSizeCount(24f, 44f, true));
            defaultPageSizes.Add(new PageSizeCount(26f, 36f, true));
            defaultPageSizes.Add(new PageSizeCount(30f, 36f, true));
            defaultPageSizes.Add(new PageSizeCount(30f, 40f, true));
            defaultPageSizes.Add(new PageSizeCount(30f, 42f, true));
            defaultPageSizes.Add(new PageSizeCount(34f, 44f, true));
            defaultPageSizes.Add(new PageSizeCount(36f, 42f, true));
            defaultPageSizes.Add(new PageSizeCount(36f, 48f, true));

            return defaultPageSizes;
        }


        private IEnumerable<string> GetFileList(bool includeSubdirectories, bool includeNonPdfs)
        {
            string folderPath = rootFolderPath;
            string fileSearchPattern;
            if (includeNonPdfs)
                fileSearchPattern = "*";
            else
                fileSearchPattern = "*.pdf";

            Queue<string> pending = new Queue<string>();
            pending.Enqueue(folderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                folderPath = pending.Dequeue();
                try
                {
                    tmp = Directory.GetFiles(folderPath, fileSearchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                if (includeSubdirectories)
                { 
                tmp = Directory.GetDirectories(folderPath);
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        pending.Enqueue(tmp[i]);
                    }
                }
            }
        }

        private void generatePdfReport()
        {
            byte[] buffer = getMainReportAsByte();

            byte[] buffer2 = addHeaderToReport(buffer);

            writeReportToPdf(buffer2);
            
            System.Diagnostics.Process.Start(reportPath);
        }

        private byte[] getMainReportAsByte()
        {
            byte[] buffer;
            using (MemoryStream stream = new MemoryStream())
            {
                int totalPageCount = 0;
                foreach (PageSizeCount pageSizeCount in pageSizeCounts)
                {
                    totalPageCount += pageSizeCount.NumberOfPages;
                }

                // Must have write permissions to the path folder
                //var stream = new MemoryStream();
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf, new PageSize(612, 792));
                document.SetMargins(72, 36, 54, 36);

                // Header
                Paragraph header = new Paragraph("Page Size Report")
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(20);

                // New line
                Paragraph newline = new Paragraph(new Text("\n"));

                // document.Add(newline);
                document.Add(header);

                // Add sub-header
                Paragraph subheader = new Paragraph(rootFolderPath)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(15);
                document.Add(subheader);

                // Line separator
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                document.Add(newline);

                // Table
                Table table = new Table(2, false)
                    .SetWidth(288)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                List<Cell> cellsList = new List<Cell>();

                Cell cell11 = new Cell(1, 1)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph("Size"));
                cellsList.Add(cell11);

                Cell cell12 = new Cell(1, 1)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph("Quantity"));
                cellsList.Add(cell12);

                Cell cell21 = new Cell(1, 1)
                    .SetBackgroundColor(Color.MakeColor(ColorConstants.LIGHT_GRAY.GetColorSpace(), new float[] { 0.85f, 0.85f, 0.85f }))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph("TOTAL").SetBold());
                cellsList.Add(cell21);

                Cell cell22 = new Cell(1, 1)
                    .SetBackgroundColor(Color.MakeColor(ColorConstants.LIGHT_GRAY.GetColorSpace(), new float[] { 0.85f, 0.85f, 0.85f }))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph(totalPageCount.ToString()).SetBold());
                cellsList.Add(cell22);

                foreach (PageSizeCount pageSizeCount in pageSizeCounts)
                {
                    cellsList.Add(new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(pageSizeCount.SizeX + "\" x " + pageSizeCount.SizeY + "\"")));
                    cellsList.Add(new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(pageSizeCount.NumberOfPages.ToString())));
                }

                foreach (Cell cell in cellsList)
                {
                    table.AddCell(cell);
                }

                document.Add(table);

                /*
                // Hyper link
                Link link = new Link("click here",
                   PdfAction.CreateURI("https://www.google.com"));
                Paragraph hyperLink = new Paragraph("Please ")
                   .Add(link.SetBold().SetUnderline()
                   .SetItalic().SetFontColor(ColorConstants.BLUE))
                   .Add(" to go www.google.com.");

                document.Add(newline);
                document.Add(hyperLink);

                */

                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                // Header
                header = new Paragraph("Error Report")
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(20);

                document.Add(newline);
                document.Add(header);

                // Add sub-header
                subheader = new Paragraph("Files not included in page counts")
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(15);
                document.Add(subheader);

                // Line separator
                document.Add(ls);
                document.Add(newline);

                // Table
                Table errorTable = new Table(3, false)
                    .SetWidth(450)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                List<Cell> errorCellsList = new List<Cell>();

                Cell errorCell11 = new Cell(1, 1)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph("File Name"));
                errorCellsList.Add(errorCell11);

                Cell errorCell12 = new Cell(1, 1)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph("Path"));
                errorCellsList.Add(errorCell12);

                Cell errorCell13 = new Cell(1, 1)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph("Error Message"));
                errorCellsList.Add(errorCell13);

                foreach (PageSizeCountError pageSizeCountError in pageSizeCountErrors)
                {
                    errorCellsList.Add(new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(pageSizeCountError.FileName).SetFontSize(9)));
                    errorCellsList.Add(new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(pageSizeCountError.FilePath).SetFontSize(9)));
                    errorCellsList.Add(new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(pageSizeCountError.ErrorMessage).SetFontSize(9)));
                }
                
                foreach (Cell cell in errorCellsList)
                {
                    errorTable.AddCell(cell);
                }

                document.Add(errorTable);
                
                document.Close();

                buffer = stream.ToArray();
            }

            return buffer;
        }

        private byte[] addHeaderToReport(byte[] buffer)
        {
            byte[] outBuffer;

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (MemoryStream outMemoryStream = new MemoryStream())
                {


                    PdfReader reader = new PdfReader(memoryStream);
                    PdfWriter writer = new PdfWriter(outMemoryStream);
                    PdfDocument pdf = new PdfDocument(reader, writer);
                    Document document = new Document(pdf, new PageSize(612, 792));


                    // Page numbers
                    int n = pdf.GetNumberOfPages();
                    for (int i = 1; i <= n; i++)
                    {
                        document.ShowTextAligned(new Paragraph(String
                            .Format("Page " + i + " of " + n)),
                            565, 760, i, TextAlignment.RIGHT,
                            VerticalAlignment.TOP, 0);

                        iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory
                            .Create(@"../../Images/Logo/Logo.png"))
                            .SetTextAlignment(TextAlignment.CENTER).SetHeight(36).SetWidth(36).SetFixedPosition(i, 40, 735);
                        document.Add(img);

                        document.ShowTextAligned(new Paragraph(String
                            .Format("Hyperdrive PDF")),
                            80, 765, i, TextAlignment.LEFT,
                            VerticalAlignment.TOP, 0);
                    }

                    document.Close();

                    outBuffer = outMemoryStream.ToArray();
                }
            }

            return outBuffer;
        }

        private bool writeReportToPdf(byte[] buffer)
        {
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                PdfReader reader = new PdfReader(memoryStream);
                PdfWriter writer = new PdfWriter(reportPath);
                PdfDocument pdf = new PdfDocument(reader, writer);
                Document document = new Document(pdf, new PageSize(612, 792));


                // Page numbers
                int n = pdf.GetNumberOfPages();
                for (int i = 1; i <= n; i++)
                {
                    document.ShowTextAligned(new Paragraph(String
                        .Format("Page " + i + " of " + n)),
                        565, 760, i, TextAlignment.RIGHT,
                        VerticalAlignment.TOP, 0);

                    iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory
                        .Create(@"../../Images/Logo/Logo.png"))
                        .SetTextAlignment(TextAlignment.CENTER).SetHeight(36).SetWidth(36).SetFixedPosition(i, 40, 735);
                    document.Add(img);

                    document.ShowTextAligned(new Paragraph(String
                        .Format("Hyperdrive PDF")),
                        80, 765, i, TextAlignment.LEFT,
                        VerticalAlignment.TOP, 0);
                }

                document.Close();

            }

            return true;
        }

        public void Cancel()
        {
            cts.Cancel();
        }
    }
}
