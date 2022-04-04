using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OTISCZ.ScmDemand.UI.ScmUserControl {
    /// <summary>
    /// Interaction logic for UcDataGridFooter.xaml
    /// </summary>
    public partial class UcDataGridFooter2 : UserControl, IView {
        #region Properties
        private VmBaseGrid2 m_VmBaseGrid2 {
            get { return ((VmBaseGrid2)DataContext); }
        }

        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }

        public Visibility AddNewVisibility {
            get { return btnAdd.Visibility; }
            set {
                btnAdd.Visibility = value;
                lnDelimiterAddNew.Visibility = value;
            }
        }

        public string AddNewText {
            get { return txtAddNew.Text; }
            set {
                txtAddNew.Text = value;
            }
        }
        #endregion

        #region Constructor
        public UcDataGridFooter2() {
            InitializeComponent();
            LocalizeUc();
            btnAdd.Visibility = Visibility.Collapsed;
            lnDelimiterAddNew.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Methods
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void LocalizeUc() {
            btnFirst.ToolTip = ScmResource.FirstPage;
            btnPrevious.ToolTip = ScmResource.PreviousPage;
            txtPageNr.ToolTip = ScmResource.GoToPage;
            btnNext.ToolTip = ScmResource.NextPage;
            btnLast.ToolTip = ScmResource.LastPage;
            btnImport.ToolTip = ScmResource.Import;
            btnExcel.ToolTip = ScmResource.ExportToExcel;
            btnRefresh2.ToolTip = ScmResource.Reload;
            cmbPageSize.ToolTip = ScmResource.PageSize;
            txtOf.Text = ScmResource.Of;
        }

        public void RefreshData(
            int pageNr,
            int pagesCount,
            int pageSize,
            string strDisplayingRows) {

            cmbPageSize.SelectedValue = pageSize;
            txtPageNr.Text = pageNr.ToString();
            txtPagesCount.Text = pagesCount.ToString();
            txtDiplayingRows.Text = strDisplayingRows;
        }

        public void SetLayout() {
            //dpPanel.Style = m_MainWindow.GetLayoutStyle("DpGrdFooter");
        }
        #endregion

        private void BtnNext_Click(object sender, RoutedEventArgs e) {
            m_VmBaseGrid2.IsBusy = true;
            m_VmBaseGrid2.PageNr2++;
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e) {
            m_VmBaseGrid2.PageNr2 = m_VmBaseGrid2.PagesCount2;
        }

        private void BtnFirst_Click(object sender, RoutedEventArgs e) {
            m_VmBaseGrid2.PageNr2 = 1;
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e) {
            m_VmBaseGrid2.PageNr2--;
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e) {
            m_VmBaseGrid2.ExportToExcel2();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) {
            m_VmBaseGrid2.RefreshGridData();
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e) {
            m_VmBaseGrid2.Import2();
        }

        private void BtnRefresh2_Click(object sender, RoutedEventArgs e) {
            m_VmBaseGrid2.RefreshGridData2();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e) {
            m_VmBaseGrid2.AddNew2();
        }
    }
}
