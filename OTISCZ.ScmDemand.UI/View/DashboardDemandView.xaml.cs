using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.ScmWindow;
using OTISCZ.ScmDemand.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static OTISCZ.ScmDemand.UI.ViewModel.VmBase;
using static OTISCZ.ScmDemand.UI.ViewModel.VmBaseGrid;
using static OTISCZ.ScmDemand.UI.ViewModel.VmNewDemand;

namespace OTISCZ.ScmDemand.UI.View {
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class DashboardDemandView : UserControl, IView {
        #region Properties
        private MainWindow m_MainWindow {
            get { return ((MainWindow)Application.Current.MainWindow); }
            //get { return ((ScmDemandStart)Application.Current.MainWindow).MainWindow; }
        }

        private VmDashboardDemand m_ViewModel {
            get { return ((VmDashboardDemand)DataContext); }
        }

       // public string Loc_NewDemand = "adsasdsa";
        #endregion

        #region Constructor
        public DashboardDemandView() {
            InitializeComponent();

            //foreach (var col in grdDemand.Columns) {


            //    if (col.Header != null && col.Header.ToString().Trim().ToLower() == "status") {
            //        var headerTemplCont = col.HeaderTemplate.LoadContent();

            //        var header = MainWindow.FindChild<UcGrdColHeaderFilterComboBox>(headerTemplCont);
            //        var grd = (Grid)header[0].Content;
            //        var sp = grd.Children[1];
            //        var cmbs = MainWindow.FindChild<ComboBox>(sp);
            //        ComboBox cmb = cmbs[0];


            //        ComboBoxItem item = new ComboBoxItem();
            //        item.Content = "A";
            //        cmb.Items.Add(item);

            //        // cmb.Items.Add("ds");
            //        //Binding listBinding = new Binding();
            //        //listBinding.Source = m_ViewModel.DemandStatuses;
            //        //////listBinding.Converter = new Converter();

            //        //////cmb.ItemsSource = m_ViewModel.DemandStatuses;
            //        //////cmb.SelectedValuePath = "id";
            //        //cmb.DisplayMemberPath = "name";
            //        //cmb.SetBinding(ComboBox.ItemsSourceProperty, listBinding);
            //        ////cmb.DataContext = m_ViewModel.DemandStatuses;

            //        int i = 5;
            //    }
            //}
        }
        #endregion

        #region Interface Methods
        public void LocalizeUc() {
            txtGridColHeaderDemand.Text = ScmResource.PendingDemands;
            lblTimePeriodDemand.Content = ScmResource.TimePeriod;
            ckbOnlyMyDemands.Content = ScmResource.OnlyMyDemands;
            
            VmBaseGrid.LocalizeDemandGridColumnsLabels(grdDemand);
            
        }

        public void SetLayout() {
            
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            VmDashboardDemand vmDashboardDemand = new VmDashboardDemand(
                m_MainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher);

            this.DataContext = vmDashboardDemand;

            vmDashboardDemand.UcGrdColHeaderFilterDateNom = MainWindow.FindChild<UcGrdColHeaderFilterDate>(grdDemand, "fltCreatedDem");

            foreach (var col in grdDemand.Columns) {
                if (col.SortMemberPath == "last_status_modif_date_text") {
                    col.SortDirection = ListSortDirection.Ascending;
                }

                //if (col.Header != null && col.Header.ToString().Trim().ToLower() == "status") {
                //    var headerTemplCont = col.HeaderTemplate.LoadContent();

                //    var header = MainWindow.FindChild<UcGrdColHeaderFilterComboBox>(headerTemplCont);
                //    header[0].CmbItemsSource = m_ViewModel.DemandStatuses;

                //    //var grd = (Grid)header[0].Content;
                //    //var sp = (StackPanel)grd.Children[1];
                //    //var o = sp.Children[0];
                //    //    var cmbs = MainWindow.FindChild<ComboBox>(sp);
                //    //    ComboBox cmb = cmbs[0];


                //    //    //ComboBoxItem item = new ComboBoxItem();
                //    //    //item.Content = "A";
                //    //    //cmb.Items.Add(item);

                //    //    // cmb.Items.Add("ds");
                //    //    Binding listBinding = new Binding();
                //    //    listBinding.Source = m_ViewModel.DemandStatuses;
                //    //    ////listBinding.Converter = new Converter();

                //    //    ////cmb.ItemsSource = m_ViewModel.DemandStatuses;
                //    //    ////cmb.SelectedValuePath = "id";
                //    //    cmb.DisplayMemberPath = "name";
                //    //    cmb.SetBinding(ComboBox.ItemsSourceProperty, listBinding);
                //    //    //cmb.DataContext = m_ViewModel.DemandStatuses;

                //    //    int i = 5;
                //}
            }

            LocalizeUc();
        }

       

        private void CkbSelectedNomAll_Checked(object sender, RoutedEventArgs e) {
            m_ViewModel.SelectNomenclature(sender);
            
        }

        private void CkbSelectedNom_Checked(object sender, RoutedEventArgs e) {
            
        }

        
        private void BtnRemoveNom_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.RemoveNomenclatures();
        }

        
        private void BtnNewDemand_Click(object sender, RoutedEventArgs e) {
            Button btnNewDemand = sender as Button;
            NomenclatureExtend nom = (NomenclatureExtend)btnNewDemand.DataContext;

            WinNewDemand wn = new WinNewDemand(nom, new DlgRefreshDataGrid(m_ViewModel.RefreshDashboard));
            wn.Title = ScmResource.NewDemand;
            wn.Show();
            wn.Owner = m_MainWindow;
        }

        private void UcDataGridFilterText_FilterTextChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender);
        }
        
        private void FltCreatedNom_FilterToChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender);
        }

        private void FltCreatedNom_FilterFromChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender, FilterFromTo.From);
        }

        private void UcDataGridFilterTextDemand_FilterTextChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData2(sender, FilterFromTo.To);
        }

        private void BtnDemandDetail_Click(object sender, RoutedEventArgs e) {
            Button btnDem = sender as Button;
            Demand dem = (Demand)btnDem.DataContext;

            WinDemandDetail winDemandDetail = new WinDemandDetail(dem.id, new DlgRefreshDataGrid(m_ViewModel.RefreshDashboard));
            winDemandDetail.Owner = m_MainWindow;
            winDemandDetail.Show();
        }

        private void DataGridColumnHeaderDemand_Click(object sender, RoutedEventArgs e) {
            if (e.Source != e.OriginalSource) {
                return;
            }
            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
            m_ViewModel.SetSort2(grdDemand, grdColumn);

        }

       
        private void DataGridColumnHeaderNomenclature_Click(object sender, RoutedEventArgs e) {
            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
        }

        private void FltCreatedDem_FilterFromChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData2(sender, FilterFromTo.From);
        }

        private void FltCreatedDem_FilterToChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData2(sender, FilterFromTo.To);
        }

        private void FltDaysInStatusDem_FilterFromChanged(object sender, RoutedEventArgs e) {

        }

        private void FltDaysInStatusDem_FilterToChanged(object sender, RoutedEventArgs e) {

        }

        private void UcGrdColHeaderFilterComboBox_FilterComboBoxChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData2(sender);
        }

       
    }
}
