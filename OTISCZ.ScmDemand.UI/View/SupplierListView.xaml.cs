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

namespace OTISCZ.ScmDemand.UI.View {
    /// <summary>
    /// Interaction logic for SupplierListView.xaml
    /// </summary>
    public partial class SupplierListView : UserControl {
        private VmSupplierList m_ViewModel {
            get { return ((VmSupplierList)DataContext); }
        }

        #region Constructor
        public SupplierListView() {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void LoadInit() {
            MainWindow mainWindow = MainWindow.GetMainWindow(this);
            VmSupplierList vmSupplierList = new VmSupplierList(
                mainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher);
            this.DataContext = vmSupplierList;
            ucDataGridFooter.DataContext = vmSupplierList;
        }
        #endregion

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            LoadInit();
        }

        private void UcDataGridFilterText_FilterTextChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender);
        }

        
    }
}
