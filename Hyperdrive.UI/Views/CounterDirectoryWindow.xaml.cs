using Hyperdrive.Core.Stats.PageSizeCount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hyperdrive.UI.Views
{
    /// <summary>
    /// Interaction logic for CounterDirectoryWindow.xaml
    /// </summary>
    public partial class CounterDirectoryWindow : Window, INotifyPropertyChanged
    {
        public CounterDirectoryWindow(Window owner)
        {
            Owner = owner;
            InitializeComponent();
            DataContext = this;
        }

        public string FolderPath { get; set; }

        public bool IncludeSubdirectories { get; set; } = true;

        public bool IncludeNonPdfs { get; set; } = false;

        public string ReportPath { get; set; }

        public bool GenerateReportButtonEnabled { get { return !String.IsNullOrEmpty(FolderPath) && !String.IsNullOrEmpty(ReportPath); } }

        private void BrowseFolderPath_Click(object sender, RoutedEventArgs e)
        {
            
            var dlg = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dlg.Title = "Select Folder";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                FolderPath = dlg.FileName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void BrowseReportPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "PageSizeReport"; // Default file name
            dlg.DefaultExt = ".pdf"; // Default file extension
            dlg.Filter = "Text documents (.pdf)|*.pdf"; // Filter files by extension
            dlg.RestoreDirectory = true;

            if (!String.IsNullOrEmpty(FolderPath))
                dlg.InitialDirectory = FolderPath;

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                ReportPath = dlg.FileName;
            }
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            CounterDirectory counterDirectory = new CounterDirectory(FolderPath, ReportPath);
            counterDirectory.PrintPageSizeCounts();
            LoadingScreenWindow loadingScreenWindow = new LoadingScreenWindow(this, counterDirectory);
            loadingScreenWindow.Show();
        }
    }
}
