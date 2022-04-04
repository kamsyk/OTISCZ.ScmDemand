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
    /// Interaction logic for WinSupplier.xaml
    /// </summary>
    public partial class WinSupplier : Window {
        #region Properties
        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }
        #endregion

        #region Constructor
        public WinSupplier(int id, DlgRefreshEditableDataGrid dlgRefreshDashboard) {
            InitializeComponent();

            DlgCloseWindow dlgCloseWindow = new DlgCloseWindow(CloseWindow);

            VmSupplier vmSupplier = new VmSupplier(
                id,
                m_MainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher,
                dlgRefreshDashboard,
                dlgCloseWindow);

            ucSupplierView.DataContext = vmSupplier;
        }
        #endregion

        #region Methods
        private void CloseWindow() {
            this.Close();
        }
        #endregion
    }
}
