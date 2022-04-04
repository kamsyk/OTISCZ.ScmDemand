using Kamsyk.ExcelOpenXml;
using OTISCZ.ScmDemand.Interface;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Common;
using OTISCZ.ScmDemand.UI.Resource;
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
using System.Windows;
using System.Windows.Threading;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class VmNomenclatureList : VmBaseGridDemand, INotifyPropertyChanged, INotifyDataErrorInfo {
        #region Properties
        
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public Visibility NewDemandVisibility {
            get {
                //if(CurrentUser.IsScmReferent) {
                //    return Visibility.Visible;
                //}

                return Visibility.Collapsed;
            }
            
        }
        #endregion

        #region Constructor
        public VmNomenclatureList(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {
            this.DataKey = DataKeyEnum.Nomenclature;
        }
        #endregion

        #region Interface Methods
        public IEnumerable GetErrors(string propertyName) {
            throw new NotImplementedException();
        }
        #endregion

        #region Abstract Methods Implementation
        //public override void RefreshGridData() {
        //    LoadNomenclatures();
        //    GridInit();
        //}

        //public async override void Import() {
        //    ProgressBarVisibility = System.Windows.Visibility.Visible;
        //    try {
        //        var task = ImportNomenclaturesAsync();
        //        await task;

        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    } finally {
        //        ProgressBarVisibility = System.Windows.Visibility.Collapsed;
        //    }
        //}

        //public async override void ExportToExcel() {
        //    try {
        //        //LoadNomenclaturesReport();

        //        var nomenclatures = GetNomenclaturesReportAsync();
        //        var nomenclaturesReport = await nomenclatures;

        //        string colNameNomenclature = ScmResource.Nomenclature;
        //        string colNameName = ScmResource.NameIt;
        //        string colNameMaterialGroup = ScmResource.MaterialGroup;


        //        DataTable nomenclaturesTable = new DataTable();
        //        DataColumn col = new DataColumn(colNameNomenclature, typeof(string));
        //        nomenclaturesTable.Columns.Add(col);
        //        col = new DataColumn(colNameName, typeof(string));
        //        nomenclaturesTable.Columns.Add(col);
        //        col = new DataColumn(colNameMaterialGroup, typeof(string));
        //        nomenclaturesTable.Columns.Add(col);

        //        foreach (var nomenclature in nomenclaturesReport) {
        //            DataRow newRow = nomenclaturesTable.NewRow();
        //            newRow[colNameNomenclature] = nomenclature.nomenclature_key;
        //            newRow[colNameName] = nomenclature.name;
        //            newRow[colNameMaterialGroup] = nomenclature.material_group_text;

        //            nomenclaturesTable.Rows.Add(newRow);
        //        }

        //        Excel excel = new Excel();
        //        string fileName = GetXlsFileName("Nomenclatures");
        //        using (var excelDoc = excel.GenerateExcelWorkbookDoc(nomenclaturesTable, new List<double> { 25, 70, 20, 50, 40, 40 })) {

        //            var pack = excelDoc.SaveAs(fileName);
        //            excelDoc.Close();
        //            pack.Close();

        //        }

        //        excel = null;
        //        Process.Start(fileName);
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    } finally {
        //        //IsBusy = false;
        //    }
        //}
        #endregion

        #region Async Methods
        //private Task<ObservableCollection<NomenclatureExtend>> GetNomenclaturesAsync() {
        //    return Task.Run(() => {
        //        IsBusy = true;
        //        try {
        //            int rowsCount;
        //            bool rowsCountSpecified;
        //            string strFilter = GetFilter();
        //            string strOrder = GetOrder();

        //            var wsNomenclatures = WsScm.GetNomenclatures(
        //                strFilter,
        //                strOrder,
        //                PageSize,
        //                true,
        //                PageNr,
        //                true,
        //                out rowsCount,
        //                out rowsCountSpecified);

        //            RowsCount = rowsCount;

        //            ObservableCollection<NomenclatureExtend> nomenclatures = new ObservableCollection<NomenclatureExtend>();
        //            foreach (var wsNomenclature in wsNomenclatures) {
        //                NomenclatureExtend nomenclature = new NomenclatureExtend();
        //                SetValues(wsNomenclature, nomenclature);

        //                nomenclatures.Add(nomenclature);
        //            }

        //            return nomenclatures;
        //        }
        //        catch (Exception ex) {
        //            throw ex;
        //        }
        //        finally {
        //            IsBusy = false;
        //        }
        //    });
        //}

        //private Task<List<NomenclatureExtend>> GetNomenclaturesReportAsync() {
        //    return Task.Run(() => {
        //        IsBusy = true;
        //        try {
        //            string strFilter = GetFilter();
        //            string strOrder = GetOrder();

        //            var wsNomenclatures = WsScm.GetNomenclaturesReport(
        //                strFilter,
        //                strOrder);

        //            List<NomenclatureExtend> nomenclatures = new List<NomenclatureExtend>();
        //            foreach (var wsNomenclature in wsNomenclatures) {
        //                NomenclatureExtend nomenclature = new NomenclatureExtend();
        //                SetValues(wsNomenclature, nomenclature);

        //                nomenclatures.Add(nomenclature);
        //            }

        //            return nomenclatures;
        //        } catch (Exception ex) {
        //            IsBusy = false;
        //            throw ex;
        //        } finally {
        //            IsBusy = false;
        //        }
        //    });
        //}

        //private async void LoadNomenclatures() {
        //    try {
        //        var nomenclatures = GetNomenclaturesAsync();
        //        NomenclatureList = await nomenclatures;
        //        DisplayingRows = ScmResource.DisplayingFromToOf
        //            .Replace("{0}", GetDisplayItemsFromInfo().ToString())
        //            .Replace("{1}", GetDisplayItemsToInfo().ToString())
        //            .Replace("{2}", RowsCount.ToString());
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //    }
        //}



        //private Task ImportNomenclaturesAsync() {
        //    return Task.Run(() => {
        //        try {

        //            ScmFileImport.DlgLoadFileInfo dlgLoadFileInfo = new ScmFileImport.DlgLoadFileInfo(LoadFileInfo);
        //            ScmFileImport.DlgLoadProgressInfo dlgLoadProgressInfo = new ScmFileImport.DlgLoadProgressInfo(LoadProgressInfo);
        //            new ScmFileImport().ImportData(
        //                ProdisFolder,
        //                CurrentUser.User.id,
        //                dlgLoadFileInfo,
        //                dlgLoadProgressInfo);

        //            ProgressBarVisibility = Visibility.Collapsed;
        //            ScmDispatcher.Invoke(() => {
        //                MessageBox.Show(ScmResource.ErrorMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        //            });
        //            ProgressBarVisibility = Visibility.Collapsed;

        //            RefreshGridData();
        //        } catch (Exception ex) {

        //                ScmDispatcher.Invoke(() => {
        //                    HandleError(ex);
        //                });

        //        } finally {
        //            ProgressBarVisibility = Visibility.Collapsed;
        //        }
        //    });
        //}

        public void RefreshNomenclatures() {
            LoadNomenclatures();
        }
        #endregion

        #region Override Methods
        protected override Task ImportNomenclaturesAsync() {
            return Task.Run(() => {
                try {

                    Prodis.DlgLoadFileInfo dlgLoadFileInfo = new Prodis.DlgLoadFileInfo(LoadFileInfo);
                    Prodis.DlgLoadProgressInfo dlgLoadProgressInfo = new Prodis.DlgLoadProgressInfo(LoadProgressInfo);
                    new ScmFileImport().ImportData(
                        ProdisFolder,
                        CurrentUser.User.id,
                        dlgLoadFileInfo,
                        dlgLoadProgressInfo,
                        ScmDispatcher);

                    ProgressBarVisibility = Visibility.Collapsed;
                    ScmDispatcher.Invoke(() => {
                        MessageBox.Show(ScmResource.ErrorMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                    ProgressBarVisibility = Visibility.Collapsed;

                    ScmDispatcher.Invoke(() => {
                        RefreshGridData();
                    });
                } catch (Exception ex) {

                    ScmDispatcher.Invoke(() => {
                        HandleError(ex);
                    });

                } finally {
                    ProgressBarVisibility = Visibility.Collapsed;
                }
            });
        }

        #endregion
    }
}
