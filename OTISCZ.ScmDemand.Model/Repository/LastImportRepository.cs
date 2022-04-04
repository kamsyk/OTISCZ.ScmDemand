using OTISCZ.ScmDemand.Model.DataDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class LastImportRepository : BaseRepository<Last_Import> {
        #region Methods
        public string GetLastImportDate() {
            var lastImportDate = (from lastImportDb in m_dbContext.Last_Import
                                  select lastImportDb).FirstOrDefault();

            if (lastImportDate != null) {
                return lastImportDate.last_nomenclature_import_date.ToString("dd.MM.yyyy HH:mm");
            }

            return null;
        }

        public void SetLastImportDate() {
            var lastImportDate = (from lastImportDb in m_dbContext.Last_Import
                                  select lastImportDb).FirstOrDefault();

            if (lastImportDate == null) {
                Last_Import lastImport = new Last_Import();
                lastImport.id = 0;
                lastImport.last_nomenclature_import_date = DateTime.Now;
                m_dbContext.Last_Import.Add(lastImport);
            } else {
                lastImportDate.last_nomenclature_import_date = DateTime.Now;
            }

            SaveChanges();
        }
        #endregion
    }
}
