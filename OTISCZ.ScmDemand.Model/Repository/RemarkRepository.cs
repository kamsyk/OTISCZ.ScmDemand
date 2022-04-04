using OTISCZ.ScmDemand.Model.DataDictionary;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class RemarkRepository : BaseRepository<Remark> {
        #region Methods
        public void AddRemark(int demandId, int demandVersion, int authorId, string strRemark) {
            Remark remark = new Remark();

            remark.demand_id = demandId;
            remark.demand_version = demandVersion;
            remark.remark_text = strRemark;
            remark.modif_user_id = authorId;
            remark.modif_date = DateTime.Now;

            int lastId = GetLastId();
            int newId = ++lastId;
            remark.id = newId;

            m_dbContext.Remark.Add(remark);
            m_dbContext.SaveChanges();
        }

        public List<RemarkExtend> GetRemarks(int demandId) {
            var remarks = (from remarkDb in m_dbContext.Remark
                           where remarkDb.demand_id == demandId
                           orderby remarkDb.modif_date
                           select remarkDb).ToList();
            if (remarks == null || remarks.Count == 0) {
                return null;
            }

            List<RemarkExtend> remarksExtend = new List<RemarkExtend>();
            foreach (var remark in remarks) {
                RemarkExtend remarkExtend = new RemarkExtend();
                SetValues(remark, remarkExtend);
                remarkExtend.user_firstname = remark.ScmUser.first_name;
                remarkExtend.user_surname = remark.ScmUser.surname;

                remarksExtend.Add(remarkExtend);
            }


            return remarksExtend;
        }

        private int GetLastId() {

            var dbRemark = (from remarkDb in m_dbContext.Remark
                            orderby remarkDb.id descending
                            select remarkDb).FirstOrDefault();

            if (dbRemark == null) {
                return -1;
            }

            return dbRemark.id;
        }
        #endregion
    }
}
