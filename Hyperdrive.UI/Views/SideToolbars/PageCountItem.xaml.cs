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
using System.ComponentModel;
using System.Drawing;

namespace Hyperdrive.UI.Views.SideToolbars
{
    /// <summary>
    /// Interaction logic for PageCountItem.xaml
    /// </summary>
    public partial class PageCountItem : UserControl, INotifyPropertyChanged
    {

        public PageCountItem()
        {
            InitializeComponent();
        }

        public PageCountItem(float width = 8.5f, float height = 11f, int count = 0)
        {
            InitializeComponent();

            pageCount = count;
            sizeName = width + "x" + height;

            OnPropertyChanged(nameof(pageCount));
            OnPropertyChanged(nameof(sizeName));
        }

        public PageCountItem(string name, int totalPageCount)
        {
            InitializeComponent();

            pageCount = totalPageCount;
            sizeName = name;

            changeTextToBold();

            OnPropertyChanged(nameof(pageCount));
            OnPropertyChanged(nameof(sizeName));
        }

        
        public int pageCount { get; set; } = 0;

        public string sizeName { get; set; }

        private void changeTextToBold()
        {
            TextBlock nameText = (TextBlock)this.FindName("nameText");
            if (nameText != null)
                nameText.FontWeight = FontWeights.Bold;

            TextBlock countText = (TextBlock)this.FindName("countText");
            if (countText != null)
                countText.FontWeight = FontWeights.Bold;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
