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

/// <summary>
/// The name space for all UI related tasks
/// </summary>
namespace Hyperdrive.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MoonPdfPanel MoonPdfPanel { get { return this.moonPdfPanel; } }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new WindowViewModel(this);
        }
    }
}
