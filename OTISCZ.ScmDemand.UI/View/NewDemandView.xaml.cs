using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static OTISCZ.ScmDemand.UI.ViewModel.VmBaseGrid;

namespace OTISCZ.ScmDemand.UI.View {
    /// <summary>
    /// Interaction logic for NewDemandView.xaml
    /// </summary>
    public partial class NewDemandView : UserControl, IView {
        #region Properties
        private VmNewDemand m_ViewModel {
            get { return ((VmNewDemand)DataContext); }
        }

        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }
        #endregion

        #region Constructor
        public NewDemandView() {
            InitializeComponent();
            LocalizeUc();
            SetLayout();

        }
        #endregion

        #region Interface Methods
        public void LocalizeUc() {
            ucDataGridHeaderNomenclature.GrdTitle = ScmResource.PendingNomenclatures;
            ucGrdHeadeSelectedNoms.GrdTitle = ScmResource.SelectedNomenclature;

            ucDataGridHeaderSupplier.GrdTitle = ScmResource.Suppliers;
            ucGrdHeadeSelectedSupp.GrdTitle = ScmResource.SelectedSuppliers;
            ucAttDataGridHeader.GrdTitle = ScmResource.Attachments;


            ucWindowHeader.WindowTitle = ScmResource.NewDemand;

            tbCustNomKey.Text = ScmResource.Nomenclature;
            tbCustNomName.Text = ScmResource.NameIt;
            tbCustNomSpec.Text = ScmResource.Specification;

            VmBaseGrid.LocalizeNomenclatureGridColumnsLabels(grdNomenclature);
            ucDataGridFooterNomenclature.LocalizeUc();
                        
            SetListViewHeader(lstSelectedNomenclatures, 1, ScmResource.Nomenclature);
            SetListViewHeader(lstSelectedNomenclatures, 2, ScmResource.NameIt);
            SetListViewHeader(lstSelectedNomenclatures, 3, ScmResource.Specification);


            VmBaseGrid.LocalizeSupplierGridColumnsLabels(grdSupplier);
            ucDataGridFooterSupplier.LocalizeUc();

            SetListViewHeader(lstSelectedSuppliers, 1, ScmResource.NameIt);
            SetListViewHeader(lstSelectedSuppliers, 2, ScmResource.SupplierId);
            

            SetListViewHeader(lstAttNom, 1, ScmResource.Nomenclature);
            SetListViewHeader(lstAttNom, 2, ScmResource.NameIt);
            SetListViewHeader(lstAttNom, 3, ScmResource.Specification);

            SetListViewHeader(lstAttSupp, 1, ScmResource.NameIt);
            SetListViewHeader(lstAttSupp, 2, ScmResource.SupplierId);

            
        }

        private void SetListViewHeader(ListView listView, int colIndex, string headerText) {
            var header = ((GridView)listView.View).Columns[colIndex].Header as GridViewColumnHeader;
            header.Content = headerText;
        }

        public void SetLayout() {
            MainWindow.RefreshStyles(grdMain);

            //ucWindowHeader.SetLayout();

            //grdMain.Style = m_MainWindow.GetLayoutStyle("GrdBkg");

            //btnPrevious.Style = m_MainWindow.GetLayoutStyle("ButtonStandard");
            ////btnNext.Style = m_MainWindow.GetLayoutStyle("ButtonStandard");

            //btnNext.Style = null;
            //btnNext.Style = (Style)FindResource("ButtonStandard");

            //ucDataGridHeaderNomenclature.SetLayout();
            //grdNomenclature.Style = m_MainWindow.GetLayoutStyle("ScmDataGrid");
            //svGrdNomenclature.Style = m_MainWindow.GetLayoutStyle("GrdScroll");

            //ucDataGridHeaderSupplier.SetLayout();
            //grdSupplier.Style = m_MainWindow.GetLayoutStyle("ScmDataGrid");
            //svGrdSupplier.Style = m_MainWindow.GetLayoutStyle("GrdScroll");
            //foreach (var col in grdNomenclature.Columns) {
            //    if (col is DataGridTextColumn) {
            //        ((DataGridTextColumn)col).ElementStyle = m_MainWindow.GetLayoutStyle("GridColumnElementStyleTextBlock");
            //    } else if(col is DataGridTemplateColumn) {
            //        //var templateContent = ((DataGridTemplateColumn)col).CellTemplate.LoadContent();
            //        //if (templateContent is Button) {
            //        //    //((Button)templateContent).Style= m_MainWindow.GetLayoutStyle("ButtonStandard");
            //        //    //grdNomenclature.Items.Refresh();

            //        //}

            //    }
            //}

            //ucDataGridFooterNomenclature.SetLayout();
        }

        //public void RefreshStyles(DependencyObject parent) {
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
        //        var child = VisualTreeHelper.GetChild(parent, i);
        //        if ((child is Control)) {
        //            Control c = (Control)child;
        //            Style bkpStyle = c.Style;
        //            c.Style = null;
        //            c.Style = bkpStyle; 
        //        }

                

        //        RefreshStyles(child);

        //    }
            
        //}
        #endregion

        #region Methods
        //private void RefreshGridFooter() {
        //    ucDataGridFooterNomenclature.RefreshData(
        //        m_ViewModel.PageNr,
        //        m_ViewModel.PagesCount,
        //        m_ViewModel.PageSize,
        //        m_ViewModel.DisplayingRows);

        //    ucDataGridFooterSupplier.RefreshData(
        //        m_ViewModel.PageNr2,
        //        m_ViewModel.PagesCount2,
        //        m_ViewModel.PageSize2,
        //        m_ViewModel.DisplayingRows2);
        //}
        #endregion

        private void ColumnHeader_Click(object sender, RoutedEventArgs e) {
            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
            m_ViewModel.SetSort(grdNomenclature, grdColumn);
        }

        
        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            if (ucDataGridFooterNomenclature.DataContext == null) {
                VmNewDemand vmNewDemand = new VmNewDemand(
                    m_MainWindow.CurrentUser.User,
                    grdSupplier,
                    System.Windows.Application.Current.MainWindow.Dispatcher,
                    null,
                    null);
                this.DataContext = vmNewDemand;
                ucDataGridFooterNomenclature.DataContext = vmNewDemand;
                
            }

            // m_ViewModel.SupplierEditVisibility = Visibility.Collapsed;

            grdSupplier.Columns[1].Visibility = m_ViewModel.SupplierEditVisibility;
            grdSupplier.Columns[2].Visibility = m_ViewModel.SupplierEditVisibility;
        }

        private void UcDataGridFilterText_FilterTextChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender);
        }

        private void SelectedNomKeyHeader_Click(object sender, RoutedEventArgs e) {
            
        }


        private void CkbSelectedNom_Checked(object sender, RoutedEventArgs e) {
            m_ViewModel.SelectNomenclatures(sender);
            
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.Previous();
            //RefreshGridFooter();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.Next();

            
        }

        private void BtnRemoveNomenclature_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.IsBusy = true;
            m_ViewModel.RemoveNomenclatureAsync(sender, grdNomenclature);
        }

        private void BtnRemoveSupplier_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.IsBusy = true;
            m_ViewModel.RemoveSupplierAsync(sender, grdSupplier);
            lstMails.Items.Refresh();
        }

        private void CkbSelectedSup_Checked(object sender, RoutedEventArgs e) {
            m_ViewModel.SelectSupplier(sender);
        }

        private void DockPanel_DragEnter(object sender, DragEventArgs e) {
            
        }

        private void DockPanel_DragOver(object sender, DragEventArgs e) {
            
        }

        private void DockPanel_Drop(object sender, DragEventArgs e) {
            m_ViewModel.DropAttachment(e);
        }

        private void UcWindowHeader_Loaded(object sender, RoutedEventArgs e) {

            if (m_ViewModel.CurrentUser.IsScmReferent) {
                ucDataGridFooterSupplier.AddNewVisibility = Visibility.Visible;
                ucDataGridFooterSupplier.AddNewText = ScmResource.NewSupplier;
            }
        }

        private void BtnCollapseExpand_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.CollapseExpandMail(sender);

            lstMails.Items.Refresh();
        }

        
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                m_ViewModel.OpenAttachmentGeneral(sender);
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.SendMail(sender, false);
        }

        private void BtnRemoveMailAtt_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.RemoveMailAtt(sender);
        }

        private void UcGrdColHeaderFilterText_FilterTextChanged(object sender, RoutedEventArgs e) {

        }

        private void UcGrdColHeaderFilterTextSupp_FilterTextChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData2(sender);
        }

        private void BtnRemoveAttachment_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.RemoveAtt(sender);
        }

        private void CmbLang_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            m_ViewModel.LangChanged(sender);
            lstMails.Items.Refresh();
        }

        private void BtnEditSupp_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.SupplierEdit(sender);
        }

        private void FltCreatedNom_FilterToChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender);
        }

        private void FltCreatedNom_FilterFromChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender, FilterFromTo.From);
        }

        private void DataGridColumnHeaderNomenclature_Click(object sender, RoutedEventArgs e) {
            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
            m_ViewModel.SetSort(grdNomenclature, grdColumn);
        }

        private void DataGridColumnHeaderSupplier_Click(object sender, RoutedEventArgs e) {
            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
            m_ViewModel.SetSort2(grdSupplier, grdColumn);
        }

        private void BtnSendtoMe_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.SendMail(sender, true);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.CancelMail(sender);
        }

        

        private void ImgDispHalf_MouseDown(object sender, MouseButtonEventArgs e) {
            grdNom.ColumnDefinitions.ElementAt(0).Width = new GridLength(50, GridUnitType.Star);
            grdNom.ColumnDefinitions.ElementAt(2).Width = new GridLength(45, GridUnitType.Star);

            brDispHalf.Visibility = Visibility.Collapsed;
        }

        private void BtnAddNom_Click(object sender, RoutedEventArgs e) {
           m_ViewModel.AddCustNom(txtCustNomKey, txtCustNomName, txtCustNomSpec);
               
           
        }
               
        private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e) {
            brDispHalf.Visibility = Visibility.Visible;
        }

        private void LstSelectedNomenclatures_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            ListView lv = (ListView)sender;
            NomenclatureExtend selNom = (NomenclatureExtend)lv.SelectedItem;

            m_ViewModel.CustNomenclatureSelected(selNom, txtCustNomKey, txtCustNomName, txtCustNomSpec);
        }

        private void BtnSaveNom_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.SaveCustNom(txtCustNomKey, txtCustNomName, txtCustNomSpec);
        }

        private void BtnCancelNom_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.CancelCustNom(txtCustNomKey, txtCustNomName, txtCustNomSpec);
        }
    }

   
}
