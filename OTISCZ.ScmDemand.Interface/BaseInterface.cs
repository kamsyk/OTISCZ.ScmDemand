using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Interface {
    public class BaseInterface {
        protected void SetValues(object sourceObject, object targetObject) {
            SetValues(sourceObject, targetObject, null, false);
        }

        protected void SetValues(object sourceObject, object targetObject, List<string> properties) {
            SetValues(sourceObject, targetObject, properties, false);
        }

        protected void SetValues(object sourceObject, object targetObject, List<string> properties, bool isRecursive) {
            Type tSource = sourceObject.GetType();
            Type tTarget = targetObject.GetType();

            PropertyInfo[] sourceAttributes = tSource.GetProperties();
            PropertyInfo[] targetAttributes = tTarget.GetProperties();

            foreach (PropertyInfo sourceAttribute in sourceAttributes) {
                if (properties != null && !properties.Contains(sourceAttribute.Name)) {
                    continue;
                }

                PropertyInfo targetProperty = tTarget.GetProperty(sourceAttribute.Name);
                if (targetProperty == null) {
                    continue;
                }


                if (sourceAttribute.PropertyType.FullName.IndexOf("WsScmDemand") > -1 || sourceAttribute.PropertyType.FullName.IndexOf("ScmDemand.Model") > -1) {
                    if (isRecursive) {
                        SetValues(sourceAttribute.GetValue(sourceObject, null), targetProperty.GetValue(targetObject, null), null, true);
                    }
                    continue;
                }

                object oSourceValue = sourceAttribute.GetValue(sourceObject, null);
                targetProperty.SetValue(targetObject, oSourceValue, null);

                //created_dateSpecified = true
                //Neccessary for passing param throug webservice
                PropertyInfo targetPropertySpecified = tTarget.GetProperty(sourceAttribute.Name + "Specified");
                if (targetPropertySpecified != null) {
                    targetPropertySpecified.SetValue(targetObject, true, null);
                }
            }

        }
    }
}
