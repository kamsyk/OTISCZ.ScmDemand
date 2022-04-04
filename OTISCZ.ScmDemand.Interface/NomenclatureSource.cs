using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Interface {
    public class NomenclatureSource : BaseInterface {
        #region Properties
#if RELEASE
        private WsScmDemand.ScmDemand m_WsScm = null;

        protected WsScmDemand.ScmDemand WsScm {
            get {
                if (m_WsScm == null) {
                    m_WsScm = new WsScmDemand.ScmDemand();
                    m_WsScm.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }

                return m_WsScm;
            }
        }
#else
        private WsScmDemandDebug.ScmDemand m_WsScm = null;

        protected WsScmDemandDebug.ScmDemand WsScm {
            get {
                if (m_WsScm == null) {
                    m_WsScm = new WsScmDemandDebug.ScmDemand();

                }

                return m_WsScm;
            }
        }
#endif

#if RELEASE
        private WsScmDemand.ScmSetting m_scmSetting = null;

        protected WsScmDemand.ScmSetting ScmSetting {
            get {
                if (m_scmSetting == null) {
                    try {
                        m_scmSetting = WsScm.GetScmSetting();
                        
                    } catch (Exception ex) {
                        throw (ex);

                    }
                }

                return m_scmSetting;
            }
        }
#else
        private WsScmDemandDebug.ScmSetting m_scmSetting = null;

        protected WsScmDemandDebug.ScmSetting ScmSetting {
            get {
                if (m_scmSetting == null) {
                    try {
                        m_scmSetting = WsScm.GetScmSetting();

                    } catch (Exception ex) {
                        throw (ex);

                    }
                }

                return m_scmSetting;
            }
        }
#endif
        #endregion

        #region Methods
        protected int GetMaterialGroupId(string strMaterialGroupName, Hashtable htMg) {

            if (htMg.ContainsKey(strMaterialGroupName)) {
                return (int)htMg[strMaterialGroupName];
            } else {
                int mgId;
                bool isSpec;
                WsScm.GetMaterialGroupId(strMaterialGroupName, out mgId, out isSpec);
                htMg.Add(strMaterialGroupName, mgId);
                return mgId;
            }
        }

        //protected void SetValues(object sourceObject, object targetObject) {
        //    SetValues(sourceObject, targetObject, null, false);
        //}

        //protected void SetValues(object sourceObject, object targetObject, List<string> properties) {
        //    SetValues(sourceObject, targetObject, properties, false);
        //}

        //protected void SetValues(object sourceObject, object targetObject, List<string> properties, bool isRecursive) {
        //    Type tSource = sourceObject.GetType();
        //    Type tTarget = targetObject.GetType();

        //    PropertyInfo[] sourceAttributes = tSource.GetProperties();
        //    PropertyInfo[] targetAttributes = tTarget.GetProperties();

        //    foreach (PropertyInfo sourceAttribute in sourceAttributes) {
        //        if (properties != null && !properties.Contains(sourceAttribute.Name)) {
        //            continue;
        //        }

        //        PropertyInfo targetProperty = tTarget.GetProperty(sourceAttribute.Name);
        //        if (targetProperty == null) {
        //            continue;
        //        }


        //        if (sourceAttribute.PropertyType.FullName.IndexOf("WsScmDemand") > -1 || sourceAttribute.PropertyType.FullName.IndexOf("ScmDemand.Model") > -1) {
        //            if (isRecursive) {
        //                SetValues(sourceAttribute.GetValue(sourceObject, null), targetProperty.GetValue(targetObject, null), null, true);
        //            }
        //            continue;
        //        }

        //        object oSourceValue = sourceAttribute.GetValue(sourceObject, null);
        //        targetProperty.SetValue(targetObject, oSourceValue, null);

        //        //created_dateSpecified = true
        //        //Neccessary for passing param throug webservice
        //        PropertyInfo targetPropertySpecified = tTarget.GetProperty(sourceAttribute.Name + "Specified");
        //        if (targetPropertySpecified != null) {
        //            targetPropertySpecified.SetValue(targetObject, true, null);
        //        }
        //    }

        //}
        #endregion
    }
}
