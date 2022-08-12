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
using AutoUpdaterDotNET;

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

            AutoUpdater.Start("https://displace.international/HyperdrivePDF/LatestRelease.xml");

            licenseUtil = new LicenseUtil();
            licenseUtil.LicenseInfoComplete += LicenseInfoComplete;

            licenseUtil.StartLicenseCheck();
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

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
