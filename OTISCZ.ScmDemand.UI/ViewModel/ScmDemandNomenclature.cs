using OTISCZ.ScmDemand.Interface.WsScmDemand;
using OTISCZ.ScmDemand.UI.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class ScmDemandNomenclature : Demand_Nomenclature, INotifyPropertyChanged, INotifyDataErrorInfo {
        #region NotifyProperty
        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName) {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null) {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Properties
        private bool m_IsSentForApproval = false;
        public bool IsSentForApproval {
            get { return m_IsSentForApproval; }
            set { m_IsSentForApproval = value; }
        }

        public string nomenclature_key { get; set; }
        public string name { get; set; }
        public string specification { get; set; }
        public int status_id { get; set; }
        public string status_text { get; set; }

        private string m_price_text;
        public string price_text {
            get {
                return m_price_text;
            }
            set {
                try {
                    try {
                        if (m_IsSentForApproval) {
                            ValidatePrice(value, false);
                        }
                    } catch (Exception ex) {
                        //VmBase.HandleError();
                    }
                    m_price_text = value;

                    OnPropertyChanged("price_text");
                } catch (Exception ex) {
                    ValidatePrice(m_price_text, false);

                }
            }
        }

        public int? CurrencyCodeId {
            get {
                return currency_id;
            }
            set {
                try {
                    try {
                        if (m_IsSentForApproval) {
                            ValidateCurrency(value, false);
                        }
                    } catch (Exception ex) {
                        //VmBase.HandleError();
                    }
                    currency_id = value;

                    OnPropertyChanged("CurrencyCodeId");
                } catch (Exception ex) {
                    ValidateCurrency(currency_id, false);

                }
            }
        }

        public string price_text_orig { get; set; }
        public int? currency_id_orig { get; set; }
        public string img_status_path { get; set; }
        public bool is_locked { get; set; }
        public string currency_text { get; set; }
        public string remark { get; set; }
        public string copy_text { get; set; }
        public string delete_text {
            get { return ScmResource.Delete; }
        }
        public System.Windows.Visibility lock_visibility { get; set; }
        public System.Windows.Visibility edit_visibility { get; set; }
        public System.Windows.Visibility read_only_visibility { get; set; }
        public System.Windows.Visibility select_visibility { get; set; }
        public System.Windows.Visibility remove_visibility { get; set; }
        public bool is_selected { get; set; }
        
        public List<DemandLight> other_demands { get; set; }
        #endregion

        #region Validations
        public bool HasErrors { get; set; } = false;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void ValidatePrice(string value, bool isSkipException) {
            if (IsPriceValid(value)) {
                m_price_text = value;
            } else { 
                HasErrors = true;
                if (!isSkipException) {
                    throw new Exception(ScmResource.EnterMandatoryValues);
                }
            }

            HasErrors = IsHasErros();
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("price_text"));
        }

        private void ValidateCurrency(int? value, bool isSkipException) {
            if (IsCurrencyValid(value)) {
                currency_id = value;
            } else {
                HasErrors = true;
                if (!isSkipException) {
                    throw new Exception(ScmResource.EnterMandatoryValues);
                }
            }

            HasErrors = IsHasErros();
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("CurrencyCodeId"));
        }

        private bool IsHasErros() {
            bool isValid = IsPriceValid(m_price_text) 
                && IsCurrencyValid(currency_id);

            return !isValid;
        }

        private bool IsPriceValid(string price) {
            if (String.IsNullOrWhiteSpace(price)) {
                return false;
            }

            decimal d;
            bool isDecimal = Decimal.TryParse(price, out d);
            if (!isDecimal) {
                return false;
            }

            return true;
        }

        private bool IsCurrencyValid(int? currencyId) {
            if (currencyId == null || currencyId < 0) {
                return false;
            }

           
            return true;
        }

        public IEnumerable GetErrors(string propertyName) {
            List<string> errMsg = new List<string>();

            if (propertyName == "price_text") {
                if (!IsPriceValid(price_text)) {
                    HasErrors = true;
                    errMsg.Add(ScmResource.EnterPrice);
                }
            } else if (propertyName == "CurrencyCodeId") {
                if (!IsCurrencyValid(CurrencyCodeId)) {
                    HasErrors = true;
                    errMsg.Add(ScmResource.SelectCurrency);
                }
            }

            return errMsg;
        }

        public bool IsValid() {
            ValidatePrice(m_price_text, true);
            ValidateCurrency(currency_id, true);

            bool isValid = IsPriceValid(m_price_text) && IsCurrencyValid(currency_id);

            //GetErrors("price_text");
            //GetErrors("CurrencyCodeId");

            return isValid;
        }
        #endregion
    }
}
