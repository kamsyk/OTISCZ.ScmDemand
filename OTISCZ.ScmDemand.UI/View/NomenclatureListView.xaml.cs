using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.ScmWindow;
using OTISCZ.ScmDemand.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static OTISCZ.ScmDemand.UI.ViewModel.VmBase;
using static OTISCZ.ScmDemand.UI.ViewModel.VmBaseGrid;

namespace OTISCZ.ScmDemand.UI.View {
    /// <summary>
    /// Interaction logic for NomenclatureList.xaml
    /// </summary>
    public partial class NomenclatureListView : UserControl, IView {
        #region Properties
        private VmNomenclatureList m_ViewModel {
            get { return ((VmNomenclatureList)DataContext); }
        }

        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }

        //public static readonly DependencyProperty NewDemandVisibilityProperty = DependencyProperty.Register(
        //    "NewDemandVisibility",
        //    typeof(Visibility),
        //    typeof(NomenclatureListView),
        //    new UIPropertyMetadata(Visibility.Collapsed));

        //public Visibility NewDemandVisibility {
        //    get { return Visibility.Collapsed; }
        //    //get { return (Visibility)GetValue(NewDemandVisibilityProperty); }
        //    //set { SetValue(NewDemandVisibilityProperty, value); }
        //}

        //public Visibility NewDemandVisibility {
        //    get {
        //        //if(CurrentUser.IsScmReferent) {
        //        //    return Visibility.Visible;
        //        //}

        //        return Visibility.Collapsed;
        //    }

        //}
        #endregion

        #region Constructor
        public NomenclatureListView() {
            InitializeComponent();
            LocalizeUc();
        }

        public void SetLayout() {

        }
        #endregion

        #region Interface Methods
        public void LocalizeUc() {
            ucWindowHeader.WindowTitle = ScmResource.Nomenclatures;
            //grdNomenclature.Columns[3].Header = ScmResource.Nomenclature;
            //grdNomenclature.Columns[4].Header = ScmResource.NameIt;
            ////grdNomenclature.Columns[4].Header = ScmResource.stat;
            //grdNomenclature.Columns[6].Header = ScmResource.MaterialGroup;
            
            ucDataGridFooter.LocalizeUc();
        }

        //public void AdjustWidth(double dWidth) {
        //    ucWindowHeader.Width = dWidth;
        //}
        #endregion

        private void LoadInit() {
            MainWindow mainWindow = MainWindow.GetMainWindow(this);
            VmNomenclatureList vmNomenclatureList = new VmNomenclatureList(
                mainWindow.CurrentUser.User,
                System.Windows.Application.Current.MainWindow.Dispatcher);
            this.DataContext = vmNomenclatureList;
            ucDataGridFooter.DataContext = vmNomenclatureList;

            foreach (var col in grdNomenclature.Columns) {
                
                var scmName = DataGridUtil.GetScmColumnName(col);
                if (scmName == "new_demand") {
                    if (m_ViewModel.CurrentUser.IsScmReferent) {
                        col.Visibility = Visibility.Visible;
                    } else {
                        col.Visibility = Visibility.Collapsed;
                    }
                    break;
                }
            }
        }
        
        //private async void FilterGridData(object sender) {
        //    if (!MainWindow.IsDebounceRunning) {
        //        var t = MainWindow.DebounceTimerAsync();
        //        await t;

        //        UcGrdColHeaderFilterText ft = (UcGrdColHeaderFilterText)sender;
        //        m_ViewModel.UpdateFilter(ft.FieldName, ft.FilterText);
        //        m_ViewModel.PageNr = 1;
        //        //m_ViewModel.RefreshGridData();
        //        MainWindow.IsDebounceRunning = false;
        //    }
        //}
               

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            LoadInit();
        }

        private void UcDataGridFilterTextNomenclature_FilterTextChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender);
        }

        private void ColumnHeader_Click(object sender, RoutedEventArgs e) {
            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
            m_ViewModel.SetSort(grdNomenclature, grdColumn);
        }

        private void BtnNomDetail_Click(object sender, RoutedEventArgs e) {
            Button btnNom = sender as Button;
            NomenclatureExtend nom = (NomenclatureExtend)btnNom.DataContext;

            WinNomenclature wn = new WinNomenclature(nom);
            wn.Show();
            wn.Owner = m_MainWindow;
        }

        private void DataGridColumnHeaderNomenclature_Click(object sender, RoutedEventArgs e) {
            DataGridColumnHeader grdColumnHeader = sender as DataGridColumnHeader;
            if (grdColumnHeader == null) {
                return;
            }

            DataGridColumn grdColumn = grdColumnHeader.Column;
            m_ViewModel.SetSort(grdNomenclature, grdColumn);
        }

        private void BtnNewDemand_Click(object sender, RoutedEventArgs e) {
            Button btnNewDemand = sender as Button;
            NomenclatureExtend nom = (NomenclatureExtend)btnNewDemand.DataContext;

            WinNewDemand wn = new WinNewDemand(nom, new DlgRefreshDataGrid(m_ViewModel.RefreshNomenclatures));
            wn.Title = ScmResource.NewDemand;
            wn.Show();
            wn.Owner = m_MainWindow;
        }

        private void UcDataGridFilterText_FilterTextChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender);
        }

        private void FltCreatedNom_FilterFromChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender, FilterFromTo.From);
        }

        private void FltCreatedNom_FilterToChanged(object sender, RoutedEventArgs e) {
            m_ViewModel.FilterGridData(sender, FilterFromTo.To);
        }
    }

   
}
