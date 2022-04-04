using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class BaseRepository<T> where T : class {
        #region Enums
        public enum FilterFromTo {
            No,
            From,
            To
        }
        #endregion

       
        #region Properties
        protected ScmDemandEntities m_dbContext = new ScmDemandEntities();

        protected DbSet<T> DbSet {
            get; set;
        }

        public static string UrlParamDelimiter {
            get { return "|"; }
        }
        public static string UrlParamValueDelimiter {
            get { return "~"; }
        }
        #endregion

        #region Constructor
        public BaseRepository() {
            DbSet = m_dbContext.Set<T>();
        }

        #endregion

        #region Methods
        public void SaveChanges() {
            m_dbContext.SaveChanges();
        }
                
        public T GetFirstOrDefault(Expression<Func<T, bool>> predicate) {
            return DbSet.FirstOrDefault(predicate);

        }
        
        public void Add(T entity) {
            DbSet.Add(entity);
        }
        
        protected void ErrorHandle(Exception ex) {
            var entityValidationErrors = ((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors;

            throw ex;
        }

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


                if (sourceAttribute.PropertyType.FullName.IndexOf("OTISCZ.ScmDemand.Model") > -1) {
                    if (isRecursive) {
                        SetValues(sourceAttribute.GetValue(sourceObject, null), targetProperty.GetValue(targetObject, null), null, true);
                    }
                    continue;
                }

                object oSourceValue = sourceAttribute.GetValue(sourceObject, null);
                targetProperty.SetValue(targetObject, oSourceValue, null);
            }

        }

        protected string GetSqlIn(List<int> items) {
            string sqlIn = "";

            foreach (int i in items) {
                if (sqlIn.Length > 0) {
                    sqlIn += ",";
                }

                sqlIn += i;
            }

            return "(" + sqlIn + ")";
        }


        #endregion
    }

    
}
