using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OTISCZ.ScmDemand.UI.ViewModel
{
    public class VmNomenclatureDetail : VmBase, INotifyPropertyChanged, INotifyDataErrorInfo {
        #region Events
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion

        #region Properties
        private NomenclatureDetailExtend m_Nomenclature = null;
        public NomenclatureDetailExtend Nomenclature {
            get {
                if (m_Nomenclature == null) {
                    LoadNomenclature();
                }
                return m_Nomenclature;
            }
            set {
                m_Nomenclature = value;
                OnPropertyChanged("Nomenclature");
            }
        }

        private int m_NomenclatureId = -1;
        public int NomenclatureId {
            get {
                return m_NomenclatureId;
            }
            set {
                m_NomenclatureId = value;
                OnPropertyChanged("NomenclatureId");
            }
        }

        private string m_StatusText = null;
        public string StatusText {
            get {
                //if (m_StatusText == null) {
                //    m_StatusText = Nomenclature.status_text;
                //}
                return m_StatusText;
            }
            set {
                m_StatusText = value;
                OnPropertyChanged("StatusText");
            }
        }
        #endregion

        #region Constructor
        public VmNomenclatureDetail(ScmUser scmUser, Dispatcher dispatcher) : base(scmUser, dispatcher) {
            
        }
        #endregion

        #region Interface Methods
        public IEnumerable GetErrors(string propertyName) {
            throw new NotImplementedException();
        }
        #endregion

        #region Async Methods
        private async void LoadNomenclature() {
            try {
                var nomenclature = GetNomenclatureAsync();
                Nomenclature = await nomenclature;

                StatusText = Nomenclature.status_text;
            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        private Task<NomenclatureDetailExtend> GetNomenclatureAsync() {
            return Task.Run(() => {
                //IsBusy = true;
                try {
                    
                    var wsNomenclature = WcfScm.GetNomenclatureById(m_NomenclatureId);
                                       
                    NomenclatureDetailExtend nomenclature = new NomenclatureDetailExtend();
                                            
                    SetValues(wsNomenclature, nomenclature);
                    nomenclature.status_text = GetNomenclatureStatusText(nomenclature.status_id);
                    nomenclature.evaluation_methods = new List<EvaluationMethod>();

                    //EvaluationMethod m = new EvaluationMethod();
                    //m.id = -1;
                    //m.name = "Select evaluation Methods";
                    //nomenclature.evaluation_methods.Add(m);

                    foreach (var wsEvalMethod in wsNomenclature.evaluation_methods) {
                        EvaluationMethod evaluationMethod = new EvaluationMethod();
                        SetValues(wsEvalMethod, evaluationMethod);
                        nomenclature.evaluation_methods.Add(evaluationMethod);
                    }

                    return nomenclature;
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    //IsBusy = false;
                }
            });
        }

        #endregion

        #region Methods
        public void SendPriceForApproval() {
            Nomenclature.status_id = (int)NomenclatureRepository.Status.WaitForApproval;
            //Nomenclature.status_text = ScmResource.StatusWaitForApproval;
            StatusText = ScmResource.StatusWaitForApproval;

#if RELEASE
            //WsScmDemandDebug.NomenclatureDetailExtend wsNom = new WsScmDemandDebug.NomenclatureDetailExtend();
#else
#endif

            //SetValues(Nomenclature, wsNom);
            WcfScm.SaveNomenclature(Nomenclature, CurrentUser.User.id);
            //NomenclatureDetailExtend tmpNom = Nomenclature;
            //Nomenclature = null;
            //Nomenclature = tmpNom;
        }
#endregion
    }
}
