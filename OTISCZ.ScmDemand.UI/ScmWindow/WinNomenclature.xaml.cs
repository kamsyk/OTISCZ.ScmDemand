using OTISCZ.ScmDemand.Model.ExtendedModel;
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

namespace OTISCZ.ScmDemand.UI.ScmWindow
{
    /// <summary>
    /// Interaction logic for Nomenclature.xaml
    /// </summary>
    public partial class WinNomenclature : Window
    {

        #region Properties
        private NomenclatureExtend m_Nomenclature = null;

        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }
        #endregion

        public WinNomenclature(NomenclatureExtend nomenclature)
        {
            InitializeComponent();
            m_Nomenclature = nomenclature;

            VmNomenclatureDetail vmNomenclatureDetail = new VmNomenclatureDetail(
                m_MainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher);
            vmNomenclatureDetail.NomenclatureId = m_Nomenclature.id;
            ucNomenclatureDetail.DataContext = vmNomenclatureDetail;
            //((VmNomenclatureDetail)ucNomenclatureDetail.DataContext).Nomenclature = m_Nomenclature;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            //((VmNomenclatureDetail)ucNomenclatureDetail.DataContext).Nomenclature = m_Nomenclature;
        }
    }
}
