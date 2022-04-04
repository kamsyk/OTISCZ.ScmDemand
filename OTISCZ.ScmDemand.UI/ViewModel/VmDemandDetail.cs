using OTISCZ.CommonDb;
using OTISCZ.Report;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using static OTISCZ.ScmDemand.Model.Repository.NomenclatureRepository;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class VmDemandDetail : VmBase, INotifyPropertyChanged, INotifyDataErrorInfo {
        #region Events
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion

        #region Properties
        private int m_DemandId = DataNulls.INT_NULL;

        private DlgCloseWindow m_DlgClosewindow = null;
        public DlgCloseWindow CloseWindow {
            get { return m_DlgClosewindow; }
        }

        private DlgRefreshDataGrid m_DlgRefreshDashboard = null;
        public DlgRefreshDataGrid DlgRefreshDashboard {
            get { return m_DlgRefreshDashboard; }
        }

        private DemandExtend m_DemandExtend = null;
        public DemandExtend DemandExtend {
            get {
                
                return m_DemandExtend;
            }
            set {
                m_DemandExtend = value;
                OnPropertyChanged("DemandExtend");
            }
        }

        private RemarkExtend[] m_RemarkExtend = null;
        public RemarkExtend[] RemarkExtend {
            get {

                return m_RemarkExtend;
            }
            set {
                m_RemarkExtend = value;
                //OnPropertyChanged("RemarkExtend");
            }
        }

        private List<ScmAttachment> m_RecipientAttachment = null;
        public List<ScmAttachment> RecipientAttachment {
            get {

                return m_RecipientAttachment;
            }
            set {
                m_RecipientAttachment = value;
                OnPropertyChanged("RecipientAttachment");
            }
        }

        private List<ScmAttachment> m_SupplierAttachment = null;
        public List<ScmAttachment> SupplierAttachment {
            get {

                return m_SupplierAttachment;
            }
            set {
                m_SupplierAttachment = value;
                OnPropertyChanged("SupplierAttachment");
            }
        }

        private ScmUserExtend[] m_AppMen = null;
        public ScmUserExtend[] AppMen {
            get {
                if (m_AppMen == null) {
                    m_AppMen = WcfScm.GetActiveAppMen();
                }
                return m_AppMen;
            }
            set {
                m_AppMen = value;
                OnPropertyChanged("AppMen");
            }
        }

        private Currency[] m_Currencies = null;
        public Currency[] Currencies {
            get {
                if (m_Currencies == null) {
                    m_Currencies = WcfScm.GetActiveCurrencies();
                }
                return m_Currencies;
            }
            set {
                m_Currencies = value;
                OnPropertyChanged("Currencies");
            }
        }

        private Visibility m_BtnSendForApprovalVisibility = Visibility.Collapsed;
        public Visibility BtnSendForApprovalVisibility {
            get {
                return m_BtnSendForApprovalVisibility;
            }
            set {
                m_BtnSendForApprovalVisibility = value;
                OnPropertyChanged("BtnSendForApprovalVisibility");
            }
        }

        private Visibility m_BtnBackVisibility = Visibility.Collapsed;
        public Visibility BtnBackVisibility {
            get {
                return m_BtnBackVisibility;
            }
            set {
                m_BtnBackVisibility = value;
                OnPropertyChanged("BtnBackVisibility");
            }
        }

        private Visibility m_BtnTakeOverVisibility = Visibility.Collapsed;
        public Visibility BtnTakeOverVisibility {
            get {
                return m_BtnTakeOverVisibility;
            }
            set {
                m_BtnTakeOverVisibility = value;
                OnPropertyChanged("BtnTakeOverVisibility");
            }
        }

        private Visibility m_BtnPriceWasSetVisibility = Visibility.Collapsed;
        public Visibility BtnPriceWasSetVisibility {
            get {
                return m_BtnPriceWasSetVisibility;
            }
            set {
                m_BtnPriceWasSetVisibility = value;
                OnPropertyChanged("BtnPriceWasSetVisibility");
            }
        }

        private Visibility m_BtnApproveVisibility = Visibility.Collapsed;
        public Visibility BtnApproveVisibility {
            get {
                return m_BtnApproveVisibility;
            }
            set {
                m_BtnApproveVisibility = value;
                OnPropertyChanged("BtnApproveVisibility");
            }
        }

        private Visibility m_BtnFinishVisibility = Visibility.Collapsed;
        public Visibility BtnFinishVisibility {
            get {
                return m_BtnFinishVisibility;
            }
            set {
                m_BtnFinishVisibility = value;
                OnPropertyChanged("BtnFinishVisibility");
            }
        }
                
        private Visibility m_BtnDeleteVisibility = Visibility.Collapsed;
        public Visibility BtnDeleteVisibility {
            get {
                return m_BtnDeleteVisibility;
            }
            set {
                m_BtnDeleteVisibility = value;
                OnPropertyChanged("BtnDeleteVisibility");
            }
        }

        private Visibility m_BtnActivateVisibility = Visibility.Collapsed;
        public Visibility BtnActivateVisibility {
            get {
                return m_BtnActivateVisibility;
            }
            set {
                m_BtnActivateVisibility = value;
                OnPropertyChanged("BtnActivateVisibility");
            }
        }

        private Visibility m_CmbAppManVisibility = Visibility.Collapsed;
        public Visibility CmbAppManVisibility {
            get {
                return m_CmbAppManVisibility;
            }
            set {
                m_CmbAppManVisibility = value;
                OnPropertyChanged("CmbAppManVisibility");
            }
        }

        private Visibility m_TxtAppManVisibility = Visibility.Collapsed;
        public Visibility TxtAppManVisibility {
            get {
                return m_TxtAppManVisibility;
            }
            set {
                m_TxtAppManVisibility = value;
                OnPropertyChanged("TxtAppManVisibility");
            }
        }


        private bool m_IsAllowDragDrop = false;
        public bool IsAllowDragDrop {
            get {

                return m_IsAllowDragDrop;
            }
            set {
                m_IsAllowDragDrop = value;
                OnPropertyChanged("IsAllowDragDrop");
            }
        }

        //private Visibility m_BtnCloseVisibility = Visibility.Collapsed;
        //public Visibility BtnCloseVisibility {
        //    get {
        //        return m_BtnCloseVisibility;
        //    }
        //    set {
        //        m_BtnCloseVisibility = value;
        //        OnPropertyChanged("BtnCloseVisibility");
        //    }
        //}

        private int m_SelectedAppManId = DataNulls.INT_NULL;
        public int SelectedAppManId {
            get {
                return m_SelectedAppManId;
            }
            set {
                try {
                    ValidateAppManInputData(value);
                    m_SelectedAppManId = value;
                    if (m_SelectedAppManId >= 0 && m_SelectedAppManId != DemandExtend.app_man_id) {
                        m_DemandExtend.app_man_id = m_SelectedAppManId;
                        //SetAppManName();
                        SaveDemandRefresh();
                    }
                    OnPropertyChanged("SelectedAppManId");
                } catch (Exception ex) {
                    m_IsSaveError = true;
                    HandleError(ex);
                    ValidateAppManInputData(m_SelectedAppManId);
                    
                }
            }
        }

        private string m_SelectedAppManName = null;
        public string SelectedAppManName {
            get {
                if (m_SelectedAppManName == null && DemandExtend.app_man_id != null) {
                    if (DemandExtend.app_man_id != DataNulls.INT_NULL) {
                        m_SelectedAppManName = WcfScm.GetUserName((int)DemandExtend.app_man_id);
                    }
                }
                return m_SelectedAppManName;
            }
            set {
                try {
                    m_SelectedAppManName = value;
                    OnPropertyChanged("SelectedAppManName");
                } catch (Exception ex) {
                    HandleError(ex);
                    
                }
            }
        }

        private bool m_IsSaveError = false;

        //private DockPanel m_DpRemark = null;

        private Grid m_RemarkGrid = null;

        //BrushConverter m_BrushConverter = new System.Windows.Media.BrushConverter();

        //EnvironmentVariableTarget col1 = (Brush)m_BrushConverter.ConvertFromString("#5c6bc0");

        Brush[] m_brushesUser = null;
        Brush[] brushesUser {
            get {
                if (m_brushesUser == null) {
                    m_brushesUser = PopulateBrushUser();
                }

                return m_brushesUser;
            } 
        }

        Brush[] m_brushesRemark = null;
        Brush[] brushesRemark {
            get {
                if (m_brushesRemark == null) {
                    m_brushesRemark = PopulateBrushRemark();
                }

                return m_brushesRemark;
            }
        }

        private Hashtable m_UserBrush = new Hashtable();

        #region LocTexts
        public string LocSendForApprovalText {
            get { return ScmResource.SendForApproval; }
        }

        public string LocCloseText {
            get { return ScmResource.Close; }
        }

        public string LocFinishText {
            get { return ScmResource.FinishDemand; }
        }

        public string LocReportText {
            get { return ScmResource.Report; }
        }

        public string LocCopyText {
            get { return ScmResource.Copy; }
        }

        public string LocApproveText {
            get { return ScmResource.Approve; }
        }

        public string LocRejectText {
            get { return ScmResource.Reject; }
        }

        public string LocProdisPriceText {
            get { return ScmResource.ProdisPriceSet; }
        }

        public string LocTakeOverText {
            get { return ScmResource.TakeOver; }
        }

        public string LocInsertText {
            get { return ScmResource.InsertRemark; }
        }

        public string LocDeleteText {
            get { return ScmResource.Delete; }
        }

        public string LocCancelText {
            get { return ScmResource.Cancel; }
        }
        
        public string LocBackToPreviousStatusText {
            get { return ScmResource.BackToPreviousStatus; }
        }

        public string LocPriceWasSetText {
            get { return ScmResource.StatusPriceWasSet; }
        }

        public string LocActivateText {
            get { return ScmResource.Activate; }
        }

        private List<ScmDemandNomenclature> m_ScmDemandNomenclatures = null;
        public List<ScmDemandNomenclature> ScmDemandNomenclatures {
            get { return m_ScmDemandNomenclatures; }
            set {
                m_ScmDemandNomenclatures = value;
                OnPropertyChanged("ScmDemandNomenclatures");
            }
        }
        #endregion

        //private int m_SelectedCurrencyId = DataNulls.INT_NULL;
        //public int SelectedCurrencyId {
        //    get {
        //        return m_SelectedCurrencyId;
        //    }
        //    set {
        //        //ValidateAppManInputData(value);
        //        m_SelectedCurrencyId = value;
        //        OnPropertyChanged("SelectedCurrencyId");
        //    }
        //}

        //SelectedAppManId
        //private bool m_iSLoading = false;
        #endregion

        #region Constructor
        public VmDemandDetail(
            ScmUser scmUser,
            Dispatcher dispatcher,
            int demandId,
            DlgCloseWindow dlgCloseWindow,
            DlgRefreshDataGrid dlgRefreshDashboard,
            Grid grdRemark) : base(scmUser, dispatcher) {

            m_DemandId = demandId;

            m_DlgClosewindow = dlgCloseWindow;
            m_DlgRefreshDashboard = dlgRefreshDashboard;

            if (m_Currencies == null) {
                m_Currencies = WcfScm.GetActiveCurrencies();
            }

            if (m_DemandExtend == null) {
                LoadDemandDetail();
                EnableDragDrop();
            }

            //m_DpRemark = dpRemark;
            m_RemarkGrid = grdRemark;
        }
        #endregion

        #region Interface Methods
        public IEnumerable GetErrors(string propertyName) {
            if (propertyName == "SelectedAppManId") {
                List<string> errMsg = new List<string>();

                if (SelectedAppManId < 0) {
                    HasErrors = true;

                    errMsg.Add(ScmResource.SelectAppMan);

                } else if (m_IsSaveError) {
                    HasErrors = true;

                    errMsg.Add(ScmResource.DataWasNotSaved);
                    m_IsSaveError = false;
                }

                if (!HasErrors) {
                    return null;
                }

                return errMsg;
            }

            //if (propertyName == "price_text") {
            //    List<string> errMsg = new List<string>();
            //    errMsg.Add("Select App MAn");
            //    return errMsg;
            //}

                return null;
        }
        #endregion

        #region Static Methods
        public static string GetDemandStatusText(int iStatus) {
            switch (iStatus) {
                case DemandRepository.DEM_STATUS_DRAFT:
                    return ScmResource.StatusDraft;
                case DemandRepository.DEM_STATUS_SENT:
                    return ScmResource.StatusSentToSupplier;
                case DemandRepository.DEM_STATUS_SUPPLIER_REPLIED:
                    return ScmResource.StatusSupplierReplied;
                case DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL:
                    return ScmResource.StatusWaitForApproval;
                case DemandRepository.DEM_STATUS_APPROVED:
                    return ScmResource.StatusApproved;
                case DemandRepository.DEM_STATUS_REJECTED:
                    return ScmResource.StatusRejected;
                case DemandRepository.DEM_STATUS_PRICE_SET:
                    return ScmResource.StatusPriceWasSet;
                case DemandRepository.DEM_STATUS_PRICE_CONFIRMED:
                    return ScmResource.StatusPriceWasSetConfirmed;
                case DemandRepository.DEM_STATUS_CLOSED:
                    return ScmResource.StatusClosed;
                default:
                    return ScmResource.StatusUnknown;
            }
        }
        #endregion

        #region Async Methods
        protected void LoadDemandDetail() {
            try {
                
                DemandExtend = GetDemand();
                

            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        private Task<RemarkExtend[]> GetRemarksAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    
                    var wsRemarks = WcfScm.GetRemarks(m_DemandExtend.id);

                    return wsRemarks;
                } catch (Exception ex) {
                    IsBusy = false;
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        public async void GetRemarks() {
            try {

                var remarks = GetRemarksAsync();
                var currRemarkExtend = await remarks;
                DisplayRemarks(currRemarkExtend);              

            } catch (Exception ex) {
                HandleError(ex);
            } finally {
                //IsBusy = false;
            }
        }

        //protected virtual Task<DemandExtend> GetDemandAsync() {

        //    return Task.Run(() => {
        //        IsBusy = true;
        //        try {

        //            var wsDemand = WsScm.GetDemandDetail(m_DemandId, true);
        //            DemandExtend demandExtend = new DemandExtend();
        //            SetValues(wsDemand, demandExtend);

        //            demandExtend.status_text = GetStatusText(demandExtend.status_id);

        //            demandExtend.nomenclatures_extend = new List<NomenclatureExtend>();
        //            foreach (var wsNom in wsDemand.nomenclatures_extend) {
        //                NomenclatureExtend nomenclatureExtend = new NomenclatureExtend();
        //                SetValues(wsNom, nomenclatureExtend);
        //                demandExtend.nomenclatures_extend.Add(nomenclatureExtend);
        //            }

        //            demandExtend.recipient_attachments_extend = new List<AttachmentExtend>();
        //            foreach (var wsAtt in wsDemand.recipient_attachments_extend) {
        //                AttachmentExtend attachmentExtend = new AttachmentExtend();
        //                SetValues(wsAtt, attachmentExtend);
        //                //attachmentExtend.icon_bitmap = VmBase.LoadImage(attachmentExtend.file_icon);
        //                //File.WriteAllBytes(@"c:\temp\bit.bmp", attachmentExtend.file_icon);
        //                demandExtend.recipient_attachments_extend.Add(attachmentExtend);
        //            }

        //            return demandExtend;
        //        } catch (Exception ex) {
        //            throw ex;
        //        } finally {
        //            IsBusy = false;
        //        }
        //    });
        //}

        public async void OpenAttachmentGeneral(object sender) {
            StackPanel sp = (StackPanel)sender;
            ScmAttachment attachment = (ScmAttachment)sp.DataContext;

            var task = OpenAttachmentGeneralAsync(attachment.id, attachment.file_name);
            await task;
        }

        public Task OpenAttachmentGeneralAsync(int attId, string fileName) {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    byte[] attContent = WcfScm.GetAttachmentContent(attId);

                    string strFileName = GetTmpFileName(fileName);
                    File.WriteAllBytes(strFileName, attContent);
                    Process.Start(strFileName);
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        
        #endregion

        #region Methods
        protected DemandExtend GetDemand() {
            
            IsBusy = true;
            try {
                var demandExtend = WcfScm.GetDemandDetail(m_DemandId, CurrentUser.User.id);
                demandExtend.status_text = GetDemandStatusText(demandExtend.status_id);
                if (demandExtend.app_man_id != null) {
                    m_SelectedAppManId = (int)demandExtend.app_man_id;
                }

                if (demandExtend.demand_nomenclatures_extend != null) {
                    m_ScmDemandNomenclatures = new List<ScmDemandNomenclature>();
                    foreach (var demNom in demandExtend.demand_nomenclatures_extend) {
                        ScmDemandNomenclature scmDemandNomenclature = new ScmDemandNomenclature();
                        scmDemandNomenclature.other_demands = new List<Interface.WsScmDemand.DemandLight>();
                        
                        if (demNom.other_demands != null) {
                            foreach (var otherDem in demNom.other_demands) {
                                otherDem.status_text = GetDemandStatusText(otherDem.status_id);
                                Interface.WsScmDemand.DemandLight scmOtherDemand = new Interface.WsScmDemand.DemandLight();
                                SetValues(otherDem, scmOtherDemand);
                                scmDemandNomenclature.other_demands.Add(scmOtherDemand);
                            }
                        }
                        if (!String.IsNullOrWhiteSpace(demNom.remark)) {
                            if (demNom.remark.StartsWith(DemandRepository.LOCK_REASON_OTHERDEMAND)) {
                                demNom.remark = String.Format(ScmResource.NomenclatureWillBeEvaluatedAsPartOf, demNom.nomenclature_key, demNom.remark);
                            } else if (demNom.remark.StartsWith(DemandRepository.LOCK_REASON_PRICE_SET)) {
                                demNom.remark = String.Format(ScmResource.PriceWasSetConfirmed, demNom.remark.Replace(DemandRepository.LOCK_REASON_PRICE_SET, ""));
                            }
                        }

                        if (demNom.source_id == (int)NomSource.Custom) {
                            if (!String.IsNullOrWhiteSpace(demNom.remark)) {
                                demNom.remark += ", ";
                            }
                            demNom.remark += ScmResource.UserNomenclature;
                        }

                        SetValues(demNom, scmDemandNomenclature);
                        scmDemandNomenclature.status_text = GetNomenclatureStatusText(scmDemandNomenclature.status_id);
                        scmDemandNomenclature.copy_text = ScmResource.CopyNomKey;
                        
                        m_ScmDemandNomenclatures.Add(scmDemandNomenclature);
                    }
                }
                var tmpDemNom = m_ScmDemandNomenclatures;
                ScmDemandNomenclatures = null;
                ScmDemandNomenclatures = tmpDemNom;

                m_RecipientAttachment = new List<ScmAttachment>();
                foreach (var att in demandExtend.recipient_attachments_extend) {
                    ScmAttachment scmAttachment = new ScmAttachment();
                    SetValues(att, scmAttachment);
                    scmAttachment.icon = VmBase.LoadImage(att.file_icon);
                    if (IsAttachmentEditAllowed(demandExtend, att.att_type)) {
                        scmAttachment.deletebtn_visibility = Visibility.Visible;
                    } else {
                        scmAttachment.deletebtn_visibility = Visibility.Collapsed;
                    }
                    m_RecipientAttachment.Add(scmAttachment);
                }
                var tmpRecipAtt = m_RecipientAttachment;
                RecipientAttachment = null;
                RecipientAttachment = tmpRecipAtt;

                m_SupplierAttachment = new List<ScmAttachment>();
                foreach (var att in demandExtend.supplier_attachments_extend) {
                    //att.icon_bitmap = VmBase.LoadImage(att.file_icon);
                    ScmAttachment scmAttachment = new ScmAttachment();
                    SetValues(att, scmAttachment);
                    scmAttachment.icon = VmBase.LoadImage(att.file_icon);
                    if (IsAttachmentEditAllowed(demandExtend, att.att_type)) {
                        scmAttachment.deletebtn_visibility = Visibility.Visible;
                    } else {
                        scmAttachment.deletebtn_visibility = Visibility.Collapsed;
                    }
                    m_SupplierAttachment.Add(scmAttachment);
                }
                var tmpSuppAtt = m_SupplierAttachment;
                SupplierAttachment = null;
                SupplierAttachment = tmpSuppAtt;

                //set app man
                if (demandExtend.app_man_id != null) {
                    SelectedAppManName = WcfScm.GetUserName((int)demandExtend.app_man_id);
                }

                SetButtonsVisibility(demandExtend);
                SetAppManVisibility(demandExtend);
                

                return demandExtend;
            } catch (Exception ex) {
                throw ex;
            } finally {
                IsBusy = false;
            }

        }

        private void DisplayRemarks(RemarkExtend[] currRemarks) {
           // GetRemarks();

            if (currRemarks == null || currRemarks.Length == 0) {
                return;
            }

            if (m_RemarkGrid == null) {
                m_RemarkGrid = GetRemarkGrid();
            }
                        
            int iLoadedRemarkIndex = 0;
            if (m_RemarkExtend != null) {
                iLoadedRemarkIndex = m_RemarkExtend.Length;
            }

            for (int i= iLoadedRemarkIndex; i < currRemarks.Length; i++) {
                
                int iBrushIndex = 0;
                if (m_UserBrush.ContainsKey(currRemarks[i].modif_user_id)) {
                    iBrushIndex = (int)m_UserBrush[currRemarks[i].modif_user_id];
                } else {
                    if (m_UserBrush.Count == brushesUser.Length) {
                        iBrushIndex = GetRandomBrushIndex();
                        m_UserBrush.Add(currRemarks[i].modif_user_id, iBrushIndex);
                    } else {

                        bool isGoNext = true;
                        while (isGoNext) {
                            iBrushIndex = GetRandomBrushIndex();
                            IDictionaryEnumerator iEnum = m_UserBrush.GetEnumerator();
                            isGoNext = false;
                            while (iEnum.MoveNext()) {
                                if ((int)iEnum.Value == iBrushIndex) {
                                    isGoNext = true;
                                    break;
                                }
                            }

                            
                        }
                        m_UserBrush.Add(currRemarks[i].modif_user_id, iBrushIndex);

                    }
                }

                var brushUser = brushesUser[iBrushIndex];
                var brushRemark = brushesRemark[iBrushIndex];

                RowDefinition row1 = new RowDefinition();
                row1.Height = GridLength.Auto;
                m_RemarkGrid.RowDefinitions.Add(row1);

                Border borderUser = new Border();
                borderUser.Height = 48;
                borderUser.Width = 48;
                borderUser.VerticalAlignment = VerticalAlignment.Top;
                borderUser.HorizontalAlignment = HorizontalAlignment.Left;
                
                borderUser.BorderBrush = brushUser;
                borderUser.Background = brushUser;
                borderUser.BorderThickness = new Thickness(0);
                borderUser.CornerRadius = new CornerRadius(24);
                borderUser.Margin = new Thickness(10,10,5,0);

                Label lblUserInitials = new Label();
                lblUserInitials.Content = currRemarks[i].user_firstname.Substring(0, 1).ToUpper() + currRemarks[i].user_surname.Substring(0, 1).ToUpper();
                lblUserInitials.VerticalAlignment = VerticalAlignment.Center;
                lblUserInitials.HorizontalAlignment = HorizontalAlignment.Center;
                lblUserInitials.Foreground = new SolidColorBrush(Colors.White);
                lblUserInitials.FontSize = 14;
                lblUserInitials.FontWeight = FontWeights.Bold;
                borderUser.Child = lblUserInitials;

                Grid.SetRow(borderUser, i);
                Grid.SetColumn(borderUser, 0);
                m_RemarkGrid.Children.Add(borderUser);

                Border borderRemark = new Border();
                borderRemark.BorderThickness = new Thickness(1);
                borderRemark.BorderBrush = brushUser;
                borderRemark.Background = brushRemark;
                borderRemark.CornerRadius = new CornerRadius(8);
                borderRemark.HorizontalAlignment = HorizontalAlignment.Left;
                borderRemark.VerticalAlignment = VerticalAlignment.Top;
                borderRemark.Margin = new Thickness(0, 10, 0, 10);
                borderRemark.Padding = new Thickness(15);

                StackPanel spRemark = new StackPanel();

                TextBlock tbRemarkInfo = new TextBlock();
                tbRemarkInfo.Text = currRemarks[i].user_firstname + " " + currRemarks[i].user_surname 
                    + "(" + currRemarks[i].modif_date.ToString("dd.MM.yyyy HH:mm") + ")";
                tbRemarkInfo.FontWeight = FontWeights.Bold;
                spRemark.Children.Add(tbRemarkInfo);

                TextBlock tbRemark = new TextBlock();
                tbRemark.TextWrapping = TextWrapping.Wrap;
                tbRemark.Text = currRemarks[i].remark_text;

                spRemark.Children.Add(tbRemark);

                borderRemark.Child = spRemark;


                Grid.SetRow(borderRemark, i);
                Grid.SetColumn(borderRemark, 1);

                m_RemarkGrid.Children.Add(borderRemark);

                m_RemarkExtend = currRemarks;
            }
        }

        private int GetRandomBrushIndex() {
            int iIndex = new Random().Next(0, brushesUser.Length - 1);

            return iIndex;
        }

        private Grid GetRemarkGrid() {
            //Grid grid = (Grid)VisualTreeHelper.GetChild(m_DpRemark, 0);
            //ColumnDefinition col1 = new ColumnDefinition();
            //col1.Width = GridLength.Auto;
            //grid.ColumnDefinitions.Add(col1);

            //ColumnDefinition col2 = new ColumnDefinition();
            //col2.Width = GridLength.Auto;
            //grid.ColumnDefinitions.Add(col2);

            //grid.ColumnDefinitions.Add(new ColumnDefinition());

            return m_RemarkGrid;
        }

        private bool IsAttachmentEditAllowed(DemandExtend demandExtend, int attType) {
            if (CurrentUser.User.id == demandExtend.requestor_id
                && CurrentUser.IsScmReferent
                        && attType != AttachmentRepository.ATT_TYPE_MAIL_RECIPIENT
                        && attType != AttachmentRepository.ATT_TYPE_GENERATED_DEMAND
                        && (demandExtend.status_id == DemandRepository.DEM_STATUS_DRAFT
                        || demandExtend.status_id == DemandRepository.DEM_STATUS_SENT
                        || demandExtend.status_id == DemandRepository.DEM_STATUS_SUPPLIER_REPLIED
                        || demandExtend.status_id == DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL
                        || demandExtend.status_id == DemandRepository.DEM_STATUS_REJECTED)) {
                return true;
            }

            return false;
        }

        private void SetButtonsVisibility(DemandExtend demandExtend) {
            bool isHasActiveNomenclatures = IsHasDemandActiveNomenclatures(demandExtend);
            bool isHasNomenclaturesWaitForApp = IsHasDemandNomenclaturesInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_WAIT_FOR_APPROVAL);
            bool isHasNomenclaturesSupplierReplied = IsHasDemandNomenclaturesInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_SUPPLIER_REPLIED);
            bool isHasNomenclaturesApproved = IsHasDemandNomenclaturesInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_APPROVED);
            bool isHasNomenclaturesRejected = IsHasDemandNomenclaturesInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_REJECTED);
            bool isHasNomenclaturesSent = IsHasDemandNomenclaturesInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_SENT_TO_SUPPLIER);
            bool isHasNomenclaturesPriceSet = IsHasDemandNomenclaturesInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_PRICE_SET);
            bool isHasNomenclaturesNotPriceConfirmed = IsHasDemandNomenclaturesNotInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_PRICE_CONFIRMED);
            bool isHasNomenclaturesNotPriceSet = IsHasDemandNomenclaturesNotInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_PRICE_SET);

            //Send For Approval
            if (CurrentUser.User.id == demandExtend.requestor_id 
                && CurrentUser.IsScmReferent 
                && (isHasNomenclaturesSent || isHasNomenclaturesSupplierReplied || isHasNomenclaturesRejected)
                ) {
                BtnSendForApprovalVisibility = Visibility.Visible;
            } else {
                BtnSendForApprovalVisibility = Visibility.Collapsed;
            }

            //Back
            if (CurrentUser.User.id == demandExtend.requestor_id 
                && CurrentUser.IsScmReferent 
                && (isHasNomenclaturesSupplierReplied 
                || isHasNomenclaturesWaitForApp
                || isHasNomenclaturesRejected
                || isHasNomenclaturesPriceSet)
                ) {
                BtnBackVisibility = Visibility.Visible;
            } else if (CurrentUser.User.id == demandExtend.app_man_id && CurrentUser.IsApproveManager 
                && (isHasNomenclaturesApproved || isHasNomenclaturesRejected)) {
                BtnBackVisibility = Visibility.Visible;
            } else {
                BtnBackVisibility = Visibility.Collapsed;
            }

            //Take Over
            if ((CurrentUser.User.id != demandExtend.requestor_id && CurrentUser.IsScmReferent)
                || (demandExtend.app_man_id != null && CurrentUser.User.id != demandExtend.app_man_id && CurrentUser.IsApproveManager)) {
                BtnTakeOverVisibility = Visibility.Visible;
            } else {
                BtnTakeOverVisibility = Visibility.Collapsed;
            }

            //Approve && Reject
            if (CurrentUser.User.id == demandExtend.app_man_id && CurrentUser.IsApproveManager 
                && isHasNomenclaturesWaitForApp) {
                BtnApproveVisibility = Visibility.Visible;
            } else {
                BtnApproveVisibility = Visibility.Collapsed;
            }

            //finish
            if (CurrentUser.User.id == demandExtend.requestor_id 
                && CurrentUser.IsScmReferent
                && demandExtend.status_id != (int)DemandRepository.Status.PriceConfirmed
                && demandExtend.status_id != (int)DemandRepository.Status.PriceSet
                && demandExtend.status_id != (int)DemandRepository.Status.Closed) {
                BtnFinishVisibility = Visibility.Visible;
            } else {
                BtnFinishVisibility = Visibility.Collapsed;
            }

            //Activate
            if (CurrentUser.User.id == demandExtend.requestor_id
                && CurrentUser.IsScmReferent
                && demandExtend.status_id == (int)DemandRepository.Status.Closed) {
                BtnActivateVisibility = Visibility.Visible;
            } else {
                BtnActivateVisibility = Visibility.Collapsed;
            }

            //Price Was Set
            if (CurrentUser.User.id == demandExtend.requestor_id && CurrentUser.IsScmReferent 
                && (isHasNomenclaturesNotPriceConfirmed || isHasNomenclaturesNotPriceSet)) {
                BtnPriceWasSetVisibility = Visibility.Visible;
            } else {
                BtnPriceWasSetVisibility = Visibility.Collapsed;
            }

            //Delete
            BtnDeleteVisibility = Visibility.Collapsed;
            if (CurrentUser.User.id == demandExtend.requestor_id && CurrentUser.IsScmReferent) {
                BtnDeleteVisibility = Visibility.Visible;
            } else {
                BtnDeleteVisibility = Visibility.Collapsed;
            }

           
        }

        private void SetAppManVisibility(DemandExtend demandExtend) {
            bool isWaitForRefProcessItems = IsHasDemandNomenclaturesInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_LOADED)
                || IsHasDemandNomenclaturesInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_SENT_TO_SUPPLIER)
                || IsHasDemandNomenclaturesInStatus(demandExtend, NomenclatureRepository.NOM_STATUS_SUPPLIER_REPLIED);

            //App Man
            if (!CurrentUser.IsScmReferent
                || CurrentUser.User.id != demandExtend.requestor_id
                || !isWaitForRefProcessItems) { 
                
                CmbAppManVisibility = Visibility.Collapsed;
                TxtAppManVisibility = Visibility.Visible;
            } else {
                CmbAppManVisibility = Visibility.Visible;
                TxtAppManVisibility = Visibility.Collapsed;
            }

            //switch (demandExtend.status_id) {
            //    case DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL:
            //    case DemandRepository.DEM_STATUS_APPROVED:
            //    case DemandRepository.DEM_STATUS_REJECTED:
            //    case DemandRepository.DEM_STATUS_PRICE_SET:
            //    case DemandRepository.DEM_STATUS_PRICE_CONFIRMED:
            //        CmbAppManVisibility = Visibility.Collapsed;
            //        TxtAppManVisibility = Visibility.Visible;
            //        break;
            //    default:
            //        CmbAppManVisibility = Visibility.Visible;
            //        TxtAppManVisibility = Visibility.Collapsed;
            //        break;
            //}

            ////WcfScm.GetUserData()
            ////SelectedAppManName
        }

        private void EnableDragDrop() {
            if (DemandExtend.requestor_id == CurrentUser.User.id 
                && CurrentUser.IsScmReferent
               && (DemandExtend.status_id == DemandRepository.DEM_STATUS_DRAFT
                        || DemandExtend.status_id == DemandRepository.DEM_STATUS_SENT
                        || DemandExtend.status_id == DemandRepository.DEM_STATUS_SUPPLIER_REPLIED
                        || DemandExtend.status_id == DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL
                        || DemandExtend.status_id == DemandRepository.DEM_STATUS_REJECTED)
                ) {
                IsAllowDragDrop = true;
            } else {
                IsAllowDragDrop = false;
            }
        }

        private bool IsHasDemandActiveNomenclatures(DemandExtend demand) {
            if (demand.Demand_Nomenclature == null || demand.demand_nomenclatures_extend.Count == 0) {
                return false;
            }

            foreach (var demNom in demand.demand_nomenclatures_extend) {
                if (demNom.locked_by_demand_id == null) {
                    return true;
                }
            }

            return false;
        }

        private bool IsHasDemandNomenclaturesInStatus(DemandExtend demand, int statusId) {
            if (demand.Demand_Nomenclature == null || demand.demand_nomenclatures_extend.Count == 0) {
                return false;
            }

            foreach (var demNom in demand.demand_nomenclatures_extend) {
                if (demNom.locked_by_demand_id == null && demNom.status_id == statusId && demNom.is_locked == false) {
                    return true;
                }
            }

            return false;
        }

        private bool IsHasDemandNomenclaturesNotInStatus(DemandExtend demand, int statusId) {
            if (demand.Demand_Nomenclature == null || demand.demand_nomenclatures_extend.Count == 0) {
                return true;
            }

            foreach (var demNom in demand.demand_nomenclatures_extend) {
                if (demNom.locked_by_demand_id == null && demNom.status_id == statusId) {
                    return false;
                }
            }

            return true;
        }

        public void DropAttachment(DragEventArgs e, int attType) {

            List<DropFile> dropFiles = GetDropAttachment(e);

            foreach (var dropFile in dropFiles) {
                try {
                    AttachmentExtend attachmentExtend = new AttachmentExtend();
                    attachmentExtend.file_name = dropFile.FileName;
                    //attachmentExtend.file_content = dropFile.FileContent;
                    attachmentExtend.file_icon = VmBase.GetBytesFromBitmapSource(dropFile.Icon);

                    int attId = WcfScm.AddAttachment(
                        m_DemandId,
                        dropFile.FileName,
                        dropFile.FileContent,
                        VmBase.GetBytesFromBitmapSource(dropFile.Icon),
                        attType,
                        CurrentUser.User.id);

                    ScmAttachment scmAttachment = new ScmAttachment();
                    scmAttachment.file_name = dropFile.FileName;
                    scmAttachment.icon = dropFile.Icon;
                    scmAttachment.id = attId;
                    scmAttachment.added_by = "( " + CurrentUser.User.surname + " " + CurrentUser.User.first_name + " )";
                    scmAttachment.att_type = attType;
                    //attachmentExtend.id = attId;
                    ////attachmentExtend.icon_bitmap = dropFile.Icon;

                    if (attType == AttachmentRepository.ATT_TYPE_RECIPIENT) {
                        // DemandExtend.recipient_attachments_extend.Add(attachmentExtend);
                        RecipientAttachment.Add(scmAttachment);
                        var tmp = RecipientAttachment;
                        RecipientAttachment = null;
                        RecipientAttachment = tmp;
                    } else if (attType == AttachmentRepository.ATT_TYPE_SUPPLIER) {
                        
                        SupplierAttachment.Add(scmAttachment);
                        var tmp = SupplierAttachment;
                        SupplierAttachment = null;
                        SupplierAttachment = tmp;
                        if (DemandExtend.status_id == (int)DemandRepository.Status.Sent) {
                            DemandExtend.status_id = (int)DemandRepository.Status.Replied;
                            DemandExtend.status_text = GetDemandStatus(DemandExtend.status_id);
                            DemandExtend.img_status_path = DemandRepository.GetStatusImagePath(DemandExtend.status_id);
                            DemandExtend = null;
                            LoadDemandDetail();

                            m_DlgRefreshDashboard?.Invoke();
                        }
                    }

                    SetButtonsVisibility(m_DemandExtend);
                } catch (Exception ex) {
                    HandleError(ex);
                }
            }
        }

        //private ScmAttachment CopyAttachment(ScmAttachment sourceAtt) {
        //    ScmAttachment scmAttachment = new ScmAttachment();
        //    scmAttachment.file_name = sourceAtt.file_name;
        //    scmAttachment.file_content = sourceAtt.file_content;
        //    scmAttachment.icon = sourceAtt.icon;
        //    scmAttachment.att_type = sourceAtt.att_type;
        //    scmAttachment.id = DataNulls.INT_NULL;

        //    return scmAttachment;

        //    //int attId = WcfScm.AddAttachment(
        //    //    targetDemandId,
        //    //    sourceAtt.file_name,
        //    //    sourceAtt.file_content,
        //    //    VmBase.GetBytesFromBitmapSource(sourceAtt.icon),
        //    //    sourceAtt.att_type,
        //    //    CurrentUser.User.id);

        //    //ScmAttachment scmAttachment = new ScmAttachment();
        //    //scmAttachment.file_name = sourceAtt.file_name;
        //    //scmAttachment.icon = sourceAtt.icon;
        //    //scmAttachment.id = attId;
        //    //scmAttachment.added_by = "( " + CurrentUser.User.surname + " " + CurrentUser.User.first_name + " )";
        //    //scmAttachment.att_type = sourceAtt.att_type;


        //    //if (sourceAtt.att_type == AttachmentRepository.ATT_TYPE_RECIPIENT) {
        //    //    // DemandExtend.recipient_attachments_extend.Add(attachmentExtend);
        //    //    RecipientAttachment.Add(scmAttachment);
        //    //    var tmp = RecipientAttachment;
        //    //    RecipientAttachment = null;
        //    //    RecipientAttachment = tmp;
        //    //} else if (sourceAtt.att_type == AttachmentRepository.ATT_TYPE_SUPPLIER) {

        //    //    SupplierAttachment.Add(scmAttachment);
        //    //    var tmp = SupplierAttachment;
        //    //    SupplierAttachment = null;
        //    //    SupplierAttachment = tmp;
        //    //    if (DemandExtend.status_id == (int)DemandRepository.Status.Sent) {
        //    //        DemandExtend.status_id = (int)DemandRepository.Status.Replied;
        //    //        DemandExtend.status_text = GetDemandStatus(DemandExtend.status_id);
        //    //        DemandExtend.img_status_path = DemandRepository.GetStatusImagePath(DemandExtend.status_id);
        //    //        DemandExtend = null;
        //    //        LoadDemandDetail();

        //    //        m_DlgRefreshDashboard?.Invoke();
        //    //    }
        //    //}

        //    //SetButtonsVisibility(m_DemandExtend);
        //}

        public void SendForApproval() {
            if (!IsNomSelected()) {
                MessageBox.Show(ScmResource.SelectNomenclatures, ScmResource.SelectNomenclatures, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!IsValid()) {
                return;
            }

            var selNoms = (from lstNom in m_ScmDemandNomenclatures
                           where lstNom.is_selected == true
                           select lstNom).ToList();

            int[] selIds = new int[selNoms.Count];
            int[] nomStatusesId = new int[selNoms.Count];
            for (int i=0; i< selNoms.Count; i++) {
                selIds[i] = (selNoms[i].nomenclature_id);
                nomStatusesId[i] = (int)DemandRepository.Status.WaitForApproval;
                selNoms[i].IsSentForApproval = true;
            }

            WcfScm.SaveNomenclaturesStatus(m_DemandId, selIds, nomStatusesId, CurrentUser.User.id);

            //Sent mail info
            SendApprovalRequest();

            DemandExtend = null;
            LoadDemandDetail();
            SetButtonsVisibility(DemandExtend);

            //m_DlgClosewindow?.Invoke();
            m_DlgRefreshDashboard?.Invoke();
        }

        private void SendApprovalRequest() {
            //Sent mail info
            WsMail.OtWsMail wsMail = new WsMail.OtWsMail();
            string strSender = CurrentUser.User.email;
            string strRecipient = WcfScm.GetUserMail((int)m_DemandExtend.app_man_id);
            string strBcc = "kamil.sykora@otis.com";
            string strSubject = "SCM Demand - žádost o schválení poptávky č." + m_DemandExtend.demand_nr;
            string strBody = CurrentUser.User.first_name + " " + CurrentUser.User.surname + " Vás žádá o schválení poptávky č." + m_DemandExtend.demand_nr
                + Environment.NewLine
                + "Otevřete si prosím aplikaci SCM Demand a poptávku schvalte nebo zamítněte."
                + Environment.NewLine
                + Environment.NewLine
                + "Instalační balíček aplikace je dostupný na této adrese: http://intranetcz.cz.eu.otis.utc.com/ScmDemandInstall/publish.htm";

#if DEBUG
            strRecipient = "kamil.sykora@otis.com";
#endif

#if !DEBUG
            wsMail.SendMailBcc(
                strSender,
                strRecipient,
                null,
                strBcc,
                strSubject,
                strBody,
                null,
                (int)System.Net.Mail.MailPriority.Normal);
#endif
        }


        public void Approve() {
            try {
                if (!IsNomSelected()) {
                    MessageBox.Show(ScmResource.SelectNomenclatures, ScmResource.SelectNomenclatures, MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (!IsValid()) {
                    return;
                }

                var selNoms = (from lstNom in m_ScmDemandNomenclatures
                               where lstNom.is_selected == true
                               select lstNom).ToList();

                int[] selIds = new int[selNoms.Count];
                int[] nomStatusesId = new int[selNoms.Count];
                for (int i = 0; i < selNoms.Count; i++) {
                    selIds[i] = (selNoms[i].nomenclature_id);
                    nomStatusesId[i] = (int)DemandRepository.Status.Approved;
                }

                WcfScm.SaveNomenclaturesStatus(m_DemandId, selIds, nomStatusesId, CurrentUser.User.id);

                DemandExtend = null;
                LoadDemandDetail();
                SetButtonsVisibility(DemandExtend);

                //m_DlgClosewindow?.Invoke();
                m_DlgRefreshDashboard?.Invoke();

                //m_DemandExtend.status_id = (int)DemandRepository.Status.Approved;
                //SaveDemandRefresh();

                //m_DlgClosewindow?.Invoke();
                //m_DlgRefreshDashboard?.Invoke();
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        public void Reject() {
            if (!IsNomSelected()) {
                MessageBox.Show(ScmResource.SelectNomenclatures, ScmResource.SelectNomenclatures, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!IsValid()) {
                return;
            }

            var selNoms = (from lstNom in m_ScmDemandNomenclatures
                           where lstNom.is_selected == true
                           select lstNom).ToList();

            int[] selIds = new int[selNoms.Count];
            int[] nomStatusesId = new int[selNoms.Count];
            for (int i = 0; i < selNoms.Count; i++) {
                selIds[i] = (selNoms[i].nomenclature_id);
                nomStatusesId[i] = (int)DemandRepository.Status.Rejected;
            }

            WcfScm.SaveNomenclaturesStatus(m_DemandId, selIds, nomStatusesId, CurrentUser.User.id);

            DemandExtend = null;
            LoadDemandDetail();
            SetButtonsVisibility(DemandExtend);

            m_DlgRefreshDashboard?.Invoke();

        }

        public void PriceSet() {
            if (!IsNomSelected()) {
                MessageBox.Show(ScmResource.SelectNomenclatures, ScmResource.SelectNomenclatures, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!IsValid()) {
                return;
            }

            var selNoms = (from lstNom in m_ScmDemandNomenclatures
                           where lstNom.is_selected == true
                           select lstNom).ToList();

            int[] selIds = new int[selNoms.Count];
            int[] nomStatusesId = new int[selNoms.Count];
            for (int i = 0; i < selNoms.Count; i++) {
                selIds[i] = (selNoms[i].nomenclature_id);
                nomStatusesId[i] = (int)DemandRepository.Status.PriceSet;
            }

            WcfScm.SaveNomenclaturesStatus(m_DemandId, selIds, nomStatusesId, CurrentUser.User.id);

            DemandExtend = null;
            LoadDemandDetail();
            SetButtonsVisibility(DemandExtend);

            m_DlgRefreshDashboard?.Invoke();

        }

        
        public void Revert() {
            if (!IsNomSelected()) {
                MessageBox.Show(ScmResource.SelectNomenclatures, ScmResource.SelectNomenclatures, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selNoms = (from lstNom in m_ScmDemandNomenclatures
                           where lstNom.is_selected == true
                           select lstNom).ToList();

            int[] selIds = new int[selNoms.Count];
            int[] nomStatusesId = new int[selNoms.Count];
            for (int i = 0; i < selNoms.Count; i++) {
                selIds[i] = (selNoms[i].nomenclature_id);

                switch (selNoms[i].status_id) {
                    case NomenclatureRepository.NOM_STATUS_SUPPLIER_REPLIED:
                        nomStatusesId[i] = (int)DemandRepository.Status.Sent;
                        break;
                    case NomenclatureRepository.NOM_STATUS_WAIT_FOR_APPROVAL:
                        nomStatusesId[i] = (int)DemandRepository.Status.Replied;
                        break;
                    case NomenclatureRepository.NOM_STATUS_APPROVED:
                        nomStatusesId[i] = (int)DemandRepository.Status.WaitForApproval;
                        break;
                    case NomenclatureRepository.NOM_STATUS_REJECTED:
                        if (m_DemandExtend.app_man_id == CurrentUser.User.id && CurrentUser.IsApproveManager) {
                            nomStatusesId[i] = (int)DemandRepository.Status.WaitForApproval;
                        } else if (m_DemandExtend.requestor_id == CurrentUser.User.id && CurrentUser.IsScmReferent) {
                            nomStatusesId[i] = (int)DemandRepository.Status.Replied;
                        }
                        break;
                    case NomenclatureRepository.NOM_STATUS_PRICE_SET:
                        int prevStatus = WcfScm.GetDemandNomPrevStatus(m_DemandExtend.id, selNoms[i].nomenclature_id);
                        nomStatusesId[i] = prevStatus;
                        break;
                    default:
                        throw new Exception("Unknown Demand Revert Status");
                }
            }

            WcfScm.SaveNomenclaturesStatus(m_DemandId, selIds, nomStatusesId, CurrentUser.User.id);

            //int prevStatusId = WcfScm.GetDemandPreviousStatus(m_DemandExtend.id);
            //if (prevStatusId == m_DemandExtend.status_id) {
            //    return;
            //}
            //m_DemandExtend.status_id = prevStatusId;
            //SaveDemandRefresh();

            DemandExtend = null;
            LoadDemandDetail();
            SetButtonsVisibility(DemandExtend);

            //m_DlgClosewindow?.Invoke();
            m_DlgRefreshDashboard?.Invoke();
        }

        public void RemoveNomenclature(ScmDemandNomenclature demandNomenclature) {
            if (this.DemandExtend.requestor_id != CurrentUser.User.id || !CurrentUser.IsScmReferent) {
                MessageBox.Show("Demand Nomenclature cannot be deleted");
                return;
            }

            WcfScm.RemoveDemandNomenclature(demandNomenclature.demand_id, demandNomenclature.nomenclature_id, CurrentUser.User.id);
            DemandExtend = GetDemand();
            m_DlgRefreshDashboard?.Invoke();
        }
        

        //public void PriceSet() {
        //    if (!IsValid()) {
        //        return;
        //    }

        //    m_DemandExtend.status_id = (int)DemandRepository.Status.PriceSet;
        //    m_DemandExtend.is_active = false;
        //    SaveDemandRefresh();
        //    SetButtonsVisibility(m_DemandExtend);

        //    m_DlgClosewindow?.Invoke();
        //    m_DlgRefreshDashboard?.Invoke();
        //}

        public void TakeOver() {
            try {
                bool isTakeOverAllowed = false;

                if (CurrentUser.IsScmReferent) {
                    //switch (DemandExtend.status_id) {
                    //    case DemandRepository.DEM_STATUS_SENT:
                    //    case DemandRepository.DEM_STATUS_SUPPLIER_REPLIED:
                    //    case DemandRepository.DEM_STATUS_REJECTED:
                    //    case DemandRepository.DEM_STATUS_APPROVED:
                    //    case DemandRepository.DEM_STATUS_PRICE_SET:
                    //    case DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL:
                    //        isTakeOverAllowed = true;
                    //        break;
                    //}
                    isTakeOverAllowed = true;
                }

                if (CurrentUser.IsApproveManager) {
                    switch (DemandExtend.status_id) {
                        case DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL:
                            isTakeOverAllowed = true;
                            break;
                    }
                }

                if (!isTakeOverAllowed) {
                    MessageBox.Show(ScmResource.CannotTakeOverDemand, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (CurrentUser.IsScmReferent) {
                    m_DemandExtend.requestor_id = (int)CurrentUser.User.id;
                }

                if (CurrentUser.IsApproveManager) {
                    SelectedAppManId = (int)CurrentUser.User.id;
                    m_DemandExtend.app_man_id = (int)CurrentUser.User.id;
                }

                SaveDemandRefresh();
                SetButtonsVisibility(DemandExtend);
                EnableDragDrop();

                //m_DlgClosewindow?.Invoke();
                m_DlgRefreshDashboard?.Invoke();
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        public void FinishDemand() {
            try {
                if (!CurrentUser.IsScmReferent) {
                    MessageBox.Show(ScmResource.YouAreNotAuthorized, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                m_DemandExtend.status_id = (int)DemandRepository.Status.Closed;
                SaveDemandRefresh();
                SetButtonsVisibility(DemandExtend);

                //m_DlgClosewindow?.Invoke();
                m_DlgRefreshDashboard?.Invoke();
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        public void ActivateDemand() {
            try {
                if (!CurrentUser.IsScmReferent) {
                    MessageBox.Show(ScmResource.YouAreNotAuthorized, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                int prevStatus = WcfScm.GetDemandPreviousStatus(DemandExtend.id);
                m_DemandExtend.status_id = prevStatus;
                SaveDemandRefresh();
                SetButtonsVisibility(DemandExtend);
                                
                m_DlgRefreshDashboard?.Invoke();
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        public void CopyDemand(out List<NomenclatureExtend> nomenclatures, out List<ScmAttachment> scmAttachments) {
            nomenclatures = new List<NomenclatureExtend>();
            scmAttachments = new List<ScmAttachment>();

            try {
                                
                if (ScmDemandNomenclatures != null) {
                    foreach (var demNom in ScmDemandNomenclatures) {
                        NomenclatureExtend nomenclatureExtend = new NomenclatureExtend();
                        nomenclatureExtend.id = demNom.nomenclature_id;
                        nomenclatureExtend.nomenclature_key = demNom.nomenclature_key;
                        nomenclatureExtend.name = demNom.name;
                        nomenclatureExtend.specification = demNom.specification;

                        nomenclatures.Add(nomenclatureExtend);
                    }
                }

                if (RecipientAttachment != null) {
                    
                    foreach (var att in RecipientAttachment) {
                        try {
                            if (att.att_type == AttachmentRepository.ATT_TYPE_GENERATED_DEMAND) {
                                continue;
                            }

                            ScmAttachment scmAttachment = new ScmAttachment();
                            scmAttachment.file_name = att.file_name;
                            scmAttachment.file_content = WcfScm.GetAttachmentContent(att.id); 
                            scmAttachment.icon = att.icon;
                            scmAttachment.att_type = att.att_type;
                            scmAttachment.id = DataNulls.INT_NULL;

                            scmAttachments.Add(scmAttachment);
                        } catch (Exception ex) {
                            MessageBox.Show(String.Format(ScmResource.AttachmentCopyFailed, att.file_name) 
                                + Environment.NewLine + Environment.NewLine + ex.Message, 
                                ScmResource.ErrorTitle, 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Exclamation);
                        }
                    }
                }
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        
        private bool IsNomSelected() {
            var selNoms = (from lstNom in m_ScmDemandNomenclatures
                           where lstNom.is_selected == true
                           select lstNom).FirstOrDefault();
            if (selNoms != null) {
                return true;
            }

            return false;
        }

        public bool IsValid() {

            if (!IsAppManValid(SelectedAppManId)) {
                HasErrors = true;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SelectedAppManId"));
                return false;

            }

            var selNoms = (from lstNom in m_ScmDemandNomenclatures
                           where lstNom.is_selected == true
                           select lstNom).ToList();

            bool isDemNomValid = true;
            foreach (var selNom in selNoms) {
                if (!selNom.IsValid()) {
                    isDemNomValid = false;
                }
            }
            if (!isDemNomValid) {
                return false;
            }

            return true;
        }

        private bool IsAppManValid(int appManId) {
            if (appManId == DataNulls.INT_NULL) {
                return false;
            }

            return true;
        }

        public void SelectAllDemNoms(bool isSelect) {

            foreach (var demNom in ScmDemandNomenclatures) {
                if (demNom.select_visibility == Visibility.Collapsed) {
                    continue;
                }

                demNom.is_selected = isSelect;
            }

            var tmpDemNom = m_ScmDemandNomenclatures;
            ScmDemandNomenclatures = null;
            ScmDemandNomenclatures = tmpDemNom;
        }

        //private bool IsPriceValid() {
        //    var selNoms = (from lstNom in m_DemandExtend.demand_nomenclatures_extend
        //                   where lstNom.is_selected == true
        //                   select lstNom).ToList();

        //    foreach (var selNom in selNoms) {
        //        if(String.IsNullOrWhiteSpace(selNom.price_text)) {
        //            return false;
        //        }
        //        if (selNom.currency_id == null && selNom.currency_id < 0) {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        public void SaveDemandNomenclaturePrice(object sender) {
            TextBox txt = (TextBox)sender;

            if (ValidateDecimalTextBox(txt)) {
                ScmDemandNomenclature scmNomExt = (ScmDemandNomenclature)txt.DataContext;
                if (scmNomExt.price_text != scmNomExt.price_text_orig) {
                    DemandNomenclatureExtend nomExt = new DemandNomenclatureExtend();
                    //nomExt.other_demands = new List<DemandLight>();
                    SetValues(scmNomExt, nomExt);

                    WcfScm.SaveDemandNomenclature(nomExt, CurrentUser.User.id);
                    scmNomExt.price_text_orig = scmNomExt.price_text;
                }
            }
        }

        public void SaveDemandNomenclatureCurrency(object sender) {
            ComboBox cmb = (ComboBox)sender;
           
            ScmDemandNomenclature scmNomExt = (ScmDemandNomenclature)cmb.DataContext;
            if (scmNomExt.currency_id != scmNomExt.currency_id_orig) {
                DemandNomenclatureExtend nomExt = new DemandNomenclatureExtend();
                SetValues(scmNomExt, nomExt);
                WcfScm.SaveDemandNomenclature(nomExt, CurrentUser.User.id);
                scmNomExt.currency_id_orig = scmNomExt.currency_id;
            }
        }

        public bool ValidateDecimalTextBox(TextBox textBox) {
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);

            BindingExpressionBase bindingExpressionBase = BindingOperations.GetBindingExpressionBase(
                textBox, 
                TextBox.TextProperty);

            ValidationError validationError = new ValidationError(
                new ExceptionValidationRule(), 
                bindingExpression);

            bool isValid = true;
            if (!String.IsNullOrWhiteSpace(textBox.Text)) {
                string uiSep = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
                string strDecimalFix = textBox.Text.Replace(",", uiSep);
                decimal d;
                isValid = Decimal.TryParse(strDecimalFix, out d);
            }

            if (isValid) {
                Validation.ClearInvalid(bindingExpressionBase);
            } else {
                Validation.MarkInvalid(bindingExpressionBase, validationError);
            }


            return isValid;

            //ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("price_text"));

            //        //ValidationRule vr = new ValidationRule();
            //        ValidationError validationError =    new ValidationError(regexValidationRule,
            //textBox.GetBindingExpression(TextBox.TextProperty));

            //        validationError.ErrorContent = "This is not a valid e-mail address";
            //        Validation.MarkInvalid(
            //textBox.GetBindingExpression(TextBox.TextProperty),
            //validationError);
        }

        private void SaveDemandRefresh() {
            WcfScm.SaveDemand(DemandExtend, CurrentUser.User.id);
            DemandExtend = GetDemand();
        }

        public void RemoveAtt(object sender) {
            try {
                Button btn = (Button)sender;
                ScmAttachment scmAttachment = (ScmAttachment)btn.DataContext;

                if (scmAttachment.att_type == AttachmentRepository.ATT_TYPE_MAIL_RECIPIENT
                    || scmAttachment.att_type == AttachmentRepository.ATT_TYPE_GENERATED_DEMAND) {
                    return;
                }

                WcfScm.DeleteAttachment(m_DemandId, scmAttachment.id, CurrentUser.User.id);

                if (scmAttachment.att_type == AttachmentRepository.ATT_TYPE_SUPPLIER) {
                    SupplierAttachment.Remove(scmAttachment);
                    var tmpAtts = SupplierAttachment;
                    SupplierAttachment = null;
                    SupplierAttachment = tmpAtts;
                } else if (scmAttachment.att_type == AttachmentRepository.ATT_TYPE_RECIPIENT) {
                    RecipientAttachment.Remove(scmAttachment);
                    var tmpAtts = RecipientAttachment;
                    RecipientAttachment = null;
                    RecipientAttachment = tmpAtts;
                }
            } catch(Exception ex) {
                HandleError(ex);
            }

        }

        //public void CancelDemand() {
        //    try {
        //        if (!CurrentUser.IsScmReferent || m_DemandExtend.requestor_id != CurrentUser.User.id) {
        //            MessageBox.Show(ScmResource.NotAuthorized, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //            return;
        //        }

        //        m_DemandExtend.status_id = (int)DemandRepository.Status.Canceled;
        //        //m_DemandExtend.is_active = false;
        //        SaveDemandRefresh();
        //        SetButtonsVisibility(m_DemandExtend);

        //        m_DlgClosewindow?.Invoke();
        //        m_DlgRefreshDashboard?.Invoke();
                                
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    }
        //}

        public void DeleteDemand() {
            try {
                if (!CurrentUser.IsScmReferent || m_DemandExtend.requestor_id != CurrentUser.User.id) {
                    MessageBox.Show(ScmResource.NotAuthorized, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string strMsg = String.Format(ScmResource.DemandDeleteConfirmation, m_DemandExtend.demand_nr);
                MessageBoxResult res = MessageBox.Show(strMsg, ScmResource.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (res != MessageBoxResult.Yes) {
                    return;
                }

                m_DemandExtend.last_version = false;
                WcfScm.SaveDemand(m_DemandExtend, CurrentUser.User.id);

                m_DlgClosewindow?.Invoke();
                m_DlgRefreshDashboard?.Invoke();

            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        public void Report()
        {
            DemandReport demandReport = new DemandReport();
            SetValues(m_DemandExtend, demandReport);
            demandReport.demand_nomenclatures_extend = new List<DemandNomenclatureExtend>();

            foreach (var demNom in m_DemandExtend.demand_nomenclatures_extend) {
                DemandNomenclatureExtend demandNomenclatureExtend = new DemandNomenclatureExtend();
                SetValues(demNom, demandNomenclatureExtend);
                demandNomenclatureExtend.status_text = GetNomenclatureStatusText(demandNomenclatureExtend.status_id);
                demandReport.demand_nomenclatures_extend.Add(demandNomenclatureExtend);
            }

            //History
            demandReport.demand_nomenclatures_history = WcfScm.GetDemandNomHistory(m_DemandExtend.id);
            foreach (var demHist in demandReport.demand_nomenclatures_history) {
                demHist.status_text = GetNomenclatureStatusText(demHist.status_id);
            }

            //Remarks
            demandReport.remarks = WcfScm.GetRemarks(m_DemandExtend.id);

            PdfReports pdfReports = new PdfReports();
            var strReportPath = pdfReports.GenerateDemandReport(demandReport);
            Process.Start(strReportPath);

        }

        public void SendRemark(string demandText) {
            if (String.IsNullOrWhiteSpace(demandText)) {
                return;
            }

            WcfScm.AddRemark(
                m_DemandExtend.id,
                m_DemandExtend.version,
                CurrentUser.User.id,
                demandText);
            GetRemarks();
        }

        private Brush[] PopulateBrushUser() {
            var brushConverter = new System.Windows.Media.BrushConverter();

            var brushesUser = new Brush[] {
                (Brush)brushConverter.ConvertFromString("#B71C1C"),
                (Brush)brushConverter.ConvertFromString("#880E4F"),
                (Brush)brushConverter.ConvertFromString("#4A148C"),
                (Brush)brushConverter.ConvertFromString("#311B92"),
                (Brush)brushConverter.ConvertFromString("#004D40"),
                (Brush)brushConverter.ConvertFromString("#827717")
            };
                        
            return brushesUser;
        }

        private Brush[] PopulateBrushRemark() {
            var brushConverter = new System.Windows.Media.BrushConverter();
                        
            var brushesRemark = new Brush[] {
                (Brush)brushConverter.ConvertFromString("#FFCDD2"),
                (Brush)brushConverter.ConvertFromString("#F8BBD0"),
                (Brush)brushConverter.ConvertFromString("#E1BEE7"),
                (Brush)brushConverter.ConvertFromString("#D1C4E9"),
                (Brush)brushConverter.ConvertFromString("#B2DFDB"),
                (Brush)brushConverter.ConvertFromString("#F0F4C3")
            };

            return brushesRemark;
        }
        #endregion

        #region Validations
        private void ValidateAppManInputData(int value) {
            if (value < 0) {
                HasErrors = true;
                throw new Exception(ScmResource.SelectAppMan);
            } else if (m_IsSaveError) {
                HasErrors = true;
                m_IsSaveError = false;
                throw new Exception(ScmResource.DataWasNotSaved);
                
            }
            
            HasErrors = false;
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SelectedAppManId"));
        }

        //private void SetAppManName() {
        //    var appMan = (from appMenDb in AppMen
        //                  where appMenDb.id == m_DemandExtend.app_man_id
        //                  select appMenDb).FirstOrDefault();
        //    if (appMan == null) {
        //        SelectedAppManName = null;
        //    } else {
        //        SelectedAppManName = appMan.surname + " " + appMan.first_name;
        //    }


        //}
        #endregion
    }
}
