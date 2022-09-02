using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Microsoft.WindowsAPICodePack;
using Hyperdrive.UI.ViewModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Runtime.CompilerServices;
using MoonPdfLib;
using System.Windows.Controls.Primitives;
using System.Windows.Automation;
using Hyperdrive.Core.Security;
using System.Diagnostics;
using Hyperdrive.Core.Utils;
using Microsoft.Win32;
using Hyperdrive.Core.StepAndRepeat;
using Hyperdrive.UI.Views;
using Hyperdrive.Core.License.Utils;
using Hyperdrive.Core.License;

/// <summary>
/// The name space for all UI related tasks
/// </summary>
namespace Hyperdrive.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        internal MoonPdfPanel MoonPdfPanel { get { return this.moonPdfPanel; } }
        private LicenseUtil licenseUtil;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new WindowViewModel(this);

            licenseUtil = new LicenseUtil();
            licenseUtil.LicenseInfoComplete += LicenseInfoComplete;

            licenseUtil.StartLicenseCheck();

            // DBConnection.TestConnection3();

            // PasswordResetter.SendPasswordResetEmail("dmvanzee@gmail.com");
            PasswordResetter passwordResetter = new PasswordResetter();
            //passwordResetter.SendEmail3();

            Console.WriteLine(ResetCodeGenerator.GetCode());
        }

        private void LicenseInfoComplete(object sender, EventArgs e)
        {
            if (licenseUtil.LicenseActive == false)
            {
                switch(licenseUtil.DaysLeftInLicense)
                {
                    case -2:
                        MessageBox.Show("This trial license has expired. Please email dmvanzee@gmail.com for a license.", "HyperdrivePDF License", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    case -1:
                        MessageBox.Show("An active license could not be found. Please make sure you are connected to the internet", "HyperdrivePDF License", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    case 0:
                        MessageBox.Show("There are 0 more uses in your license. Please make sure you have an active internet connection\n\nIf you continue having issues, please email dmvanzee@gmail.com for licensing support.", "HyperdrivePDF License", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    default:
                        MessageBox.Show("There are " + licenseUtil.DaysLeftInLicense + " more uses in your license. Please make sure you have an active internet connection\n\nIf you continue having issues, please email dmvanzee@gmail.com for licensing support.", "HyperdrivePDF License", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                }
                   
                if (licenseUtil.DaysLeftInLicense <= 0)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Close();
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.SetGlobalDisable(false);
                    });
                }
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.SetGlobalDisable(false);
                });
            }
        }

        public void SetGlobalDisable(bool value)
        {
            ((WindowViewModel)(DataContext)).GlobalDisable = value;
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
        private void PageSizesInFolder_Click(object sender, RoutedEventArgs e)
        {
            CounterDirectoryWindow counterWindow = new CounterDirectoryWindow(this);
            counterWindow.ShowDialog();
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

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
