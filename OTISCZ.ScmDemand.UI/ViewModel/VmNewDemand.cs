using DocumentFormat.OpenXml.Spreadsheet;
using Kamsyk.ExcelOpenXml;
using OTISCZ.CommonDb;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.DataDictionary;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Common;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.ScmWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static OTISCZ.ScmDemand.Model.Repository.NomenclatureRepository;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class VmNewDemand : VmBaseGrid2, INotifyPropertyChanged, INotifyDataErrorInfo {
        
        #region Overriden Properties
        private List<SupplierExtend> m_SupplierList = null;
        public override List<SupplierExtend> SupplierList {
            get {
                if (m_SupplierList == null) {
                    try {
                        LoadSuppliers();

                    } catch (Exception ex) {
                        HandleError(ex);
                    }
                }
                return m_SupplierList;
            }
            set {
                m_SupplierList = value;
                OnPropertyChanged("SupplierList");
                SetDataGridColumnSort2();
            }
        }


        #endregion

        #region Properties
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private DlgRefreshDataGrid m_DlgRefreshDashboard = null;
        private DlgCloseWindow m_DlgClosewindow = null;
        private DlgRefreshEditableDataGrid m_DlgRefreshSupplier = null; 

        

        private int m_StepNr = 0;
        public int StepNr {
            get {
                return m_StepNr;
            }
            set {
                m_StepNr = value;
                if (m_StepNr < 0) {
                    m_StepNr = 0;
                }

                if (m_StepNr > 3) {
                    m_StepNr = 3;
                }

                if (m_StepNr == 0) {
                    PreviousVisibility = Visibility.Collapsed;
                } else {
                    PreviousVisibility = Visibility.Visible;
                }

                if (m_StepNr == 3) {
                    NextVisibility = Visibility.Collapsed;
                } else {
                    NextVisibility = Visibility.Visible;
                }

                switch (m_StepNr) {
                    case 0:
                        //DataKey = DataKeyEnum.Nomenclature;
                        NomenclaturesVisibility = Visibility.Visible;
                        NomenclaturesHeight = "*";
                        
                        SuppliersVisibility = Visibility.Collapsed;
                        SuppliersHeight = "0";
                        
                        //AttachmentsVisibility = Visibility.Collapsed;
                        AttachmentsHeight = "0";

                        MailsHeight = "0";

                        NextButtonText = ScmResource.Suppliers;

                        //PageNr = m_PageNrNomenclature;

                        break;
                    case 1:
                        //DataKey = DataKeyEnum.Supplier;
                        if (SupplierList == null) {
                            //FilterFields2 = m_FilterFieldsSupplier;
                            //PageNr = 1;
                            //LoadSuppliers();
                           
                        }
                        NomenclaturesVisibility = Visibility.Collapsed;
                        NomenclaturesHeight = "0";

                        SuppliersVisibility = Visibility.Visible;
                        SuppliersHeight = "*";

                        //AttachmentsVisibility = Visibility.Collapsed;
                        AttachmentsHeight = "0";

                        MailsHeight = "0";

                        PreviousButtonText = ScmResource.Nomenclatures;
                        NextButtonText = ScmResource.Attachments;

                        //PageNr = m_PageNrSupplier;

                        break;
                    case 2:
                        NomenclaturesVisibility = Visibility.Collapsed;
                        NomenclaturesHeight = "0";

                        SuppliersVisibility = Visibility.Collapsed;
                        SuppliersHeight = "0";

                        AttachmentsVisibility = Visibility.Visible;
                        AttachmentsHeight = "*";

                        MailsHeight = "0";

                        PreviousButtonText = ScmResource.Suppliers;
                        NextButtonText = ScmResource.Mails;

                        break;
                    case 3:
                        NomenclaturesVisibility = Visibility.Collapsed;
                        NomenclaturesHeight = "0";

                        SuppliersVisibility = Visibility.Collapsed;
                        SuppliersHeight = "0";

                        AttachmentsVisibility = Visibility.Collapsed;
                        AttachmentsHeight = "0";

                        MailsHeight = "*";

                        PreviousButtonText = ScmResource.Attachments;

                        break;
                    
                }

                OnPropertyChanged("StepNr");
            }
        }

        private ObservableCollection<NomenclatureExtend> m_SelectedNomenclatures = null;
        public ObservableCollection<NomenclatureExtend> SelectedNomenclatures {
            get {
                if (m_SelectedNomenclatures == null) {
                    m_SelectedNomenclatures = new ObservableCollection<NomenclatureExtend>();
                }
                return m_SelectedNomenclatures;
            }
            set {
                m_SelectedNomenclatures = value;
                OnPropertyChanged("SelectedNomenclatures");
            }
        }

        private ObservableCollection<SupplierExtend> m_SelectedSuppliers = null;
        public ObservableCollection<SupplierExtend> SelectedSuppliers {
            get {
                if (m_SelectedSuppliers == null) {
                    m_SelectedSuppliers = new ObservableCollection<SupplierExtend>();
                }
                return m_SelectedSuppliers;
            }
            set {
                m_SelectedSuppliers = value;
                OnPropertyChanged("SelectedSuppliers");
            }
        }

        private ObservableCollection<ScmAttachment> m_SelectedAttachments = null;
        public ObservableCollection<ScmAttachment> SelectedAttachments {
            get {
                if (m_SelectedAttachments == null) {
                    m_SelectedAttachments = new ObservableCollection<ScmAttachment>();
                }
                return m_SelectedAttachments;
            }
            set {
                m_SelectedAttachments = value;
                OnPropertyChanged("SelectedAttachemnts");
            }
        }

        private ObservableCollection<ScmMail> m_ScmMails = null;
        public ObservableCollection<ScmMail> ScmMails {
            get {
                if (m_ScmMails == null) {
                    m_ScmMails = new ObservableCollection<ScmMail>();
                }
                return m_ScmMails;
            }
            set {
                m_ScmMails = value;
                OnPropertyChanged("ScmMails");
            }
        }

        private Visibility m_PreviousVisibility = Visibility.Collapsed;
        public Visibility PreviousVisibility {
            get {
                return m_PreviousVisibility;
            }
            set {
                m_PreviousVisibility = value;
                OnPropertyChanged("PreviousVisibility");
            }
        }

        private Visibility m_NextVisibility = Visibility.Visible;
        public Visibility NextVisibility {
            get {
                return m_NextVisibility;
            }
            set {
                m_NextVisibility = value;
                OnPropertyChanged("NextVisibility");
            }
        }

        private Visibility m_NomenclaturesVisibility = Visibility.Visible;
        public Visibility NomenclaturesVisibility {
            get {
                return m_NomenclaturesVisibility;
            }
            set {
                m_NomenclaturesVisibility = value;
                OnPropertyChanged("NomenclaturesVisibility");
            }
        }

        private Visibility m_SuppliersVisibility = Visibility.Collapsed;
        public Visibility SuppliersVisibility {
            get {
                return m_SuppliersVisibility;
            }
            set {
                m_SuppliersVisibility = value;
                OnPropertyChanged("SuppliersVisibility");
            }
        }

        private Visibility m_AttachmentsVisibility = Visibility.Visible;
        public Visibility AttachmentsVisibility {
            get {
                return m_AttachmentsVisibility;
            }
            set {
                m_AttachmentsVisibility = value;
                OnPropertyChanged("AttachmentsVisibility");
            }
        }

        private Visibility m_MailsVisibility = Visibility.Visible;
        public Visibility MailsVisibility {
            get {
                return m_MailsVisibility;
            }
            set {
                m_MailsVisibility = value;
                OnPropertyChanged("MailsVisibility");
            }
        }

        private string m_NomenclaturesHeight = "*";
        public string NomenclaturesHeight {
            get {
                return m_NomenclaturesHeight;
            }
            set {
                m_NomenclaturesHeight = value;
                OnPropertyChanged("NomenclaturesHeight");
            }
        }

        private string m_SuppliersHeight = "0";
        public string SuppliersHeight {
            get {
                return m_SuppliersHeight;
            }
            set {
                m_SuppliersHeight = value;
                OnPropertyChanged("SuppliersHeight");
            }
        }

        private string m_AttachmentsHeight = "0";
        public string AttachmentsHeight {
            get {
                return m_AttachmentsHeight;
            }
            set {
                m_AttachmentsHeight = value;
                OnPropertyChanged("AttachmentsHeight");
            }
        }

        private string m_MailsHeight = "0";
        public string MailsHeight {
            get {
                return m_MailsHeight;
            }
            set {
                m_MailsHeight = value;
                OnPropertyChanged("MailsHeight");
            }
        }

        private string m_WindowTitle = "New Demand";
        public string WindowTitle {
            get {
                return m_WindowTitle;
            }
            set {
                m_WindowTitle = value;
                OnPropertyChanged("WindowTitle");
            }
        }

        private string m_NextButtonText = ScmResource.Suppliers;
        public string NextButtonText {
            get {
                return m_NextButtonText;
            }
            set {
                m_NextButtonText = value;
                OnPropertyChanged("NextButtonText");
            }
        }

        private string m_BtnAddNomText = ScmResource.AddNomenclature;
        public string BtnAddNomText {
            get {
                return m_BtnAddNomText;
            }
            set {
                m_BtnAddNomText = value;
                OnPropertyChanged("BtnAddNomText");
            }
        }

        private string m_BtnCancelNomText = ScmResource.Cancel;
        public string BtnCancelNomText {
            get {
                return m_BtnCancelNomText;
            }
            set {
                m_BtnCancelNomText = value;
                OnPropertyChanged("BtnCancelNomText");
            }
        }

        private string m_BtnSaveNomText = ScmResource.Save;
        public string BtnSaveNomText {
            get {
                return m_BtnSaveNomText;
            }
            set {
                m_BtnSaveNomText = value;
                OnPropertyChanged("BtnSaveNomText");
            }
        }

        private string m_PreviousButtonText = "Previous";
        public string PreviousButtonText {
            get {
                return m_PreviousButtonText;
            }
            set {
                m_PreviousButtonText = value;
                OnPropertyChanged("PreviousButtonText");
            }
        }

        private Visibility m_SupplierEditVisibility = Visibility.Collapsed;
        public Visibility SupplierEditVisibility {
            get {
                return m_SupplierEditVisibility;
            }
            set {
                m_SupplierEditVisibility = value;
                OnPropertyChanged("SupplierEditVisibility");
            }
        }

        private string m_AddNomText = ScmResource.AddNomencaltures;
        public string AddNomText {
            get {
                return m_AddNomText;
            }
            set {
                m_AddNomText = value;
                OnPropertyChanged("AddNomText");
            }
        }

        private string m_CustNomKey = null;
        public string CustNomKey {
            get {
                return m_CustNomKey;
            }
            set {
                ValidateCustNomKey(value);
                m_CustNomKey = value;
                OnPropertyChanged("CustNomKey");
            }
        }

        private string m_CustNomName = null;
        public string CustNomName {
            get {
                return m_CustNomName;
            }
            set {
                ValidateCustNomName(value);
                m_CustNomName = value;
                OnPropertyChanged("CustNomName");
            }
        }

        private string m_CustNomSpec = null;
        public string CustNomSpec {
            get {
                return m_CustNomSpec;
            }
            set {
                ValidateCustNomSpec(value);
                m_CustNomSpec = value;
                OnPropertyChanged("CustNomSpec");
            }
        }

        private Visibility m_EditNomVisibility = Visibility.Collapsed;
        public Visibility EditNomVisibility {
            get {
                return m_EditNomVisibility;
            }
            set {
                m_EditNomVisibility = value;
                OnPropertyChanged("EditNomVisibility");
            }
        }

        private Visibility m_NewNomVisibility = Visibility.Visible;
        public Visibility NewNomVisibility {
            get {
                return m_NewNomVisibility;
            }
            set {
                m_NewNomVisibility = value;
                OnPropertyChanged("NewNomVisibility");
            }
        }

        private int m_ParentAttId = -1;

        DataGrid m_GrdSuppliers = null;

        private NomenclatureExtend m_SelCustNom = null;
        #endregion

        #region Constructor
        public VmNewDemand(
            ScmUser scmUser, 
            DataGrid grdSuppliers,
            Dispatcher dispatcher,
            DlgRefreshDataGrid dlgRefreshDashboard,
            DlgCloseWindow dlgCloseWindow) : base(scmUser, dispatcher) {
            //this.DataKey = DataKeyEnum.Nomenclature;
            m_DlgRefreshDashboard = dlgRefreshDashboard;
            m_DlgClosewindow = dlgCloseWindow;

            m_GrdSuppliers = grdSuppliers;

            DlgSetDisplyingInfoNomenclatures = new DlgSetDisplyingInfo(SetDisplyingInfoNomenclatures);
            DlgSetDisplyingInfoSuppliers = new DlgSetDisplyingInfo(SetDisplyingInfoSuppliers);
            m_DlgRefreshSupplier = new DlgRefreshEditableDataGrid(RefreshSuppliers);

            SupplierEditVisibility = (CurrentUser.IsScmReferent ? Visibility.Visible : Visibility.Collapsed);

            

    }
        #endregion

        #region Interface Methods
        public IEnumerable GetErrors(string propertyName) {
            List<string> errMsg = new List<string>();

            HasErrors = true;

            errMsg.Add(ScmResource.EnterValidMailAddresses);

            return errMsg;
        }
        #endregion

        #region Abstract Methods
        public override void ExportToExcel() {
            ExportToExcelNomenclatures();
        }

        public override void ExportToExcel2() {
            ExportToExcelSuppliers();
        }

        public override void Import() {
            ImportNomenclaturesAsync();
        }

        public override void Import2() {
            ImportSuppliersAsync();
        }

        public override void RefreshGridData() {
            LoadNomenclatures();
            GridInit();
        }

        public override void RefreshGridData2() {
            LoadSuppliers();
            GridInit2();
        }
        #endregion

        #region Overriden Methods
        protected override Task<NomenclatureExtend[]> GetNomenclaturesAsync() {
            SetPredefinedNomenclatureFilter();
            var nomenclatures = base.GetNomenclaturesAsync();
                        
            return nomenclatures;
        }

        protected override void CalculatePagesCount() {
            if (PageSize == 0) {
                PagesCount = 0;
                return;
            }

            decimal dPgc = (Convert.ToDecimal(RowsCount) / Convert.ToDecimal(PageSize));
            decimal dPgcFloor = Decimal.Floor(dPgc);

            int iPgc = Convert.ToInt16(dPgcFloor);
            if (dPgc != dPgcFloor) {
                iPgc++;
            }

            PagesCount = iPgc;
        }

        protected override async void LoadSuppliers() {
            try {
                var suppliers = GetSuppliersAsync();
                SupplierList = await suppliers;

                if (DlgSetDisplyingInfoSuppliers != null) {
                    DlgSetDisplyingInfoSuppliers();
                }
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        protected override Task<List<SupplierExtend>> GetSuppliersAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    int rowsCount;
                    string strFilter = GetFilter2();
                    string strOrder = GetOrder2();

                    var wsSuppliers = WcfScm.GetSuppliers(
                        strFilter,
                        strOrder,
                        PageSize2,
                        PageNr2,
                        out rowsCount);

                    RowsCount2 = rowsCount;
                                        
                    var suppliers = GetSuppliersExtends(wsSuppliers);

                    return suppliers;
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        public override void AddNew() {

        }

        public override void AddNew2() {
            WinSupplier wsupp = new WinSupplier(DataNulls.INT_NULL, m_DlgRefreshSupplier);
            wsupp.Title = ScmResource.NewSupplier;
            wsupp.Show();
        }


        public override async void ExportToExcelSuppliers() {
            try {

                var suppliers = GetSuppliersReportAsync();
                var suppliersReport = await suppliers;

                ExportToExcelSuppliersOpen(suppliersReport);

                
            } catch (Exception ex) {
                HandleError(ex);
            } finally {
                //IsBusy = false;
            }
        }

        protected override Task<SupplierExtend[]> GetSuppliersReportAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    string strFilter = GetFilter2();
                    string strOrder = GetOrder2();

                    var wsSuppliers = WcfScm.GetSuppliersReport(
                        strFilter,
                        strOrder);

                    return wsSuppliers;
                } catch (Exception ex) {
                    IsBusy = false;
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }
        #endregion

        #region Async Methods
        private Task RemoveNomenclature(Button btn, DataGrid grd) {
            return Task.Run(() => {
                IsBusy = true;
                try {

                    //while (gridBusyIndicator.Visibility == Visibility.Visible) {
                    //    Thread.Sleep(100);
                    //}

                    ScmDispatcher.Invoke(() => {

                        var tmplt = (ContentPresenter)btn.TemplatedParent;
                        var nom = (NomenclatureExtend)tmplt.Content;

                        var dbNom = (from nomList in NomenclatureList
                                     where nomList.id == nom.id
                                     select nomList).FirstOrDefault();
                        if (dbNom != null) {
                            dbNom.is_selected = false;
                        }

                        //grd.ItemsSource = null;
                        grd.ItemsSource = NomenclatureList;
                        grd.Items.Refresh();

                        var selNoms = (from selNomDb in SelectedNomenclatures
                                       where selNomDb.id == nom.id
                                       select selNomDb).FirstOrDefault();

                        if (selNoms != null) {
                            SelectedNomenclatures.Remove(selNoms);
                        }
                    });
                } catch (Exception ex) {

                    ScmDispatcher.Invoke(() => {
                        HandleError(ex);
                    });

                } finally {
                    IsBusy = false;
                }
            });
        }

        private Task RemoveSupplier(Button btn, DataGrid grd) {
            return Task.Run(() => {
                IsBusy = true;
                try {

                    ScmDispatcher.Invoke(() => {

                        var tmplt = (ContentPresenter)btn.TemplatedParent;
                        var supp = (SupplierExtend)tmplt.Content;

                        var dbSupp = (from suppList in SupplierList
                                     where suppList.id == supp.id
                                     select suppList).FirstOrDefault();
                        if (dbSupp != null) {
                            dbSupp.is_selected = false;
                        }

                        //grd.ItemsSource = null;
                        grd.ItemsSource = SupplierList;
                        grd.Items.Refresh();

                        var selSupp = (from selSupDb in SelectedSuppliers
                                       where selSupDb.id == supp.id
                                       select selSupDb).FirstOrDefault();

                        if (selSupp != null) {
                            SelectedSuppliers.Remove(selSupp);
                        }

                        if (ScmMails != null && ScmMails.Count > 0) {
                            foreach (var scmMail in ScmMails) {
                                if (scmMail.supplier.id == supp.id) {
                                    ScmMails.Remove(scmMail);
                                    break;
                                }
                            }
                            
                        }
                    });
                } catch (Exception ex) {

                    ScmDispatcher.Invoke(() => {
                        HandleError(ex);
                    });

                } finally {
                    IsBusy = false;
                }
            });
        }

        public async void RemoveNomenclatureAsync(object sender, DataGrid grd) {
            //IsBusy = true;
            try {
                Button btn = (Button)sender;
                var t = RemoveNomenclature(btn, grd);
                await t;

            } catch (Exception ex) {
                HandleError(ex);
            } finally {
                //IsBusy = false;
            }
        }

        public async void RemoveSupplierAsync(object sender, DataGrid grd) {
            //IsBusy = true;
            try {
                Button btn = (Button)sender;
                var t = RemoveSupplier(btn, grd);
                await t;

            } catch (Exception ex) {
                HandleError(ex);
            } finally {
                //IsBusy = false;
            }
        }
        #endregion

        #region Methods
        public void Next() {
            if (StepNr == 0 && (SelectedNomenclatures == null || SelectedNomenclatures.Count == 0)) {
                MessageBox.Show(ScmResource.SelectNomenclatures, ScmResource.SelectNomenclatures, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (StepNr == 1 && (SelectedSuppliers == null || SelectedSuppliers.Count == 0)) {
                MessageBox.Show(ScmResource.SelectSupplier, ScmResource.SelectSupplier, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            StepNr++;
        }

        public void Previous() {
            StepNr--;
        }

        public void SelectNomenclatures(object sender) {
            CheckBox ckb = (CheckBox)sender;
            NomenclatureExtend nom = (NomenclatureExtend)ckb.DataContext;

            if (ckb.IsChecked == true) {
                var selNoms = (from selNomDb in SelectedNomenclatures
                               where selNomDb.id == nom.id
                               select selNomDb).FirstOrDefault();

                if (selNoms == null) {
                    nom.is_selected = true;
                    SelectedNomenclatures.Add(nom);
                }
            } else {
                var selNoms = (from selNomDb in SelectedNomenclatures
                               where selNomDb.id == nom.id
                               select selNomDb).FirstOrDefault();

                if (selNoms != null) {
                    SelectedNomenclatures.Remove(selNoms);
                }

                nom.is_selected = false;
            }
        }

        public void SelectSupplier(object sender) {
            CheckBox ckb = (CheckBox)sender;
            SupplierExtend supp = (SupplierExtend)ckb.DataContext;

            if (ckb.IsChecked == true) {
                var selSupps = (from selSuppDb in SelectedSuppliers
                                where selSuppDb.id == supp.id
                                select selSuppDb).FirstOrDefault();

                if (selSupps == null) {
                    supp.is_selected = true;
                    SelectedSuppliers.Add(supp);
                    AddMail(supp);
                }
            } else {
                DeleteSupplier(supp.id);
                //var selSupps = (from selSuppDb in SelectedSuppliers
                //                where selSuppDb.id == supp.id
                //                select selSuppDb).FirstOrDefault();

                //if (selSupps != null) {
                //    SelectedSuppliers.Remove(selSupps);
                //    for (int i = ScmMails.Count - 1; i >= 0; i--) {
                //        var scMail = ScmMails.ElementAt(i);
                //        if (scMail.supplier != null && scMail.supplier.id == selSupps.id) {
                //            ScmMails.RemoveAt(i);
                //        }
                //    }
                //}

                supp.is_selected = false;
            }
        }

        private void DeleteSupplier(int suppId) {
            var selSupps = (from selSuppDb in SelectedSuppliers
                            where selSuppDb.id == suppId
                            select selSuppDb).FirstOrDefault();

            if (selSupps != null) {
                SelectedSuppliers.Remove(selSupps);
                for (int i = ScmMails.Count - 1; i >= 0; i--) {
                    var scMail = ScmMails.ElementAt(i);
                    if (scMail.supplier != null && scMail.supplier.id == selSupps.id) {
                        ScmMails.RemoveAt(i);
                    }
                }
            }
        }

        private void AddMail(SupplierExtend supplier) {
            ScmMail scmMail = new ScmMail();
            scmMail.supplier = supplier;
            scmMail.from = CurrentUser.User.email;
            scmMail.is_send_bcc = true;
            //scmMail.addresses = CurrentUser.User.email;

            scmMail.loc_select_lang = ScmResource.SelectLang;
            scmMail.loc_from = ScmResource.MailSender;
            scmMail.loc_to = ScmResource.MailRecipient;
            scmMail.loc_cc = ScmResource.MailCc;
            scmMail.loc_bcc = ScmResource.MailBcc;
            scmMail.loc_subject = ScmResource.MailSubject;
            scmMail.loc_send = ScmResource.SendToSupplier;
            scmMail.loc_send_to_me = ScmResource.SendToMe;
            scmMail.loc_cancel = ScmResource.Delete;
            scmMail.loc_attachment = ScmResource.Attachments;

            string strTo = "";

            SupplierExtend currSupplier = WcfScm.GetSupplierById(supplier.id);
            if (currSupplier.supplier_contact_extended != null) {
                foreach (var userCont in currSupplier.supplier_contact_extended) {
                    if (!String.IsNullOrWhiteSpace(userCont.email)) {
                        if (!String.IsNullOrWhiteSpace(strTo)) {
                            strTo += ";";
                        }

                        strTo += userCont.email;

                    }
                }
            }

            scmMail.Recipients = strTo;
            
            scmMail.demand_nr = WcfScm.GetNewDemandRequestNr();

            List<MailCulture> mailCultures = new List<MailCulture>();
            MailCulture mailCulture = new MailCulture();
            mailCulture.culture_code = "CZ";
            mailCulture.culture_name = "Česky";
            mailCultures.Add(mailCulture);

            mailCulture = new MailCulture();
            mailCulture.culture_code = "EN";
            mailCulture.culture_name = "English";
            mailCultures.Add(mailCulture);

            scmMail.MailCultures = mailCultures;

            //Add Attachments
            foreach (var scmAttachment in m_SelectedAttachments) {
                ScmAttachment mailAtt = scmAttachment;
                mailAtt.mail = scmMail;
                mailAtt.parent_id = m_ParentAttId;
                scmMail.Attachments.Add(mailAtt);
            }
            

            ScmMails.Add(scmMail);
        }

        private ScmAttachment GetGeneratedDemand(ScmMail scmMail) {
            ScmAttachment scmAttachment = new ScmAttachment();

            string cultureName = Thread.CurrentThread.CurrentCulture.Name;
            string docName = "Poptavka";
            if (cultureName == VmBase.CULTURE_EN) {
                docName = "Demand";
            }

            scmAttachment.file_name = docName + scmMail.demand_nr + ".xlsx";
            scmAttachment.file_content = (new ScmExcel().GenerateDemandRequestExcelForm(
                scmMail.demand_nr,
                CurrentUser.User,
                SelectedNomenclatures,
                scmMail.supplier.supp_name));
            string filePath = GetTmpFileName(scmAttachment.file_name);
            File.WriteAllBytes(filePath, scmAttachment.file_content);
            scmAttachment.file_path = filePath;
            scmAttachment.att_type = AttachmentRepository.ATT_TYPE_GENERATED_DEMAND;//(int)AttachmentType.GeneratedDemand;
            //string fileName = GetDemandFile(scmAttachment.file_content);

            using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(filePath)) {
                var icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            sysicon.Handle,
                            System.Windows.Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                scmAttachment.icon = icon;
            }
            scmAttachment.mail = scmMail;

            return scmAttachment;
        }

        //private void DisplayGridFooterButtons() {
        //    bool isPreviousEnabled = true;
        //    bool isLastEnabled = true;

        //    if (DataKey == DataKeyEnum.Nomenclature) {
        //        isPreviousEnabled = (m_PageNrNomenclature > 1);
        //        isLastEnabled = (m_PageNrNomenclature < m_PagesCountNomenclature);
        //    } else if (DataKey == DataKeyEnum.Supplier) {
        //        isPreviousEnabled = (m_PageNrSupplier > 1);
        //        isLastEnabled = (m_PageNrSupplier < m_PagesCountSupplier);
        //    } else {
        //        throw new Exception("Unknown Data Key");
        //    }

        //    if (isPreviousEnabled) {
        //        PreviousEnabledButtonVisibility = Visibility.Visible;
        //        PreviousDisabledButtonVisibility = Visibility.Collapsed;
        //    } else {
        //        PreviousEnabledButtonVisibility = Visibility.Collapsed;
        //        PreviousDisabledButtonVisibility = Visibility.Visible;
        //    }

        //    if (isLastEnabled) {
        //        NextEnabledButtonVisibility = Visibility.Visible;
        //        NextDisabledButtonVisibility = Visibility.Collapsed;
        //    } else {
        //        NextEnabledButtonVisibility = Visibility.Collapsed;
        //        NextDisabledButtonVisibility = Visibility.Visible;
        //    }
        //}

        public void DropAttachment(DragEventArgs e) {
                      
           List<DropFile> dropFiles =  GetDropAttachment(e);

            foreach (var dropFile in dropFiles) {
                try {
                    string attFileNamePath = GetTmpFileName(dropFile.FileName);

                    ScmAttachment scmAttachment = new ScmAttachment();
                    scmAttachment.id = m_ParentAttId;

                    scmAttachment.file_name = dropFile.FileName;
                    scmAttachment.file_content = dropFile.FileContent;
                    scmAttachment.icon = dropFile.Icon;
                    scmAttachment.file_path = attFileNamePath;
                    File.WriteAllBytes(attFileNamePath, dropFile.FileContent);
                    SelectedAttachments.Add(scmAttachment);

                    foreach (var mail in ScmMails) {
                        ScmAttachment mailAtt = scmAttachment;
                        mailAtt.mail = mail;
                        mailAtt.parent_id = m_ParentAttId;
                        mail.Attachments.Add(mailAtt);
                    }

                    m_ParentAttId--;
                } catch (Exception ex) {
                    HandleError(ex);
                }
            }

            //if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true) {
            //    //file
            //    string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            //    foreach (string tmpFileName in fileNames) {
            //        FileInfo fi = new FileInfo(tmpFileName);
            //        fileName = fi.Name;

            //        using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName)) {
            //            var icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
            //                sysicon.Handle,
            //                System.Windows.Int32Rect.Empty,
            //                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            //            ScmAttachment scmAttachment = new ScmAttachment();
            //            scmAttachment.file_name = fileName;
            //            scmAttachment.file_content = File.ReadAllBytes(fi.FullName);
            //            scmAttachment.icon = (icon);
            //            SelectedAttachments.Add(scmAttachment);

            //            foreach (var mail in ScmMails) {
            //                ScmAttachment mailAtt = scmAttachment;
            //                mailAtt.mail = mail;
            //                mail.Attachments.Add(mailAtt);
            //            }
            //        }
            //    }

            //} else if (e.Data.GetDataPresent(DRAG_OBJECT_DESCRIPTOR)) {
            //    //1 attachment
            //    Stream attFileNameStream = (Stream)e.Data.GetData(DRAG_FILE_GROUP_DESCRIPTOR);
            //    MemoryStream attFileContentStream = (MemoryStream)e.Data.GetData("FileContents", true);


            //} else if (e.Data.GetDataPresent(DRAG_FILE_GROUP_DESCRIPTOR)) {
            //    //more attachments
            //    Stream attFileNameStream = (Stream)e.Data.GetData(DRAG_FILE_GROUP_DESCRIPTOR);
            //    MemoryStream attFileContentStream = (MemoryStream)e.Data.GetData("FileContents", true);

            //}
        }

        public void AddAttachment(ScmAttachment att) {

            
                    string attFileNamePath = GetTmpFileName(att.file_name);

                    ScmAttachment scmAttachment = new ScmAttachment();
                    scmAttachment.id = m_ParentAttId;

                    scmAttachment.file_name = att.file_name;
                    scmAttachment.file_content = att.file_content;
                    scmAttachment.icon = att.icon;
                    scmAttachment.file_path = attFileNamePath;
                    File.WriteAllBytes(attFileNamePath, att.file_content);
                    SelectedAttachments.Add(scmAttachment);

                    foreach (var mail in ScmMails) {
                        ScmAttachment mailAtt = scmAttachment;
                        mailAtt.mail = mail;
                        mailAtt.parent_id = m_ParentAttId;
                        mail.Attachments.Add(mailAtt);
                    }

                    m_ParentAttId--;
                
        }

        public void CollapseExpandMail(object sender) {
            Button btn = (Button)sender;
            ScmMail scmMail = (ScmMail)btn.DataContext;
            if (scmMail.IsCollapsed) {
                scmMail.IsCollapsed = false;
            } else {
                scmMail.IsCollapsed = true;
            }
             
        }

        public void RemoveMailAtt(object sender) {
            Button btn = (Button)sender;
            ScmAttachment scmAttachment = (ScmAttachment)btn.DataContext;

            scmAttachment.mail.Attachments.Remove(scmAttachment);
        }

        public void RemoveAtt(object sender) {
            Button btn = (Button)sender;
            ScmAttachment scmAttachment = (ScmAttachment)btn.DataContext;

            SelectedAttachments.Remove(scmAttachment);

            if (ScmMails != null && ScmMails.Count > 0) {
                foreach (var scmMail in ScmMails) {
                    if (scmMail.Attachments == null) {
                        continue;
                    }
                    var att = (from scmMailsAtt in scmMail.Attachments
                               where scmMailsAtt.parent_id == scmAttachment.id
                               select scmMailsAtt).FirstOrDefault();
                    if (att != null) {
                        scmMail.Attachments.Remove(att);
                    }
                }
            }
        }

        public void OpenAttachmentGeneral(object sender) {
            StackPanel sp = (StackPanel)sender;
            ScmAttachment scmAttachment = (ScmAttachment)sp.DataContext;

            if (!String.IsNullOrWhiteSpace(scmAttachment.file_path)) {
                Process.Start(scmAttachment.file_path);
            } else {
                string strFileName = GetTmpFileName(scmAttachment.file_name);
                File.WriteAllBytes(strFileName, scmAttachment.file_content);
                Process.Start(strFileName);
            }
        }

        public void SendMail(object sender, bool isSendToMe) {
            try {
                Button btn = (Button)sender;
                ScmMail scmMail = (ScmMail)btn.DataContext;

                if (!scmMail.IsValid()) {
                    MessageBox.Show(
                        ScmResource.CannotBeSent + " " + ScmResource.FixErrors,
                        ScmResource.CannotBeSent, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                DemandExtend newDemand = GetNewDemand(scmMail);
                newDemand.status_id = (int)DemandRepository.Status.Sent;

                if (!isSendToMe) {
                    WcfScm.SaveDemandWasSent(newDemand, CurrentUser.User.id);
                }

                SendDemandRequest(scmMail, isSendToMe);

                if (isSendToMe) {
                    MessageBox.Show(ScmResource.MailWasSent, ScmResource.MailWasSent, MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                

                if (m_DlgRefreshDashboard != null) {
                    m_DlgRefreshDashboard();
                }

                ScmMails.Remove(scmMail);

                if (ScmMails.Count == 0 && m_DlgClosewindow == null) {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).ClearChildren();
                } else if (ScmMails.Count == 0 && m_DlgClosewindow != null) {
                    m_DlgClosewindow();
                }
            } catch (Exception ex) {
                HandleError(ex);
            }
        }


        public void CancelMail(object sender) {
            try {
                Button btn = (Button)sender;
                ScmMail scmMail = (ScmMail)btn.DataContext;
                                
                ScmMails.Remove(scmMail);

                if (ScmMails.Count == 0 && m_DlgClosewindow == null) {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).ClearChildren();
                } else if (ScmMails.Count == 0 && m_DlgClosewindow != null) {
                    m_DlgClosewindow();
                }
            } catch (Exception ex) {
                HandleError(ex);
            }
        }


        private DemandExtend GetNewDemand(ScmMail scmMail) {
            DemandExtend demand = new DemandExtend();

            demand.id = DataNulls.INT_NULL;
            demand.supplier_id = scmMail.supplier.id;
            demand.requestor_id = CurrentUser.User.id;
            demand.status_id = (int)DemandRepository.Status.Draft;
            demand.is_active = true;
            demand.demand_nr = scmMail.demand_nr;

            //string demandNr = WcfScm.GetNewDemandRequestNr();
            //demand.demand_nr = demandNr;

            demand.Demand_Nomenclature = new List<Demand_Nomenclature>();
            //int lastDemNonId = new DemandNomenclatureRepository().GetLastId();
            //int newDemNomId = ++lastDemNonId;
            foreach (var nom in SelectedNomenclatures) {
                Demand_Nomenclature demNom = new Demand_Nomenclature();
                //demNom.id = newDemNomId;
                //newDemNomId++;
                demNom.demand_id = demand.id;
                demNom.demand_version = demand.version;
                demNom.nomenclature_id = nom.id;
                demNom.nomenclature_version = nom.version;
                demand.Demand_Nomenclature.Add(demNom);
            }

            demand.recipient_attachments_extend = new List<AttachmentExtend>();
            foreach (var att in scmMail.Attachments) {
                AttachmentExtend attExtend = new AttachmentExtend();
                attExtend.file_icon = VmBase.GetBytesFromBitmapSource(att.icon);
                attExtend.file_content = File.ReadAllBytes(att.file_path);//att.file_content;
                attExtend.file_name = att.file_name;
                attExtend.att_type = att.att_type;//AttachmentRepository.ATT_TYPE_MAIL_RECIPIENT;
                demand.recipient_attachments_extend.Add(attExtend);
            }

            return demand;
        }

        

        private void SendDemandRequest(ScmMail scmMail, bool isSendToMe) {
            object[] attContent = null;
            string[] attNames = null;

            if (scmMail.Attachments.Count > 0) {
                attContent = new object[scmMail.Attachments.Count];
                attNames = new string[scmMail.Attachments.Count];
            }

            for (int i = 0; i < scmMail.Attachments.Count; i++) {
                byte[] fileContent = File.ReadAllBytes(scmMail.Attachments[i].file_path);
                //attContent[i] = scmMail.Attachments[i].file_content;
                attContent[i] = fileContent;
                attNames[i] = scmMail.Attachments[i].file_name;
            }

            string to = null;

            if (isSendToMe) {
                to = CurrentUser.User.email;
            } else {
                to = scmMail.Recipients;
            }

            string bcc = "kamil.sykora@otis.com";
            if (scmMail.is_send_bcc) {
                bcc += ";" + CurrentUser.User.email;
            }

#if DEBUG
            to = "kamil.sykora@otis.com";
#endif


//#if !DEBUG
            WsMail.OtWsMail wsMail = new WsMail.OtWsMail();
            wsMail.SendMailByteAttachementsBcc(
                scmMail.from,
                to,
                null,
                bcc,
                scmMail.subject,
                scmMail.body,
                attContent,
                attNames,
                (int)MailPriority.Normal);
//#endif
        }


        public void LangChanged(object sender) {
            ComboBox cmb = (ComboBox)sender;
            if (cmb.DataContext == null) {
                return;
            }
            if(!(cmb.DataContext is ScmMail)) {
                return;
            }
            ScmMail scmMail = (ScmMail)cmb.DataContext;

            string cultureName = GetCulture(scmMail.SelectedCulture);


            if (scmMail.Attachments != null && scmMail.Attachments.Count > 0) {
                foreach (var att in scmMail.Attachments) {
                    if (att.att_type == AttachmentRepository.ATT_TYPE_GENERATED_DEMAND) {
                        scmMail.Attachments.Remove(att);
                        break;
                    }
                }
            }

            string origCulture = Thread.CurrentThread.CurrentCulture.Name;
            try {
                CultureInfo ci = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = ci;
                //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
                Thread.CurrentThread.CurrentUICulture = ci;

                scmMail.subject = ScmResource.Demand + " " + scmMail.demand_nr;
                string subjectDemand = "";
                if (SelectedNomenclatures != null && SelectedNomenclatures.Count > 0) {
                    foreach (var nom in SelectedNomenclatures) {
                        if (subjectDemand.Length < 80) {
                            if (subjectDemand.Length == 0) {
                                subjectDemand = " - " + nom.nomenclature_key;
                            } else {
                                subjectDemand += ", " + nom.nomenclature_key;
                            }
                        } else {
                            subjectDemand += ", ...";
                            break;
                        }
                    }
                }
                scmMail.subject += subjectDemand;

                scmMail.body = String.Format(ScmResource.MailBody, scmMail.demand_nr);
                scmMail.body += Environment.NewLine;
                scmMail.body += Environment.NewLine;
                scmMail.body += CurrentUser.User.first_name + " " + CurrentUser.User.surname;
                scmMail.body += Environment.NewLine;
                scmMail.body += "OTIS a.s.";
                if (!String.IsNullOrWhiteSpace(CurrentUser.User.phone_nr)) {
                    scmMail.body += Environment.NewLine;
                    scmMail.body += ScmResource.DocPhoneNr + ": " + CurrentUser.User.phone_nr;
                }

                ScmAttachment scmAttachment = GetGeneratedDemand(scmMail);
                scmMail.Attachments.Insert(0, scmAttachment);
            } catch (Exception ex) {
                HandleError(ex);
            } finally {
                CultureInfo ci = new CultureInfo(origCulture);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }

        }

        private void SetDisplyingInfoNomenclatures() {
            DisplayingRows = ScmResource.DisplayingFromToOf
                    .Replace("{0}", GetDisplayItemsFromInfo().ToString())
                    .Replace("{1}", GetDisplayItemsToInfo().ToString())
                    .Replace("{2}", RowsCount.ToString());

        }

        private void SetDisplyingInfoSuppliers() {
            DisplayingRows2 = ScmResource.DisplayingFromToOf
                    .Replace("{0}", GetDisplayItemsFromInfo2().ToString())
                    .Replace("{1}", GetDisplayItemsToInfo2().ToString())
                    .Replace("{2}", RowsCount2.ToString());

        }

        public void SupplierEdit(object sender) {
            Button btn = (Button)sender;
            if (btn.DataContext == null) {
                return;
            }
            if (!(btn.DataContext is SupplierExtend)) {
                return;
            }
            int supplierId = ((SupplierExtend)btn.DataContext).id;
            WinSupplier winSupplier = new WinSupplier(supplierId, m_DlgRefreshSupplier);
            winSupplier.Title = ScmResource.Supplier + " " + ((SupplierExtend)btn.DataContext).supp_name;
            winSupplier.ShowDialog();
        }

        private void RefreshSuppliers(int newId) {
            SupplierExtend supplierExtend = WcfScm.GetSupplierById(newId);
            supplierExtend = GetSupplierExtend(supplierExtend);
            supplierExtend.row_index = SupplierList.Count + 1;


            SupplierList.Add(supplierExtend);

            //var tmpSupplierList = SupplierList;
            //SupplierList = null;
            //SupplierList = tmpSupplierList;

            if (m_GrdSuppliers != null) {
                
                m_GrdSuppliers.Items.Refresh();
            }
        }

        private void SetPredefinedNomenclatureFilter() {
            SetFilterField(NomenclatureData.STATUS_ID_FIELD, "100");
            SetFilterField(NomenclatureData.IS_ACTIVE_FIELD, "1");
        }

        private void ValidateCustNomKey(string value) {
            if (String.IsNullOrWhiteSpace(value)) {
                //HasErrors = true;
                //ErrorsChanged.Invoke(this, new DataErrorsChangedEventArgs("CustNomKey"));
                throw new Exception(ScmResource.EnterNomenclatureKey);
                
            }

            //HasErrors = IsHasErros();
            //ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("CustNomKey"));
        }

        private void ValidateCustNomName(string value) {

            if (!IsCustNomNameValid(value)) {
            //    HasErrors = true;
                throw new Exception(ScmResource.EnterNomenclatureName);
            }
            
            //HasErrors = IsHasErros();
            //ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("CustNomName"));
        }

        private void ValidateCustNomSpec(string value) {
            
            //HasErrors = IsHasErros();
            ////ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("CustNomSpec"));
        }

        //public void OnErrorsChanged(string propertyName) {
        //    var handler = ErrorsChanged;
        //    if (handler != null)
        //        handler(this, new DataErrorsChangedEventArgs(propertyName));
        //}

        public bool IsNewNomValid(TextBox txtCustNomKey, TextBox txtCustNomName, TextBox txtCustNomSpec) {
            bool isValid = true;

            try {
                ValidateCustNomKey(txtCustNomKey.Text);
            } catch (Exception ex) {
                isValid = false;
                ValidationError validationError = new ValidationError(
                    new ExceptionValidationRule(),
                    txtCustNomKey.GetBindingExpression(TextBox.TextProperty));

                validationError.ErrorContent = ex.Message;

                Validation.MarkInvalid(
                    txtCustNomKey.GetBindingExpression(TextBox.TextProperty),
                    validationError);
            }

            try {
                ValidateCustNomName(txtCustNomName.Text);
            } catch (Exception ex) {
                isValid = false;
                ValidationError validationError = new ValidationError(
                    new ExceptionValidationRule(),
                    txtCustNomName.GetBindingExpression(TextBox.TextProperty));

                validationError.ErrorContent = ex.Message;

                Validation.MarkInvalid(
                    txtCustNomName.GetBindingExpression(TextBox.TextProperty),
                    validationError);
            }

            try {
                ValidateCustNomSpec(txtCustNomSpec.Text);
            } catch (Exception ex) {
                isValid = false;
                ValidationError validationError = new ValidationError(
                    new ExceptionValidationRule(),
                    txtCustNomSpec.GetBindingExpression(TextBox.TextProperty));

                validationError.ErrorContent = ex.Message;

                Validation.MarkInvalid(
                    txtCustNomSpec.GetBindingExpression(TextBox.TextProperty),
                    validationError);
            }

            return isValid;
        }

        public void AddCustNom(TextBox txtCustNomKey, TextBox txtCustNomName, TextBox txtCustNomSpec) {
            if (!IsNewNomValid(txtCustNomKey, txtCustNomName, txtCustNomSpec)) {
                //ScmDispatcher.Invoke(() => {
                    MessageBox.Show(ScmResource.EnterValidData, ScmResource.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
                //});
                
                return;
            }

            try {
                Nomenclature nomenclature = new Nomenclature();
                nomenclature.id = -1;
                nomenclature.nomenclature_key = m_CustNomKey;
                nomenclature.name = m_CustNomName;
                nomenclature.specification = m_CustNomSpec;
                nomenclature.source_id = (int)NomSource.Custom;

                string strMaterialGroup = "Uživatelská";
                int materialGroupId = WcfScm.GetMaterialGroupId(strMaterialGroup);
                if (materialGroupId > -1) {
                    nomenclature.material_group_id = materialGroupId;
                } else {
                    throw new Exception("Material Group '" + strMaterialGroup + "' was not found");
                }
                nomenclature.material_group_id = materialGroupId;
                nomenclature.created_date = DateTime.Now;

                int nomId = WcfScm.ImportNomenclature(nomenclature, CurrentUser.User.id);
                nomenclature.id = nomId;

                NomenclatureExtend nomExt = new NomenclatureExtend();
                SetValues(nomenclature, nomExt);

                SelectedNomenclatures.Add(nomExt);

                ClearCustomNom(txtCustNomKey, txtCustNomName, txtCustNomSpec);
            } catch(Exception ex) {
                HandleError(ex, ScmDispatcher);
            }
        }

        private void ClearCustomNom(TextBox txtCustNomKey, TextBox txtCustNomName, TextBox txtCustNomSpec) {
            txtCustNomKey.Text = "";
            txtCustNomName.Text = "";
            txtCustNomSpec.Text = "";

            Validation.ClearInvalid(txtCustNomKey.GetBindingExpression(TextBox.TextProperty));
            Validation.ClearInvalid(txtCustNomName.GetBindingExpression(TextBox.TextProperty));
            Validation.ClearInvalid(txtCustNomSpec.GetBindingExpression(TextBox.TextProperty));
        }

        //private bool IsHasErros() {

        //    bool isValid = IsCustNomKeyValid(this.CustNomKey)
        //        && IsCustNomNameValid(this.CustNomName)
        //        && IsCustNomSpecValid(this.CustNomSpec);

        //    return !isValid;

        //    //return false;
        //}

        private bool IsCustNomKeyValid(string custNomKey) {
            if (String.IsNullOrWhiteSpace(custNomKey)) {
                return false;
            }

            return true;
        }

        private bool IsCustNomNameValid(string custNomName) {
            if (String.IsNullOrWhiteSpace(custNomName)) {
                return false;
            }

            return true;
        }

        private bool IsCustNomSpecValid(string custNomName) {
            //if (String.IsNullOrWhiteSpace(custNomName)) {
            //    return false;
            //}

            return true;
        }

        public void CustNomenclatureSelected(
            NomenclatureExtend nom,
            TextBox txtCustNomKey, 
            TextBox txtCustNomName, 
            TextBox txtCustNomSpec) {

            NewNomVisibility = Visibility.Visible;
            EditNomVisibility = Visibility.Collapsed;

            m_SelCustNom = nom;
            if (m_SelCustNom == null) {
                return;
            }

            if (m_SelCustNom.source_id != (int)NomSource.Custom) {
                ClearCustomNom(txtCustNomKey, txtCustNomName, txtCustNomSpec);
                return;
            }

            CustNomKey = m_SelCustNom.nomenclature_key;
            CustNomName = m_SelCustNom.name;
            CustNomSpec = m_SelCustNom.specification;

            NewNomVisibility = Visibility.Collapsed;
            EditNomVisibility = Visibility.Visible;
        }

        public void SaveCustNom(
            TextBox txtCustNomKey,
            TextBox txtCustNomName,
            TextBox txtCustNomSpec) {

            if (m_SelCustNom == null) {
                return;
            }

            m_SelCustNom.nomenclature_key = CustNomKey;
            m_SelCustNom.name = CustNomName;
            m_SelCustNom.specification = CustNomSpec;

            m_SelCustNom.source_id = (int)NomSource.Custom;
            string strMaterialGroup = "Uživatelská";
            int materialGroupId = WcfScm.GetMaterialGroupId(strMaterialGroup);
            m_SelCustNom.material_group_id = materialGroupId;
            m_SelCustNom.created_date = DateTime.Now;
            m_SelCustNom.import_date = DateTime.Now;
            m_SelCustNom.status_id = NOM_STATUS_LOADED;
            m_SelCustNom.last_status_modif_date = DateTime.Now;

            WcfScm.SaveNomenclature(m_SelCustNom, CurrentUser.User.id);
            
            //var selNom = SelectedNomenclatures

            ClearCustomNom(txtCustNomKey, txtCustNomName, txtCustNomSpec);

            var lstNomList = SelectedNomenclatures;
            SelectedNomenclatures = null;
            SelectedNomenclatures = lstNomList;

            NewNomVisibility = Visibility.Visible;
            EditNomVisibility = Visibility.Collapsed;

            m_SelCustNom = null;
        }

        public void CancelCustNom(
            TextBox txtCustNomKey,
            TextBox txtCustNomName,
            TextBox txtCustNomSpec) {

            ClearCustomNom(txtCustNomKey, txtCustNomName, txtCustNomSpec);

            NewNomVisibility = Visibility.Visible;
            EditNomVisibility = Visibility.Collapsed;

            m_SelCustNom = null;
        }

        //static byte[] GetBytesFromBitmapSource(BitmapSource bmp) {
        //    int width = bmp.PixelWidth;
        //    int height = bmp.PixelHeight;
        //    int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

        //    byte[] pixels = new byte[height * stride];

        //    bmp.CopyPixels(pixels, stride, 0);

        //    return pixels;
        //}
        #endregion

        #region Validations

        //private void ValidateRecipientsInputData(string value) {
        //    if (String.IsNullOrWhiteSpace(value)) {
        //        HasErrors = true;
        //        throw new Exception(ScmResource.SelectAppMan);
        //    } else if (m_IsSaveError) {
        //        HasErrors = true;
        //        m_IsSaveError = false;
        //        throw new Exception(ScmResource.SelectAppMan);

        //    }

        //    HasErrors = false;
        //    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SupplierName"));
        //}

        //public bool IsValid(ScmMail scmMail) {

        //    //List<string> errMsg = new List<string>();
        //    if (String.IsNullOrWhiteSpace(scmMail.addresses)) {
        //        HasErrors = true;
        //    }

        //    if (HasErrors) {
        //        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SupplierName"));
        //    } else {
        //        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SupplierName"));
        //        return true;
        //    }
        //    return false;
        //}
        #endregion
    }
}
