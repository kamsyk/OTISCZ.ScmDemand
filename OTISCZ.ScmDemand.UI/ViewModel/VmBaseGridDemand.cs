using Kamsyk.ExcelOpenXml;
using OTISCZ.CommonDb;
using OTISCZ.ConcordeDataDictionary;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Common;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.Security;
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
    public class VmBaseGridDemand : VmBaseGrid, INotifyPropertyChanged {
        //#region Constants
        //private const string CXAL_USER_NAME = "ConcordeWebProxyClient";
        //private const string CXAL_PASSWORD = "d4ghj6,p}87'";
        //private const string CXAL_ENCRYPT_PASSWORD = "rg.;r5er8ee.-";
        //#endregion

        #region enums
        public enum DataKeyEnum {
            Nomenclature,
            Supplier,
            Unknown
        }
        #endregion

        #region Properties
        private DataKeyEnum m_DataKey = DataKeyEnum.Unknown;
        public DataKeyEnum DataKey {
            get { return m_DataKey; }
            set { m_DataKey = value; }
        }

        //private ObservableCollection<NomenclatureExtend> m_NomenclatureList = null;
        //public ObservableCollection<NomenclatureExtend> NomenclatureList {
        //    get {
        //        if (m_NomenclatureList == null) {
        //            try {
        //                LoadNomenclatures();
        //            } catch (Exception ex) {
        //                HandleError(ex);
        //            }
        //        }
        //        return m_NomenclatureList;
        //    }
        //    set {
        //        m_NomenclatureList = value;
        //        OnPropertyChanged("NomenclatureList");
        //        SetDataGridColumnSort();
        //    }
        //}


        //private ObservableCollection<SupplierExtend> m_SupplierList = null;
        //public virtual ObservableCollection<SupplierExtend> SupplierList {
        //    get {
        //        if (m_SupplierList == null) {
        //            try {
        //                LoadSuppliers();

        //            } catch (Exception ex) {
        //                HandleError(ex);
        //            }
        //        }
        //        return m_SupplierList;
        //    }
        //    set {
        //        m_SupplierList = value;
        //        OnPropertyChanged("SupplierList");
        //        SetDataGridColumnSort();
        //    }
        //}


        private ScmSetting m_scmSetting = null;
        private ScmSetting ScmSetting {
            get {
                if (m_scmSetting == null) {
                    try {
                        var setting = WcfScm.GetScmSetting();
                        m_scmSetting = new ScmSetting();
                        SetValues(setting, m_scmSetting);
                    } catch (Exception ex) {
                        HandleError(ex);

                    }
                }

                return m_scmSetting;
            }
        }

        //public string ProdisFolder {
        //    get {
        //        return ScmSetting.prodis_input_folder;
        //    }
        //}

        
        #endregion

        #region Constructor
        public VmBaseGridDemand(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {

        }
        #endregion

        #region Abstract Methods
        public override void ExportToExcel() {
            switch (m_DataKey) {
                case DataKeyEnum.Nomenclature:
                    ExportToExcelNomenclatures();
                    break;
                default:
                    throw new Exception("Unknown DataKey");
            }
        }

        public override void Import() {
            switch (m_DataKey) {
                case DataKeyEnum.Nomenclature:
                    ImportNomenclaturesAsync();
                    break;
                case DataKeyEnum.Supplier:
                    ImportSuppliersAsync();
                    break;
                default:
                    throw new Exception("Unknown DataKey");
            }
        }

        public override void RefreshGridData() {
            switch (m_DataKey) {
                case DataKeyEnum.Nomenclature:
                    LoadNomenclatures();
                    break;
                case DataKeyEnum.Supplier:
                    LoadSuppliers();
                    break;
                default:
                    throw new Exception("Unknown DataKey");
            }

            GridInit();
        }

        public override void AddNew() {
            
        }
        #endregion

        #region Async Methods
        //private async void LoadNomenclatures() {
        //    try {
        //        var nomenclatures = GetNomenclaturesAsync();
        //        NomenclatureList = await nomenclatures;
        //        DisplayingRows = ScmResource.DisplayingFromToOf
        //            .Replace("{0}", GetDisplayItemsFromInfo().ToString())
        //            .Replace("{1}", GetDisplayItemsToInfo().ToString())
        //            .Replace("{2}", RowsCount.ToString());
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    }
        //}

        //protected async void LoadSuppliers() {
        //    try {
        //        var suppliers = GetSuppliersAsync();
        //        SupplierList = await suppliers;
        //        DisplayingRows = ScmResource.DisplayingFromToOf
        //            .Replace("{0}", GetDisplayItemsFromInfo().ToString())
        //            .Replace("{1}", GetDisplayItemsToInfo().ToString())
        //            .Replace("{2}", RowsCount.ToString());

        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    }
        //}

        //protected virtual Task<ObservableCollection<NomenclatureExtend>> GetNomenclaturesAsync() {
        //    return Task.Run(() => {
        //        IsBusy = true;
        //        try {
        //            int rowsCount;
        //            bool rowsCountSpecified;
        //            string strFilter = GetFilter();
        //            string strOrder = GetOrder();

        //            var wsNomenclatures = WsScm.GetNomenclatures(
        //                strFilter,
        //                strOrder,
        //                PageSize,
        //                true,
        //                PageNr,
        //                true,
        //                out rowsCount,
        //                out rowsCountSpecified);

        //            RowsCount = rowsCount;

        //            ObservableCollection<NomenclatureExtend> nomenclatures = new ObservableCollection<NomenclatureExtend>();
        //            foreach (var wsNomenclature in wsNomenclatures) {
        //                NomenclatureExtend nomenclature = new NomenclatureExtend();
        //                SetValues(wsNomenclature, nomenclature);
        //                nomenclature.status_text = GetNomenclatureStatus(nomenclature.status_id);

        //                nomenclatures.Add(nomenclature);
        //            }

        //            return nomenclatures;
        //        } catch (Exception ex) {
        //            throw ex;
        //        } finally {
        //            IsBusy = false;
        //        }
        //    });
        //}



        //private Task<ObservableCollection<SupplierExtend>> GetSuppliersAsync() {
        //    return Task.Run(() => {
        //        IsBusy = true;
        //        try {
        //            int rowsCount;
        //            bool rowsCountSpecified;
        //            string strFilter = GetFilter();
        //            string strOrder = GetOrder();

        //            var wsSuppliers = WsScm.GetSuppliers(
        //                strFilter,
        //                strOrder,
        //                PageSize,
        //                true,
        //                PageNr,
        //                true,
        //                out rowsCount,
        //                out rowsCountSpecified);

        //            RowsCount = rowsCount;

        //            ObservableCollection<SupplierExtend> suppliers = new ObservableCollection<SupplierExtend>();
        //            foreach (var wsSupplier in wsSuppliers) {
        //                SupplierExtend supplier = new SupplierExtend();
        //                SetValues(wsSupplier, supplier);

        //                suppliers.Add(supplier);
        //            }

        //            return suppliers;
        //        } catch (Exception ex) {
        //            throw ex;
        //        } finally {
        //            IsBusy = false;
        //        }
        //    });
        //}

        //private Task ImportNomenclaturesAsync() {
        //    return Task.Run(() => {
        //        try {

        //            ScmFileImport.DlgLoadFileInfo dlgLoadFileInfo = new ScmFileImport.DlgLoadFileInfo(LoadFileInfo);
        //            ScmFileImport.DlgLoadProgressInfo dlgLoadProgressInfo = new ScmFileImport.DlgLoadProgressInfo(LoadProgressInfo);
        //            new ScmFileImport().ImportData(
        //                ProdisFolder,
        //                CurrentUser.User.id,
        //                dlgLoadFileInfo,
        //                dlgLoadProgressInfo);

        //            ProgressBarVisibility = Visibility.Collapsed;
        //            ScmDispatcher.Invoke(() => {
        //                MessageBox.Show(ScmResource.ErrorMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        //            });
        //            ProgressBarVisibility = Visibility.Collapsed;

        //            RefreshGridData();
        //        } catch (Exception ex) {

        //            ScmDispatcher.Invoke(() => {
        //                HandleError(ex);
        //            });

        //        } finally {
        //            ProgressBarVisibility = Visibility.Collapsed;
        //        }
        //    });
        //}

        //public async void ExportToExcelNomenclatures() {
        //    try {

        //        var nomenclatures = GetNomenclaturesReportAsync();
        //        var nomenclaturesReport = await nomenclatures;

        //        string colNameNomenclature = ScmResource.Nomenclature;
        //        string colNameName = ScmResource.NameIt;
        //        string colNameMaterialGroup = ScmResource.MaterialGroup;


        //        DataTable nomenclaturesTable = new DataTable();
        //        DataColumn col = new DataColumn(colNameNomenclature, typeof(string));
        //        nomenclaturesTable.Columns.Add(col);
        //        col = new DataColumn(colNameName, typeof(string));
        //        nomenclaturesTable.Columns.Add(col);
        //        col = new DataColumn(colNameMaterialGroup, typeof(string));
        //        nomenclaturesTable.Columns.Add(col);

        //        foreach (var nomenclature in nomenclaturesReport) {
        //            DataRow newRow = nomenclaturesTable.NewRow();
        //            newRow[colNameNomenclature] = nomenclature.nomenclature_key;
        //            newRow[colNameName] = nomenclature.name;
        //            newRow[colNameMaterialGroup] = nomenclature.material_group_text;

        //            nomenclaturesTable.Rows.Add(newRow);
        //        }

        //        Excel excel = new Excel();
        //        string fileName = GetXlsFileName("Nomenclatures");
        //        using (var excelDoc = excel.GenerateExcelWorkbookDoc(nomenclaturesTable, new List<double> { 25, 70, 20, 50, 40, 40 })) {

        //            var pack = excelDoc.SaveAs(fileName);
        //            excelDoc.Close();
        //            pack.Close();

        //        }

        //        excel = null;
        //        Process.Start(fileName);
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    } finally {
        //        //IsBusy = false;
        //    }
        //}

        //private Task<List<NomenclatureExtend>> GetNomenclaturesReportAsync() {
        //    return Task.Run(() => {
        //        IsBusy = true;
        //        try {
        //            string strFilter = GetFilter();
        //            string strOrder = GetOrder();

        //            var wsNomenclatures = WsScm.GetNomenclaturesReport(
        //                strFilter,
        //                strOrder);

        //            List<NomenclatureExtend> nomenclatures = new List<NomenclatureExtend>();
        //            foreach (var wsNomenclature in wsNomenclatures) {
        //                NomenclatureExtend nomenclature = new NomenclatureExtend();
        //                SetValues(wsNomenclature, nomenclature);

        //                nomenclatures.Add(nomenclature);
        //            }

        //            return nomenclatures;
        //        } catch (Exception ex) {
        //            IsBusy = false;
        //            throw ex;
        //        } finally {
        //            IsBusy = false;
        //        }
        //    });
        //}

        //protected async void ImportSuppliers() {
        //    try {
        //        var task = ImportSuppliersAsync();
        //        await task;

        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    }
        //}

        //private Task ImportSuppliersAsync() {
        //    return Task.Run(() => {
        //        IsBusy = true;
        //        try {
        //            WsConcordeSupplier.InternalRequest wsSupplier = new WsConcordeSupplier.InternalRequest();
        //            WsConcordeSupplier.AuthHeader authHeader = new WsConcordeSupplier.AuthHeader();

        //            authHeader.Username = CXAL_USER_NAME;
        //            authHeader.Password = Des.Encrypt(CXAL_PASSWORD, CXAL_ENCRYPT_PASSWORD);
        //            wsSupplier.AuthHeaderValue = authHeader;
        //            wsSupplier.Timeout = 1800000;

        //            var wsSuppliers = wsSupplier.GetActiveCreditors(WsConcordeSupplier.CXALVersion.ESC, true);

        //            Hashtable htActiveSuppliers = new Hashtable();

        //            foreach (DataRow concordeSuppRow in wsSuppliers.Tables[0].Rows) {
        //                //id
        //                string accNr = null;
        //                if (concordeSuppRow[CredTableTable.ACCOUNTNUMBER] != null) {
        //                    accNr = GetDbText(concordeSuppRow[CredTableTable.ACCOUNTNUMBER].ToString());
        //                }
        //                if (accNr == null) {
        //                    continue;
        //                }
        //                accNr = accNr.Trim();

        //                //name
        //                string name = null;
        //                if (concordeSuppRow[CredTableTable.NAME] != null) {
        //                    name = ConvertCzSKString(GetDbText(concordeSuppRow[CredTableTable.NAME].ToString()));
        //                }
        //                if (name == null || name.Trim().Length == 0) continue;

        //                var wsSupplierData = WsScm.GetSupplierBySupplierId(accNr);
        //                Supplier supplier = null;
        //                if (wsSupplierData != null) {
        //                    supplier = new Supplier();
        //                    SetValues(wsSupplierData, supplier);
        //                }
        //                if (supplier == null) {
        //                    supplier = new Supplier();
        //                    supplier.id = DataNulls.INT_NULL;
        //                } else {
        //                    if (!htActiveSuppliers.ContainsKey(supplier.id)) {
        //                        htActiveSuppliers.Add(supplier.id, null);
        //                    }
        //                }

        //                supplier.supplier_id = RemoveNotAllowedExcelChars(accNr);
        //                supplier.active = true;
        //                supplier.supp_name = RemoveNotAllowedExcelChars(name);

        //                //dic
        //                string dic = null;
        //                if (concordeSuppRow[CredTableTable.VATNUMBER] != null) {
        //                    dic = GetDbText(concordeSuppRow[CredTableTable.VATNUMBER].ToString());
        //                }
        //                supplier.dic = RemoveNotAllowedExcelChars(dic);

        //                //country
        //                string country = null;
        //                if (concordeSuppRow[CredTableTable.COUNTRY] != null) {
        //                    country = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.COUNTRY].ToString()));
        //                }
        //                supplier.country = RemoveNotAllowedExcelChars(country);

        //                //contact person
        //                string contactPerson = null;
        //                if (concordeSuppRow[CredTableTable.ATTENTION] != null) {
        //                    contactPerson = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ATTENTION].ToString()));
        //                }
        //                supplier.contact_person = RemoveNotAllowedExcelChars(contactPerson);

        //                //phone
        //                string phone = null;
        //                if (concordeSuppRow[CredTableTable.PHONE] != null) {
        //                    phone = GetDbText(concordeSuppRow[CredTableTable.PHONE].ToString());
        //                }
        //                supplier.phone = RemoveNotAllowedExcelChars(phone);


        //                //only for CZ SK
        //                //mobile phone
        //                string mobilePhone = null;
        //                if (concordeSuppRow[CredTableTable.OTISMOBIL] != null) {
        //                    mobilePhone = GetDbText(concordeSuppRow[CredTableTable.OTISMOBIL].ToString());
        //                }
        //                supplier.mobile_phone = RemoveNotAllowedExcelChars(mobilePhone);


        //                //fax
        //                string fax = null;
        //                if (concordeSuppRow[CredTableTable.FAX] != null) {
        //                    fax = GetDbText(concordeSuppRow[CredTableTable.FAX].ToString());
        //                }
        //                supplier.fax = RemoveNotAllowedExcelChars(fax);

        //                //mail
        //                string mail = null;
        //                if (concordeSuppRow[CredTableTable.OTISEMAIL] != null) {
        //                    mail = GetDbText(concordeSuppRow[CredTableTable.OTISEMAIL].ToString());
        //                }
        //                supplier.email = RemoveNotAllowedExcelChars(mail);


        //                //street_part1
        //                string streetPart1 = null;
        //                if (concordeSuppRow[CredTableTable.ADDRESS1] != null) {
        //                    streetPart1 = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ADDRESS1].ToString()));
        //                }
        //                supplier.street_part1 = RemoveNotAllowedExcelChars(streetPart1);

        //                //street_part2
        //                string streetPart2 = null;
        //                if (concordeSuppRow[CredTableTable.ADDRESS2] != null) {
        //                    streetPart2 = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ADDRESS2].ToString()));
        //                }
        //                supplier.street_part2 = RemoveNotAllowedExcelChars(streetPart2);

        //                //city
        //                string city = null;
        //                if (concordeSuppRow[CredTableTable.ADDRESS3] != null) {
        //                    city = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ADDRESS3].ToString()));
        //                }
        //                supplier.city = RemoveNotAllowedExcelChars(city);

        //                //zip code
        //                string zip = null;
        //                if (concordeSuppRow[CredTableTable.ZIP] != null) {
        //                    zip = GetDbText(concordeSuppRow[CredTableTable.ZIP].ToString());
        //                }
        //                supplier.zip = RemoveNotAllowedExcelChars(zip);

        //                //creditor group
        //                int creditorGroup = DataNulls.INT_NULL;
        //                if (concordeSuppRow[CredTableTable.CREDITORGROUP] != null) {
        //                    creditorGroup = ConvertData.ToInt32(concordeSuppRow[CredTableTable.CREDITORGROUP].ToString().Trim());
        //                }
        //                supplier.creditor_group = creditorGroup;

        //                //bank account
        //                string bankAccount = null;
        //                if (concordeSuppRow[CredTableTable.BANKACCOUNT] != null) {
        //                    bankAccount = GetDbText(concordeSuppRow[CredTableTable.BANKACCOUNT].ToString().Trim());
        //                }
        //                supplier.bank_account = RemoveNotAllowedExcelChars(bankAccount);

        //                var wsUpdSupplier = GetWsSupplier(supplier);
        //                WsScm.UpdateSupplier(wsUpdSupplier, CurrentUser.User.id, true);
        //            }

        //        } catch (Exception ex) {
        //            IsBusy = false;
        //            throw ex;
        //        } finally {
        //            IsBusy = false;
        //        }
        //    });

        //}
        #endregion

        #region Methods
        //private string ConvertCzSKString(string czSkString) {
        //    string unicodeString = czSkString;

        //    unicodeString = unicodeString.Replace("Ŕ", "Ü");

        //    return unicodeString;
        //}

        //private string GetDbText(string rawText) {
        //    if (rawText == null) {
        //        return null;
        //    }

        //    return rawText.Trim();
        //}

        //private string RemoveNotAllowedExcelChars(string strRawString) {
        //    if (strRawString == null) {
        //        return null;
        //    }

        //    string fixedValue = strRawString.Replace('\u0006'.ToString(), "");

        //    return fixedValue;
        //}

        //private string TransformConcordeString(string rawString) {

        //    return ConvertCzSKString(rawString).Trim();

        //}

        //#if RELEASE
        //#else
        //        private WsScmDemandDebug.Supplier GetWsSupplier(Supplier supplier) {
        //            WsScmDemandDebug.Supplier wsSupplier = new WsScmDemandDebug.Supplier();
        //            VmBase.SetValues(supplier, wsSupplier);

        //            return wsSupplier;
        //        }
        //#endif
        #endregion
    }
}
