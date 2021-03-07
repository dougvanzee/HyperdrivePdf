using Hyperdrive.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hyperdrive.UI.Views.Tools
{
    /// <summary>
    /// Interaction logic for ResizerWindow.xaml
    /// </summary>
    public partial class ResizerWindow : Window
    {
        public ResizerWindow()
        {
            InitializeComponent();

            this.DataContext = new ResizerWindowViewModel(this);
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        static App application = Application.Current as App;
        public bool IsAppActive {  get { return application.IsActive; } }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            var dict = new ResourceDictionary();
            dict.Source = new Uri("/Styles/Colors.xaml", UriKind.RelativeOrAbsolute);
            var brush = (SolidColorBrush)dict["BlueHighlightBrush"];

            this.Resources["WindowBorderBrush"] = brush;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            var dict = new ResourceDictionary();
            dict.Source = new Uri("/Styles/Colors.xaml", UriKind.RelativeOrAbsolute);
            var brush = (SolidColorBrush)dict["MainBorderBrush"];

            this.Resources["WindowBorderBrush"] = brush;
        }
    }
}
