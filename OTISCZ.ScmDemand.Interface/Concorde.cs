using OTISCZ.CommonDb;
using OTISCZ.ConcordeDataDictionary;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Interface {
    public class Concorde : BaseInterface {
        #region Constants
        private const string CXAL_USER_NAME = "ConcordeWebProxyClient";
        private const string CXAL_PASSWORD = "d4ghj6,p}87'";
        private const string CXAL_ENCRYPT_PASSWORD = "rg.;r5er8ee.-";
        #endregion

        #region Properties
#if RELEASE
        private WsScmDemand.ScmDemand m_WsScm = null;

        protected WsScmDemand.ScmDemand WsScm {
            get {
                if (m_WsScm == null) {
                    m_WsScm = new WsScmDemand.ScmDemand();
                    m_WsScm.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }

                return m_WsScm;
            }
        }
#else
        private WsScmDemandDebug.ScmDemand m_WsScm = null;

        protected WsScmDemandDebug.ScmDemand WsScm {
            get {
                if (m_WsScm == null) {
                    m_WsScm = new WsScmDemandDebug.ScmDemand();

                }

                return m_WsScm;
            }
        }
#endif
        #endregion

        public void ImportSuppliers() {
            WsConcordeSupplier.InternalRequest wsSupplier = new WsConcordeSupplier.InternalRequest();
            WsConcordeSupplier.AuthHeader authHeader = new WsConcordeSupplier.AuthHeader();

            authHeader.Username = CXAL_USER_NAME;
            authHeader.Password = Des.Encrypt(CXAL_PASSWORD, CXAL_ENCRYPT_PASSWORD);
            wsSupplier.AuthHeaderValue = authHeader;
            wsSupplier.Timeout = 1800000;

            var wsSuppliers = wsSupplier.GetActiveCreditors(WsConcordeSupplier.CXALVersion.ESC, true);

            List<int> htActiveSuppliers = new List<int>();

            foreach (DataRow concordeSuppRow in wsSuppliers.Tables[0].Rows) {
                try {
                    //id
                    string accNr = null;
                    if (concordeSuppRow[CredTableTable.ACCOUNTNUMBER] != null) {
                        accNr = GetDbText(concordeSuppRow[CredTableTable.ACCOUNTNUMBER].ToString());
                    }
                    if (accNr == null) {
                        continue;
                    }
                    accNr = accNr.Trim();

                    //name
                    string name = null;
                    if (concordeSuppRow[CredTableTable.NAME] != null) {
                        name = ConvertCzSKString(GetDbText(concordeSuppRow[CredTableTable.NAME].ToString()));
                    }
                    if (name == null || name.Trim().Length == 0) continue;

#if DEBUG
                    //if (accNr != "99999111") {
                    //    continue;
                    //}
#endif

                    var wsSupplierData = WsScm.GetSupplierBySupplierId(accNr);
#if DEBUG
                    WsScmDemandDebug.SupplierExtend supplier = null;
#else
                    WsScmDemand.SupplierExtend supplier = null;
#endif
                    if (wsSupplierData != null) {
#if DEBUG
                        supplier = new WsScmDemandDebug.SupplierExtend();
#else
                        supplier = new WsScmDemand.SupplierExtend();
#endif
                        SetValues(wsSupplierData, supplier);
                    }
                    if (supplier == null) {
#if DEBUG
                        supplier = new WsScmDemandDebug.SupplierExtend();
#else
                        supplier = new WsScmDemand.SupplierExtend();
#endif
                        supplier.id = DataNulls.INT_NULL;
                    } else {
                        if (!htActiveSuppliers.Contains(supplier.id)) {
                            htActiveSuppliers.Add(supplier.id);
                        }
                    }

                    supplier.supplier_id = RemoveNotAllowedExcelChars(accNr);
                    supplier.active = true;
                    supplier.supp_name = RemoveNotAllowedExcelChars(name);

                    //dic
                    string dic = null;
                    if (concordeSuppRow[CredTableTable.VATNUMBER] != null) {
                        dic = GetDbText(concordeSuppRow[CredTableTable.VATNUMBER].ToString());
                    }
                    supplier.dic = RemoveNotAllowedExcelChars(dic);

                    //country
                    string country = null;
                    if (concordeSuppRow[CredTableTable.COUNTRY] != null) {
                        country = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.COUNTRY].ToString()));
                    }
                    supplier.country = RemoveNotAllowedExcelChars(country);

                    //contact person
                    string contactPerson = null;
                    if (concordeSuppRow[CredTableTable.ATTENTION] != null) {
                        contactPerson = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ATTENTION].ToString()));
                    }
                    supplier.contact_person = RemoveNotAllowedExcelChars(contactPerson);

                    //phone
                    string phone = null;
                    if (concordeSuppRow[CredTableTable.PHONE] != null) {
                        phone = GetDbText(concordeSuppRow[CredTableTable.PHONE].ToString());
                    }
                    supplier.phone = RemoveNotAllowedExcelChars(phone);


                    //only for CZ SK
                    //mobile phone
                    string mobilePhone = null;
                    if (concordeSuppRow[CredTableTable.OTISMOBIL] != null) {
                        mobilePhone = GetDbText(concordeSuppRow[CredTableTable.OTISMOBIL].ToString());
                    }
                    supplier.mobile_phone = RemoveNotAllowedExcelChars(mobilePhone);


                    //fax
                    string fax = null;
                    if (concordeSuppRow[CredTableTable.FAX] != null) {
                        fax = GetDbText(concordeSuppRow[CredTableTable.FAX].ToString());
                    }
                    supplier.fax = RemoveNotAllowedExcelChars(fax);

                    //mail
                    string mail = null;
                    if (concordeSuppRow[CredTableTable.OTISEMAIL] != null) {
                        mail = GetDbText(concordeSuppRow[CredTableTable.OTISEMAIL].ToString());
                    }
                    supplier.email = RemoveNotAllowedExcelChars(mail);


                    //street_part1
                    string streetPart1 = null;
                    if (concordeSuppRow[CredTableTable.ADDRESS1] != null) {
                        streetPart1 = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ADDRESS1].ToString()));
                    }
                    supplier.street_part1 = RemoveNotAllowedExcelChars(streetPart1);

                    //street_part2
                    string streetPart2 = null;
                    if (concordeSuppRow[CredTableTable.ADDRESS2] != null) {
                        streetPart2 = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ADDRESS2].ToString()));
                    }
                    supplier.street_part2 = RemoveNotAllowedExcelChars(streetPart2);

                    //city
                    string city = null;
                    if (concordeSuppRow[CredTableTable.ADDRESS3] != null) {
                        city = TransformConcordeString(GetDbText(concordeSuppRow[CredTableTable.ADDRESS3].ToString()));
                    }
                    supplier.city = RemoveNotAllowedExcelChars(city);

                    //zip code
                    string zip = null;
                    if (concordeSuppRow[CredTableTable.ZIP] != null) {
                        zip = GetDbText(concordeSuppRow[CredTableTable.ZIP].ToString());
                    }
                    supplier.zip = RemoveNotAllowedExcelChars(zip);

                    //creditor group
                    int creditorGroup = DataNulls.INT_NULL;
                    if (concordeSuppRow[CredTableTable.CREDITORGROUP] != null) {
                        creditorGroup = ConvertData.ToInt32(concordeSuppRow[CredTableTable.CREDITORGROUP].ToString().Trim());
                    }
                    supplier.creditor_group = creditorGroup;

                    //bank account
                    string bankAccount = null;
                    if (concordeSuppRow[CredTableTable.BANKACCOUNT] != null) {
                        bankAccount = GetDbText(concordeSuppRow[CredTableTable.BANKACCOUNT].ToString().Trim());
                    }
                    supplier.bank_account = RemoveNotAllowedExcelChars(bankAccount);

                    int iResult = -1;
                    bool isResult = false;
                    WsScm.SaveSupplier(supplier, UserRepository.SYSTEM_USER_ID, true, false, true, true, true, out iResult, out isResult);
                } catch (Exception ex) {
                    HandleError(ex);
                }

            }

            if (htActiveSuppliers.Count > 0) {
                int[] iActiveSupps = new int[htActiveSuppliers.Count];
                for (int i = 0; i < htActiveSuppliers.Count; i++) {
                    iActiveSupps[i] = htActiveSuppliers.ElementAt(i);
                }
                WsScm.DeactiveSuppliers(iActiveSupps, UserRepository.SYSTEM_USER_ID, true);
            }
        }

        private void HandleError(Exception ex) {
            throw ex;
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
    }
}
