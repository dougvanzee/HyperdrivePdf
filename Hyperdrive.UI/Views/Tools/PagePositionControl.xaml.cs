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
using Hyperdrive.Core.Utils;
using Hyperdrive.Core.Resizer;

namespace Hyperdrive.UI.Views.Tools
{
    /// <summary>
    /// Interaction logic for PagePositionControl.xaml
    /// </summary>
    public partial class PagePositionControl : UserControl
    {
        private PageAlignment pageAlignment = PageAlignment.CENTER;

        public PagePositionControl()
        {
            InitializeComponent();
        }

        public PageAlignment GetPageAlignment { get { return pageAlignment; } }

        private void TopLeft_Checked(object sender, RoutedEventArgs e)
        {
            pageAlignment = PageAlignment.TOP_LEFT;
        }

        private void Top_Checked(object sender, RoutedEventArgs e)
        {
            pageAlignment = PageAlignment.TOP;
        }

        private void TopRight_Checked(object sender, RoutedEventArgs e)
        {
            pageAlignment = PageAlignment.TOP_RIGHT;
        }

        private void Left_Checked(object sender, RoutedEventArgs e)
        {
            pageAlignment = PageAlignment.LEFT;
        }

        private void Center_Checked(object sender, RoutedEventArgs e)
        {
            pageAlignment = PageAlignment.CENTER;
        }

        private void Right_Checked(object sender, RoutedEventArgs e)
        {
            pageAlignment = PageAlignment.RIGHT;
        }

        private void BottomLeft_Checked(object sender, RoutedEventArgs e)
        {
            pageAlignment = PageAlignment.BOTTOM_LEFT;
        }

        private void Bottom_Checked(object sender, RoutedEventArgs e)
        {
            pageAlignment = PageAlignment.BOTTOM;
        }

        private void BottomRight_Checked(object sender, RoutedEventArgs e)
        {
            pageAlignment = PageAlignment.BOTTOM_RIGHT;
        }
    }
}
