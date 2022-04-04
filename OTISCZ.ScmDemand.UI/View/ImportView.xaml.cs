using Microsoft.Win32;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OTISCZ.ScmDemand.UI.View
{
    /// <summary>
    /// Interaction logic for ImportView.xaml
    /// </summary>
    public partial class ImportView : System.Windows.Controls.UserControl, IView {
        #region Properties
        private VmImportProdis m_ViewModel {
            get { return ((VmImportProdis)DataContext); }
        }
        
        #endregion

        #region Constructor
        public ImportView() {
            InitializeComponent();
            LocalizeUc();
        }
        #endregion

        #region Interface Methods
        public void LocalizeUc() {
            ucWindowHeader.WindowTitle = ScmResource.ImportProdisFile;
        }

        public void SetLayout() {
            
        }
        #endregion

        #region Methods
        private void LoadInit() {
            MainWindow mainWindow = MainWindow.GetMainWindow(this);
            this.DataContext = new VmImportProdis(
                mainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher);
        }
                
        private void SelectImportFolder() {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = fbd.ShowDialog();
            if (dr == DialogResult.OK) {
                txtFolder.Text = fbd.SelectedPath;
            }
        }

        private void Import() {
            if (m_ViewModel.CheckItems()) {
                //txtPbBind.StringFormat = "{}UserLvlddddddd:{0}/{1}";
                m_ViewModel.ImportFiles();
            } else {
                System.Windows.MessageBox.Show(ScmResource.EnterMandatoryValues, ScmResource.EnterMandatoryTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        #endregion

        private void UcImportView_Loaded(object sender, RoutedEventArgs e) {
            LoadInit();
        }

        private void BtnFolder_Click(object sender, RoutedEventArgs e) {
            SelectImportFolder();
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e) {
            Import();
        }
                
        private void UcImportView_Unloaded(object sender, RoutedEventArgs e) {

        }

        
    }
}
