using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Hyperdrive.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        private bool m_isActive;
        public bool IsActive
        {
            get { return m_isActive; }
            private set
            {
                m_isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Activated += (object sender, EventArgs ea) =>
            {
                IsActive = true;
            };
            Deactivated += (object sender, EventArgs ea) =>
            {
                IsActive = false;
            };
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
