using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using PropertyChanged;
using System.ComponentModel;
using System.Diagnostics;

public enum SideToolbarEnum
{
    None,
    PageCount,
    PageColorCount
}

 namespace Hyperdrive.UI.Views
{
    /// <summary>
    /// Interaction logic for SideToolbar.xaml
    /// </summary>
    public partial class SideToolbar : UserControl, INotifyPropertyChanged
    {
        public Window parentWindow = Application.Current.MainWindow;

        private SideToolbarEnum currentSideToolbar = SideToolbarEnum.None;

        private String _currentToolbarName = "Page Color Counts";

        public int SwitchView { get; set; } = 1;

        public String currentToolbarName { get { return _currentToolbarName; } }

        public SideToolbar()
        {
            InitializeComponent();
            
        }



        // Opens and closes the sidetoolbar
        public void SwitchToolbar(SideToolbarEnum SideToolbar)
        {
            if (SideToolbar == SideToolbarEnum.None)
            {
                // Collapse sidetoolbar (Might be redundant, possibly removable)
                currentSideToolbar = SideToolbarEnum.None;
            }
            else if (currentSideToolbar == SideToolbarEnum.None)
            {
                // If sidebar closed, open sidetoolbar that was clicked
                currentSideToolbar = SideToolbar;
            }
            else if (currentSideToolbar != SideToolbarEnum.None && SideToolbar != currentSideToolbar)
            {
                // If sidetoolbar open, switch to different toolbar
                currentSideToolbar = SideToolbar;
            }
            else
            {
                // Close sidetoolbar that was open
                currentSideToolbar = SideToolbarEnum.None;
            }

            OnPropertyChanged(nameof(currentSideToolbar));
            OnPropertyChanged(nameof(CurrentToolbarIndex));
            OnPropertyChanged(nameof(bPageCountOpen));
            OnPropertyChanged(nameof(bPageColorCountOpen));
            OnPropertyChanged(nameof(currentToolbarName));
        }

        public int CurrentToolbarIndex
        {
            get
            {
                switch (currentSideToolbar)
                {
                    case SideToolbarEnum.None:
                        return 0;

                    case SideToolbarEnum.PageCount:
                        _currentToolbarName = "Page Counts";
                        return 1;

                    case SideToolbarEnum.PageColorCount:
                        _currentToolbarName = "Page Color Counts";
                        return 2;
                }
                return 0;
            }
        }


        public bool bPageCountOpen { get { return currentSideToolbar == SideToolbarEnum.PageCount ? true : false; } }

        public bool bPageColorCountOpen { get { return currentSideToolbar == SideToolbarEnum.PageColorCount ? true : false; } }

       


        #region Button Clicks 

        private void ExitSideToolbar_Click(object sender, RoutedEventArgs e)
        {
            SwitchToolbar(SideToolbarEnum.None);
        }

        private void PageCount_Click(object sender, RoutedEventArgs e)
        {
            SwitchToolbar(SideToolbarEnum.PageCount);
        }

        private void PageColorCount_Click(object sender, RoutedEventArgs e)
        {
            SwitchToolbar(SideToolbarEnum.PageColorCount);
        }

        #endregion

        #region IPropertyNotify

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }

}
