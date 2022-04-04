using DocumentFormat.OpenXml.Spreadsheet;
using Kamsyk.ExcelOpenXml;
using OTISCZ.CommonDb;
using OTISCZ.ScmDemand.Model;
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

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class VmSupplier : VmBase, INotifyPropertyChanged, INotifyDataErrorInfo {
        #region Overriden Properties

        #endregion

        #region Properties
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private DlgRefreshEditableDataGrid m_DlgRefreshEditableDataGrid = null;
        private DlgCloseWindow m_DlgClosewindow = null;

        private int m_id = DataNulls.INT_NULL;

        private SupplierExtend m_SupplierExtend = null;
        public SupplierExtend SupplierExtend {
            get {
                if (m_SupplierExtend == null) {
                    if (m_id == DataNulls.INT_NULL) {
                        m_SupplierExtend = new SupplierExtend();
                        m_SupplierExtend.id = DataNulls.INT_NULL;
                        m_SupplierExtend.supplier_contact_extended = new List<SupplierContactExtended>();
                    } else {
                        var dbSupplierExtend = WcfScm.GetSupplierById(m_id);
                        m_SupplierExtend = new SupplierExtend();
                        SetValues(dbSupplierExtend, m_SupplierExtend);
                        m_SupplierExtend.supplier_contact_extended = new List<SupplierContactExtended>();
                        if (dbSupplierExtend.supplier_contact_extended != null) {
                            foreach (var supCont in dbSupplierExtend.supplier_contact_extended) {
                                SupplierContactExtended tmpSupContact = new SupplierContactExtended();
                                SetValues(supCont, tmpSupContact);
                                m_SupplierExtend.supplier_contact_extended.Add(tmpSupContact);
                            }
                        }
                    }
                }

                BtnNewVisibility = (m_SupplierExtend.id < 0) ? Visibility.Visible : Visibility.Collapsed;
                if (m_SupplierExtend.is_approved) {
                    ApprovedSupplierInfo = ScmResource.ApprovedSupplier;
                }

                m_IsErrorCheckSipped = true;
                SupplierName = m_SupplierExtend.supp_name;
                m_IsErrorCheckSipped = false;

                return m_SupplierExtend;
            }
            set {
                m_SupplierExtend = value;
                OnPropertyChanged("SupplierExtend");
            }
        }

        private bool m_IsSaveError = false;

        private bool m_IsErrorCheckSipped = false;

        private string m_SupplierName = null;
        public string SupplierName {
            get {
                return m_SupplierName;
            }
            set {
                try {
                    try {
                        ValidateSuppNameInputData(value);
                    } catch (Exception ex) {

                    }
                    m_SupplierName = value;
                    if (!String.IsNullOrWhiteSpace(m_SupplierName)) {
                        m_SupplierExtend.supp_name = m_SupplierName;
                        
                        //SaveDemandRefresh();
                    }
                    OnPropertyChanged("SupplierName");
                } catch (Exception ex) {
                    m_IsSaveError = true;
                    HandleError(ex);
                    ValidateSuppNameInputData(m_SupplierName);

                }
            }
        }

        private Visibility m_BtnNewVisibility = Visibility.Collapsed;
        public Visibility BtnNewVisibility {
            get {
                return m_BtnNewVisibility;
            }
            set {
                m_BtnNewVisibility = value;
                OnPropertyChanged("BtnNewVisibility");
            }
        }

        private string m_ApprovedSupplierInfo = "";
        public string ApprovedSupplierInfo {
            get {
                return m_ApprovedSupplierInfo;
            }
            set {
                m_ApprovedSupplierInfo = value;
                OnPropertyChanged("ApprovedSupplierInfo");
            }
        }

        private bool m_IsNewSupplier {
            get {
                if (m_SupplierExtend == null) {
                    return true;
                }

                if (m_SupplierExtend.id < 0) {
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region Localization
        public string LocCloseText {
            get { return ScmResource.Close; }
        }
        #endregion

        #region Constructor
        public VmSupplier(
            int id,
            ScmUser scmUser,
            Dispatcher dispatcher,
            DlgRefreshEditableDataGrid dlgRefreshDashboard,
            DlgCloseWindow dlgCloseWindow) : base(scmUser, dispatcher) {
            m_id = id;

            m_DlgRefreshEditableDataGrid = dlgRefreshDashboard;
            m_DlgClosewindow = dlgCloseWindow;
        }
        #endregion

        #region Interface Methods
        public IEnumerable GetErrors(string propertyName) {
            if (propertyName == "SupplierName") {
                List<string> errMsg = new List<string>();

                if (m_IsErrorCheckSipped) {
                    return null;
                }

                if (String.IsNullOrWhiteSpace(SupplierName)) {
                    HasErrors = true;

                    errMsg.Add(ScmResource.SelectAppMan);

                } else if (m_IsSaveError) {
                    HasErrors = true;

                    //errMsg.Add(ScmResource.DataWasNotSaved);
                    errMsg.Add(ScmResource.SelectAppMan);
                    m_IsSaveError = false;
                }

                if (!HasErrors) {
                    return null;
                }

                return errMsg;
            }
                        
            return null;
        }
        #endregion

        #region Static Methods
        //public static bool IsValidMail(string emailaddress) {
        //    try {
        //        MailAddress m = new MailAddress(emailaddress);

        //        return true;
        //    } catch (FormatException) {
        //        return false;
        //    }
        //}
        #endregion

        #region Abstract Methods

        #endregion

        #region Overriden Methods

        #endregion

        #region Async Methods

        #endregion

        #region Methods
        public void SaveSupplier(bool isCloseWindow) {
            if (!IsValid()) {
                return;
            }

            string errMsg = null;
            if (!IsSupplierValid(out errMsg)) {
                MessageBox.Show(errMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }


            int supplierId = WcfScm.SaveSupplier(m_SupplierExtend, CurrentUser.User.id, true, false);

            if (isCloseWindow) {
                m_DlgClosewindow?.Invoke();
                m_DlgRefreshEditableDataGrid?.Invoke(supplierId);
            }
        }

        private bool IsSupplierValid(out string errMsg) {
            errMsg = "";


            if (m_SupplierExtend.supplier_contact_extended != null) {
                foreach (var supCon in m_SupplierExtend.supplier_contact_extended) {
                    if (supCon.entity_error != null && supCon.entity_error.Count > 0) {
                        errMsg = supCon.entity_error.ElementAt(0).errors.ElementAt(0);
                        return false;
                    }
                }
            }

            return true;
        }

        public void SaveSupplierContact(SupplierContactExtended supplierContactExtended) {
            if (!m_IsNewSupplier) {

                int supplierContactId = WcfScm.SaveSupplierContact(supplierContactExtended, CurrentUser.User.id);
            }
            
        }

        public DlgCloseWindow CloseWindow {
            get { return m_DlgClosewindow; }
        }
        #endregion

        #region Validations
        private void ValidateSuppNameInputData(string value) {
            if (String.IsNullOrWhiteSpace(value)) {
                HasErrors = true;
                throw new Exception(ScmResource.SelectAppMan);
            } else if (m_IsSaveError) {
                HasErrors = true;
                m_IsSaveError = false;
                throw new Exception(ScmResource.SelectAppMan);
                //throw new Exception(ScmResource.DataWasNotSaved);

            }

            HasErrors = false;
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SupplierName"));
        }

        public bool IsValid() {

            //List<string> errMsg = new List<string>();
            if (String.IsNullOrWhiteSpace(SupplierName)) {
                HasErrors = true;
            }

            if (HasErrors) {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SupplierName"));
            } else {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SupplierName"));
                return true;
            }
            return false;
        }
        #endregion
    }
}
