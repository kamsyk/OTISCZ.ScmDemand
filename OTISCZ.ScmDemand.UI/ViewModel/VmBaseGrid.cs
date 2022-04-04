using Kamsyk.ExcelOpenXml;
using OTISCZ.CommonDb;
using OTISCZ.ConcordeDataDictionary;
using OTISCZ.ScmDemand.Interface;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.DataDictionary;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Common;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.View;
using OTISCZ.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public abstract class VmBaseGrid : VmBase {
        #region Constants
        private const string CXAL_USER_NAME = "ConcordeWebProxyClient";
        private const string CXAL_PASSWORD = "d4ghj6,p}87'";
        private const string CXAL_ENCRYPT_PASSWORD = "rg.;r5er8ee.-";
        #endregion

        #region Enums
        public enum FilterFromTo {
            No,
            From,
            To
        }
        #endregion

        #region Delegates
        public delegate void DlgSetDisplyingInfo();
        #endregion

        #region Properties
        private DlgSetDisplyingInfo m_DlgSetDisplyingInfoNomenclatures = null;
        protected DlgSetDisplyingInfo DlgSetDisplyingInfoNomenclatures {
            get { return m_DlgSetDisplyingInfoNomenclatures; }
            set { m_DlgSetDisplyingInfoNomenclatures = value; }
        }

        private DlgSetDisplyingInfo m_DlgSetDisplyingInfoDemands = null;
        protected DlgSetDisplyingInfo DlgSetDisplyingInfoDemands {
            get { return m_DlgSetDisplyingInfoDemands; }
            set { m_DlgSetDisplyingInfoDemands = value; }
        }

        private DlgSetDisplyingInfo m_DlgSetDisplyingInfoSuppliers = null;
        protected DlgSetDisplyingInfo DlgSetDisplyingInfoSuppliers {
            get { return m_DlgSetDisplyingInfoSuppliers; }
            set { m_DlgSetDisplyingInfoSuppliers = value; }
        }

        private List<int> m_PageSizeList = new List<int> { 10, 20, 50, 100, 200, 500, 1000 };
        public List<int> PageSizeList {
            get { return m_PageSizeList; }
        }

        private int m_PageSize = 10;
        public virtual int PageSize {
            get {
                return m_PageSize;
            }
            set {
                m_PageSize = value;
                PageNr = 1;
                OnPropertyChanged("PageSize");
            }
        }

        private int m_PageNr = 1;
        public virtual int PageNr {
            get { return m_PageNr; }
            set {
                if (value < 1) {
                    return;
                }
                if (value > m_PagesCount) {
                    return;
                }
                m_PageNr = value;
                RefreshGridData();
                DisplayGridFooterButtons();
                OnPropertyChanged("PageNr");
            }
        }

        
        private int m_RowsCount = 0;
        public virtual int RowsCount {
            get { return m_RowsCount; }
            set {
                m_RowsCount = value;
                CalculatePagesCount();
            }
        }

        private int m_PagesCount = 0;
        public virtual int PagesCount {
            get {
                return m_PagesCount;
            }
            set {
                if (value < 1) {
                    value = 1;
                }
                m_PagesCount = value;
                OnPropertyChanged("PagesCount");
            }
        }

        private string m_DisplayingRows = null;
        public virtual string DisplayingRows {
            get {
                return m_DisplayingRows;
            }
            set {
                m_DisplayingRows = value;
                OnPropertyChanged("DisplayingRows");
            }
        }

        private Visibility m_PreviousEnabledButtonVisibility = Visibility.Collapsed;
        public Visibility PreviousEnabledButtonVisibility {
            get {
                return m_PreviousEnabledButtonVisibility;
            }
            set {
                m_PreviousEnabledButtonVisibility = value;
                OnPropertyChanged("PreviousEnabledButtonVisibility");
            }
        }

        private Visibility m_PreviousDisabledButtonVisibility = Visibility.Visible;
        public Visibility PreviousDisabledButtonVisibility {
            get {
                return m_PreviousDisabledButtonVisibility;
            }
            set {
                m_PreviousDisabledButtonVisibility = value;
                OnPropertyChanged("PreviousDisabledButtonVisibility");
            }
        }

        private Visibility m_NextEnabledButtonVisibility = Visibility.Visible;
        public Visibility NextEnabledButtonVisibility {
            get {
                return m_NextEnabledButtonVisibility;
            }
            set {
                m_NextEnabledButtonVisibility = value;
                OnPropertyChanged("NextEnabledButtonVisibility");
            }
        }

        private Visibility m_NextDisabledButtonVisibility = Visibility.Collapsed;
        public Visibility NextDisabledButtonVisibility {
            get {
                return m_NextDisabledButtonVisibility;
            }
            set {
                m_NextDisabledButtonVisibility = value;
                OnPropertyChanged("NextDisabledButtonVisibility");
            }
        }

        private Visibility m_ImportButtonVisibility = Visibility.Visible;
        public Visibility ImportButtonVisibility {
            get {
                return m_ImportButtonVisibility;
            }
            set {
                m_ImportButtonVisibility = value;
                OnPropertyChanged("ImportButtonVisibility");
            }
        }

        //private bool m_isBusy= false;
        //public bool IsBusy {
        //    get {
        //        return m_isBusy;
        //    }
        //    set {
        //        m_isBusy = value;
        //        IsGridButtonsEnabled = !m_isBusy;
        //        OnPropertyChanged("IsBusy");
        //    }
        //}

        private bool m_isGridButtonsEnabled = true;
        public bool IsGridButtonsEnabled {
            get {
                return m_isGridButtonsEnabled;
            }
            set {
                m_isGridButtonsEnabled = value;
                OnPropertyChanged("IsGridButtonsEnabled");
            }
        }

        private Visibility m_ProgressBarVisibility = Visibility.Collapsed;
        public Visibility ProgressBarVisibility {
            get { return m_ProgressBarVisibility; }
            set {
                m_ProgressBarVisibility = value;
                OnPropertyChanged("ProgressBarVisibility");
            }
        }

        private string m_ImportInfo = null;
        public string ImportInfo {
            get { return m_ImportInfo; }
            set {
                m_ImportInfo = value;
                OnPropertyChanged("ImportInfo");
            }
        }

        private int m_PbLineCount = 0;
        public int PbLineCount {
            get { return m_PbLineCount; }
            set {
                m_PbLineCount = value;
                OnPropertyChanged("PbLineCount");
            }
        }

        private int m_PbLineNr = 0;
        public int PbLineNr {
            get { return m_PbLineNr; }
            set {
                m_PbLineNr = value;
                OnPropertyChanged("PbLineNr");
            }
        }

        private string m_SortFieldName = null;
        public string SortFieldName {
            get { return m_SortFieldName; }
            set { m_SortFieldName = value; }
        }
        private ListSortDirection m_SortDirection = ListSortDirection.Ascending;

        private List<FilterField> m_FilterFields = null;
        public List<FilterField> FilterFields {
            get { return m_FilterFields; }
            set { m_FilterFields = value; }
        }

        private DataGrid m_DataGrid = null;
        protected DataGrid DataGrid {
            get { return m_DataGrid; }
        }

        private NomenclatureExtend[] m_NomenclatureList = null;
        public virtual NomenclatureExtend[] NomenclatureList {
            get {
                if (m_NomenclatureList == null) {
                    try {
                        LoadNomenclatures();
                    } catch (Exception ex) {
                        HandleError(ex);
                    }
                }
                return m_NomenclatureList;
            }
            set {
                m_NomenclatureList = value;
                OnPropertyChanged("NomenclatureList");
                SetDataGridColumnSort();
            }
        }

        private List<SupplierExtend> m_SupplierList = null;
        public virtual List<SupplierExtend> SupplierList {
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
                SetDataGridColumnSort();
            }
        }

        private DemandExtend[] m_DemandList = null;
        public virtual DemandExtend[] DemandList {
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

        private ScmSetting m_scmSetting = null;
        private ScmSetting ScmSetting {
            get {
                if (m_scmSetting == null) {
                    try {
                        //var setting = WsScm.GetScmSetting();
                        //m_scmSetting = new ScmSetting();
                        //SetValues(setting, m_scmSetting);
                        m_scmSetting = WcfScm.GetScmSetting();
                        
                    } catch (Exception ex) {
                        HandleError(ex);

                    }
                }

                return m_scmSetting;
            }
        }

        public string ProdisFolder {
            get {
                return ScmSetting.prodis_input_folder;
            }
        }

        public string ProdisPriceFolder {
            get {
                return ScmSetting.prodis_price_input_folder;
            }
        }
        #endregion

        #region Constructor
        public VmBaseGrid(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {
            
        }
        #endregion

        #region Abstract Methods
        public abstract void RefreshGridData();
        public abstract void ExportToExcel();
        public abstract void Import();
        public abstract void AddNew();
        #endregion

        #region Async Methods
        protected virtual async void LoadNomenclatures() {
            try {
                var nomenclatures = GetNomenclaturesAsync();
                NomenclatureList = await nomenclatures;
                if (m_DlgSetDisplyingInfoNomenclatures != null) {
                    m_DlgSetDisplyingInfoNomenclatures();
                }
                //DisplayingRows = ScmResource.DisplayingFromToOf
                //    .Replace("{0}", GetDisplayItemsFromInfo().ToString())
                //    .Replace("{1}", GetDisplayItemsToInfo().ToString())
                //    .Replace("{2}", RowsCount.ToString());
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        protected virtual async void LoadDemands() {
            try {
                var demands = GetDemandsAsync();
                DemandList = await demands;
                if (m_DlgSetDisplyingInfoDemands != null) {
                    m_DlgSetDisplyingInfoDemands();
                }
                
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        protected virtual async void LoadSuppliers() {
            try {
                var suppliers = GetSuppliersAsync();
                SupplierList = await suppliers;
                //DisplayingRows = ScmResource.DisplayingFromToOf
                //    .Replace("{0}", GetDisplayItemsFromInfo().ToString())
                //    .Replace("{1}", GetDisplayItemsToInfo().ToString())
                //    .Replace("{2}", RowsCount.ToString());

            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        protected virtual Task<NomenclatureExtend[]> GetNomenclaturesAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    int rowsCount;
                    string strFilter = GetFilter();
                    string strOrder = GetOrder();

                    //var wsNomenclatures = WsScm.GetNomenclatures(
                    //    strFilter,
                    //    strOrder,
                    //    PageSize,
                    //    true,
                    //    PageNr,
                    //    true,
                    //    out rowsCount,
                    //    out rowsCountSpecified);

                    var wsNomenclatures = WcfScm.GetNomenclatures(
                        strFilter,
                        strOrder,
                        PageSize,
                        PageNr,
                        out rowsCount);

                    RowsCount = rowsCount;

                    //ObservableCollection<NomenclatureExtend> nomenclatures = new ObservableCollection<NomenclatureExtend>();
                    foreach (var wsNomenclature in wsNomenclatures) {

                        wsNomenclature.status_text = GetNomenclatureStatusText(wsNomenclature.status_id);
                        wsNomenclature.new_demand_text = ScmResource.NewDemand;
                        if (wsNomenclature.is_plnaknavrh == true) {
                            wsNomenclature.plnaknavrh_text = ScmResource.Yes;
                        } else {
                            wsNomenclature.plnaknavrh_text = ScmResource.No;
                        }
                    }

                    
                    //return nomenclatures;
                    return wsNomenclatures;
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        protected virtual Task<DemandExtend[]> GetDemandsAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    int rowsCount;
                    //string strFilter = GetFilter();
                    var filterField = GetFilterFields();
                    string strOrder = GetOrder();

                    
                    var wsDemands = WcfScm.GetDemands(
                        filterField,
                        strOrder,
                        PageSize,
                        PageNr,
                        out rowsCount);

                    RowsCount = rowsCount;

                    //ObservableCollection<DemandExtend> demands = new ObservableCollection<DemandExtend>();
                    //foreach (var wsDemand in wsDemands) {
                    //    DemandExtend demandExtend = new DemandExtend();
                    //    SetValues(wsDemand, demandExtend);
                    //    //nomenclature.status_text = GetNomenclatureStatus(nomenclature.status_id);

                    //    demands.Add(demandExtend);
                    //}

                    return wsDemands;
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        protected virtual Task<List<SupplierExtend>> GetSuppliersAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    int rowsCount;
                    string strFilter = GetFilter();
                    string strOrder = GetOrder();

                    var wsSuppliers = WcfScm.GetSuppliers(
                        strFilter,
                        strOrder,
                        PageSize,
                        PageNr,
                        out rowsCount);

                    RowsCount = rowsCount;

                    var suppliers = GetSuppliersExtends(wsSuppliers);

                    return suppliers;
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        protected virtual Task ImportNomenclaturesAsync() {
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
                    });
                    ProgressBarVisibility = Visibility.Collapsed;

                    //RefreshGridData();
                } catch (Exception ex) {

                    ScmDispatcher.Invoke(() => {
                        HandleError(ex);
                    });

                } finally {
                    ProgressBarVisibility = Visibility.Collapsed;
                }
            });
        }

        public async void ExportToExcelNomenclatures() {
            try {

                var nomenclatures = GetNomenclaturesReportAsync();
                var nomenclaturesReport = await nomenclatures;

                string colNameNomenclature = ScmResource.Nomenclature;
                string colNameName = ScmResource.NameIt;
                string colNameSpec = ScmResource.Specification;
                string colNameMaterialGroup = ScmResource.MaterialGroup;
                string colNameCreated = ScmResource.Created;
                string colSource = ScmResource.NomenclatureSource;


                DataTable nomenclaturesTable = new DataTable();
                DataColumn col = new DataColumn(colNameNomenclature, typeof(string));
                nomenclaturesTable.Columns.Add(col);
                col = new DataColumn(colNameName, typeof(string));
                nomenclaturesTable.Columns.Add(col);
                col = new DataColumn(colNameSpec, typeof(string));
                nomenclaturesTable.Columns.Add(col);
                col = new DataColumn(colNameMaterialGroup, typeof(string));
                nomenclaturesTable.Columns.Add(col);
                col = new DataColumn(colNameCreated, typeof(DateTime));
                nomenclaturesTable.Columns.Add(col);
                col = new DataColumn(colSource, typeof(string));
                nomenclaturesTable.Columns.Add(col);

                foreach (var nomenclature in nomenclaturesReport) {
                    DataRow newRow = nomenclaturesTable.NewRow();
                    newRow[colNameNomenclature] = nomenclature.nomenclature_key;
                    newRow[colNameName] = nomenclature.name;
                    newRow[colNameSpec] = nomenclature.specification;
                    newRow[colNameMaterialGroup] = nomenclature.material_group_text;
                    if (nomenclature.created_date == null) {
                        newRow[colNameCreated] = DBNull.Value;
                    } else {
                        newRow[colNameCreated] = nomenclature.created_date;
                    }
                    newRow[colSource] = NomenclatureRepository.GetSourceText(nomenclature.source_id);

                    nomenclaturesTable.Rows.Add(newRow);
                }

                Excel excel = new Excel();
                string fileName = GetXlsFileName("Nomenclatures");
                using (var excelDoc = excel.GenerateExcelWorkbookDoc(nomenclaturesTable, new List<double> { 25, 70, 70, 20, 40, 40 })) {

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

        public virtual async void ExportToExcelSuppliers() {
            try {

                var suppliers = GetSuppliersReportAsync();
                var suppliersReport = await suppliers;

                ExportToExcelSuppliersOpen(suppliersReport);

            //    string colNameSuppName = ScmResource.NameIt;
            //    string colNameSuppId = ScmResource.SupplierId;
            //    string colNameStreet = ScmResource.Street;
            //    string colNameCity = ScmResource.City;
            //    string colNameZip = ScmResource.Zip;
            //    string colNameCountry = ScmResource.Country;
            //    string colNameApproved = ScmResource.ApprovedSupplier;
            //    string colNameActive = ScmResource.Active;

                //    DataTable suppliersTable = new DataTable();
                //    DataColumn col = new DataColumn(colNameSuppName, typeof(string));
                //    suppliersTable.Columns.Add(col);
                //    col = new DataColumn(colNameSuppId, typeof(string));
                //    suppliersTable.Columns.Add(col);
                //    col = new DataColumn(colNameStreet, typeof(string));
                //    suppliersTable.Columns.Add(col);
                //    col = new DataColumn(colNameCity, typeof(string));
                //    suppliersTable.Columns.Add(col);
                //    col = new DataColumn(colNameZip, typeof(string));
                //    suppliersTable.Columns.Add(col);
                //    col = new DataColumn(colNameCountry, typeof(string));
                //    suppliersTable.Columns.Add(col);
                //    col = new DataColumn(colNameApproved, typeof(bool));
                //    suppliersTable.Columns.Add(col);
                //    col = new DataColumn(colNameActive, typeof(bool));
                //    suppliersTable.Columns.Add(col);

                //    foreach (var supplier in suppliersReport) {
                //        DataRow newRow = suppliersTable.NewRow();
                //        newRow[colNameSuppName] = supplier.supp_name;
                //        newRow[colNameSuppId] = supplier.supplier_id;
                //        newRow[colNameStreet] = supplier.street_part1;
                //        newRow[colNameCity] = supplier.city;
                //        newRow[colNameZip] = supplier.zip;
                //        newRow[colNameCountry] = supplier.country;
                //        newRow[colNameApproved] = supplier.is_approved;
                //        newRow[colNameActive] = supplier.active;

                //        suppliersTable.Rows.Add(newRow);
                //    }

                //    Excel excel = new Excel();
                //    string fileName = GetXlsFileName("Suppliers");
                //    using (var excelDoc = excel.GenerateExcelWorkbookDoc(suppliersTable, new List<double> { 25, 70, 20, 50, 40, 40 })) {

                //        var pack = excelDoc.SaveAs(fileName);
                //        excelDoc.Close();
                //        pack.Close();

                //    }

                //    excel = null;
                //    Process.Start(fileName);
                } catch (Exception ex) {
                    HandleError(ex);
                } finally {
                    //IsBusy = false;
                }
            }

        public void ExportToExcelSuppliersOpen(SupplierExtend[] suppliersReport) {
           
                                
                string colNameSuppName = ScmResource.NameIt;
                string colNameSuppId = ScmResource.SupplierId;
                string colNameStreet = ScmResource.Street;
                string colNameCity = ScmResource.City;
                string colNameZip = ScmResource.Zip;
                string colNameCountry = ScmResource.Country;
                string colNameApproved = ScmResource.ApprovedSupplier;
                string colNameActive = ScmResource.Active;

                DataTable suppliersTable = new DataTable();
                DataColumn col = new DataColumn(colNameSuppName, typeof(string));
                suppliersTable.Columns.Add(col);
                col = new DataColumn(colNameSuppId, typeof(string));
                suppliersTable.Columns.Add(col);
                col = new DataColumn(colNameStreet, typeof(string));
                suppliersTable.Columns.Add(col);
                col = new DataColumn(colNameCity, typeof(string));
                suppliersTable.Columns.Add(col);
                col = new DataColumn(colNameZip, typeof(string));
                suppliersTable.Columns.Add(col);
                col = new DataColumn(colNameCountry, typeof(string));
                suppliersTable.Columns.Add(col);
                col = new DataColumn(colNameApproved, typeof(bool));
                suppliersTable.Columns.Add(col);
                col = new DataColumn(colNameActive, typeof(bool));
                suppliersTable.Columns.Add(col);

                foreach (var supplier in suppliersReport) {
                    DataRow newRow = suppliersTable.NewRow();
                    newRow[colNameSuppName] = supplier.supp_name;
                    newRow[colNameSuppId] = supplier.supplier_id;
                    newRow[colNameStreet] = supplier.street_part1;
                    newRow[colNameCity] = supplier.city;
                    newRow[colNameZip] = supplier.zip;
                    newRow[colNameCountry] = supplier.country;
                    newRow[colNameApproved] = supplier.is_approved;
                    newRow[colNameActive] = supplier.active;

                    suppliersTable.Rows.Add(newRow);
                }

                Excel excel = new Excel();
                string fileName = GetXlsFileName("Suppliers");
                using (var excelDoc = excel.GenerateExcelWorkbookDoc(suppliersTable, new List<double> { 50, 25, 50, 50, 25, 40 })) {

                    var pack = excelDoc.SaveAs(fileName);
                    excelDoc.Close();
                    pack.Close();

                }

                excel = null;
                Process.Start(fileName);
            
        }

        private Task<NomenclatureExtend[]> GetNomenclaturesReportAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    string strFilter = GetFilter();
                    string strOrder = GetOrder();

                    var wsNomenclatures = WcfScm.GetNomenclaturesReport(
                        strFilter,
                        strOrder);

                    //List<NomenclatureExtend> nomenclatures = new List<NomenclatureExtend>();
                    //foreach (var wsNomenclature in wsNomenclatures) {
                    //    NomenclatureExtend nomenclature = new NomenclatureExtend();
                    //    SetValues(wsNomenclature, nomenclature);

                    //    nomenclatures.Add(nomenclature);
                    //}

                    return wsNomenclatures;
                } catch (Exception ex) {
                    IsBusy = false;
                    throw ex;
                } finally {
                    IsBusy = false;
                }
            });
        }

        protected virtual Task<SupplierExtend[]> GetSuppliersReportAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    string strFilter = GetFilter();
                    string strOrder = GetOrder();

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

        protected async void ImportSuppliers() {
            try {
                var task = ImportSuppliersAsync();
                await task;

            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        protected Task ImportSuppliersAsync() {
            return Task.Run(() => {
                IsBusy = true;
                try {
                    new Concorde().ImportSuppliers();
                    //    WsConcordeSupplier.InternalRequest wsSupplier = new WsConcordeSupplier.InternalRequest();
                    //    WsConcordeSupplier.AuthHeader authHeader = new WsConcordeSupplier.AuthHeader();

                    //    authHeader.Username = CXAL_USER_NAME;
                    //    authHeader.Password = Des.Encrypt(CXAL_PASSWORD, CXAL_ENCRYPT_PASSWORD);
                    //    wsSupplier.AuthHeaderValue = authHeader;
                    //    wsSupplier.Timeout = 1800000;

                    //    var wsSuppliers = wsSupplier.GetActiveCreditors(WsConcordeSupplier.CXALVersion.ESC, true);

                    //    List<int> htActiveSuppliers = new List<int>();

                    //    foreach (DataRow concordeSuppRow in wsSuppliers.Tables[0].Rows) {
                    //        //id
                    //        string accNr = null;
                    //        if (concordeSuppRow[CredTableTable.ACCOUNTNUMBER] != null) {
                    //            accNr = GetDbText(concordeSuppRow[CredTableTable.ACCOUNTNUMBER].ToString());
                    //        }
                    //        if (accNr == null) {
                    //            continue;
                    //        }
                    //        accNr = accNr.Trim();

                    //        //name
                    //        string name = null;
                    //        if (concordeSuppRow[CredTableTable.NAME] != null) {
                    //            name = ConvertCzSKString(GetDbText(concordeSuppRow[CredTableTable.NAME].ToString()));
                    //        }
                    //        if (name == null || name.Trim().Length == 0) continue;

                    //        var wsSupplierData = WcfScm.GetSupplierBySupplierId(accNr);
                    //        SupplierExtend supplier = null;
                    //        if (wsSupplierData != null) {
                    //            supplier = new SupplierExtend();
                    //            SetValues(wsSupplierData, supplier);
                    //        }
                    //        if (supplier == null) {
                    //            supplier = new SupplierExtend();
                    //            supplier.id = DataNulls.INT_NULL;
                    //        } else {
                    //            if (!htActiveSuppliers.Contains(supplier.id)) {
                    //                htActiveSuppliers.Add(supplier.id);
                    //            }
                    //        }

                    //        supplier.supplier_id = RemoveNotAllowedExcelChars(accNr);
                    //        supplier.active = true;
                    //        supplier.supp_name = RemoveNotAllowedExcelChars(name);

                    //        //dic
                    //        string dic = null;
                    //        if (concordeSuppRow[CredTableTable.VATNUMBER] != null) {
                    //            dic = GetDbText(concordeSuppRow[CredTableTable.VATNUMBER].ToString());
                    //        }
                    //        supplier.dic = RemoveNotAllowedExcelChars(dic);

                    //        //country
                    //        string country = null;
                    //        if (concordeSuppRow[CredTableTable.COUNTRY] != null) {
                    //            country = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.COUNTRY].ToString()));
                    //        }
                    //        supplier.country = RemoveNotAllowedExcelChars(country);

                    //        //contact person
                    //        string contactPerson = null;
                    //        if (concordeSuppRow[CredTableTable.ATTENTION] != null) {
                    //            contactPerson = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ATTENTION].ToString()));
                    //        }
                    //        supplier.contact_person = RemoveNotAllowedExcelChars(contactPerson);

                    //        //phone
                    //        string phone = null;
                    //        if (concordeSuppRow[CredTableTable.PHONE] != null) {
                    //            phone = GetDbText(concordeSuppRow[CredTableTable.PHONE].ToString());
                    //        }
                    //        supplier.phone = RemoveNotAllowedExcelChars(phone);


                    //        //only for CZ SK
                    //        //mobile phone
                    //        string mobilePhone = null;
                    //        if (concordeSuppRow[CredTableTable.OTISMOBIL] != null) {
                    //            mobilePhone = GetDbText(concordeSuppRow[CredTableTable.OTISMOBIL].ToString());
                    //        }
                    //        supplier.mobile_phone = RemoveNotAllowedExcelChars(mobilePhone);


                    //        //fax
                    //        string fax = null;
                    //        if (concordeSuppRow[CredTableTable.FAX] != null) {
                    //            fax = GetDbText(concordeSuppRow[CredTableTable.FAX].ToString());
                    //        }
                    //        supplier.fax = RemoveNotAllowedExcelChars(fax);

                    //        //mail
                    //        string mail = null;
                    //        if (concordeSuppRow[CredTableTable.OTISEMAIL] != null) {
                    //            mail = GetDbText(concordeSuppRow[CredTableTable.OTISEMAIL].ToString());
                    //        }
                    //        supplier.email = RemoveNotAllowedExcelChars(mail);


                    //        //street_part1
                    //        string streetPart1 = null;
                    //        if (concordeSuppRow[CredTableTable.ADDRESS1] != null) {
                    //            streetPart1 = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ADDRESS1].ToString()));
                    //        }
                    //        supplier.street_part1 = RemoveNotAllowedExcelChars(streetPart1);

                    //        //street_part2
                    //        string streetPart2 = null;
                    //        if (concordeSuppRow[CredTableTable.ADDRESS2] != null) {
                    //            streetPart2 = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ADDRESS2].ToString()));
                    //        }
                    //        supplier.street_part2 = RemoveNotAllowedExcelChars(streetPart2);

                    //        //city
                    //        string city = null;
                    //        if (concordeSuppRow[CredTableTable.ADDRESS3] != null) {
                    //            city = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ADDRESS3].ToString()));
                    //        }
                    //        supplier.city = RemoveNotAllowedExcelChars(city);

                    //        //zip code
                    //        string zip = null;
                    //        if (concordeSuppRow[CredTableTable.ZIP] != null) {
                    //            zip = GetDbText(concordeSuppRow[CredTableTable.ZIP].ToString());
                    //        }
                    //        supplier.zip = RemoveNotAllowedExcelChars(zip);

                    //        //creditor group
                    //        int creditorGroup = DataNulls.INT_NULL;
                    //        if (concordeSuppRow[CredTableTable.CREDITORGROUP] != null) {
                    //            creditorGroup = ConvertData.ToInt32(concordeSuppRow[CredTableTable.CREDITORGROUP].ToString().Trim());
                    //        }
                    //        supplier.creditor_group = creditorGroup;

                    //        //bank account
                    //        string bankAccount = null;
                    //        if (concordeSuppRow[CredTableTable.BANKACCOUNT] != null) {
                    //            bankAccount = GetDbText(concordeSuppRow[CredTableTable.BANKACCOUNT].ToString().Trim());
                    //        }
                    //        supplier.bank_account = RemoveNotAllowedExcelChars(bankAccount);

                    //        //var wsUpdSupplier = GetWsSupplier(supplier);
                    //        WcfScm.SaveSupplier(supplier, CurrentUser.User.id, false, true);
                    //    }

                    //    if (htActiveSuppliers.Count > 0) {
                    //        int[] iActiveSupps = new int[htActiveSuppliers.Count];
                    //        for (int i = 0; i < htActiveSuppliers.Count; i++) {
                    //            iActiveSupps[i] = htActiveSuppliers.ElementAt(i);
                    //        }
                    //        WcfScm.DeactiveSuppliers(iActiveSupps, CurrentUser.User.id);
                    //    }
                } catch (Exception ex) {
                    IsBusy = false;
                    HandleError(ex);
                } finally {
                    IsBusy = false;
                }
            });

        }
        #endregion

        #region Methods
        protected virtual void CalculatePagesCount() {
            if (m_PageSize == 0) {
                PagesCount = 0;
                return;
            }

            decimal dPgc = (Convert.ToDecimal( m_RowsCount) / Convert.ToDecimal(m_PageSize));
            decimal dPgcFloor = Decimal.Floor(dPgc);

            int iPgc = Convert.ToInt16(dPgcFloor);
            if (dPgc != dPgcFloor) {
                iPgc++;
            }

            PagesCount = iPgc;
        }

        private void DisplayGridFooterButtons() {
            if (m_PageNr > 1) {
                PreviousEnabledButtonVisibility = Visibility.Visible;
                PreviousDisabledButtonVisibility = Visibility.Collapsed;
            } else {
                PreviousEnabledButtonVisibility = Visibility.Collapsed;
                PreviousDisabledButtonVisibility = Visibility.Visible;
            }

            if (m_PageNr < m_PagesCount) {
                NextEnabledButtonVisibility = Visibility.Visible;
                NextDisabledButtonVisibility = Visibility.Collapsed;
            }
            else {
                NextEnabledButtonVisibility = Visibility.Collapsed;
                NextDisabledButtonVisibility = Visibility.Visible;
            }
        }

        private void FixPageNr() {
            if (m_PageNr < 1) {
                PageNr = 1;
            }

            if (m_PageNr > m_PagesCount) {
                PageNr = m_PagesCount;
            }
        }

        protected void GridInit() {
            DisplayGridFooterButtons();
            FixPageNr();
        }

        protected string GetXlsFileName(string fileNameType) {
            string folder = new FileInfo(System.Windows.Application.ResourceAssembly.Location).Directory.FullName;

            folder = System.IO.Path.Combine(folder, "TmpExcel");

            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            string[] files = Directory.GetFiles(folder);
            foreach (var file in files) {
                try {
                    FileInfo fi = new FileInfo(file);
                    if (fi.CreationTime.AddDays(7) < DateTime.Now) {
                        File.Delete(file);
                    }
                } catch { }
            }

            string pureFileName = "ScmDemand_" + fileNameType;
            string fileName = System.IO.Path.Combine(folder, pureFileName + ".xlsx");
            int iIndex = 1;
            while (File.Exists(fileName)) {
                fileName = System.IO.Path.Combine(folder, pureFileName + "_" + iIndex + ".xlsx");
                iIndex++;
            }

            return fileName;
        }

        protected int GetDisplayItemsFromInfo() {
            if (m_PageNr == 0) {
                return 0;
            }

            if (RowsCount == 0) {
                return 0;
            }

            int iFrom = (PageNr - 1) * PageSize + 1;
                        
            return iFrom;

        }

        
        protected int GetDisplayItemsToInfo() {
            var iToInfo = (PageNr) * PageSize;
            if (iToInfo > RowsCount) {
                iToInfo = RowsCount;
            }
            return iToInfo;
        }

        protected void LoadProgressInfo(int lineNr) {
            PbLineNr = lineNr;
        }

        protected void LoadFileInfo(string info, int lineCount) {
            if (ProgressBarVisibility != Visibility.Visible) {
                ProgressBarVisibility = Visibility.Visible;
            }
            ImportInfo = info;
            PbLineCount = lineCount;
        }

        public void SetSort(DataGrid grid, DataGridColumn grdColumn) {
            m_DataGrid = grid;

            bool isSortDisabled = DataGridUtil.GetIsSortDisabled(grdColumn);

            if (isSortDisabled) {
                return;
            }

            string sortField = grdColumn.SortMemberPath;
            if (grdColumn.SortDirection == ListSortDirection.Ascending) {
                grdColumn.SortDirection = ListSortDirection.Descending;
            } else {
                grdColumn.SortDirection = ListSortDirection.Ascending;
            }

            //Clear other Columns Sort
            foreach (DataGridColumn col in grid.Columns) {
                if (col.SortMemberPath != sortField) {
                    col.SortDirection = null;
                }
            }

            m_SortFieldName = sortField;
            m_SortDirection = (ListSortDirection)grdColumn.SortDirection;

            PageNr = 1;
        }

        

        protected string GetOrder() {
            if (String.IsNullOrWhiteSpace(m_SortFieldName)) {
                return null;
            }

            string strSort = m_SortFieldName;
            if (m_SortDirection == ListSortDirection.Descending) {
                strSort += BaseRepository<Nomenclature>.UrlParamValueDelimiter + "DESC";
            } else {
                strSort += BaseRepository<Nomenclature>.UrlParamValueDelimiter + "ASC";
            }

            return strSort;
        }

        protected void SetDataGridColumnSort() {
            if (String.IsNullOrWhiteSpace(m_SortFieldName) || m_DataGrid == null) {
                return;
            }

            foreach (DataGridColumn col in m_DataGrid.Columns) {
                if (col.SortMemberPath == m_SortFieldName) {
                    col.SortDirection = m_SortDirection;
                    break;
                }
            }
        }

        public virtual void FilterGridData(object sender) {
            FilterGridData(sender, FilterFromTo.No);
        }

       
        public virtual async void FilterGridData(object sender, FilterFromTo fromTo) {
            if (!MainWindow.IsDebounceRunning) {
                var t = MainWindow.DebounceTimerAsync();
                await t;
                                
                UpdateFilter(sender, fromTo, ref m_FilterFields);

                PageNr = 1;
                
                MainWindow.IsDebounceRunning = false;
            }
        }

        protected void UpdateFilter(object sender, FilterFromTo fromTo, ref List<FilterField> FilterFields) {
            if (sender is UcGrdColHeaderFilterText) {
                UcGrdColHeaderFilterText ft = (UcGrdColHeaderFilterText)sender;
                UpdateFilter(ref FilterFields, ft.FieldName, ft.FilterText, fromTo);
            } else if (sender is UcGrdColHeaderFilterDate) {
                UcGrdColHeaderFilterDate ft = (UcGrdColHeaderFilterDate)sender;
                if (fromTo == FilterFromTo.From) {
                    UpdateFilter(ref FilterFields, ft.FieldName, ft.DateFromText, fromTo);
                } else if (fromTo == FilterFromTo.To) {
                    UpdateFilter(ref FilterFields, ft.FieldName, ft.DateToText, fromTo);
                }
            } else if (sender is UcGrdColHeaderFilterComboBox) {
                UcGrdColHeaderFilterComboBox cmb = (UcGrdColHeaderFilterComboBox)sender;
                if (cmb.CmbSelectedValue is Demand_Status) {
                    int statusId = DataNulls.INT_NULL;
                    if (cmb.CmbSelectedValue != null) {
                        Demand_Status demandStatus = (Demand_Status)cmb.CmbSelectedValue;
                        statusId = demandStatus.id;
                    }
                    string strStatus = (statusId == DataNulls.INT_NULL) ? null : statusId.ToString();
                    UpdateFilter(ref FilterFields, cmb.FieldName, strStatus, fromTo);
                } else if (cmb.CmbSelectedValue is Nomenclature_Source) {
                    int nomSourceId = DataNulls.INT_NULL;
                    if (cmb.CmbSelectedValue != null) {
                        Nomenclature_Source nomSource = (Nomenclature_Source)cmb.CmbSelectedValue;
                        nomSourceId = nomSource.id;
                    }
                    string strNomSource = (nomSourceId == DataNulls.INT_NULL) ? null : nomSourceId.ToString();
                    UpdateFilter(ref FilterFields, NomenclatureData.SOURCE_ID_FIELD, strNomSource, fromTo);
                } else if (cmb.CmbSelectedValue is YesNo) {
                    int yesNoId = DataNulls.INT_NULL;
                    if (cmb.CmbSelectedValue != null) {
                        YesNo yesNo = (YesNo)cmb.CmbSelectedValue;
                        yesNoId = yesNo.value;
                    }
                    string strYesNo = (yesNoId == DataNulls.INT_NULL) ? null : yesNoId.ToString();
                    UpdateFilter(ref FilterFields, NomenclatureData.IS_PLNAKNAVRH_FIELD, strYesNo, fromTo);
                }
            }
        }

        private string ConvertCzSKString(string czSkString) {
            string unicodeString = czSkString;

            unicodeString = unicodeString.Replace("Ŕ", "Ü");

            return unicodeString;
        }

        private string GetDbText(string rawText) {
            if (rawText == null) {
                return null;
            }

            return rawText.Trim();
        }

        private string RemoveNotAllowedExcelChars(string strRawString) {
            if (strRawString == null) {
                return null;
            }

            string fixedValue = strRawString.Replace('\u0006'.ToString(), "");

            return fixedValue;
        }

        private string TransformConcordeString(string rawString) {

            return ConvertCzSKString(rawString).Trim();

        }

        private DataGridColumn FindColumnByScmName(DataGrid dataGrid, string scmColName) {

            foreach (var col in dataGrid.Columns) {
                string tscmColName = DataGridUtil.GetScmColumnName(col);
                if (tscmColName != null && tscmColName == scmColName) {
                    return col;
                }
            }

            return null;
        }

        public void SetColumnVisibilityNomenclature(DataGrid dataGrid) {
            if (CurrentUser.IsScmReferent) {
                return;
            }
                       
            var col = FindColumnByScmName(dataGrid, "without_demand");
            if (col != null) {
                col.Visibility = Visibility.Hidden;
            }

            col = FindColumnByScmName(dataGrid, "new_demand");
            if (col != null) {
                col.Visibility = Visibility.Hidden;
            }

        }

        protected List<SupplierExtend> GetSuppliersExtends(Supplier[] wsSuppliers) {
            List<SupplierExtend> suppliers = new List<SupplierExtend>();
            foreach (var wsSupplier in wsSuppliers) {
                SupplierExtend supplierExtend = GetSupplierExtend(wsSupplier);

                suppliers.Add(supplierExtend);
            }

            return suppliers;
        }

        protected SupplierExtend GetSupplierExtend(Supplier wsSupplier) {
            SupplierExtend supplierExtend = new SupplierExtend();
            SetValues(wsSupplier, supplierExtend);

            if (supplierExtend.is_approved) {
                supplierExtend.approved_visibility = Visibility.Visible;
            } else {
                supplierExtend.approved_visibility = Visibility.Collapsed;
            }
            supplierExtend.tooltip_approved = ScmResource.ApprovedSupplier;



            return supplierExtend;
        }

        //protected void SetPredefinedNomDashboardFilter() {
        //    FilterField nomStatus = new FilterField(NomenclatureData.STATUS_ID_FIELD, "100");
        //    if (m_FilterFields == null) {
        //        m_FilterFields = new List<FilterField>();
        //    }

        //    var sfilterField = (from sfilterFieldD in FilterFields
        //                        where sfilterFieldD.FieldName == NomenclatureData.STATUS_ID_FIELD
        //                        select sfilterFieldD).FirstOrDefault();

        //    if (sfilterField == null) {
        //        m_FilterFields.Add(nomStatus);
        //    }
        //}

        protected void SetFilterField(ref List<FilterField> filterFields, string fieldName, string filterValue) {
            SetFilterField(ref filterFields, fieldName, filterValue, FilterFromTo.No);
        }

        protected void SetFilterField(
            ref List<FilterField> filterFields, 
            string fieldName, 
            string filterValue,
            FilterFromTo fromTo) {
            
            if (filterFields == null) {
                filterFields = new List<FilterField>();
            }

            var sfilterField = (from sfilterFieldD in filterFields
                                where sfilterFieldD.FieldName == fieldName
                                && sfilterFieldD.FromTo == fromTo
                                select sfilterFieldD).FirstOrDefault();

            if (sfilterField == null) {
                FilterField ff = new FilterField(fieldName, filterValue, fromTo, null);
                filterFields.Add(ff);
            } else {
                sfilterField.FilterText = filterValue;
            }
        }

        protected void SetFilterField(
            ref List<FilterField> filterFields,
            string sqlFilter) {

            bool isFound = false;
            foreach (var filterField in filterFields) {
                if (!String.IsNullOrWhiteSpace(filterField.SqlFilter)) {
                    filterField.SqlFilter = sqlFilter;
                    isFound = true;
                    break;
                }
            }

            if (!isFound) {
                FilterField ff = new FilterField(null, null, FilterFromTo.No, sqlFilter);
                filterFields.Add(ff);
            }
        }


        protected void RemoveFilterField(ref List<FilterField> filterFields, string fieldName) {
            RemoveFilterField(ref filterFields, fieldName, FilterFromTo.No);
        }

        protected void RemoveFilterField(ref List<FilterField> filterFields, string fieldName, FilterFromTo fromTo) {

            var sfilterField = (from sfilterFieldD in filterFields
                                where sfilterFieldD.FieldName == fieldName
                                && sfilterFieldD.FromTo == fromTo
                                select sfilterFieldD).FirstOrDefault();

            if (sfilterField != null) {
                filterFields.Remove(sfilterField);
            }
        }

        protected void SetFilterField(string fieldName, string filterValue) {
            SetFilterField(ref m_FilterFields, fieldName, filterValue, FilterFromTo.No);
        }

        protected void SetFilterField(string fieldName, string filterValue, FilterFromTo fromTo) {
            SetFilterField(ref m_FilterFields, fieldName, filterValue, fromTo);
        }

        protected void RemoveFilterField(string fieldName) {
            RemoveFilterField(ref m_FilterFields, fieldName);
        }

        public void UpdateFilter(ref List<FilterField> filterFields, string filterFieldName, string filterText, FilterFromTo fromTo) {

            if (filterFields == null) {
                filterFields = new List<FilterField>();
            }

            if (!String.IsNullOrWhiteSpace(filterText)) {
                //add
                SetFilterField(ref filterFields, filterFieldName, filterText, fromTo);
                //var sfilterField = (from sfilterFieldD in filterFields
                //                    where sfilterFieldD.FieldName == filterFieldName
                //                    && sfilterFieldD.FromTo == fromTo
                //                    select sfilterFieldD).FirstOrDefault();
                //if (sfilterField != null) {
                //    sfilterField.FilterText = filterText;
                //} else {
                //    FilterField newFf = new FilterField(filterFieldName, filterText, fromTo);
                //    filterFields.Add(newFf);
                //}
            } else {
                //remove
                RemoveFilterField(ref filterFields, filterFieldName, fromTo);
                //var sfilterField = (from sfilterFieldD in filterFields
                //                    where sfilterFieldD.FieldName == filterFieldName
                //                    select sfilterFieldD).FirstOrDefault();
                //if (sfilterField != null) {
                //    filterFields.Remove(sfilterField);
                //}
            }
        }

        protected string GetFilter() {
            if (m_FilterFields == null || m_FilterFields.Count == 0) {
                return null;
            }

            string strFilter = "";
            foreach (var ff in m_FilterFields) {
                if (strFilter.Length > 0) {
                    strFilter += BaseRepository<Nomenclature>.UrlParamDelimiter;
                }
                strFilter += ff.FieldName + BaseRepository<Nomenclature>.UrlParamValueDelimiter + ff.FilterText;
                if (ff.FromTo == FilterFromTo.From) {
                    strFilter += BaseRepository<Nomenclature>.UrlParamValueDelimiter + "FROM";
                } else if (ff.FromTo == FilterFromTo.To) {
                    strFilter += BaseRepository<Nomenclature>.UrlParamValueDelimiter + "TO";
                }
            }

            return strFilter;
        }

        protected WcfFilterField[] GetFilterFields() {
            if (m_FilterFields == null || m_FilterFields.Count == 0) {
                return null;
            }

            WcfFilterField[] wcfFilterFields = new WcfFilterField[m_FilterFields.Count];
            for (int i=0; i< m_FilterFields.Count; i++) {
                WcfFilterField wcfFilterField = new WcfFilterField();
                wcfFilterField.FieldName = m_FilterFields[i].FieldName;
                wcfFilterField.FilterText = m_FilterFields[i].FilterText;
                wcfFilterField.FromTo = (int)m_FilterFields[i].FromTo;
                wcfFilterField.SqlFilter = m_FilterFields[i].SqlFilter;
                wcfFilterFields[i] = wcfFilterField;
            }

            return wcfFilterFields;
        }

        public static void LocalizeDemandGridColumnsLabels(DataGrid grid) {
            if (grid == null) {
                return;
            }

            if (grid.Columns == null) {
                return;
            }

            foreach (var grdColumn in grid.Columns) {
                string colName = DataGridUtil.GetScmColumnName(grdColumn);
                switch (colName) {
                    case DemandData.DEMAND_NR_FIELD:
                        grdColumn.Header = ScmResource.DemandNr;
                        break;
                    case "supplier_text":
                        grdColumn.Header = ScmResource.Supplier;
                        break;
                    case "nomenclatures_text":
                        grdColumn.Header = ScmResource.Nomenclatures;
                        break;
                    case "requestor_name":
                        grdColumn.Header = ScmResource.Requestor;
                        break;
                    case "price_list":
                        grdColumn.Header = ScmResource.Requestor;
                        break;
                    case "created_date_text":
                        grdColumn.Header = ScmResource.Created;
                        break;
                    case "last_status_modif_date_text":
                        grdColumn.Header = ScmResource.LastStatusModifDate;
                        break;
                    case "days_in_status":
                        grdColumn.Header = ScmResource.DaysInStatus;
                        break;
                    case "status_text":
                        grdColumn.Header = ScmResource.Status;
                        break;
                    case "app_man_name":
                        grdColumn.Header = ScmResource.ApprovalManager;
                        break;
                }
            }

            LocalizeGridHeaderControls(grid);
        }

        public static void LocalizeContactGridColumnsLabels(DataGrid grid) {
            if (grid == null) {
                return;
            }

            if (grid.Columns == null) {
                return;
            }

            foreach (var grdColumn in grid.Columns) {
                string colName = DataGridUtil.GetScmColumnName(grdColumn);
                switch (colName) {
                    case SupplierContactData.FIRST_NAME_FIELD:
                        grdColumn.Header = ScmResource.FirstName;
                        break;
                    case SupplierContactData.SURNAME_FIELD:
                        grdColumn.Header = ScmResource.Surname;
                        break;
                    case SupplierContactData.EMAIL_FIELD:
                        grdColumn.Header = ScmResource.eMail;
                        break;
                    case SupplierContactData.PHONE_NR_FIELD:
                        grdColumn.Header = ScmResource.Phone;
                        break;
                    case SupplierContactData.PHONE_NR2_FIELD:
                        grdColumn.Header = ScmResource.MobilePhone;
                        break;

                }
            }
        }

        public static void LocalizeNomenclatureGridColumnsLabels(DataGrid grid) {
            if (grid == null) {
                return;
            }

            if (grid.Columns == null) {
                return;
            }

            foreach (var grdColumn in grid.Columns) {
                string colName = DataGridUtil.GetScmColumnName(grdColumn);
                switch (colName) {
                    case "price_list":
                        grdColumn.Header = ScmResource.PriceList;
                        break;
                    case "nomenclature_key":
                        grdColumn.Header = ScmResource.Nomenclature;
                        break;
                    case "name":
                        grdColumn.Header = ScmResource.NameIt;
                        break;
                    case "material_group_text":
                        grdColumn.Header = ScmResource.MaterialGroup;
                        break;
                    case "created_date":
                    case "created_date_text":
                        grdColumn.Header = ScmResource.CreatedDate;
                        break;
                    case "specification":
                        grdColumn.Header = ScmResource.Specification;
                        break;
                    case "source_text":
                        grdColumn.Header = ScmResource.NomenclatureSource;
                        break;
                }
            }

            LocalizeGridHeaderControls(grid);
        }

        public static void LocalizeSupplierGridColumnsLabels(DataGrid grid) {
            if (grid == null) {
                return;
            }

            if (grid.Columns == null) {
                return;
            }

            foreach (var grdColumn in grid.Columns) {
                string colName = DataGridUtil.GetScmColumnName(grdColumn);
                switch (colName) {
                    case "supp_name":
                        grdColumn.Header = ScmResource.NameIt;
                        break;
                    case "supplier_id":
                        grdColumn.Header = ScmResource.SupplierId;
                        break;
                    case "street_part1":
                        grdColumn.Header = ScmResource.Street;
                        break;
                    case "city":
                        grdColumn.Header = ScmResource.City;
                        break;
                    case "zip":
                        grdColumn.Header = ScmResource.Zip;
                        break;
                    case "country":
                        grdColumn.Header = ScmResource.Country;
                        break;
                }
            }

            LocalizeGridHeaderControls(grid);
        }

        public static void LocalizeNomenclatureGridColumnsLabels(ListView listView) {
            if (listView == null) {
                return;
            }

            GridView gv = null;
            if (listView.View is GridView) {
                gv = (GridView)listView.View;
            }

            if (gv == null) {
                return;
            }

            if (gv.Columns == null) {
                return;
            }

            foreach (var grdColumn in gv.Columns) {
                string colName = DataGridUtil.GetScmColumnName(grdColumn);
                switch (colName) {
                    case "nomenclature_key":
                        ((GridViewColumnHeader)grdColumn.Header).Content = ScmResource.Nomenclature;
                        break;
                    case "name":
                        ((GridViewColumnHeader)grdColumn.Header).Content = ScmResource.NameIt;
                        break;
                    case "suggested_price":
                        ((GridViewColumnHeader)grdColumn.Header).Content = ScmResource.SuggestedPrice;
                        break;
                    case "currency":
                        ((GridViewColumnHeader)grdColumn.Header).Content = ScmResource.Currency;
                        break;
                    case "other_demands":
                        ((GridViewColumnHeader)grdColumn.Header).Content = ScmResource.OtherDemands;
                        break;
                    case "remark":
                        ((GridViewColumnHeader)grdColumn.Header).Content = ScmResource.Remark;
                        break;

                }
            }

            //LocalizeGridHeaderControls(grid);
        }

        private static void LocalizeGridHeaderControls(DataGrid grid) {
            foreach (var grdColumn in grid.Columns) {
                if (grdColumn is DataGridTextColumn) {
                    var headerTemplate = ((DataGridTextColumn)grdColumn).HeaderTemplate;
                    if (headerTemplate != null) {
                        var cont = headerTemplate.LoadContent();

                        List<UcGrdColHeaderFilterDate> ucHeaderFilters = MainWindow.FindChild<UcGrdColHeaderFilterDate>(cont);
                        if (ucHeaderFilters != null && ucHeaderFilters.Count > 0) {
                            foreach (var ucHeaderFilter in ucHeaderFilters) {
                                ucHeaderFilter.LocalizeUc();
                            }
                        }
                    }
                }
            }
        }

#if RELEASE
        //private WsScmDemandDebug.Supplier GetWsSupplier(Supplier supplier) {
        //    WsScmDemandDebug.Supplier wsSupplier = new WsScmDemandDebug.Supplier();
        //    VmBase.SetValues(supplier, wsSupplier);

        //    return wsSupplier;
        //}
#else
        //private WsScmDemandDebug.Supplier GetWsSupplier(Supplier supplier) {
        //    WsScmDemandDebug.Supplier wsSupplier = new WsScmDemandDebug.Supplier();
        //    VmBase.SetValues(supplier, wsSupplier);

        //    return wsSupplier;
        //}
#endif

#if DEBUG
        //protected WsScmDemandDebug.Nomenclature GetWsNomenclature(Nomenclature nomenclature) {
        //    WsScmDemandDebug.Nomenclature wsNom = new WsScmDemandDebug.Nomenclature();


        //    SetValues(nomenclature, wsNom);

        //    return wsNom;
        //}

#else
#endif

#if DEBUG
        //protected WsScmDemandDebug.NomenclatureDetailExtend GetWsNomenclatureDetail(NomenclatureDetailExtend nomenclature) {
        //    WsScmDemandDebug.NomenclatureDetailExtend wsNom = new WsScmDemandDebug.NomenclatureDetailExtend();


        //    SetValues(nomenclature, wsNom);

        //    return wsNom;
        //}

#else
#endif


        #endregion
    }
}
