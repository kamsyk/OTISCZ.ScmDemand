using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static OTISCZ.ScmDemand.UI.ViewModel.VmNewDemand;

namespace OTISCZ.ScmDemand.UI.ScmWindow
{
    /// <summary>
    /// Interaction logic for WinNewDemand.xaml
    /// </summary>
    public partial class WinNewDemand : Window
    {
        #region Delegates
        //public delegate void DlgRefreshDashboard();
        #endregion

        #region Properties
        //private NomenclatureExtend m_Nomenclature = null;

        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }

        //private DlgRefreshDashboard m_DlgRefreshDashboard = null;
        //public DlgRefreshDashboard RefreshDashboard {
        //    get { return m_DlgRefreshDashboard; }
        //    set { m_DlgRefreshDashboard = value; }
        //}

        
        #endregion

        public WinNewDemand(List<NomenclatureExtend> nomenclatures, List<ScmAttachment>  scmAttachment, DlgRefreshDataGrid dlgRefreshDashboard) {
            ConstructorInit(nomenclatures, scmAttachment, dlgRefreshDashboard);
        }

        public WinNewDemand(NomenclatureExtend nomenclature, DlgRefreshDataGrid dlgRefreshDashboard) {
            List<NomenclatureExtend> nomenclatures = new List<NomenclatureExtend>();
            nomenclatures.Add(nomenclature);
            ConstructorInit(nomenclatures, null, dlgRefreshDashboard);
        }

        #region Methods
        private void CloseWindow() {
            this.Close();
        }

        private void ConstructorInit(List<NomenclatureExtend> nomenclatures, List<ScmAttachment> scmAttachments, DlgRefreshDataGrid dlgRefreshDashboard) {
            InitializeComponent();

            DlgCloseWindow dlgCloseWindow = new DlgCloseWindow(CloseWindow);

            //m_Nomenclature = nomenclature;

            VmNewDemand vmNewDemand = new VmNewDemand(
                m_MainWindow.CurrentUser.User,
                ucNewDemandView.grdSupplier,
                System.Windows.Application.Current.MainWindow.Dispatcher,
                dlgRefreshDashboard,
                dlgCloseWindow);

            vmNewDemand.HeaderTitleVisibility = Visibility.Collapsed;
            vmNewDemand.SelectedNomenclatures = new ObservableCollection<NomenclatureExtend>();
            if (nomenclatures != null && nomenclatures.Count > 0) {
                foreach (var nom in nomenclatures) {
                    vmNewDemand.SelectedNomenclatures.Add(nom);
                }
            }

            if (scmAttachments != null && scmAttachments.Count > 0) {
                foreach (var att in scmAttachments) {
                    vmNewDemand.AddAttachment(att);
                }
            }

            ucNewDemandView.DataContext = vmNewDemand;
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            //var vmNewDemand = (VmNewDemand)ucNewDemandView.DataContext;
            //if (m_DlgRefreshDashboard != null) {
            //    m_DlgRefreshDashboard();
            //}
        }
    }
}
