<h2 align="left">HyperdrivePDF</h2>

A WPF application for counting the number of PDF page sizes and for processing PDFs for printing. Utilizes the iText7 library for PDF manipulation, generation, and gathering statistics.

### Features

* <b>PDF Viewer</b> (Fix for memory leak with large PDF files has not been committed yet).
* <b>Page Size Toolbar</b> - Counts the different page sizes in the opened PDF.
* <b>Directory Page Size Counter</b> - Generates a PDF report of pages sizes for all PDFs in a folder. Includes error report for invalid PDFs and non-PDF files.

### Features in Development

* <b>Page Resizer</b> - Resize pages to a particular page size and scale.
* <b>Letter / Tabloid Resizer</b> - Force all pages to be either 8.5"x11 or 11x17. Used for RFQs and RPFs.
* <b>Step-And-Repeat / N-Upping</b> - Place multiple PDF pages or multiple copies of the same PDF page on a single page.
* <b>Color Detection</b> - Detect whether a page has any color elements on it. Most commercial PDF applications return false positives when it comes to detecting color, especially when the page contains a grayscale image.
* <b>Combine PDFs</b> - Combine multiple PDFs as one and combine a folder of PDFs alphabetically by subdirectory.


### Main Window

<p>
  <img width="600" align='center' src="https://raw.githubusercontent.com/dougvanzee/HyperdrivePdf/master/Hyperdrive.Core/Images/Readme/MainWindow.png?raw=true">
</p>

### Page Count Toolbar

<p>
  <img width="600" align='center' src="https://raw.githubusercontent.com/dougvanzee/HyperdrivePdf/master/Hyperdrive.Core/Images/Readme/PageCountToolbar.PNG?raw=true">
</p>

### Directory Page Count Menu

<p>
  <img width="600" align='center' src="https://raw.githubusercontent.com/dougvanzee/HyperdrivePdf/master/Hyperdrive.Core/Images/Readme/DirectoryPageCountMenu.PNG?raw=true">
</p>

### Directory Page Count Report

<table border="0">
 <tr>
    <td><img align='center' src="https://raw.githubusercontent.com/dougvanzee/HyperdrivePdf/master/Hyperdrive.Core/Images/Readme/DirectoryPageCountReport.PNG?raw=true"></td>
    <td><img align='center' src="https://raw.githubusercontent.com/dougvanzee/HyperdrivePdf/master/Hyperdrive.Core/Images/Readme/DirectoryPageCountErrorReport.PNG?raw=true"></td>
 </tr>
</table>
