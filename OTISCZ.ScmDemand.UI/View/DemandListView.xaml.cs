using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.ScmWindow;
using OTISCZ.ScmDemand.UI.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static OTISCZ.ScmDemand.UI.ViewModel.VmBase;
using static OTISCZ.ScmDemand.UI.ViewModel.VmBaseGrid;

namespace OTISCZ.ScmDemand.UI.View {
    /// <summary>
    /// Interaction logic for DemandListView.xaml
    /// </summary>
    public partial class DemandListView : UserControl, IView {
        #region Properties
        private MainWindow m_MainWindow {
            get { return ((MainWindow)Application.Current.MainWindow); }
            //get { return ((ScmDemandStart)Application.Current.MainWindow).MainWindow; }
        }

        private VmDemandList m_ViewModel {
            get { return ((VmDemandList)DataContext); }
        }


        #endregion

        #region Constructor
        public DemandListView() {
            InitializeComponent();
        }
        #endregion

        #region Interface Methods
        public void LocalizeUc() {
            //txtGridColHeaderDemand.Text = ScmResource.PendingDemands;
            //lblTimePeriodDemand.Content = ScmResource.TimePeriod;
            //ckbOnlyMyDemands.Content = ScmResource.OnlyMyDemands;
                        
            VmBaseGrid.LocalizeDemandGridColumnsLabels(grdDemand);
        }

        public void SetLayout() {

        }
        #endregion

        #region Methods
        private void LoadInit() {
            MainWindow mainWindow = MainWindow.GetMainWindow(this);
            VmDemandList vmDemandList = new VmDemandList(
                mainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher);
            this.DataContext = vmDemandList;
            ucDataGridFooter.DataContext = vmDemandList;

            LocalizeUc();
        }
        #endregion

        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e) {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            LoadInit();
        }

        private void BtnDemandDetail_Click(object sender, RoutedEventArgs e) {
            Button btnDem = sender as Button;
            Demand dem = (Demand)btnDem.DataContext;

            WinDemandDetail winDemandDetail = new WinDemandDetail(dem.id, new DlgRefreshDataGrid(m_ViewModel.RefreshGridData));
            winDemandDetail.Owner = m_MainWindow;
            winDemandDetail.Show();
        }

        private void UcDataGridFilterTextDemand_FilterTextChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender, FilterFromTo.To);
        }
    }
}
