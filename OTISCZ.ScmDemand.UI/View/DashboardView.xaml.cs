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
    public partial class DashboardView : UserControl, IView {
        #region Properties
        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }

        private VmDashboard m_ViewModel {
            get { return ((VmDashboard)DataContext); }
        }

       // public string Loc_NewDemand = "adsasdsa";
        #endregion

        #region Constructor
        public DashboardView() {
            InitializeComponent();
                        
        }
        #endregion

        #region Interface Methods
        public void LocalizeUc() {
            ucWindowHeaderNomenclatures.WindowTitle = ScmResource.PendingDemandsNomenclatures;
            txtGridColHeaderNom.Text = ScmResource.PendingNomenclatures;
            txtGridColHeaderDemand.Text = ScmResource.PendingDemands;
            lblTimePeriodNom.Content = ScmResource.TimePeriod;
            lblTimePeriodDemand.Content = ScmResource.TimePeriod;
            ckbOnlyMyDemands.Content = ScmResource.OnlyMyDemands;
            lblLastImport.Content = ScmResource.LastImportDate;

            VmBaseGrid.LocalizeDemandGridColumnsLabels(grdDemand);
            VmBaseGrid.LocalizeNomenclatureGridColumnsLabels(grdNomenclature);

            ////************* Does not work **********************************************
            //var tpl = colPriceList.HeaderTemplate.LoadContent();
            //Button btnRemoveNom = MainWindow.FindChild<Button>(tpl, "btnRemoveNom");
            //if (btnRemoveNom != null) {
            //    btnRemoveNom.Content = "ccc";
            //}
            ////*************************************************************************


        }

        public void SetLayout() {
            
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            VmDashboard vmDashboard = new VmDashboard(
                m_MainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher);
            //m_ViewModel.GrdDemands = grdDemand;
            vmDashboard.UcGrdColHeaderFilterDateNom = MainWindow.FindChild<UcGrdColHeaderFilterDate>(grdNomenclature, "fltCreatedNom");
            vmDashboard.CkbSelectedNom = MainWindow.FindChild<CheckBox>(grdNomenclature, "ckbSelectedNom");
            this.DataContext = vmDashboard;
            m_ViewModel.SetColumnVisibilityNomenclature(grdNomenclature);

            foreach (var col in grdDemand.Columns) {
                if (col.SortMemberPath == "last_status_modif_date_text") {
                    col.SortDirection = ListSortDirection.Ascending;
                }
            }

            LocalizeUc();
        }

        private void CkbSelectedNomAll_Checked(object sender, RoutedEventArgs e) {
            m_ViewModel.SelectNomenclature(sender);
            grdNomenclature.Items.Refresh();
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
            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
            m_ViewModel.SetSort2(grdDemand, grdColumn);


            //grdDemand.Columns[4].SortDirection = System.ComponentModel.ListSortDirection.Descending;
            //grdDemand.Items.Refresh();

            //var headTemplate = grdColumn.HeaderTemplate;


            //var headContent = (StackPanel)headTemplate.LoadContent();
            //var ucGrdColHeaderFilterText = (UcGrdColHeaderFilterText)headContent.Children[0];
            //ucGrdColHeaderFilterText.Sorted = System.ComponentModel.ListSortDirection.Ascending;

            //var imgs = MainWindow.FindChild<Image>(ucGrdColHeaderFilterText);
            //if (imgs != null) {
            ////    imgs[0].Source = null;
            //}
        }

       
        private void DataGridColumnHeaderNomenclature_Click(object sender, RoutedEventArgs e) {
            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
        }

        private void FltCreatedDem_FilterFromChanged(object sender, RoutedEventArgs e) {

        }

        private void FltCreatedDem_FilterToChanged(object sender, RoutedEventArgs e) {

        }

        private void FltDaysInStatusDem_FilterFromChanged(object sender, RoutedEventArgs e) {

        }

        private void FltDaysInStatusDem_FilterToChanged(object sender, RoutedEventArgs e) {

        }
    }
}
