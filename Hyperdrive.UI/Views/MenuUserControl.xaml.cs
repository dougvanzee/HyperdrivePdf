using Hyperdrive.UI.ViewModel;
using Hyperdrive.UI.Views.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Hyperdrive.Core.StepAndRepeat;
using iText.Kernel.Pdf;
using iText.Forms;
using Hyperdrive.Core.Utils;
using System.IO;
// using Windows.UI.Xaml;

namespace Hyperdrive.UI.Views
{
    /// <summary>
    /// Interaction logic for MenuUserControl.xaml
    /// </summary>
    public partial class MenuUserControl : UserControl
    {
        public Window parentWindow;

        public MenuUserControl()
        {
            InitializeComponent();

            // parentWindow = Window.GetWindow(this);
            parentWindow = Application.Current.MainWindow;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            MenuItem mi = btn.Parent as MenuItem;
            if (mi != null)
                mi.IsSubmenuOpen = !mi.IsSubmenuOpen;
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                //fileOpen = true;
                //MenuFileOpen.IsEnabled = false;
                //MenuCloseFile.IsEnabled = true;
                //filePath = openFileDialog.FileName;
                ((WindowViewModel)(this.DataContext)).FilePath = openFileDialog.FileName;
            }
        }

        private void BusinessCard8Up_Click(object sender, RoutedEventArgs e)
        {
            // Prepare a dummy string, this would appear in the dialog
            string FileName = System.IO.Path.GetFileNameWithoutExtension(((WindowViewModel)(this.DataContext)).FilePath) + "_8up.pdf";
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "PDF (*.pdf)|*.pdf";

            // Feed the dummy name to the save dialog
            sf.FileName = FileName;

            if (sf.ShowDialog() == true)
            {
                // Now here's our save folder
                // ((WindowViewModel)(this.DataContext)).FileOutPath = sf.FileName;

                if (sf.FileName == ((WindowViewModel)(this.DataContext)).FilePath)
                {
                    MessageBox.Show("Destination file cannot be same as source file.",
                                          "Confirmation",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Warning);
                    return;
                }

                bool bResult = StepAndRepeatManager.BusinessCard8Up(((WindowViewModel)(this.DataContext)).FilePath, sf.FileName);

                if (!bResult)
                {
                    MessageBox.Show("An unknown error occurred. Make sure the destination PDF is not open.",
                      "Confirmation",
                      MessageBoxButton.OK,
                      MessageBoxImage.Error);
                    return;
                }

                ((WindowViewModel)(this.DataContext)).FilePath = sf.FileName;
            }
        }

        private void BusinessCard9Up_Click(object sender, RoutedEventArgs e)
        {
            // Prepare a dummy string, this would appear in the dialog
            string FileName = System.IO.Path.GetFileNameWithoutExtension(((WindowViewModel)(this.DataContext)).FilePath) + "_9up.pdf";
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "PDF (*.pdf)|*.pdf";

            // Feed the dummy name to the save dialog
            sf.FileName = FileName;

            if (sf.ShowDialog() == true)
            {
                // Now here's our save folder
                // ((WindowViewModel)(this.DataContext)).FileOutPath = sf.FileName;

                if (sf.FileName == ((WindowViewModel)(this.DataContext)).FilePath)
                {
                    MessageBox.Show("Destination file cannot be same as source file.",
                                          "Confirmation",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Warning);
                    return;
                }

                bool bResult = StepAndRepeatManager.BusinessCard9Up(((WindowViewModel)(this.DataContext)).FilePath, sf.FileName);

                if (!bResult)
                {
                    MessageBox.Show("An unknown error occurred. Make sure the destination PDF is not open.",
                      "Confirmation",
                      MessageBoxButton.OK,
                      MessageBoxImage.Error);
                    return;
                }

                ((WindowViewModel)(this.DataContext)).FilePath = sf.FileName;
            }
        }

        private void SaddleStitchFullBleed(object sender, RoutedEventArgs e)
        {
            // Prepare a dummy string, this would appear in the dialog
            string FileName = System.IO.Path.GetFileNameWithoutExtension(((WindowViewModel)(this.DataContext)).FilePath) + "_SaddleStitch.pdf";
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "PDF (*.pdf)|*.pdf";

            // Feed the dummy name to the save dialog
            sf.FileName = FileName;

            if (sf.ShowDialog() == true)
            {
                // Now here's our save folder
                // ((WindowViewModel)(this.DataContext)).FileOutPath = sf.FileName;

                if (sf.FileName == ((WindowViewModel)(this.DataContext)).FilePath)
                {
                    MessageBox.Show("Destination file cannot be same as source file.",
                                          "Confirmation",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Warning);
                    return;
                }

                bool bResult = StepAndRepeatManager.SaddleStitchFullBleed(((WindowViewModel)(this.DataContext)).FilePath, sf.FileName);

                if (!bResult)
                {
                    MessageBox.Show("An unknown error occurred. Make sure the destination PDF is not open.",
                      "Confirmation",
                      MessageBoxButton.OK,
                      MessageBoxImage.Error);
                    return;
                }

                ((WindowViewModel)(this.DataContext)).FilePath = sf.FileName;
            }
        }

        private void ScaleDownTest_Click(object sender, RoutedEventArgs e)
        {
            // Prepare a dummy string, this would appear in the dialog
            string FileName = System.IO.Path.GetFileNameWithoutExtension(((WindowViewModel)(this.DataContext)).FilePath) + "_1.pdf";

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "PDF (*.pdf)|*.pdf";

            // Feed the dummy name to the save dialog
            sf.FileName = FileName;

            if (sf.ShowDialog() == true)
            {
                // Now here's our save folder
                ((WindowViewModel)(this.DataContext)).FileOutPath = sf.FileName;

                ((WindowViewModel)(this.DataContext)).ScaleDownTest();
            }
        }

        private void Resizer_Click(object sender, RoutedEventArgs e)
        {
            ResizerWindow resizerWindow = new ResizerWindow();
            resizerWindow.Owner = parentWindow;
            resizerWindow.ShowDialog();
        }

        private void InsertBlank_Click(object sender, RoutedEventArgs e)
        {
            string src = ((WindowViewModel)(this.DataContext)).FilePath;
            string dest = ((WindowViewModel)(this.DataContext)).FilePath + "_blank.pdf";
            byte[] blankPage = BlankPage.GetBlankPage(612, 792);
            using (MemoryStream memoryStream = new MemoryStream(blankPage))
            {
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
                PdfDocument cover = new PdfDocument(new PdfReader(memoryStream));
                cover.CopyPagesTo(1, 1, pdfDoc, 5, new PdfPageFormCopier());

                cover.Close();
                pdfDoc.Close();
            }
        }

        private void InsertIntentionallyBlank_Click(object sender, RoutedEventArgs e)
        {
            string src = ((WindowViewModel)(this.DataContext)).FilePath;
            string dest = ((WindowViewModel)(this.DataContext)).FilePath + "_intentionally.pdf";
            byte[] blankPage = BlankPage.GetIntentionallyLeftBlankPage(612, 792);
            using (MemoryStream memoryStream = new MemoryStream(blankPage))
            {
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
                PdfDocument cover = new PdfDocument(new PdfReader(memoryStream));
                cover.CopyPagesTo(1, 1, pdfDoc, 5, new PdfPageFormCopier());

                cover.Close();
                pdfDoc.Close();
            }
        }

        private void PageSizesInFolder_Click(object sender, RoutedEventArgs e)
        {
            CounterDirectoryWindow counterWindow = new CounterDirectoryWindow(parentWindow);
            counterWindow.ShowDialog();

            /*
            CounterDirectory counterDirectory = new CounterDirectory("C:/Users/thevfxguy13/Downloads/Resized");
            counterDirectory.PrintPageSizeCounts();
            LoadingScreenWindow loadingScreenWindow = new LoadingScreenWindow(this.parentWindow, counterDirectory);
            loadingScreenWindow.Show();
            */
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Doug Van Zee\n\nwww.dougvanzee.com\nwww.displacemedia.com\n\ndmvanzee@gmail.com",
                      "About HyperdrivePdf                                                                 ",
                      MessageBoxButton.OK);
        }

        private void LicenseAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This software has been provided on a trial basis and the license can be revoked at anytime without notice. This product is currently in early development so not all features are guaranteed to work as expected.",
                        "About HyperdrivePdf License                                                         ",
                        MessageBoxButton.OK);
        }
    }
}
