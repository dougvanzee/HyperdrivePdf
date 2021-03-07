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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hyperdrive.Core.Stats.PageSizeCount;

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
            string FileName = System.IO.Path.GetFileNameWithoutExtension(((WindowViewModel)(this.DataContext)).FilePath) + "_1.pdf";

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "PDF (*.pdf)|*.pdf";

            // Feed the dummy name to the save dialog
            sf.FileName = FileName;

            if (sf.ShowDialog() == true)
            {
                // Now here's our save folder
                ((WindowViewModel)(this.DataContext)).FileOutPath = sf.FileName;

                ((WindowViewModel)(this.DataContext)).BusinessCard8Up();
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

        private void PageSizesInFolder_Click(object sender, RoutedEventArgs e)
        {
            //CounterDirectoryWindow counterDirectoryWindow = new CounterDirectoryWindow();
            //counterDirectoryWindow.ShowDialog();

            CounterDirectory counterDirectory = new CounterDirectory("C:/Users/thevfxguy13/Downloads/Resized");
            counterDirectory.PrintPageSizeCounts();
            LoadingScreenWindow loadingScreenWindow = new LoadingScreenWindow(this.parentWindow, counterDirectory);
            loadingScreenWindow.Show();
        }
    }
}
