using Hyperdrive.UI.ViewModel;
using Hyperdrive.Core.Utils;
using Hyperdrive.Core.Interfaces;
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
using System.Windows.Shapes;
using Hyperdrive.Core.Stats.PageSizeCount;
using System.Timers;
using System.ComponentModel;

namespace Hyperdrive.UI.Views
{
    /// <summary>
    /// Interaction logic for LoadingScreenWindow.xaml
    /// </summary>
    public partial class ProgressBarWindow : Window, INotifyPropertyChanged
    {
        private IProgressBarWindow LinkedProcess;

        public int CurrentProgress { get; set; } = 0;
        public int MaxProgress { get; set; } = 1;
        public string Progress { get { return CurrentProgress + " of " + MaxProgress; } }
        public Visibility ProgressVisibility { get; set; } = Visibility.Visible;
        public string WindowTitle { get; set; } = "";
        public string StatusText { get; set; } = "";
        public Visibility StatusVisibility { get; set; } = Visibility.Visible;


        public ProgressBarWindow(Window owner, IProgressBarWindow linkedProcess, string windowTitle = "", int maxProgress = 1, string status = "")
        {
            InitializeComponent();
            DataContext = this;
            Owner = owner;
            LinkedProcess = linkedProcess;
            WindowTitle = windowTitle;
            MaxProgress = maxProgress;
            StatusText = status;
            LinkedProcess.CurrentProgressChanged += CurrentProgressChanged;
            LinkedProcess.MaxProgressChanged += MaxProgressChanged;
            LinkedProcess.ProgressCompelete += ProgressComplete;
            LinkedProcess.StatusChanged += StatusChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        void CurrentProgressChanged(object sender, EventArgs e)
        {
            CurrentProgress = LinkedProcess.LoadingScreenCurrentProgress();
        }

        void MaxProgressChanged(object sender, EventArgs e)
        {
            MaxProgress = LinkedProcess.LoadingScreenMaxProgress();
        }

        void StatusChanged(object sender, EventArgs e)
        {
            StatusText = LinkedProcess.LoadingScreenStatusText();
        }

        void ProgressComplete(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
        }

        private void exit()
        {
            CancelLinkedProcess();
            Owner.Dispatcher.Invoke(() =>
            {
                Owner.Close();
            });
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
        }

        private void CancelLinkedProcess()
        {
            LinkedProcess.LoadingScreenCancel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            exit();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                CancelLinkedProcess();
            }
            catch
            {

            }
            base.OnClosing(e);
            if (null != Owner)
            {
                Owner.Activate();
            }
        }
    }
}
