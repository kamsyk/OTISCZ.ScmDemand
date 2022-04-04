
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.UI.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class ScmMail : INotifyPropertyChanged, INotifyDataErrorInfo {
                
        public SupplierExtend supplier { get; set; }
        public string from { get; set; }

        private string m_Recipients = null;
        public string Recipients {
            get {
                return m_Recipients;
            }
            set {
                try {
                    try {
                        ValidateRecipientsInputData(value);
                    } catch (Exception ex) {
                        //VmBase.HandleError();
                    }
                    m_Recipients = value;
                    
                    OnPropertyChanged("Recipients");
                } catch (Exception ex) {
                    ValidateRecipientsInputData(m_Recipients);

                }
            }
        }

        private string m_CarbonCopy = null;
        public string CarbonCopy {
            get {
                return m_CarbonCopy;
            }
            set {
                try {
                    try {
                        ValidateCcInputData(value);
                    } catch (Exception ex) {
                        //VmBase.HandleError();
                    }
                    m_CarbonCopy = value;

                    OnPropertyChanged("CarbonCopy");
                } catch (Exception ex) {
                    ValidateCcInputData(m_CarbonCopy);

                }
            }
        }


        //public string addresses { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string demand_nr { get; set; }
        public bool is_send_bcc { get; set; }

        public string loc_from { get; set; }
        public string loc_to { get; set; }
        public string loc_cc { get; set; }
        public string loc_bcc { get; set; }
        public string loc_select_lang { get; set; }
        public string loc_subject { get; set; }
        public string loc_send { get; set; }
        public string loc_send_to_me { get; set; }
        public string loc_cancel { get; set; }
        public string loc_attachment { get; set; }

        private ObservableCollection<ScmAttachment> m_Attachments = null;
        public ObservableCollection<ScmAttachment> Attachments {
            get {
                if (m_Attachments == null) {
                    m_Attachments = new ObservableCollection<ScmAttachment>();
                }
                return m_Attachments;
            }
            set {
                m_Attachments = value;
                OnPropertyChanged("Attachments");
            }
        }

        private Visibility m_CollapseButtonVisibility = Visibility.Visible;
        public Visibility CollapseButtonVisibility {
            get {
                return m_CollapseButtonVisibility;
            }
            set {
                m_CollapseButtonVisibility = value;
                OnPropertyChanged("CollapseButtonVisibility");
            }
        }

        private Visibility m_ExpandButtonVisibility = Visibility.Collapsed;
        public Visibility ExpandButtonVisibility {
            get {
                return m_ExpandButtonVisibility;
            }
            set {
                m_ExpandButtonVisibility = value;
                OnPropertyChanged("ExpandButtonVisibility");
            }
        }

        private string m_MailContentHeight = "*";
        public string MailContentHeight {
            get {
                return m_MailContentHeight;
            }
            set {
                m_MailContentHeight = value;
                OnPropertyChanged("MailContentHeight");
            }
        }

        private string m_SelectedCulture = null;
        public string SelectedCulture {
            get {
                return m_SelectedCulture;
            }
            set {
                try {
                    try {
                        ValidateCultureInputData(value);
                    } catch (Exception ex) {
                        //VmBase.HandleError();
                    }
                    m_SelectedCulture = value;

                    OnPropertyChanged("SelectedCulture");
                } catch (Exception ex) {
                    ValidateCultureInputData(m_SelectedCulture);

                }

                
            }
        }

        private bool m_IsCollapsed = false;
        public bool IsCollapsed {
            get { return m_IsCollapsed; }
            set {
                m_IsCollapsed = value;
                if (m_IsCollapsed) {
                    CollapseButtonVisibility = Visibility.Collapsed;
                    ExpandButtonVisibility = Visibility.Visible;
                    MailContentHeight = "0";
                } else {
                    CollapseButtonVisibility = Visibility.Visible;
                    ExpandButtonVisibility = Visibility.Collapsed;
                    MailContentHeight = "*";
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        

        protected void OnPropertyChanged(string propertyName) {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null) {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private List<MailCulture> m_MailCultures;
        public List<MailCulture> MailCultures {
            get { return m_MailCultures; }
            set { m_MailCultures = value; }
        }


        #region Validations
        public bool HasErrors { get; set; } = false;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void ValidateRecipientsInputData(string value) {
            if (!IsRecipientsValid(value)) {
                HasErrors = true;
                throw new Exception(ScmResource.EnterValidMailAddresses);
            }

            HasErrors = IsHasErros();
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Recipients"));
        }

        private void ValidateCcInputData(string value) {
            if (!IsCcValid(value)) {
                HasErrors = true;
                throw new Exception(ScmResource.EnterValidMailAddresses);
                
            }

            HasErrors = IsHasErros();
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("CarbonCopy"));
        }

        private void ValidateCultureInputData(string value) {
            if (String.IsNullOrWhiteSpace(value)) {
                HasErrors = true;
                throw new Exception(ScmResource.SelectLang);

            }

            HasErrors = IsHasErros();
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SelectedCulture"));
        }

        public bool IsValid() {

            //List<string> errMsg = new List<string>();
            if (!IsRecipientsValid(Recipients)) {
                HasErrors = true;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Recipients"));
                return false;
       
            }

            if (!IsCcValid(CarbonCopy)) {
                HasErrors = true;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("CarbonCopy"));
                return false;

            }

            if (String.IsNullOrWhiteSpace(SelectedCulture)) {
                HasErrors = true;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SelectedCulture"));
                return false;

                
            }

            return true;
        }

        public IEnumerable GetErrors(string propertyName) {
            List<string> errMsg = new List<string>();

            if (propertyName == "Recipients") {
                

                //if (m_IsErrorCheckSipped) {
                //    return null;
                //}

                if (!IsRecipientsValid(Recipients)) {
                    HasErrors = true;

                    errMsg.Add(ScmResource.EnterValidMailAddresses);

                    //} else if (m_IsSaveError) {
                    //    HasErrors = true;

                    //    //errMsg.Add(ScmResource.DataWasNotSaved);
                    //    errMsg.Add(ScmResource.SelectAppMan);
                    //    m_IsSaveError = false;
                }

                //if (!HasErrors) {
                //    return null;
                //}

                //return errMsg;
            } else if (propertyName == "CarbonCopy") {
                

                if (!IsCcValid(CarbonCopy)) {
                    HasErrors = true;

                    errMsg.Add(ScmResource.EnterValidMailAddresses);

                }

                //if (!HasErrors) {
                //    return null;
                //}

                //return errMsg;
            } else if (propertyName == "SelectedCulture") {
                

                if (!IsSelectedCultureValid()) {
                    HasErrors = true;

                    errMsg.Add(ScmResource.SelectLang);

                }

                //if (!HasErrors) {
                //    return null;
                //}

                //return errMsg;
            }

            return errMsg;
        }

        private bool IsRecipientsValid(string recipients) {
            if (String.IsNullOrWhiteSpace(recipients)) {
                return false;
            }

            if (!VmBase.IsMailAddressValid(Recipients)) {
                return false;
            }

            return true;
        }

        private bool IsCcValid(string cc) {
            if (String.IsNullOrWhiteSpace(cc)) {
                return true;
            }

            if (!VmBase.IsMailAddressValid(CarbonCopy)) {
                return false;
            }

            return true;
        }

        private bool IsSelectedCultureValid() {
            if (String.IsNullOrWhiteSpace(this.SelectedCulture)) {
                return false;
            } else {
                return true;
            }
        }

        private bool IsHasErros() {
            bool isValid = IsRecipientsValid(this.Recipients)
                && IsCcValid(this.CarbonCopy)
                && IsSelectedCultureValid();

            return !isValid;
        }
        #endregion
    }

    public class MailCulture {
        public string culture_code { get; set; }
        public string culture_name { get; set; }
    }
}
