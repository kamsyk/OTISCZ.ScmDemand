using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public abstract class VmBaseGrid2 : VmBaseGrid {
        #region Properties
        private int m_PageNr = 1;
        public virtual int PageNr2 {
            get { return m_PageNr; }
            set {
                if (value < 1) {
                    return;
                }
                if (value > m_PagesCount) {
                    IsBusy = false;
                    DisplayGridFooterButtons();
                    return;
                }
                m_PageNr = value;
                RefreshGridData2();
                DisplayGridFooterButtons();
                OnPropertyChanged("PageNr2");
            }
        }

        private int m_PagesCount = 0;
        public virtual int PagesCount2 {
            get {
                return m_PagesCount;
            }
            set {
                if (value < 1) {
                    value = 1;
                }
                m_PagesCount = value;
                OnPropertyChanged("PagesCount2");
            }
        }

        private int m_PageSize = 10;
        public virtual int PageSize2 {
            get {
                return m_PageSize;
            }
            set {
                m_PageSize = value;
                PageNr2 = 1;
                OnPropertyChanged("PageSize2");
            }
        }

        private int m_RowsCount = 0;
        public virtual int RowsCount2 {
            get { return m_RowsCount; }
            set {
                m_RowsCount = value;
                CalculatePagesCount2();
            }
        }

        private string m_DisplayingRows = null;
        public virtual string DisplayingRows2 {
            get {
                return m_DisplayingRows;
            }
            set {
                m_DisplayingRows = value;
                OnPropertyChanged("DisplayingRows2");
            }
        }

        private Visibility m_PreviousEnabledButtonVisibility = Visibility.Collapsed;
        public Visibility PreviousEnabledButtonVisibility2 {
            get {
                return m_PreviousEnabledButtonVisibility;
            }
            set {
                m_PreviousEnabledButtonVisibility = value;
                OnPropertyChanged("PreviousEnabledButtonVisibility2");
            }
        }

        private Visibility m_PreviousDisabledButtonVisibility = Visibility.Visible;
        public Visibility PreviousDisabledButtonVisibility2 {
            get {
                return m_PreviousDisabledButtonVisibility;
            }
            set {
                m_PreviousDisabledButtonVisibility = value;
                OnPropertyChanged("PreviousDisabledButtonVisibility2");
            }
        }

        private Visibility m_NextEnabledButtonVisibility = Visibility.Visible;
        public Visibility NextEnabledButtonVisibility2 {
            get {
                return m_NextEnabledButtonVisibility;
            }
            set {
                m_NextEnabledButtonVisibility = value;
                OnPropertyChanged("NextEnabledButtonVisibility2");
            }
        }

        private Visibility m_NextDisabledButtonVisibility = Visibility.Collapsed;
        public Visibility NextDisabledButtonVisibility2 {
            get {
                return m_NextDisabledButtonVisibility;
            }
            set {
                m_NextDisabledButtonVisibility = value;
                OnPropertyChanged("NextDisabledButtonVisibility2");
            }
        }

        private Visibility m_ImportButtonVisibility = Visibility.Visible;
        public Visibility ImportButtonVisibility2 {
            get {
                return m_ImportButtonVisibility;
            }
            set {
                m_ImportButtonVisibility = value;
                OnPropertyChanged("ImportButtonVisibility2");
            }
        }

        private bool m_isGridButtonsEnabled = true;
        public bool IsGridButtonsEnabled2 {
            get {
                return m_isGridButtonsEnabled;
            }
            set {
                m_isGridButtonsEnabled = value;
                OnPropertyChanged("IsGridButtonsEnabled2");
            }
        }

        private DataGrid m_DataGrid = null;
        protected DataGrid DataGrid2 {
            get { return m_DataGrid; }
            set { m_DataGrid = value; }
        }

        private List<FilterField> m_FilterFields = null;
        public List<FilterField> FilterFields2 {
            get { return m_FilterFields; }
            set { m_FilterFields = value; }
        }

        private string m_SortFieldName = null;
        public string SortFieldName2 {
            get { return m_SortFieldName; }
            set { m_SortFieldName = value; }
        }

        private Visibility m_ProgressBarVisibility = Visibility.Collapsed;
        public Visibility ProgressBarVisibility2 {
            get { return m_ProgressBarVisibility; }
            set {
                m_ProgressBarVisibility = value;
                OnPropertyChanged("ProgressBarVisibility2");
            }
        }

        private ListSortDirection m_SortDirection = ListSortDirection.Ascending;
        public ListSortDirection SortDirection2 {
            get { return m_SortDirection; }
            set { m_SortDirection = value; }
        }
        #endregion

        #region Constructor
        public VmBaseGrid2(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {

        }
        #endregion

        #region Abstract Methods
        public abstract void RefreshGridData2();
        public abstract void ExportToExcel2();
        public abstract void Import2();
        public abstract void AddNew2();
        #endregion

        #region Methods
        private void DisplayGridFooterButtons() {
            if (m_PageNr > 1) {
                PreviousEnabledButtonVisibility2 = Visibility.Visible;
                PreviousDisabledButtonVisibility2 = Visibility.Collapsed;
            } else {
                PreviousEnabledButtonVisibility2 = Visibility.Collapsed;
                PreviousDisabledButtonVisibility2 = Visibility.Visible;
            }

            if (m_PageNr < m_PagesCount) {
                NextEnabledButtonVisibility2 = Visibility.Visible;
                NextDisabledButtonVisibility2 = Visibility.Collapsed;
            } else {
                NextEnabledButtonVisibility2 = Visibility.Collapsed;
                NextDisabledButtonVisibility2 = Visibility.Visible;
            }
        }

        protected int GetDisplayItemsFromInfo2() {
            if (PageNr2 == 0) {
                return 0;
            }

            if (RowsCount2 == 0) {
                return 0;
            }

            return (PageNr2 - 1) * PageSize2 + 1;

        }

        protected void CalculatePagesCount2() {
            if (m_PageSize == 0) {
                PagesCount2 = 0;
                return;
            }

            decimal dPgc = (Convert.ToDecimal(m_RowsCount) / Convert.ToDecimal(m_PageSize));
            decimal dPgcFloor = Decimal.Floor(dPgc);

            int iPgc = Convert.ToInt16(dPgcFloor);
            if (dPgc != dPgcFloor) {
                iPgc++;
            }
            
            PagesCount2 = iPgc;
        }

        protected int GetDisplayItemsToInfo2() {
            var iToInfo = (PageNr2) * PageSize2;
            if (iToInfo > RowsCount2) {
                iToInfo = RowsCount2;
            }
            return iToInfo;
        }

        protected void GridInit2() {
            DisplayGridFooterButtons();
            FixPageNr();
        }

        
        public void SetSort2(DataGrid grid, DataGridColumn grdColumn) {
            m_DataGrid = grid;

            bool isSortDisabled = DataGridUtil.GetIsSortDisabled(grdColumn);

            if (isSortDisabled) {
                grdColumn.SortDirection = null;
                return;
            }

            string sortField = grdColumn.SortMemberPath;
            //if (grdColumn.SortDirection == null) {
            //    grdColumn.SortDirection = ListSortDirection.Ascending;
            //}

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

            PageNr2 = 1;

            
            //grdColumn.SortDirection = m_SortDirection;
        }

        private void FixPageNr() {
            if (m_PageNr < 1) {
                PageNr2 = 1;
            }

            if (m_PageNr > m_PagesCount) {
                PageNr2 = m_PagesCount;
            }
        }

        protected void SetDataGridColumnSort2() {
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

        public virtual void FilterGridData2(object sender) {
            FilterGridData2(sender, FilterFromTo.No);
        }

        public virtual async void FilterGridData2(object sender, FilterFromTo fromTo) {
            if (!MainWindow.IsDebounceRunning) {
                var t = MainWindow.DebounceTimerAsync();
                await t;

                //UcGrdColHeaderFilterText ft = (UcGrdColHeaderFilterText)sender;
                //UpdateFilter(ref m_FilterFields, ft.FieldName, ft.FilterText, fromTo);

                UpdateFilter(sender, fromTo, ref m_FilterFields);

                PageNr2 = 1;

                MainWindow.IsDebounceRunning = false;
            }
        }

        protected string GetFilter2() {
            if (m_FilterFields == null || m_FilterFields.Count == 0) {
                return null;
            }

            string strFilter = "";
            foreach (var ff in m_FilterFields) {
                if (strFilter.Length > 0) {
                    strFilter += BaseRepository<Nomenclature>.UrlParamDelimiter;
                }
                strFilter += ff.FieldName + BaseRepository<Nomenclature>.UrlParamValueDelimiter + ff.FilterText;
            }

            return strFilter;
        }

        protected WcfFilterField[] GetFilterFields2() {
            if (m_FilterFields == null || m_FilterFields.Count == 0) {
                return null;
            }

            WcfFilterField[] wcfFilterFields = new WcfFilterField[m_FilterFields.Count];
            for (int i = 0; i < m_FilterFields.Count; i++) {
                WcfFilterField wcfFilterField = new WcfFilterField();
                wcfFilterField.FieldName = m_FilterFields[i].FieldName;
                wcfFilterField.FilterText = m_FilterFields[i].FilterText;
                wcfFilterField.FromTo = (int)m_FilterFields[i].FromTo;
                wcfFilterField.SqlFilter = m_FilterFields[i].SqlFilter;
                wcfFilterFields[i] = wcfFilterField;
            }

            return wcfFilterFields;
        }

        protected string GetOrder2() {
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

        protected void SetFilterField2(string fieldName, string filterValue) {
            SetFilterField(ref m_FilterFields, fieldName, filterValue, FilterFromTo.No);
        }

        protected void SetFilterField2(string sqlFilter) {
            SetFilterField(ref m_FilterFields, sqlFilter);
        }

        protected void SetFilterField2(string fieldName, string filterValue, FilterFromTo fromTo) {
            SetFilterField(ref m_FilterFields, fieldName, filterValue, fromTo);
        }

        protected void RemoveFilterField2(string fieldName) {
            RemoveFilterField(ref m_FilterFields, fieldName);
        }


       

        //protected void SetFilterField2(string fieldName, string filterValue) {
        //    FilterField nomStatus = new FilterField(fieldName, filterValue);
        //    if (m_FilterFields == null) {
        //        m_FilterFields = new List<FilterField>();
        //    }

        //    var sfilterField = (from sfilterFieldD in FilterFields
        //                        where sfilterFieldD.FieldName == fieldName
        //                        select sfilterFieldD).FirstOrDefault();

        //    if (sfilterField == null) {
        //        m_FilterFields.Add(nomStatus);
        //    } else {
        //        sfilterField.FilterText = filterValue;
        //    }
        //}

        //protected void RemoveFilterField2(string fieldName) {

        //    var sfilterField = (from sfilterFieldD in FilterFields
        //                        where sfilterFieldD.FieldName == fieldName
        //                        select sfilterFieldD).FirstOrDefault();

        //    if (sfilterField != null) {
        //        m_FilterFields.Remove(sfilterField); 
        //    } 
        //}
        #endregion
    }
}
