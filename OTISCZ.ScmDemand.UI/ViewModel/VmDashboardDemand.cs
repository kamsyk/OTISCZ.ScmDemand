using Kamsyk.ExcelOpenXml;
using OTISCZ.CommonDb;
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
    public class VmDashboardDemand : VmBaseGrid2, INotifyPropertyChanged, INotifyDataErrorInfo {
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

        //private int m_SelectedPeriodIdNom = 0;
        //public int SelectedPeriodIdNom {
        //    get {
        //        return m_SelectedPeriodIdNom;
        //    }
        //    set {
        //        m_SelectedPeriodIdNom = value;
        //        OnPropertyChanged("SelectedPeriodIdNom");
        //        SetGridNomDateFilter();
        //    }
        //}

        private int m_SelectedPeriodIdDemand = 0;
        public int SelectedPeriodIdDemand {
            get {
                return m_SelectedPeriodIdDemand;
            }
            set {
                m_SelectedPeriodIdDemand = value;
                OnPropertyChanged("SelectedPeriodIdDemand");
                SetGridDateFilter();
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
                    try {
                        m_PendingDemands = WcfScm.GetPendingDemandsNumber(GetDemandUserId());
                    } catch (Exception ex) {
                        HandleError(ex, ScmDispatcher);
                    }
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

        private List<Demand_Status> m_DemandStatuses = null;
        public List<Demand_Status> DemandStatuses {
            get {
                if (m_DemandStatuses == null) {
                    var tmpDemandStatuses = WcfScm.GetDemandStatuses();
                    m_DemandStatuses = new List<Demand_Status>();
                    foreach (var demStat in tmpDemandStatuses) {
                        Demand_Status demandStatus = new Demand_Status();
                        demandStatus.id = demStat.id;
                        demandStatus.name = VmDemandDetail.GetDemandStatusText(demStat.id);
                        m_DemandStatuses.Add(demandStatus);
                    }
                    Demand_Status demandAllStatus = new Demand_Status();
                    demandAllStatus.id = DataNulls.INT_NULL;
                    demandAllStatus.name = ScmResource.CmbFilterAll;
                    m_DemandStatuses.Insert(0, demandAllStatus);
                }

                return m_DemandStatuses;
            }
            set {
                m_DemandStatuses = value;
                OnPropertyChanged("DemandStatuses");
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
        public VmDashboardDemand(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {
            DlgSetDisplyingInfoNomenclatures = new DlgSetDisplyingInfo(SetDisplyingInfoNomenclatures);
            DlgSetDisplyingInfoDemands = new DlgSetDisplyingInfo(SetDisplyingInfoDemands);
            SetFilterField(NomenclatureData.STATUS_ID_FIELD, "100");
            SetFilterField(NomenclatureData.IS_ACTIVE_FIELD, "1");

            SetFilterField2(DemandData.IS_ACTIVE_FIELD, "1");

            
            //Default Filter
            //SortFieldName2 = "last_status_modif_date_text";
            //SortDirection2 = ListSortDirection.Ascending;
            

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
            GridInit();
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
        //protected override void LoadNomenclatures() {
        //    try {
        //        base.LoadNomenclatures();
        //        //CalculatePagesCount();
        //        DisplayingRows = ScmResource.DisplayingFromToOf
        //            .Replace("{0}", GetDisplayItemsFromInfo().ToString())
        //            .Replace("{1}", GetDisplayItemsToInfo().ToString())
        //            .Replace("{2}", RowsCount.ToString());
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    }
        //}

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
                    var errList = new ScmFileImport().ImportData(
                        ProdisFolder,
                        CurrentUser.User.id,
                        dlgLoadFileInfo,
                        dlgLoadProgressInfo,
                        ScmDispatcher);

                    ProgressBarVisibility = Visibility.Collapsed;
                    ScmDispatcher.Invoke(() => {
                        if (errList == null || errList.Count == 0) {
                            MessageBox.Show(ScmResource.DataWasImported, ScmResource.DataWasImported, MessageBoxButton.OK, MessageBoxImage.Information);
                        } else {
                            MessageBox.Show(ScmResource.ErrorMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        //MessageBox.Show(ScmResource.ErrorMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                    ProgressBarVisibility = Visibility.Collapsed;

                    ScmDispatcher.Invoke(() => {
                        RefreshGridData();
                        RefreshPendingNomenclatureCount();
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
                string colSupplier = ScmResource.Supplier;
                string colNomenclatures = ScmResource.Nomenclatures;
                string colRequestor = ScmResource.Requestor;
                string colAppMan = ScmResource.ApprovalManager;
                string colDemandStatus = ScmResource.Status;
                string colCreated = ScmResource.Created;
                string colLastStatusChange = ScmResource.LastStatusModifDate;
                string colDaysInStatus = ScmResource.DaysInStatus;

                DataTable demandTable = new DataTable();
                DataColumn col = new DataColumn(colDemandNr, typeof(string));
                demandTable.Columns.Add(col);

                col = new DataColumn(colSupplier, typeof(string));
                demandTable.Columns.Add(col);

                col = new DataColumn(colNomenclatures, typeof(string));
                demandTable.Columns.Add(col);
                                
                col = new DataColumn(colRequestor, typeof(string));
                demandTable.Columns.Add(col);

                col = new DataColumn(colAppMan, typeof(string));
                demandTable.Columns.Add(col);

                col = new DataColumn(colDemandStatus, typeof(string));
                demandTable.Columns.Add(col);

                col = new DataColumn(colCreated, typeof(DateTime));
                demandTable.Columns.Add(col);

                col = new DataColumn(colLastStatusChange, typeof(DateTime));
                demandTable.Columns.Add(col);

                col = new DataColumn(colDaysInStatus, typeof(int));
                demandTable.Columns.Add(col);

                foreach (var demand in demands) {
                    DataRow newRow = demandTable.NewRow();
                    newRow[colDemandNr] = demand.demand_nr;
                    newRow[colSupplier] = demand.supplier_text;
                    newRow[colNomenclatures] = demand.nomenclatures_text;
                    newRow[colRequestor] = demand.requestor_name;
                    newRow[colAppMan] = demand.app_man_name;
                    newRow[colDemandStatus] = demand.status_text;
                    newRow[colCreated] = demand.created_date;
                    newRow[colLastStatusChange] = demand.last_status_modif_date;
                    newRow[colDaysInStatus] = demand.days_in_status;

                    demandTable.Rows.Add(newRow);
                }

                Excel excel = new Excel();
                string fileName = GetXlsFileName("Demands");
                using (var excelDoc = excel.GenerateExcelWorkbookDoc(demandTable, new List<double> { 25, 50, 70, 25, 25, 20, 20, 20, 20 })) {

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
            switch (m_SelectedPeriodIdDemand) {
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
                        DateTime.Now.AddDays(1).Year,
                        DateTime.Now.AddDays(1).Month,
                        DateTime.Now.AddDays(1).Day);
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
            //RefreshGridData();
            RefreshGridData2();

            //RefreshPendingNomenclatureCount();

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
            if (GetDemandUserId() != DataNulls.INT_NULL) {
                //SetFilterField2(DemandData.REQUESTOR_ID_FIELD, GetDemandUserId().ToString());
                int currUserId = GetDemandUserId();
                string sql = null;
                sql = "("
                        + "(dd.requestor_id = " + currUserId + " AND actdd.is_requestor_acive = 1)"
                        + " OR(dd.app_man_id = " + currUserId + " AND actdd.is_app_man_active = 1)"
                     + ")";
                //if (CurrentUser.IsScmReferent && CurrentUser.IsApproveManager) {
                //    sql = "((dd." + DemandData.STATUS_ID_FIELD + "<>" + (int)DemandRepository.Status.WaitForApproval
                //        + " AND dd." + DemandData.REQUESTOR_ID_FIELD + "=" + GetDemandUserId().ToString()
                //        + ") OR " + "(dd." + DemandData.STATUS_ID_FIELD + "=" + (int)DemandRepository.Status.WaitForApproval
                //        + " AND dd." + DemandData.APP_MAN_ID_FIELD + "=" + GetDemandUserId().ToString() + "))";
                //} else if (CurrentUser.IsScmReferent) {
                //    sql = "(dd." + DemandData.STATUS_ID_FIELD + "<>" + (int)DemandRepository.Status.WaitForApproval
                //        + " AND dd." + DemandData.REQUESTOR_ID_FIELD + "=" + GetDemandUserId().ToString()
                //        + ")";
                //} else if (CurrentUser.IsApproveManager) {
                //    //sql = "(dd." + DemandData.STATUS_ID_FIELD + "=" + (int)DemandRepository.Status.WaitForApproval
                //    //    + " AND dd." + DemandData.APP_MAN_ID_FIELD + "=" + GetDemandUserId().ToString()
                //    //    + ")";
                //    sql = "(actdd." + ActiveDemandData.IS_APP_MAN_ACTIVE_FIELD + "=1"
                //        + " AND dd." + DemandData.APP_MAN_ID_FIELD + "=" + GetDemandUserId().ToString()
                //        + ")";
                //} else {
                //    SetFilterField2(DemandData.REQUESTOR_ID_FIELD, DataNulls.INT_NULL.ToString());
                //}
                SetFilterField2(sql);
            } else {
                RemoveFilterField2(DemandData.REQUESTOR_ID_FIELD);
                SetFilterField2(null);
            }
        }
        #endregion
    }

    
}
