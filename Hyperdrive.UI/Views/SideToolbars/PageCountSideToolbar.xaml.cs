using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Hyperdrive.UI.Views.SideToolbars
{
    /// <summary>
    /// Interaction logic for PageCountSideToolbar.xaml
    /// </summary>
    public partial class PageCountSideToolbar : UserControl, INotifyPropertyChanged
    {
        public PageCountSideToolbar()
        {
            InitializeComponent();

            var pd = DependencyPropertyDescriptor.FromProperty(UIElement.IsEnabledProperty, typeof(PageCountToolbar));
            pd.AddValueChanged(this, OnIsEnabledChanged);
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            OnIsEnabledChanged(this, new PropertyChangedEventArgs(nameof(OnIsEnabledChanged)));
        }

        public void OnIsEnabledChanged(object sender, EventArgs e)
        {
            if (((UserControl)sender).IsEnabled)
            {
                EnableToolbar();
            }
            else
            {
                DisableToolbar();
            }

        }
        public void EnableToolbar()
        {

        }

        public void DisableToolbar()
        {
            ItemsControl itemsControl = (ItemsControl)this.FindName("PageCountsContainer");
            if (itemsControl != null)
                itemsControl.Items.Clear();
        }

        #region IPropertyNotify

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
