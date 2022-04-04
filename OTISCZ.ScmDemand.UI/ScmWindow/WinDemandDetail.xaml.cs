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
using System.Windows.Shapes;
using static OTISCZ.ScmDemand.UI.ViewModel.VmBase;

namespace OTISCZ.ScmDemand.UI.ScmWindow {
    /// <summary>
    /// Interaction logic for WinDemandDetail.xaml
    /// </summary>
    public partial class WinDemandDetail : Window {
        #region Properties
        
        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }

        //private DlgRefreshDataGrid m_dlgRefreshDashboard = null;
        //public DlgRefreshDataGrid DlgRefreshDashboard {
        //    get { return m_dlgRefreshDashboard; }
        //}
        #endregion


        #region Constructor
        public WinDemandDetail(int demandId, DlgRefreshDataGrid dlgRefreshDashboard) {
            InitializeComponent();

           // m_dlgRefreshDashboard = dlgRefreshDashboard;

            DlgCloseWindow dlgCloseWindow = new DlgCloseWindow(CloseWindow);

            VmDemandDetail vmDemandDetail = new VmDemandDetail(
                m_MainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher,
                demandId,
                dlgCloseWindow,
                dlgRefreshDashboard,
                ucDemandDetail.grdRemarks);

            ucDemandDetail.DataContext = vmDemandDetail;
        }
        #endregion

        #region Methods
        private void CloseWindow() {
            this.Close();
        }
        #endregion
    }
}
