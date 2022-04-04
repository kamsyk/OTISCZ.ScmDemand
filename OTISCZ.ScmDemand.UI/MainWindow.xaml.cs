using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.UI.Common;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.View;
using OTISCZ.ScmDemand.UI.ViewModel;
using OTISCZ.ScmDemand.UI.ScmWindow;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

namespace OTISCZ.ScmDemand.UI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        #region Constants
        private const string CULTURE_CZ = "cs-CZ";
        private const string CULTURE_EN = "en-US";
        private const string CULTURE_SK = "sk-SK";
        #endregion

        #region Enums
        public enum LayoutStyle {
            Standard = 0,
            Pink = 1,
            Black = 2
        }
        #endregion

        #region Properties
        private string m_culture = CULTURE_CZ;

        private ScmVmUSer m_ScmUser = null;
        public ScmVmUSer CurrentUser {
            get { return m_ScmUser; }
        }

#if RELEASE
        //private WsScmDemand.ScmDemand m_WsScmDemand = null;
        //private WsScmDemandDebug.ScmDemand wsScmDemand {
        //    get {
        //        if (m_WsScmDemand == null) {
        //            m_WsScmDemand = new WsScmDemandDebug.ScmDemand();
        //        }

        //        return m_WsScmDemand;
        //    }
        //}
#else
        //private WsScmDemandDebug.ScmUser m_WsScmUser = null;

        //private WsScmDemandDebug.ScmDemand m_WsScmDemand = null;
        //private WsScmDemandDebug.ScmDemand wsScmDemand {
        //    get {
        //        if (m_WsScmDemand == null) {
        //            m_WsScmDemand = new WsScmDemandDebug.ScmDemand();
        //        }

        //        return m_WsScmDemand;
        //    }
        //}

#endif

#if RELEASE
        private WcfScmDemand.ScmDemandClient m_WcfScm = null;

        protected WcfScmDemand.ScmDemandClient WcfScm {
            get {
                if (m_WcfScm == null) {
                    m_WcfScm = new WcfScmDemand.ScmDemandClient();

                }

                return m_WcfScm;
            }
        }
#else
        private WcfScmDemandDebug.ScmDemandClient m_WcfScm = null;

        protected WcfScmDemandDebug.ScmDemandClient WcfScm {
            get {
                if (m_WcfScm == null) {
                    m_WcfScm = new WcfScmDemandDebug.ScmDemandClient();

                }

                return m_WcfScm;
            }
        }
#endif

        private ScrollViewer _m_SvPlaceHolder = null;
        private ScrollViewer m_SvPlaceHolder {
            get {
                if (_m_SvPlaceHolder == null) {
                    _m_SvPlaceHolder = FindChild<ScrollViewer>(this, "svScroll");
                }
                return _m_SvPlaceHolder;
            }
        }

        //private Visibility m_VisVerticalScrollBarDisplayed = Visibility.Collapsed;
        #endregion

        #region Static Methods
        public static MainWindow GetMainWindow(DependencyObject child) {
            var parent = LogicalTreeHelper.GetParent(child);
            if (parent is MainWindow) {
                return (MainWindow)parent;
            }

            return GetMainWindow(parent);
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null) {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName)) {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName) {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static List<T> FindChild<T>(DependencyObject parent) where T : DependencyObject {
            List<T> foundChildren = new List<T>(); 

            // Confirm parent and childName are valid. 
            if (parent == null) return foundChildren;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null) {
                    // recursively drill down the tree
                    var tmpFoundChildren = FindChild<T>(child);
                    if (tmpFoundChildren != null && tmpFoundChildren.Count > 0) {
                        foreach (var tchild in tmpFoundChildren) {
                            foundChildren.Add(tchild);
                        }
                    }

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                } else {
                    // child element found.
                    foundChildren.Add((T)child);
                    break;
                }
            }

            return foundChildren;
        }

        public static bool IsDebounceRunning = false;
        public static Task DebounceTimerAsync() {
            return Task.Run(() => {
                IsDebounceRunning = true;
                Thread.Sleep(500);
            });
        }

        public static LayoutStyle m_ScmLayoutStyle = LayoutStyle.Standard;
        public static LayoutStyle ScmLayoutStyle {
            get { return m_ScmLayoutStyle;  }
            set { m_ScmLayoutStyle = value; }
        }

        public static void RefreshStyles(DependencyObject parent) {
            if ((parent is Control)) {
                Control c = (Control)parent;
                Style bkpStyle = c.Style;
                c.Style = null;
                c.Style = bkpStyle;

                if (c is IView) {
                    IView iView = (IView)c;
                    iView.SetLayout();
                }
            } else if ((parent is FrameworkElement)) {
                FrameworkElement c = (FrameworkElement)parent;
                Style bkpStyle = c.Style;
                c.Style = null;
                c.Style = bkpStyle;

                if (c is IView) {
                    IView iView = (IView)c;
                    iView.SetLayout();
                }
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                if ((child is Control)) {
                    Control c = (Control)child;
                    Style bkpStyle = c.Style;
                    c.Style = null;
                    c.Style = bkpStyle;

                    if (c is IView) {
                        IView iView = (IView)c;
                        iView.SetLayout();
                    }
                } else if ((child is FrameworkElement)) {
                    FrameworkElement c = (FrameworkElement)child;
                    Style bkpStyle = c.Style;
                    c.Style = null;
                    c.Style = bkpStyle;

                    if (c is IView) {
                        IView iView = (IView)c;
                        iView.SetLayout();
                    }
                }



                RefreshStyles(child);

            }

        }

        private bool m_IsClosed = false;
        public bool IsClosed {
            get { return m_IsClosed; }
        }
        #endregion



        #region Methods
        public MainWindow() {
#if TEST
            this.Topmost = true;
#endif
            InitializeComponent();
            if (!IsUserAuthorized()) {
                string errMsg = String.Format(ScmResource.YouAreNotAuthorized, System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                MessageBox.Show(errMsg, "SCM Demand", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Environment.Exit(0);
            }

            lblUserName.Text = m_ScmUser.User.surname + " " + m_ScmUser.User.first_name;
            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            SetCulture();

            
        }

        private void SetCulture() {
            
            string strLang = "CZ";

            if (m_ScmUser.User.User_Setting != null &&
                !String.IsNullOrEmpty(m_ScmUser.User.User_Setting.culture)) {

                m_culture = m_ScmUser.User.User_Setting.culture;
                cmbCulture.SelectedValue = m_ScmUser.User.User_Setting.culture;

                switch (m_culture) {
                    case CULTURE_SK:
                        strLang = "SK";
                        break;
                    case CULTURE_EN:
                        strLang = "EN";
                        break;
                    default:
                        strLang = "CZ";
                        break;
                }
            }
                        
            foreach (var item in cmbCulture.Items) {
                ComboBoxItem cmbi = (ComboBoxItem)item;
                if (cmbi.Name == strLang) {
                    if (!cmbi.IsSelected) {
                        cmbCulture.SelectedItem = cmbi;
                    }
                    break;
                }
            }

            CultureInfo ci = new CultureInfo(m_culture);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            Localize();

        }

        private void SetLayout(string strLayout) {
            switch (strLayout) {
                case "Pink":
                    m_ScmLayoutStyle = LayoutStyle.Pink;
                    break;
                case "Black":
                    m_ScmLayoutStyle = LayoutStyle.Black;
                    break;
                default:
                    m_ScmLayoutStyle = LayoutStyle.Standard;
                    break;
            }

            SetLayout();
        }

        private void SetCulture(string strLang) {
            switch (strLang) {
                case "SK":
                    m_culture = CULTURE_SK;
                    break;
                case "EN":
                    m_culture = CULTURE_EN;
                    break;
                default:
                    m_culture = CULTURE_CZ;
                    break;
            }

            CultureInfo ci = new CultureInfo(m_culture);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            Localize();

            //Save
            WcfScm.SetUserCulture(m_ScmUser.User.id, m_culture);
        }

        private void Localize() {
            mniTxtDashboard.Text = ScmResource.PendingItems;
            txtMniDashboardDemands.Text = ScmResource.Demands;
            txtMniDashboardNomenclatures.Text = ScmResource.Nomenclatures;

            mniTxtDemand.Text = ScmResource.Demand;
            mniTxtNomenclatures.Text = ScmResource.Nomenclatures;
            mniTxtSupplier.Text = ScmResource.Suppliers;
            mniTxtSetting.Text = ScmResource.Settings;

            mniTxtNewDemand.Text = ScmResource.NewDemand;
            mniTxtDemandList.Text = ScmResource.Demands;

            LocalizeUserControl();
        }

        private void SetLayout() {
            RefreshStyles(grdMain);
            SetLayoutUserControl();
        }

        private void LocalizeUserControl() {
            foreach (var child in dpPlaceholder.Children) {
                if (child is IView) {
                    IView userControl = child as IView;
                    userControl.LocalizeUc();
                }
            }
        }

        private void SetLayoutUserControl() {
            foreach (var child in dpPlaceholder.Children) {
                if (child is IView) {
                    IView userControl = child as IView;
                    userControl.SetLayout();
                }
            }
        }

        //private void AdjustUserControlWidth(bool isNoScroll) {
        //    foreach (var child in dpPlaceholder.Children) {
        //        if (child is IView) {
        //            IView userControl = child as IView;
        //            double dScrollBarWidth = 0;
        //            var dPlaceholderWidth = dpScContent.RenderSize.Width;
        //            if (!isNoScroll && m_SvPlaceHolder != null) {
        //                if (m_SvPlaceHolder.ComputedVerticalScrollBarVisibility == Visibility.Visible) {
        //                    dScrollBarWidth = SystemParameters.VerticalScrollBarWidth;
        //                }
        //            }

        //            userControl.AdjustWidth(dPlaceholderWidth - dScrollBarWidth);
        //        }
        //    }
        //}

        private bool IsUserAuthorized() {
            try {
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //string userName = Environment.UserName;

                string pureUserName = GetPureUserName(userName);

#if DEBUG
               // pureUserName = "decsir";
                //pureUserName = "galat";
                //pureUserName = "thonovap";
                //pureUserName = "matuskos";
#endif

                var wsScmUser = WcfScm.GetUserData(pureUserName);
                                
                if (wsScmUser == null) {
                    return false;
                }

                var scmUser = new ScmUser();
                VmBase.SetValues(wsScmUser, scmUser);
                
                if (wsScmUser.Role != null) {
                    scmUser.Role = new List<Role>();
                    foreach (var role in wsScmUser.Role) {
                        Role uRole = new Role();
                        VmBase.SetValues(role, uRole);
                        scmUser.Role.Add(uRole);
                    }
                }

                m_ScmUser = new ScmVmUSer();
                m_ScmUser.User = scmUser;

                return true;
            } catch(Exception ex) {
                ErrorHandler.LogError(ex);
                MessageBox.Show(ScmResource.AuthorizationFailed, "SCM Demand", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            return false;
        }

        private string GetPureUserName(string userName) {
            string[] items = userName.Split('\\');
            return items[1];
        }

        private void SetActiveMenuItems(MenuItem mi) {
            foreach (var item in miMain.Items) {
                RemoveActiveMenuItem((MenuItem)item);
            }

            SetActiveMenuItem(mi);
        }

        //private void RemoveActiveMenuItem(MenuItem parentMi) {
        //    RemoveActiveMenuItem(parentMi);

        //    foreach (var item in parentMi.Items) {
        //        MenuItem mi = (MenuItem)item;
        //        RemoveActiveMenuItem(mi);
        //    }
        //}

        private void RemoveActiveMenuItem(MenuItem mi) {
            
            Grid grid = mi.Header as Grid;
            if (grid != null) {

                foreach (var child in grid.Children) {
                    if (child is Rectangle) {
                        Rectangle rec = child as Rectangle;
                        rec.Fill = null;
                        break;
                    }
                }
            }

            foreach (var item in mi.Items) {
                MenuItem childMi = (MenuItem)item;
                RemoveActiveMenuItem(childMi);
            }
        }

        private void SetActiveMenuItem(MenuItem mi) {
            Grid grid = mi.Header as Grid;
            if (grid == null) {
                return;
            }

            foreach (var child in grid.Children) {
                if (child is Rectangle) {
                    Rectangle rec = child as Rectangle;
                    SolidColorBrush sb = new SolidColorBrush();
                    sb.Color = Color.FromRgb(50, 205, 50);
                    rec.Fill = sb;
                    break;
                }
            }

            var parent = mi.Parent;
            while (parent != null) {
                if (parent is MenuItem) {
                    MenuItem parentMenuItem = parent as MenuItem;
                    SetActiveMenuItem(parentMenuItem);
                    break;
                }

                parent = LogicalTreeHelper.GetParent(parent);
                
            }
        }

        private void MenuClick(MenuItem menuItem) {
            SetActiveMenuItems(menuItem);
            dpPlaceholder.Children.Clear();
        }

        private void HideControls() {
            if (!CurrentUser.IsScmReferent) {
                mniNewDemand.Visibility = Visibility.Collapsed;
            }
        }

        //public Style GetLayoutStyle(string standardStyleName) {
        //    switch (m_ScmLayoutStyle) {
        //        case LayoutStyle.Pink:
        //            var pStyle = FindResource(standardStyleName + "Pink");
        //            if (pStyle != null) {
        //                return (Style)pStyle;
        //            }

        //            return (Style)FindResource(standardStyleName);
        //        case LayoutStyle.Black:
        //            var bStyle = FindResource(standardStyleName + "Black");
        //            if (bStyle != null) {
        //                return (Style)bStyle;
        //            }

        //            return (Style)FindResource(standardStyleName);
        //    }

        //    return (Style)FindResource(standardStyleName);


        //}

        
        #endregion

        private void UcImport_Loaded(object sender, RoutedEventArgs e) {

        }

        private void MenuItemImportHistory_Click(object sender, RoutedEventArgs e) {
            
            
        }

        private void MenuItemProdisImport_Click(object sender, RoutedEventArgs e) {
            MenuClick(sender as MenuItem);
                        
            ImportView ucImport = new ImportView();
            dpPlaceholder.Children.Add(ucImport);
            //AdjustUserControlWidth(true);
        }

        public void ClearChildren() {
            dpPlaceholder.Children.Clear();

            foreach (var item in miMain.Items) {
                RemoveActiveMenuItem((MenuItem)item);
            }
        }
                
        private void CmbCulture_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBoxItem cmbi = (ComboBoxItem)e.AddedItems[0];
            string lang = cmbi.Name;

            SetCulture(lang);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            //AdjustUserControlWidth(false);
        }

        private void MniNomenclaturesList_Click(object sender, RoutedEventArgs e) {
            MenuClick(sender as MenuItem);

            NomenclatureListView nomenclatureListView = new NomenclatureListView();
            dpPlaceholder.Children.Add(nomenclatureListView);
        }

       

        private void MniSupplier_Click(object sender, RoutedEventArgs e) {
            MenuClick(sender as MenuItem);

            SupplierListView supplierListView = new SupplierListView();
            dpPlaceholder.Children.Add(supplierListView);
        }

        private void MniNewDemand_Click(object sender, RoutedEventArgs e) {
            MenuClick(sender as MenuItem);

            NewDemandView newDemandView = new NewDemandView();
            dpPlaceholder.Children.Add(newDemandView);
        }

        private void CmbLayout_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBoxItem cmbi = (ComboBoxItem)e.AddedItems[0];
            string layout = cmbi.Name;
            
            SetLayout(layout);
        }

        private void MniDashboard_Click(object sender, RoutedEventArgs e) {
            MenuClick(sender as MenuItem);

            DashboardView dashboardView = new DashboardView();
            dpPlaceholder.Children.Add(dashboardView);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            DashboardDemandView dashboardDemandView = new DashboardDemandView();
            dpPlaceholder.Children.Add(dashboardDemandView);
            VmBase.DeleteTempFiles();
            HideControls();
        }

        private void MniDemandDashboard_Click(object sender, RoutedEventArgs e) {
            MenuClick(sender as MenuItem);

            DashboardDemandView dashboardDemandView = new DashboardDemandView();
            dpPlaceholder.Children.Add(dashboardDemandView);
        }

        private void MniNomenclatureDashboard_Click(object sender, RoutedEventArgs e) {
            MenuClick(sender as MenuItem);

            DashboardNomenclatureView dashboardNomenclatureView = new DashboardNomenclatureView();
            dpPlaceholder.Children.Add(dashboardNomenclatureView);
        }

        private void Window_Closed(object sender, EventArgs e) {
            m_IsClosed = true;
        }

        private void MniDemandsList_Click(object sender, RoutedEventArgs e) {
            MenuClick(sender as MenuItem);

            DemandListView demandListView = new DemandListView();
            dpPlaceholder.Children.Add(demandListView);
        }



        //private void SvScroll_ScrollChanged(object sender, ScrollChangedEventArgs e) {
        //    if (m_SvPlaceHolder != null) {
        //        if (m_SvPlaceHolder.ComputedVerticalScrollBarVisibility != m_VisVerticalScrollBarDisplayed) {
        //            AdjustUserControlWidth(false);
        //        }
        //    }
        //}
    }
}
