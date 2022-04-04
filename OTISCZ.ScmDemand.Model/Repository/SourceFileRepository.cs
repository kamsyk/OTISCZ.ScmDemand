using OTISCZ.ScmDemand.Model.DataDictionary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class SourceFileRepository : BaseRepository<Source_File> {
        #region Constants
        public const int STATUS_SUCCESS = 1;
        private const int STATUS_FAIL = 2;
        #endregion

        #region Methods
        public void SaveImportInfo(string fileName, DateTime lastModifDate, bool isError, int userId) {
            var sourceFile = (from sfDb in m_dbContext.Source_File
                              where sfDb.name == fileName 
                              select sfDb).FirstOrDefault();

            int iStatus = SourceFileRepository.STATUS_SUCCESS;
            if (isError) {
                iStatus = STATUS_FAIL;
            } else {
                iStatus = STATUS_SUCCESS;
            }

            if (sourceFile != null) {
                sourceFile.last_modification_date = lastModifDate;
                sourceFile.status = iStatus;
                sourceFile.import_date = DateTime.Now;
                sourceFile.import_user_id = userId;
            } else {
                int lastId = GetLastId();
                int newId = ++lastId;
                Source_File newSourceFile = new Source_File() {
                    id = newId,
                    status = iStatus,
                    import_user_id = userId,
                    import_date = DateTime.Now,
                    last_modification_date = lastModifDate,
                    name = fileName
                };
                m_dbContext.Source_File.Add(newSourceFile);
            }


            SaveChanges();
        }

        public Source_File GetSourceFile(string fileName, DateTime lastModifDate) {
            var sourceFile = (from sfDb in m_dbContext.Source_File
                              where sfDb.name == fileName
                              && sfDb.last_modification_date.Year == lastModifDate.Year
                              && sfDb.last_modification_date.Month == lastModifDate.Month
                              && sfDb.last_modification_date.Day == lastModifDate.Day
                              && sfDb.last_modification_date.Hour == lastModifDate.Hour
                              && sfDb.last_modification_date.Minute == lastModifDate.Minute
                              //&& sfDb.last_modification_date.Second == lastModifDate.Second
                              select sfDb).FirstOrDefault();

            return sourceFile;
        }

        private int GetLastId() {

            var dbSf = (from sourceFileDb in m_dbContext.Source_File
                              orderby sourceFileDb.id descending
                              select sourceFileDb).FirstOrDefault();

            if (dbSf == null) {
                return -1;
            }

            return dbSf.id;
        }
        #endregion
    }
}
