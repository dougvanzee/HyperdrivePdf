using System;
using IOPath = System.IO.Path;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;
using Hyperdrive.Core.StepAndRepeat;
using Hyperdrive.Core.Utils;
using Hyperdrive.Core.Resizer;
using iText.Kernel.Pdf;
using MoonPdfLib;
using MoonPdfLib.MuPdf;
// using MoonPdfLib;

namespace Hyperdrive.UI.ViewModel
{


    public class WindowViewModel : BaseViewModel
    {

        #region Private Members

        /// <summary>
        /// The window that this view model controls
        /// </summary>
        private Window mWindow;

        private MoonPdfPanel pdfPanel;

        /// <summary>
        /// The margin around the window for the drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The radius of the corners of the window
        /// </summary>
        private int mWindowRadius = 0;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        private string filePath = null;

        private string fileOutPath = null;



        #endregion

        #region Public Properties

        /// <summary>
        /// The minimum width of the window
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 400;

        /// <summary>
        /// The minimum height of the window
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 400;

        /// <summary>
        /// Whether or not the window is full screen or not, and whether the drop shadow and padding of the window should be zero or not.
        /// </summary>
        public bool Borderless { get { return (mWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked); } }

        /// <summary>
        /// THe padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        /// <summary>
        /// The size of the Resize Border around the window
        /// </summary>
        public int ResizeBorder { get { return Borderless ? 0 : 6; } }

        /// <summary>
        /// The size of the Resize Border around the window taking in account of the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        public bool bIsFileOpen { get { return !string.IsNullOrEmpty(filePath) && filePath != ""; } }

        public string FilePath
        {
            get
            {
                return filePath; 
            }
            set
            {
                if (IsValidPdf(value))
                {
                    filePath = value;
                    pdfPanel.OpenFile(FilePath);
                    OnPropertyChanged(nameof(FilePath));
                    OnPropertyChanged(nameof(bIsFileOpen));
                }
            }
        }

        public string FileOutPath { get { return fileOutPath; } set { fileOutPath = value; } }

        public int CurrentPageNumber { get { return pdfPanel.CurrentPageNumber; } }

        public string CurrentPageLabel
        {
            get
            {
                if (CurrentPageNumber == -1)
                    return "";
                else
                    return CurrentPageNumber + " of " + pdfPanel.TotalPages;
            }
        }

        public string CurrentPageSize
        {
            get
            {
                if (CurrentPageNumber == -1)
                    return "";

                PdfDocument pdfDoc = new PdfDocument(new PdfReader(FilePath));
                decimal width = (decimal)(pdfDoc.GetPage(CurrentPageNumber).GetPageSize().GetWidth() / 72);
                width = Decimal.Round(width, 1);
                decimal height = (decimal)(pdfDoc.GetPage(CurrentPageNumber).GetPageSize().GetHeight() / 72);
                height = Decimal.Round(height, 1);
                float min = Math.Min((float)width, (float)height);
                float max = Math.Max((float)width, (float)height);


                return min + " x " + max;
            }
        }

        /// <summary>
        /// The margin around the window for a dropshadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                return mWindow.WindowState == WindowState.Maximized ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }

        /// <summary>
        /// The margin around the window for a dropshadow
        /// </summary>
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        /// <summary>
        /// Radius of the corners of the window
        /// </summary>
        public int WindowRadius
        {
            get
            {
                return mWindow.WindowState == WindowState.Maximized ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }

        /// <summary>
        /// Radius of the corners of the window
        /// </summary>
        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        /// <summary>
        /// The height of the title bar caption of the window
        /// </summary>
        public int TitleHeight { get; set; } = 24;

        /// <summary>
        /// The height of the title bar caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        // public String 

        #endregion

        #region Commands

        public ICommand MinimizeCommand { get; set; }

        public ICommand MaximizeCommand { get; set; }

        public ICommand ExitApplicationCommand { get; set; }

        public ICommand SystemMenuCommand { get; set; }

        public ICommand CloseFileCommand { get; set; }

        public ICommand BusinessCard8upCommand { get; set; }

        public string Title;
        #endregion

        #region Constructor


        public WindowViewModel(Window window)
        {

            mWindow = window;
            pdfPanel = ((MainWindow)mWindow).MoonPdfPanel;

            pdfPanel.CurrentPageNumberChanged += moonPdfPanel_PageNumberChanged;
            // Listen out for the window resizing
            mWindow.StateChanged += (sender, e) =>
            {
                WindowResized();
            };

            // Create commands
            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            ExitApplicationCommand = new RelayCommand(() => mWindow.Close());
            SystemMenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));
            CloseFileCommand = new RelayCommand(() => CloseFile());

            BusinessCard8upCommand = new RelayCommand(() => BusinessCard8Up());

            // Fix window resize issue
            var resizer = new WindowResizer(mWindow);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                mDockPosition = dock;

                WindowResized();
            };
        }

        #endregion

        #region Private Methods

        void moonPdfPanel_PageNumberChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(CurrentPageNumber));
            OnPropertyChanged(nameof(CurrentPageLabel));
            OnPropertyChanged(nameof(CurrentPageSize));
        }

        private void CloseFile()
        {
            pdfPanel.Unload();
            filePath = "";
            OnPropertyChanged(nameof(FilePath));
            OnPropertyChanged(nameof(bIsFileOpen));
            OnPropertyChanged(nameof(CurrentPageNumber));
            OnPropertyChanged(nameof(CurrentPageLabel));
            OnPropertyChanged(nameof(CurrentPageSize));
        }

        public void BusinessCard8Up()
        {
            StepAndRepeatManager stepAndRepeatManager = new StepAndRepeatManager();
            stepAndRepeatManager.BusinessCard8Up(filePath, fileOutPath);
            // stepAndRepeatManager.GetPageCount(@filePath, @fileOutPath);
        }

        public void ScaleDownTest()
        {
            Resizer resizer = new Resizer(filePath, fileOutPath);

            resizer.ResizeToScale(0.50f);
        }

        #endregion

        #region Private Helpers


        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(mWindow);

            if (mWindow.WindowState == WindowState.Normal)
            {
                return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);
            }
            else
            {
                return new Point(position.X, position.Y);
            }
            
        }

        private bool IsValidPdf(string fileName)
        {
            try
            {
                new PdfReader(fileName);
                return true;
            }
            catch (iText.IO.IOException)
            {
                MessageBox.Show("Invalid PDF");
                // Sharp.text.exceptions.InvalidPdfException
                return false;
            }
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginSizeThickness));
            OnPropertyChanged(nameof(WindowRadius));
            OnPropertyChanged(nameof(WindowCornerRadius));
        }

        #endregion
    }
}
