using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hyperdrive.UI.ViewModel
{
    class LoadingScreenWindowViewModel : BaseViewModel
    {
        Window _window;

        private int _numberOfPdfs = 0;

        private int _currentProgress = 0;

        public int currentProgress { get { return _currentProgress; } set { _currentProgress = value; } }

        public int numberOfPdfs
        {
            get
            {
                if (_numberOfPdfs == 0)
                    return 1;
                else
                    return _numberOfPdfs;
            }
            set
            {
                _numberOfPdfs = value;
            }
        }

        public string currentProgressAsString { get { return _currentProgress + " of " + _numberOfPdfs; } }

        public Visibility progressTextVisibility
        {
            get
            {
                if (_numberOfPdfs == 0)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

        public LoadingScreenWindowViewModel(Window window)
        {
            _window = window;
        }
    }
}
