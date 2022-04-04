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
    public partial class DashboardNomenclatureView : UserControl, IView {
        #region Properties
        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }

        private VmDashboardNomenclature m_ViewModel {
            get { return ((VmDashboardNomenclature)DataContext); }
        }

       // public string Loc_NewDemand = "adsasdsa";
        #endregion

        #region Constructor
        public DashboardNomenclatureView() {
            InitializeComponent();
                        
        }
        #endregion

        #region Interface Methods
        public void LocalizeUc() {
            
            txtGridColHeaderNom.Text = ScmResource.PendingNomenclatures;
            
            lblTimePeriodNom.Content = ScmResource.TimePeriod;
            
            lblLastImport.Content = ScmResource.LastImportDate;

            VmBaseGrid.LocalizeNomenclatureGridColumnsLabels(grdNomenclature);

        }

        public void SetLayout() {
            
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            VmDashboardNomenclature vmDashboardNomenclature = new VmDashboardNomenclature(
                m_MainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher);

            vmDashboardNomenclature.UcGrdColHeaderFilterDateNom = MainWindow.FindChild<UcGrdColHeaderFilterDate>(grdNomenclature, "fltCreatedNom");
            vmDashboardNomenclature.CkbSelectedNom = MainWindow.FindChild<CheckBox>(grdNomenclature, "ckbSelectedNom");
            this.DataContext = vmDashboardNomenclature;
            m_ViewModel.SetColumnVisibilityNomenclature(grdNomenclature);

            
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
            m_ViewModel.FilterGridData(sender, FilterFromTo.To);
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
            
        }

       
        private void DataGridColumnHeaderNomenclature_Click(object sender, RoutedEventArgs e) {
            
            if (e.Source != e.OriginalSource) {
                //skip sort if filter control is clicked
                return;
            }

            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
            m_ViewModel.SetSort(grdNomenclature, grdColumn);
        }

        private void FltCreatedDem_FilterFromChanged(object sender, RoutedEventArgs e) {

        }

        private void FltCreatedDem_FilterToChanged(object sender, RoutedEventArgs e) {

        }

        private void FltDaysInStatusDem_FilterFromChanged(object sender, RoutedEventArgs e) {

        }

        private void FltDaysInStatusDem_FilterToChanged(object sender, RoutedEventArgs e) {

        }

        private void UcGrdColHeaderFilterComboBox_FilterComboBoxChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender);
        }
    }
}
