using Kamsyk.ExcelOpenXml;
using OTISCZ.CommonDb;
using OTISCZ.OtisUser;
using OTISCZ.ScmDemand.Interface;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.DataDictionary;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Common;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class VmDashboardNomenclature : VmBaseGrid2, INotifyPropertyChanged, INotifyDataErrorInfo {
        #region Constants
        private const int PERIOD_NOLIMIT = 0;
        private const int PERIOD_TODAY = 1;
        private const int PERIOD_YESTERDAY = 2;
        private const int PERIOD_WEEK = 3;
        private const int PERIOD_BEFORE = 4;
        #endregion

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #region Overriden Properties
        //public override ObservableCollection<NomenclatureExtend> NomenclatureList {
        //    get {
        //        if (base.NomenclatureList == null) {
        //            try {
        //                LoadNomenclatures();
        //            } catch (Exception ex) {
        //                HandleError(ex);
        //            }
        //        }
        //        return base.NomenclatureList;
        //    }
        //    set { base.NomenclatureList = value; }
        //}
        #endregion

        #region Properties
        private List<TimePeriod> m_TimePeriods = null;
        public List<TimePeriod> TimePeriods {
            get {
                if (m_TimePeriods == null) {
                    m_TimePeriods = GetTimePeriods();
                }

                return m_TimePeriods;
            }
            set {
                m_TimePeriods = value;
                OnPropertyChanged("TimePeriods");
            }
        }

        private int m_SelectedPeriodIdNom = 0;
        public int SelectedPeriodIdNom {
            get {
                return m_SelectedPeriodIdNom;
            }
            set {
                m_SelectedPeriodIdNom = value;
                OnPropertyChanged("SelectedPeriodIdNom");
                SetGridDateFilter();
            }
        }

        private int m_SelectedPeriodIdDemand = 0;
        public int SelectedPeriodIdDemand {
            get {
                return m_SelectedPeriodIdDemand;
            }
            set {
                m_SelectedPeriodIdDemand = value;
                OnPropertyChanged("SelectedPeriodIdDemand");
                //SetGridNomDateFilter();
            }
        }

        private int m_PendingNomenclatures = -1;
        public int PendingNomenclatures {
            get {
                if (m_PendingNomenclatures == -1) {
                    m_PendingNomenclatures = WcfScm.GetPendingNomenclaturesNumber();
                }
                return m_PendingNomenclatures;
            }
            set {
                m_PendingNomenclatures = value;
                OnPropertyChanged("PendingNomenclatures");
            }
        }

        private int m_PendingDemands = -1;
        public int PendingDemands {
            get {
                if (m_PendingDemands == -1) {

                    m_PendingDemands = WcfScm.GetPendingDemandsNumber(GetDemandUserId());
                }
                return m_PendingDemands;
            }
            set {
                m_PendingDemands = value;
                OnPropertyChanged("PendingDemands");
            }
        }

        private bool m_IsOnlyMyDemands = true;
        public bool IsOnlyMyDemands {
            get {
                return m_IsOnlyMyDemands;
            }
            set {
                m_IsOnlyMyDemands = value;
                OnPropertyChanged("IsOnlyMyDemands");
                RefreshGridData2();
                RefreshPendingDemandCount();
            }
        }

        private UcGrdColHeaderFilterDate m_UcGrdColHeaderFilterDateNom = null;
        public UcGrdColHeaderFilterDate UcGrdColHeaderFilterDateNom {
            get { return m_UcGrdColHeaderFilterDateNom; }
            set { m_UcGrdColHeaderFilterDateNom = value; }
        }

        private CheckBox m_CkbSelectedNom = null;
        public CheckBox CkbSelectedNom {
            get { return m_CkbSelectedNom; }
            set { m_CkbSelectedNom = value; }
        }

        private DemandExtend[] m_DemandList = null;
        public override DemandExtend[] DemandList {
            get {
                if (m_DemandList == null) {
                    try {
                        LoadDemands();
                    } catch (Exception ex) {
                        HandleError(ex);
                    }
                }
                return m_DemandList;
            }
            set {
                m_DemandList = value;
                OnPropertyChanged("DemandList");
                SetDataGridColumnSort();
            }
        }

        private string m_LocPriceList = "Price List";
        public string LocPriceList {
            get {
                return m_LocPriceList;
            }
            set {
                m_LocPriceList = value;
                OnPropertyChanged("LocPriceList");
                
            }
        }

        private string m_LastImportDate = null;
        public string LastImportDate {
            get {
                if (m_LastImportDate == null) {
                    m_LastImportDate = WcfScm.GetLastImportDateText();
                }

                return m_LastImportDate;
            }
            set {
                m_LastImportDate = value;
                OnPropertyChanged("LastImportDate");
            }
        }

        private List<Nomenclature_Source> m_NomenclatureSource = null;
        public List<Nomenclature_Source> NomenclatureSource {
            get {
                
                if (m_NomenclatureSource == null) {
                    var tmpNomenclatureSources = WcfScm.GetNomenclatureSources();
                    m_NomenclatureSource = new List<Nomenclature_Source>();
                    foreach (var tmpNomenclatureSource in tmpNomenclatureSources) {
                        Nomenclature_Source nomSource = new Nomenclature_Source();
                        nomSource.id = tmpNomenclatureSource.id;
                        nomSource.name = tmpNomenclatureSource.name;
                        m_NomenclatureSource.Add(nomSource);
                    }
                    Nomenclature_Source sourceAll = new Nomenclature_Source();
                    sourceAll.id = DataNulls.INT_NULL;
                    sourceAll.name = ScmResource.CmbFilterAll;
                    m_NomenclatureSource.Insert(0, sourceAll);
                }

                return m_NomenclatureSource;

                
            }
            set {
                m_NomenclatureSource = value;
                OnPropertyChanged("NomenclatureSource");
            }
        }

        private List<YesNo> m_YesNo = null;
        public List<YesNo> YesNo {
            get {

                if (m_YesNo == null) {
                    m_YesNo = new List<YesNo>();

                    YesNo yesNo = new YesNo();
                    yesNo.value = DataNulls.INT_NULL;
                    yesNo.text = ScmResource.CmbFilterAll;
                    m_YesNo.Add(yesNo);

                    yesNo = new YesNo();
                    yesNo.value = 0;
                    yesNo.text = ScmResource.No;
                    m_YesNo.Add(yesNo);

                    yesNo = new YesNo();
                    yesNo.value = 1;
                    yesNo.text = ScmResource.Yes;
                    m_YesNo.Add(yesNo);
                }

                return m_YesNo;

            }
            set {
                m_YesNo = value;
                OnPropertyChanged("YesNo");
            }
        }

        //private DataGrid m_grdDemands = null;
        //public DataGrid GrdDemands {
        //    get { return m_grdDemands; }
        //    set { m_grdDemands = value; }
        //}

        //private Visibility m_EditColumnVisibility = Visibility.Collapsed;
        //public Visibility EditColumnVisibility {
        //    get { return m_EditColumnVisibility; }
        //    set {
        //        m_EditColumnVisibility = value;
        //        OnPropertyChanged("EditColumnVisibility");
        //    }
        //}
        #endregion

        #region Constructor
        public VmDashboardNomenclature(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {
            DlgSetDisplyingInfoNomenclatures = new DlgSetDisplyingInfo(SetDisplyingInfoNomenclatures);
            DlgSetDisplyingInfoDemands = new DlgSetDisplyingInfo(SetDisplyingInfoDemands);
            SetFilterField(NomenclatureData.STATUS_ID_FIELD, "100");
            SetFilterField(NomenclatureData.IS_ACTIVE_FIELD, "1");

            
            //DataGrid2 = m_grdDemands;
            SortFieldName2 = "last_status_modif_date_text";
            SortDirection2 = ListSortDirection.Ascending;
            //SetDataGridColumnSort2();

            ImportButtonVisibility2 = Visibility.Collapsed;

            
        }
        #endregion

        #region Interface Methods
        public IEnumerable GetErrors(string propertyName) {
            throw new NotImplementedException();
        }
        #endregion

        #region Abstract Methods
        public override void ExportToExcel2() {
            ExportToExcelDemands();
        }

        public override void Import2() {
            throw new NotImplementedException();
        }

        public override void RefreshGridData2() {
            LoadDemands();
            GridInit2();
        }

        public override void RefreshGridData() {
            m_CkbSelectedNom.IsChecked = false;
            LoadNomenclatures();
            RefreshPendingNomenclatureCount();
            GridInit();
            //EditColumnVisibility = Visibility.Collapsed;
        }

        public override void ExportToExcel() {
            ExportToExcelNomenclatures();
        }

        public override void Import() {
            ImportNomenclaturesAsync();
            
        }

        public override void AddNew() {

        }

        public override void AddNew2() {

        }
        #endregion

        #region Overwritten Methods


        protected override async void LoadDemands() {
            try {
                SetDemandPredefinedFilter();
                var demands = GetDemandsAsync();
                DemandList = await demands;
                if (DlgSetDisplyingInfoDemands != null) {
                    DlgSetDisplyingInfoDemands();
                }
                GridInit2();

                //SetDataGridColumnSort2();

                if (DataGrid2 != null && !String.IsNullOrWhiteSpace(SortFieldName2)) {
                    foreach (var col in DataGrid2.Columns) {
                        if (SortFieldName2 == col.SortMemberPath) {
                            col.SortDirection = SortDirection2;
                        }
                    }
                }

            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        protected override Task<DemandExtend[]> GetDemandsAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    int rowsCount;
                    //string strFilter = GetFilter2();
                    var filterFields = GetFilterFields2();
                    string strOrder = GetOrder2();

                    
                    var wsDemands = WcfScm.GetDemands(
                        filterFields,
                        strOrder,
                        PageSize2,
                        PageNr2,
                        out rowsCount);

                    RowsCount2 = rowsCount;

                    foreach (var wsDemand in wsDemands) {
                        wsDemand.status_text = GetDemandStatus(wsDemand.status_id);
                    }

                        //ObservableCollection<DemandExtend> demands = new ObservableCollection<DemandExtend>();
                        //foreach (var wsDemand in wsDemands) {
                        //    DemandExtend demandExtend = new DemandExtend();
                        //    SetValues(wsDemand, demandExtend);
                        //    demandExtend.status_text = GetDemandStatus(demandExtend.status_id);

                        //    demands.Add(demandExtend);
                        //}

                        //return demands;
                        return wsDemands;
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        protected Task<DemandExtend[]> GetDemandsReportAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {

                    //string strFilter = GetFilter2();
                    var filterFields = GetFilterFields2();
                    string strOrder = GetOrder2();


                    var wsDemands = WcfScm.GetDemandsReport(
                        filterFields,
                        strOrder);

                    
                    foreach (var wsDemand in wsDemands) {
                        wsDemand.status_text = GetDemandStatus(wsDemand.status_id);
                    }

                    
                    return wsDemands;
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        protected override Task ImportNomenclaturesAsync() {
            return Task.Run(() => {
                try {

                    Prodis.DlgLoadFileInfo dlgLoadFileInfo = new Prodis.DlgLoadFileInfo(LoadFileInfo);
                    Prodis.DlgLoadProgressInfo dlgLoadProgressInfo = new Prodis.DlgLoadProgressInfo(LoadProgressInfo);
#if DEBUG
                    UserClass userClass = new UserClass();
                    var userContext = userClass.ImpersonateUser("autobom", "OT", "Heslo37.");
#endif
                    var errList = new ScmFileImport().ImportData(
                        ProdisFolder,
                        CurrentUser.User.id,
                        dlgLoadFileInfo,
                        dlgLoadProgressInfo,
                        ScmDispatcher);

                    var errPriceList = new ScmFileImport().ImportPriceData(
                        ProdisPriceFolder,
                        CurrentUser.User.id,
                        dlgLoadFileInfo,
                        dlgLoadProgressInfo,
                        ScmDispatcher);

#if DEBUG
                    userContext.Undo();
#endif

                    ProgressBarVisibility = Visibility.Collapsed;
                    ScmDispatcher.Invoke(() => {
                        if (errList == null || errList.Count == 0 || errPriceList == null || errPriceList.Count == 0) {
                            MessageBox.Show(ScmResource.DataWasImported, ScmResource.DataWasImported, MessageBoxButton.OK, MessageBoxImage.Information);
                        } else {
                            MessageBox.Show(ScmResource.ErrorMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        //MessageBox.Show(ScmResource.ErrorMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                    ProgressBarVisibility = Visibility.Collapsed;

                    ScmDispatcher.Invoke(() => {
                        RefreshGridData();
                        //RefreshPendingNomenclatureCount();
                    });
                } catch (Exception ex) {

                    ScmDispatcher.Invoke(() => {
                        HandleError(ex);
                    });

                } finally {
                    ProgressBarVisibility = Visibility.Collapsed;
                }
            });
        }
        #endregion

        #region Async Methods
        private async void RemoveNom(List<NomenclatureExtend> nomenclatures) {
            try {
                var t = RemoveNomAsync(nomenclatures);
                await t;
                               
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        protected virtual Task RemoveNomAsync(List<NomenclatureExtend> nomenclatures) {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    foreach (var nom in nomenclatures) {
                        nom.status_id = (int)NomenclatureRepository.Status.WithoutDemand;
                        //var wsNom = GetWsNomenclature(nom);
                        WcfScm.SaveNomenclature(nom, CurrentUser.User.id);
                    }

                    ScmDispatcher.Invoke(() => {
                        RefreshDashboard();
                    });
                    
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        public async void ExportToExcelDemands() {
            try {

                var demandTask = GetDemandsReportAsync();
                var demands = await demandTask;

                string colDemandNr = ScmResource.DemandNr;
                

                DataTable demandTable = new DataTable();
                DataColumn col = new DataColumn(colDemandNr, typeof(string));
                demandTable.Columns.Add(col);
                
                foreach (var demand in demands) {
                    DataRow newRow = demandTable.NewRow();
                    newRow[colDemandNr] = demand.demand_nr;

                    demandTable.Rows.Add(newRow);
                }

                Excel excel = new Excel();
                string fileName = GetXlsFileName("Demands");
                using (var excelDoc = excel.GenerateExcelWorkbookDoc(demandTable, new List<double> { 25, 70, 20, 50, 40, 40 })) {

                    var pack = excelDoc.SaveAs(fileName);
                    excelDoc.Close();
                    pack.Close();

                }

                excel = null;
                Process.Start(fileName);
            } catch (Exception ex) {
                HandleError(ex);
            } finally {
                //IsBusy = false;
            }
        }

        #endregion

        #region Methods
        private List<TimePeriod> GetTimePeriods() {
            List<TimePeriod> periods = new List<TimePeriod>();

            TimePeriod timePeriod = new TimePeriod();
            timePeriod.id = PERIOD_NOLIMIT;
            timePeriod.name = ScmResource.NoLimit;
            periods.Add(timePeriod);

            timePeriod = new TimePeriod();
            timePeriod.id = PERIOD_TODAY;
            timePeriod.name = ScmResource.Today;
            periods.Add(timePeriod);

            timePeriod = new TimePeriod();
            timePeriod.id = PERIOD_YESTERDAY;
            timePeriod.name = ScmResource.Yesterday;
            periods.Add(timePeriod);

            timePeriod = new TimePeriod();
            timePeriod.id = PERIOD_WEEK;
            timePeriod.name = ScmResource.LastWeek;
            periods.Add(timePeriod);

            timePeriod = new TimePeriod();
            timePeriod.id = PERIOD_BEFORE;
            timePeriod.name = ScmResource.Before;
            periods.Add(timePeriod);

            return periods;

        }

        public void SelectNomenclature(object sender) {
            CheckBox ckb = (CheckBox)sender;
            
            foreach (var nom in NomenclatureList) {
                nom.is_selected = (bool)ckb.IsChecked;
            }
        }

        public void RemoveNomenclatures() {
            var selNoms = (from selNomGrd in NomenclatureList
                           where selNomGrd.is_selected == true
                           select selNomGrd).ToList();

            if (selNoms == null || selNoms.Count == 0) {
                ScmDispatcher.Invoke(() => {
                    MessageBox.Show(ScmResource.SelectNomenclatures, ScmResource.SelectNomenclatures, MessageBoxButton.OK, MessageBoxImage.Information);
                });

                return;
            }

            RemoveNom(selNoms);
            
        }

        private void SetGridDateFilter() {
            switch (SelectedPeriodIdNom) {
                case PERIOD_TODAY:
                    m_UcGrdColHeaderFilterDateNom.DateFrom = new DateTime(
                        DateTime.Now.Year,
                        DateTime.Now.Month,
                        DateTime.Now.Day);
                    m_UcGrdColHeaderFilterDateNom.DateTo = new DateTime(
                        DateTime.Now.AddDays(1).Year,
                        DateTime.Now.AddDays(1).Month,
                        DateTime.Now.AddDays(1).Day);
                    break;
                case PERIOD_YESTERDAY:
                    m_UcGrdColHeaderFilterDateNom.DateFrom = new DateTime(
                        DateTime.Now.AddDays(-1).Year,
                        DateTime.Now.AddDays(-1).Month,
                        DateTime.Now.AddDays(-1).Day);
                    m_UcGrdColHeaderFilterDateNom.DateTo = DateTime.Now;
                    break;
                case PERIOD_WEEK:
                    m_UcGrdColHeaderFilterDateNom.DateFrom = new DateTime(
                        DateTime.Now.AddDays(-7).Year,
                        DateTime.Now.AddDays(-7).Month,
                        DateTime.Now.AddDays(-7).Day);
                    m_UcGrdColHeaderFilterDateNom.DateTo = new DateTime(
                        DateTime.Now.Year,
                        DateTime.Now.Month,
                        DateTime.Now.Day);
                    break;
                case PERIOD_BEFORE:
                    m_UcGrdColHeaderFilterDateNom.DateFrom = null;
                    m_UcGrdColHeaderFilterDateNom.DateTo = new DateTime(
                        DateTime.Now.AddDays(-7).Year,
                        DateTime.Now.AddDays(-7).Month,
                        DateTime.Now.AddDays(-7).Day);
                    break;
                default:
                    m_UcGrdColHeaderFilterDateNom.DateFrom = null;
                    m_UcGrdColHeaderFilterDateNom.DateTo = null;
                    break;
            }
        }

        private void SetDisplyingInfoNomenclatures() {
            DisplayingRows = ScmResource.DisplayingFromToOf
                    .Replace("{0}", GetDisplayItemsFromInfo().ToString())
                    .Replace("{1}", GetDisplayItemsToInfo().ToString())
                    .Replace("{2}", RowsCount.ToString());
            
        }

        private void SetDisplyingInfoDemands() {
            DisplayingRows2 = ScmResource.DisplayingFromToOf
                    .Replace("{0}", GetDisplayItemsFromInfo2().ToString())
                    .Replace("{1}", GetDisplayItemsToInfo2().ToString())
                    .Replace("{2}", RowsCount2.ToString());

        }

        public void RefreshDashboard() {
            RefreshGridData();
            RefreshGridData2();

            RefreshPendingNomenclatureCount();

            RefreshPendingDemandCount();
        }

        private void RefreshPendingNomenclatureCount() {
            int pendNomCount;
            pendNomCount = WcfScm.GetPendingNomenclaturesNumber();
            PendingNomenclatures = pendNomCount;
            LastImportDate = WcfScm.GetLastImportDateText();
        }

        private void RefreshPendingDemandCount() {
            int pendDemandCount;
            pendDemandCount = WcfScm.GetPendingDemandsNumber(GetDemandUserId());
            PendingDemands = pendDemandCount;
        }

        private int GetDemandUserId() {
            int userId = DataNulls.INT_NULL;
            if (IsOnlyMyDemands) {
                userId = CurrentUser.User.id;
            }

            return userId;
        }

        private void SetDemandPredefinedFilter() {
            SetFilterField2(DemandData.IS_ACTIVE_FIELD, "1");

            if (GetDemandUserId() != DataNulls.INT_NULL) {
                //SetFilterField2(DemandData.REQUESTOR_ID_FIELD, GetDemandUserId().ToString());
                string sql = null;
                
                if (CurrentUser.IsScmReferent && CurrentUser.IsApproveManager) {
                    sql = "((dd." + DemandData.STATUS_ID_FIELD + "<>" + (int)DemandRepository.Status.WaitForApproval
                        + " AND dd." + DemandData.REQUESTOR_ID_FIELD + "=" + GetDemandUserId().ToString()
                        + ") OR " + "(dd." + DemandData.STATUS_ID_FIELD + "=" + (int)DemandRepository.Status.WaitForApproval
                        + " AND dd." + DemandData.APP_MAN_ID_FIELD + "=" + GetDemandUserId().ToString() + "))";
                } else if (CurrentUser.IsScmReferent) {
                    sql = "(dd." + DemandData.STATUS_ID_FIELD + "<>" + (int)DemandRepository.Status.WaitForApproval
                        + " AND dd." + DemandData.REQUESTOR_ID_FIELD + "=" + GetDemandUserId().ToString()
                        + ")";
                } else if (CurrentUser.IsApproveManager) {
                    sql = "(dd." + DemandData.STATUS_ID_FIELD + "=" + (int)DemandRepository.Status.WaitForApproval
                        + " AND dd." + DemandData.APP_MAN_ID_FIELD + "=" + GetDemandUserId().ToString()
                        + ")";
                } else {
                    SetFilterField2(DemandData.REQUESTOR_ID_FIELD, DataNulls.INT_NULL.ToString());
                }
                SetFilterField2(sql);
            } else {
                RemoveFilterField2(DemandData.REQUESTOR_ID_FIELD);
                SetFilterField2(null);
            }
        }
        #endregion
    }

    public class TimePeriod {
        public int id { get; set; }
        public string name { get; set; }
    }
}
