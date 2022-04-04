using OTISCZ.ScmDemand.Interface;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.UI.Common;
using OTISCZ.ScmDemand.UI.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class VmImportProdis : VmBase, INotifyPropertyChanged, INotifyDataErrorInfo {
        #region Properties
        //public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private ScmSetting m_scmSetting = null;
        private ScmSetting ScmSetting {
            get {
                if (m_scmSetting == null) {
                    try {
                        m_scmSetting = WcfScm.GetScmSetting();

                        //var setting = WcfScm.GetScmSetting();
                        //m_scmSetting = new ScmSetting();
                        //SetValues(setting, m_scmSetting);
                    } catch (Exception ex) {
                        HandleError(ex);
                        
                    }
                }

                return m_scmSetting;
            }
        }

        public string ProdisFolder {
            get {
                return ScmSetting.prodis_input_folder;
            }
            set {
                m_scmSetting.prodis_input_folder = value;
                ValidateInputData(value);
                //ScmSetting.prodis_input_folder = value;
                OnPropertyChanged("ProdisFolder");
                try {
                    WcfScm.SetImportFolder(value);
                } catch (Exception ex) {
                    HandleError(ex);
                }
            }
        }

        public string LastImportDate {
            get {
                if (ScmSetting.last_import_date == null) {
                    return ScmResource.Never;
                }
                                
                return ((DateTime)ScmSetting.last_import_date).ToString("dd.mm.yyyy");
            }
            set {
                ValidateInputData(value);
                ScmSetting.prodis_input_folder = value;
                OnPropertyChanged("LastImportDate");
            }
        }

        public bool IsEditable {
            get { return CurrentUser.IsAdministrator; }
        }

        public Visibility BtnFolderVisibility {
            get {
                if (IsEditable) {
                    return Visibility.Visible;
                }

                return Visibility.Hidden;
            }
        }

        public Visibility ReadOnlyVisibility {
            get {
                if (!IsEditable) {
                    return Visibility.Visible;
                }

                return Visibility.Hidden;
            }
        }

        private string m_ImportInfo = null;
        public string ImportInfo {
            get { return m_ImportInfo; }
            set {
                m_ImportInfo = value;
                OnPropertyChanged("ImportInfo");
            }
        }

        private int m_PbLineNr = 0;
        public int PbLineNr {
            get { return m_PbLineNr; }
            set {
                m_PbLineNr = value;
                OnPropertyChanged("PbLineNr");
            }
        }

        private int m_PbLineCount = 0;
        public int PbLineCount {
            get { return m_PbLineCount; }
            set {
                m_PbLineCount = value;
                OnPropertyChanged("PbLineCount");
            }
        }

        private Visibility m_ProgressBarVisibility = Visibility.Collapsed;
        public Visibility ProgressBarVisibility {
            get { return m_ProgressBarVisibility; }
            set {
                m_ProgressBarVisibility = value;
                OnPropertyChanged("ProgressBarVisibility");
            }
        }
        #endregion

        #region Constructor
        public VmImportProdis(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {
            
        }
        #endregion

        #region Methods
        private bool IsSourceFolderValid(string value, out List<string> errMsg) {
            errMsg = null;

            if (String.IsNullOrEmpty(value)) {
                if(errMsg == null) {
                    errMsg = new List<string>();
                    errMsg.Add(ScmResource.SelectFolder);
                    return false;
                }
            }

            if (!Directory.Exists(value)) {
                if (errMsg == null) {
                    errMsg = new List<string>();
                    errMsg.Add(ScmResource.FolderDoesNotExist);
                    return false;
                }
            }

            return true;
        }

        public void ImportFiles() {
            Thread t = new Thread(ImportFilesThread);
            t.Start();
        }

        private void ImportFilesThread() {
            try {

                //ImportInfo = "Loading ...";
                Prodis.DlgLoadFileInfo dlgLoadFileInfo = new Prodis.DlgLoadFileInfo(LoadFileInfo);
                Prodis.DlgLoadProgressInfo dlgLoadProgressInfo = new Prodis.DlgLoadProgressInfo(LoadProgressInfo);
                var errorList = new ScmFileImport().ImportData(
                    ProdisFolder,
                    CurrentUser.User.id,
                    dlgLoadFileInfo, 
                    dlgLoadProgressInfo,
                    ScmDispatcher);
                //ScmDispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                //        new ThreadStart(delegate {
                //            // Your code
                //            //txtImportInfo.Visibility = Visibility.Visible;
                //            //btnInterupt.Visibility = Visibility.Collapsed;
                //            //m_mainWindow.SetStatusBarInfo("");
                //        }));
                ProgressBarVisibility = Visibility.Collapsed;
                ScmDispatcher.Invoke(() => {
                    if (errorList == null || errorList.Count == 0) {
                        MessageBox.Show(ScmResource.NomenclaturesLoadedSuccessfully, ScmResource.NomenclaturesLoadedSuccessfully, MessageBoxButton.OK, MessageBoxImage.Information);
                    } else {
                        MessageBox.Show(ScmResource.NomenclaturesLoadedFailed, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                });
            } catch (Exception ex) {
                if (ex is ThreadInterruptedException) {
                    ScmDispatcher.Invoke(() => {
                        MessageBox.Show(ScmResource.ErrorMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                } else {
                    ScmDispatcher.Invoke(() => {
                        MessageBox.Show(ScmResource.ErrorMsg, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            } finally {
                ProgressBarVisibility = Visibility.Collapsed;
            }
        }

        private void LoadFileInfo(string info, int lineCount) {
            if (ProgressBarVisibility != Visibility.Visible) {
                ProgressBarVisibility = Visibility.Visible;
            }
            ImportInfo = info;
            PbLineCount = lineCount;
        }

        private void LoadProgressInfo(int lineNr) {
            PbLineNr = lineNr;
        }

        #region Value Modified
        private void ValidateInputData(string value) {
            if (String.IsNullOrEmpty(value)) {
                HasErrors = true;
                throw new Exception(ScmResource.SelectFolder);
            }
            if (!Directory.Exists(value)) {
                HasErrors = true;
                throw new Exception(ScmResource.FolderDoesNotExist);
            }
            //HasErrors = !IsSourceFolderValid(value);
            HasErrors = false;
        }

        //protected void OnPropertyChanged(string propertyName) {
        //    var propertyChanged = PropertyChanged;
        //    if (propertyChanged != null) {
        //        propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
        #endregion

        #region Error Handling
        public IEnumerable GetErrors(string propertyName) {
            List<string> errMsg;
            HasErrors = !IsSourceFolderValid(ProdisFolder, out errMsg);
            if (!HasErrors) {
                return null;
            }

            return errMsg;
        }
        //public bool HasErrors { get; set; } = false;
        //public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public bool CheckItems() {
            List<string> errMsg;
            HasErrors = !IsSourceFolderValid(ProdisFolder, out errMsg);// !new LogInService().LogIn(Email, Pass);
            if (HasErrors) {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("ProdisFolder"));
            } else {
                return true;
            }
            return false;
        }
        #endregion

        #endregion
    }
}
