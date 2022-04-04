using Kamsyk.ExcelOpenXml;
using OTISCZ.CommonDb;
using OTISCZ.ConcordeDataDictionary;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class VmSupplierList : VmBaseGridDemand, INotifyPropertyChanged, INotifyDataErrorInfo {
        //#region Constants
        //private const string CXAL_USER_NAME = "ConcordeWebProxyClient";
        //private const string CXAL_PASSWORD = "d4ghj6,p}87'";
        //private const string CXAL_ENCRYPT_PASSWORD = "rg.;r5er8ee.-";
        //#endregion

        #region Properties

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        //private ObservableCollection<SupplierExtend> m_SupplierList = null;
        //public ObservableCollection<SupplierExtend> SupplierList {
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


        //private List<NomenclatureExtend> m_NomenclaturesReport = null;
        #endregion

        #region Constructor
        public VmSupplierList(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {

        }
        #endregion

        #region Interface Methods
        public IEnumerable GetErrors(string propertyName) {
            throw new NotImplementedException();
        }
        #endregion

        #region Abstract Methods Imlementation
        public override void RefreshGridData() {
            LoadSuppliers();
            GridInit();
        }

        public override void Import() {
            ImportSuppliers();
        }
        #endregion

        #region Async Methods
        //private async void LoadSuppliers() {
        //    try {
        //        var suppliers = GetSuppliersAsync();
        //        SupplierList = await suppliers;
                
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    }
        //}

        //private async void ImportSuppliers() {
        //    try {
        //        var task = ImportSuppliersAsync();
        //        await task;

        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    }
        //}

        public async override void ExportToExcel() {
            try {

            } catch (Exception ex) {
                HandleError(ex);
            }
        }

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
        //                    accNr = concordeSuppRow[CredTableTable.ACCOUNTNUMBER].ToString();
        //                }
        //                if (accNr == null) {
        //                    continue;
        //                }
        //                accNr = accNr.Trim();

        //                //name
        //                string name = null;
        //                if (concordeSuppRow[CredTableTable.NAME] != null) {
        //                    name = ConvertCzSKString(concordeSuppRow[CredTableTable.NAME].ToString());
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
        //                    dic = concordeSuppRow[CredTableTable.VATNUMBER].ToString();
        //                }
        //                supplier.dic = RemoveNotAllowedExcelChars(dic);

        //                //country
        //                string country = null;
        //                if (concordeSuppRow[CredTableTable.COUNTRY] != null) {
        //                    country = TransformConcordeString(concordeSuppRow[CredTableTable.COUNTRY].ToString());
        //                }
        //                supplier.country = RemoveNotAllowedExcelChars(country);

        //                //contact person
        //                string contactPerson = null;
        //                if (concordeSuppRow[CredTableTable.ATTENTION] != null) {
        //                    contactPerson = TransformConcordeString(concordeSuppRow[CredTableTable.ATTENTION].ToString());
        //                }
        //                supplier.contact_person = RemoveNotAllowedExcelChars(contactPerson);

        //                //phone
        //                string phone = null;
        //                if (concordeSuppRow[CredTableTable.PHONE] != null) {
        //                    phone = concordeSuppRow[CredTableTable.PHONE].ToString();
        //                }
        //                supplier.phone = RemoveNotAllowedExcelChars(phone);


        //                //only for CZ SK
        //                //mobile phone
        //                string mobilePhone = null;
        //                if (concordeSuppRow[CredTableTable.OTISMOBIL] != null) {
        //                    mobilePhone = concordeSuppRow[CredTableTable.OTISMOBIL].ToString();
        //                }
        //                supplier.mobile_phone = RemoveNotAllowedExcelChars(mobilePhone);


        //                //fax
        //                string fax = null;
        //                if (concordeSuppRow[CredTableTable.FAX] != null) {
        //                    fax = concordeSuppRow[CredTableTable.FAX].ToString();
        //                }
        //                supplier.fax = RemoveNotAllowedExcelChars(fax);

        //                //mail
        //                string mail = null;
        //                if (concordeSuppRow[CredTableTable.OTISEMAIL] != null) {
        //                    mail = concordeSuppRow[CredTableTable.OTISEMAIL].ToString();
        //                }
        //                supplier.email = RemoveNotAllowedExcelChars(mail);


        //                //street_part1
        //                string streetPart1 = null;
        //                if (concordeSuppRow[CredTableTable.ADDRESS1] != null) {
        //                    streetPart1 = TransformConcordeString(concordeSuppRow[CredTableTable.ADDRESS1].ToString());
        //                }
        //                supplier.street_part1 = RemoveNotAllowedExcelChars(streetPart1);

        //                //street_part2
        //                string streetPart2 = null;
        //                if (concordeSuppRow[CredTableTable.ADDRESS2] != null) {
        //                    streetPart2 = TransformConcordeString(concordeSuppRow[CredTableTable.ADDRESS2].ToString());
        //                }
        //                supplier.street_part2 = RemoveNotAllowedExcelChars(streetPart2);

        //                //city
        //                string city = null;
        //                if (concordeSuppRow[CredTableTable.ADDRESS3] != null) {
        //                    city = TransformConcordeString(concordeSuppRow[CredTableTable.ADDRESS3].ToString());
        //                }
        //                supplier.city = RemoveNotAllowedExcelChars(city);

        //                //zip code
        //                string zip = null;
        //                if (concordeSuppRow[CredTableTable.ZIP] != null) {
        //                    zip = concordeSuppRow[CredTableTable.ZIP].ToString();
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
        //                    bankAccount = concordeSuppRow[CredTableTable.BANKACCOUNT].ToString().Trim();
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

        //private Task<ObservableCollection<SupplierExtend>> GetSuppliersAsync() {
        //    return Task.Run(() => {
        //        IsBusy = true;
        //        try {
        //            int rowsCount;
        //            bool rowsCountSpecified;
        //            string strFilter = GetFilter();
        //            string strOrder = GetOrder();

        //            var wsSuppliers= WsScm.GetSuppliers(
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
        #endregion

        #region Methods
        //private string ConvertCzSKString(string czSkString) {
        //    string unicodeString = czSkString;

        //    unicodeString = unicodeString.Replace("Ŕ", "Ü");

        //    return unicodeString;
        //}

        //private string RemoveNotAllowedExcelChars(string strRawString) {
        //    if (strRawString == null) {
        //        return null;
        //    }

        //    string fixedValue = strRawString.Replace('\u0006'.ToString(), "");

        //    return fixedValue;
        //}

        //private string TransformConcordeString(string rawString) {

        //        return ConvertCzSKString(rawString).Trim();
            
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
