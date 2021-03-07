using Hyperdrive.UI.ViewModel;
using Hyperdrive.Core.Utils;
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
    public partial class LoadingScreenWindow : Window, INotifyPropertyChanged
    {
        private CounterDirectory counterDirectory;
        public int currentProgress { get; set; } = 0;

        public int maxProgress { get; set; } = 1;

        public Visibility textVisibility { get; set; } = Visibility.Visible;

        public string windowTitle { get; set; } = "Counter Directory";

        public string progressAsString { get { return currentProgress + " of " + maxProgress; } }


        public LoadingScreenWindow(Window owner, CounterDirectory counterDirectory)
        {
            Owner = owner;
            InitializeComponent();
            DataContext = this;
            this.counterDirectory = counterDirectory;

            startTimer();
        }

        private void startTimer()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 100;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            maxProgress = counterDirectory.getNumberOfPdfs();
            currentProgress = counterDirectory.getCurrentProgress();
            OnPropertyChanged(nameof(maxProgress));
            OnPropertyChanged(nameof(currentProgress));
            OnPropertyChanged(nameof(progressAsString));
            
            if (counterDirectory.getIsDone())
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    //exit();
                }));
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void exit()
        {
            cancelCount();
            this.Close();
        }

        private void cancelCount()
        {
            counterDirectory.Cancel();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            cancelCount();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            exit();
        }
    }
}
