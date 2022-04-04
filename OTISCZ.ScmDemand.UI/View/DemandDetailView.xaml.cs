using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.ScmWindow;
using OTISCZ.ScmDemand.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for DemandDetailView.xaml
    /// </summary>
    public partial class DemandDetailView : UserControl, IView {
        #region Properties
        private VmDemandDetail m_ViewModel {
            get { return ((VmDemandDetail)DataContext); }
        }

        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }

        //public DockPanel DpRemark {
        //    get { return dpRemarks; }
        //}
        #endregion

        #region Constructor
        public DemandDetailView() {
            InitializeComponent();
//#if DEBUG
//            btnReport.Visibility = Visibility.Visible;
//#else
//            btnReport.Visibility = Visibility.Collapsed;
//#endif
        }
#endregion

        #region Interface Methods
        public void LocalizeUc() {
            lblDemandNr.Content = ScmResource.DemandNr + ":";
            lblAppMan.Content = ScmResource.ApprovalManager + ":";
            lblCreated.Content = ScmResource.CreatedDate + ":";
            lblReferent.Content = ScmResource.Referent + ":";
            lblStatus.Content = ScmResource.Status + ":";
            lblSupplier.Content = ScmResource.Supplier + ":";

            VmBaseGrid.LocalizeNomenclatureGridColumnsLabels(grdAttNom);

            ucDataGridHeaderReqAtt.GrdTitle = ScmResource.OtisAttachment;
            ucDataGridHeaderSuppAtt.GrdTitle = ScmResource.SupplierAttachment;

            ucDataGridHeaderReqRemark.GrdTitle = ScmResource.Remark;
        }

        public void SetLayout() {
            throw new NotImplementedException();
        }
#endregion

        #region Methods
                private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
                    //e.Text = e.Text.Replace(",", ".");
                    Regex regex = new Regex("[^0-9,]+");
                    e.Handled = regex.IsMatch(e.Text);
                }

                private void SendInfoToApprMan() {
                    //WsMail.OtWsMail wsMail = new WsMail.OtWsMail();

                    //if (m_ViewModel.DemandExtend.app_man_id == null) {
                    //    return;
                    //}

                    //int appManId = (int)m_ViewModel.DemandExtend.app_man_id;



                    //wsMail.SendMail(
                    //    "ScmDemand@otis.com",
                    //    m_ViewModel.DemandExtend.AppMan);
                }
        #endregion

        private void SpAttRec_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                m_ViewModel.OpenAttachmentGeneral(sender);
            }
        }

        private void SwRecipient_Drop(object sender, DragEventArgs e) {
            m_ViewModel.DropAttachment(e, AttachmentRepository.ATT_TYPE_RECIPIENT);
        }

        private void SwSupplier_Drop(object sender, DragEventArgs e) {
            m_ViewModel.DropAttachment(e, AttachmentRepository.ATT_TYPE_SUPPLIER);
        }
                
        private void BtnSendForApproval_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.SendForApproval();
            ckbSelectAll.IsChecked = false;
            SendInfoToApprMan();
        }
                
        private void TxtNomPrice_LostFocus(object sender, RoutedEventArgs e) {
            m_ViewModel.SaveDemandNomenclaturePrice(sender);
        }

        private void TxtNomPrice_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox textBox = (TextBox)sender;
            m_ViewModel.ValidateDecimalTextBox(textBox);
        }

        private void CmbCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            m_ViewModel.SaveDemandNomenclatureCurrency(sender);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.CloseWindow();
        }

        private void BtnApprove_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.Approve();
            ckbSelectAll.IsChecked = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            LocalizeUc();
            m_ViewModel.GetRemarks();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.Revert();
            ckbSelectAll.IsChecked = false;
        }

        private void BtnPriceWasSet_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.PriceSet();
            ckbSelectAll.IsChecked = false;
        }

        private void BtnTakeOver_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.TakeOver();
        }

        private void BtnRemoveAttachment_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.RemoveAtt(sender);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.DeleteDemand();
        }

        private void CkbSelectAll_Checked(object sender, RoutedEventArgs e) {
            m_ViewModel.SelectAllDemNoms(true);
        }

        private void CkbSelectAll_Unchecked(object sender, RoutedEventArgs e) {
            m_ViewModel.SelectAllDemNoms(false);
        }

        private void BtnReject_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.Reject();
        }

        private void BtnRemoveNomenclature_Click(object sender, RoutedEventArgs e) {
            var dc = ((Button)sender).DataContext;
            ScmDemandNomenclature demNom = (ScmDemandNomenclature)dc;  
            m_ViewModel.RemoveNomenclature(demNom);
        }

       

        private void BtnReport_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.Report();
        }

        private void BtnSendRemark_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.SendRemark(txtRemark.Text);
            txtRemark.Text = "";
            
        }

        private void BtnFinish_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.FinishDemand();
        }

        private void BtnPriceSet_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.PriceSet();
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e) {
            var dc = (ScmDemandNomenclature)((Button)sender).DataContext;
            Clipboard.SetText(dc.nomenclature_key);
        }

        private void BtnActivate_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.ActivateDemand();
        }

        private void BtnCopyDemand_Click(object sender, RoutedEventArgs e) {
            if (!m_ViewModel.CurrentUser.IsScmReferent) {
                MessageBox.Show(ScmResource.YouAreNotAuthorized, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            List<NomenclatureExtend> nomenclatures = null;
            List<ScmAttachment> scmAtt = null;
            m_ViewModel.CopyDemand(out nomenclatures, out scmAtt);

            //List<NomenclatureExtend> nomenclatures = new List<NomenclatureExtend>();
            //if (m_ViewModel.ScmDemandNomenclatures != null) {
            //    foreach (var demNom in m_ViewModel.ScmDemandNomenclatures) {
            //        NomenclatureExtend nomenclatureExtend = new NomenclatureExtend();
            //        nomenclatureExtend.id = demNom.nomenclature_id;
            //        nomenclatureExtend.nomenclature_key = demNom.nomenclature_key;
            //        nomenclatureExtend.name = demNom.name;
            //        nomenclatureExtend.specification = demNom.specification;
                    
            //        nomenclatures.Add(nomenclatureExtend);
            //    }
            //}

            WinNewDemand wn = new WinNewDemand(nomenclatures, scmAtt, m_ViewModel.DlgRefreshDashboard);
            wn.Title = ScmResource.NewDemand;
            wn.Show();
            wn.Owner = m_MainWindow;
        }
    }
}
