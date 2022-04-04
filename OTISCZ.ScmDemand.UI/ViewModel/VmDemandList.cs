using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class VmDemandList : VmBaseGridDemand, INotifyPropertyChanged, INotifyDataErrorInfo {
        #region Interface Event
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion

        #region Virtual Properties
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

        private int m_PageNr = 1;
        public override int PageNr {
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

        private int m_PagesCount = 0;
        public override int PagesCount {
            get {
                return m_PagesCount;
            }
            set {
                if (value < 1) {
                    value = 1;
                }
                m_PagesCount = value;
                DisplayGridFooterButtons();
                OnPropertyChanged("PagesCount");
            }
        }
        #endregion

        #region Properties

        #endregion

        #region Constructor
        public VmDemandList(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {
            //this.DataKey = DataKeyEnum.Nomenclature;
            DlgSetDisplyingInfoDemands = new DlgSetDisplyingInfo(SetDisplyingInfoDemands);
        }
        #endregion

        #region Interface Methods
        public IEnumerable GetErrors(string propertyName) {
            throw new NotImplementedException();
        }


        #endregion

        #region Methods
        private void SetDisplyingInfoDemands() {
            DisplayingRows = ScmResource.DisplayingFromToOf
                    .Replace("{0}", GetDisplayItemsFromInfo().ToString())
                    .Replace("{1}", GetDisplayItemsToInfo().ToString())
                    .Replace("{2}", RowsCount.ToString());

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
            } else {
                NextEnabledButtonVisibility = Visibility.Collapsed;
                NextDisabledButtonVisibility = Visibility.Visible;
            }
        }

        public override void RefreshGridData() {
            LoadDemands();
            DisplayGridFooterButtons();
        }
        #endregion

    }
}
