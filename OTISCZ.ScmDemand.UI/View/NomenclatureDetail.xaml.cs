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
    /// Interaction logic for NomenclatureDetail.xaml
    /// </summary>
    public partial class NomenclatureDetail : UserControl {
        #region Properties
        private VmNomenclatureDetail m_ViewModel {
            get { return ((VmNomenclatureDetail)DataContext); }
        }
        #endregion

        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }

        public NomenclatureDetail() {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {

        }

        private void BtnSendForApproval_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.SendPriceForApproval();
            
        }
    }
}
