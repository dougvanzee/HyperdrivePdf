using Hyperdrive.Core.Stats.PageSizeCount;
using Hyperdrive.UI.ViewModel;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
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
        class PageSizesItem
        {
            public float _width { get; private set; }
            public float _height { get; private set; }
            public float _widthFormatted
            {
                get
                {
                    return (float)Math.Round(new decimal(_width / 72), 2);
                }
            }
            public float _heightFormatted
            {
                get
                {
                    return (float)Math.Round(new decimal(_height / 72), 2);
                }
            }
            public List<int> _pageNumbers { get; private set; }

            public PageSizesItem(int pageNumber, float width, float height)
            {
                _width= width;
                _height= height;
                _pageNumbers= new List<int>();
                _pageNumbers.Add(pageNumber);
            }

            public bool AddPageNumber(int pageNumber, float width, float height)
            {
                if (Math.Abs(_width - width) <= 0.0001 * _width && Math.Abs(_height - height) <= 0.0001 * _height)
                {
                    _pageNumbers.Add(pageNumber);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Window parentWindow = Application.Current.MainWindow;
        TreeItemViewModel rootNode = new TreeItemViewModel(null) { DisplayName = "rootNode" };
        private TreeView treeView;
        

        public PageCountSideToolbar()
        {
            InitializeComponent();

            parentWindow = Application.Current.MainWindow;

            treeView = new TreeView();

            DataContext = rootNode;

            var pd = DependencyPropertyDescriptor.FromProperty(UIElement.IsEnabledProperty, typeof(PageCountSideToolbar));
            pd.AddValueChanged(this, OnIsEnabledChanged);
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            OnIsEnabledChanged(this, new PropertyChangedEventArgs(nameof(OnIsEnabledChanged)));
        }

        public int totalPageCount { get; set; } = 1;
        public int currentProgress { get; set; } = 1;

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
            currentProgress = 0;
            
            GetPageSizes();
        }

        public void DisableToolbar()
        {
            ItemsControl itemsControl = (ItemsControl)this.FindName("PageSizesContainer");
            if (itemsControl != null)
                itemsControl.Items.Clear();
            treeView.Items.Clear();
            rootNode.Children.Clear();
        }

        private void GetPageSizes()
        {
            string inPath = ((WindowViewModel)parentWindow.DataContext).FilePath;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inPath));
            totalPageCount = pdfDoc.GetNumberOfPages();
            OnPropertyChanged(nameof(totalPageCount));

            List<PageSizesItem> pageSizes = new List<PageSizesItem>();
            for (int i = 1; i <= totalPageCount; i++)
            {
                float width;
                float height;

                currentProgress = i;
                OnPropertyChanged(nameof(currentProgress));

                PdfPage currentPage = pdfDoc.GetPage(i);
                iText.Kernel.Geom.Rectangle cropRect = currentPage.GetCropBox();

                if (i == 1)
                {
                    pageSizes.Add(new PageSizesItem(1, cropRect.GetWidth(), cropRect.GetHeight()));
                    continue;
                }

                PageSizesItem lastSizesItem = pageSizes.Last();
                if (lastSizesItem.AddPageNumber(i, cropRect.GetWidth(), cropRect.GetHeight()))
                {
                    continue;
                }
                else
                {
                    pageSizes.Add(new PageSizesItem(i, cropRect.GetWidth(), cropRect.GetHeight()));
                    continue;
                }
            }

            //ItemsControl itemsControl = (ItemsControl)this.FindName("PageSizesContainer");

            //if (itemsControl != null)
            //itemsControl.Items.Add(new PageCountItem("TOTAL PAGES", totalPageCount, true));


            
            foreach (PageSizesItem pageSize in pageSizes)
            {
                string name = pageSize._widthFormatted + " x " + pageSize._heightFormatted + "  :  (" + pageSize._pageNumbers.Count + ")";
                TreeItemViewModel treeViewItem = new TreeItemViewModel(rootNode);
                treeViewItem.DisplayName = name;
                rootNode.Children.Add(treeViewItem);
                foreach (int pageNumber in pageSize._pageNumbers)
                {
                    treeViewItem.Children.Add(new TreeItemViewModel(treeViewItem) { DisplayName = "Page " + pageNumber.ToString() });
                }
            }
            
                //TreeItemViewModel theTreeView = (TreeItemViewModel)this.FindName("TheTreeView");
                //theTreeView.Children.Add(rootNode);

            /*
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems)
            {
                if (!node.HasDummyChild)
                {
                    node.Children.Add(new TreeItemViewModel(node, false) { DisplayName = "newborn child" });
                    node.IsExpanded = true;
                }
            }
            */
            

        }

        #region IPropertyNotify

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (treeView != null)
            {
                foreach (object item in treeView.Items)
                {
                    TreeViewItem treeItem = item as TreeViewItem;
                    if (treeItem != null)
                        ExpandAll(treeItem, true);
                    treeItem.IsExpanded = true;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                    item.IsExpanded = true;
            }
        }

        private void TheTreeView_PreviewSelectionChanged(object sender, PreviewSelectionChangedEventArgs e)
        {
            // Selection is not locked, apply other conditions.
            // Require all selected items to be of the same type. If an item of another data
            // type is already selected, don't include this new item in the selection.
            if (e.Selecting && TheTreeView.SelectedItems.Count > 0)
            {
                e.CancelThis = e.Item.GetType() != TheTreeView.SelectedItems[0].GetType();
            }

            //if (e.Selecting)
            //{
            //    System.Diagnostics.Debug.WriteLine("Preview: Selecting " + e.Item + (e.Cancel ? " - cancelled" : ""));
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("Preview: Deselecting " + e.Item + (e.Cancel ? " - cancelled" : ""));
            //}
        }

        private void ClearChildrenButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selection = new object[TheTreeView.SelectedItems.Count];
            TheTreeView.SelectedItems.CopyTo(selection, 0);
            foreach (TreeItemViewModel node in selection)
            {
                if (node.Children != null)
                {
                    node.Children.Clear();
                }
            }
        }

        private void AddChildButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems)
            {
                if (!node.HasDummyChild)
                {
                    node.Children.Add(new TreeItemViewModel(node, false) { DisplayName = "newborn child" });
                    node.IsExpanded = true;
                }
            }
        }

        private void ExpandAllNodesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.Items)
            {
                node.IsExpanded = true;
            }
        }

        private void CollapseAllNodesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.Items)
            {
                node.IsExpanded = false;
            }
        }

        private void ExpandNodesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems)
            {
                node.IsExpanded = true;
            }
        }

        private void ExpandMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems)
            {
                node.IsExpanded = true;
            }
        }

        private void RenameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems)
            {
                node.IsEditing = true;
                break;
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems.Cast<TreeItemViewModel>().ToArray())
            {
                node.Parent.Children.Remove(node);
            }
        }
    }
}
