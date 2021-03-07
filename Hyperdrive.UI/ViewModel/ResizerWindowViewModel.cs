using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Hyperdrive.Core;
using Hyperdrive.Core.Resizer;
using Hyperdrive.Core.Utils;

namespace Hyperdrive.UI.ViewModel
{
    class ResizerWindowViewModel : BaseViewModel
    {
        #region Private Members

        private Window _window;

        private PagesToScaleType _pagesToScaleType = PagesToScaleType.ALL;

        private Vector _pagesToScale;

        private ScaleType _scaleType = ScaleType.FIT;

        private PageSizeType _pageSizeType = PageSizeType.CUSTOM;

        private float _pageWidth = 0;

        private float _pageHeight = 0;

        private float _scale = 100;

        private String _scaleString = "100";

        private float _contentHeight = 0;

        private float _contentWidth = 0;

        private float _longEdgeSize = 0;

        private float _shortEdgeSize = 0;

        private PageAlignment _pageAlignment = PageAlignment.CENTER;

        private RotationType _rotationType = RotationType.NONE;

        private float _xOffset = 0;

        private float _yOffset = 0;

        #endregion

        #region

        public PagesToScaleType pagesToScaleType { get { return _pagesToScaleType; } set { _pagesToScaleType = value; } }

        public ScaleType scaleType { get { return _scaleType; } set { _scaleType = value; } }

        public PageSizeType pageSizeType
        {
            get
            {
                if (_scaleType == ScaleType.FIT || _scaleType == ScaleType.SCALE_DOWN)
                {
                    pageSizeType = PageSizeType.CUSTOM;
                }
                return _pageSizeType;
            }
            set
            {
                _pageSizeType = value;
                OnPropertyChanged(nameof(pageSizeType));
            }
        }

        public bool isPageSizeTypeEnabled
        {
            get
            {
                if (scaleType == ScaleType.FIT || scaleType == ScaleType.SCALE_DOWN)
                {
                    return false;
                }
                return true;
            }
        }

        public bool isCustomScaleEnabled
        {
            get
            {
                if (scaleType != ScaleType.FIT)
                    return true;
                else
                    return false; 
            }
        }

        public bool isPageSizeCustom { get { return pageSizeType == PageSizeType.CUSTOM; } }

        public String scaleString { get { return _scaleString; } set { _scaleString = value; } }

        #endregion

        public ICommand ScaleAllCommand { get; set; }

        public ICommand ScaleCurrentCommand { get; set; }

        public ICommand ScaleSelectedCommand { get; set; }

        public ICommand ScaleRangeCommand { get; set; }

        public ICommand ScaleFitCommand { get; set; }

        public ICommand ScaleExactCommand { get; set; }

        public ICommand ScaleDownCommand { get; set; }

        public ICommand ScaleHeightWidthCommand { get; set; }


        public ResizerWindowViewModel(Window window)
        {
            _window = window;

            ScaleAllCommand = new RelayCommand(() => pagesToScaleType = PagesToScaleType.ALL);
            ScaleCurrentCommand = new RelayCommand(() => pagesToScaleType = PagesToScaleType.CURRENT);
            ScaleSelectedCommand = new RelayCommand(() => pagesToScaleType = PagesToScaleType.SELECTED);
            ScaleRangeCommand = new RelayCommand(() => pagesToScaleType = PagesToScaleType.RANGE);

            ScaleFitCommand = new RelayCommand(() => scaleType = ScaleType.FIT);
            ScaleExactCommand = new RelayCommand(() => scaleType = ScaleType.EXACT);
            ScaleDownCommand = new RelayCommand(() => scaleType = ScaleType.SCALE_DOWN);
            ScaleHeightWidthCommand = new RelayCommand(() => scaleType = ScaleType.HEIGHT_OR_WIDTH);

            
        }
    }
}
