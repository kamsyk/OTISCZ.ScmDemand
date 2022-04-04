using OTISCZ.ScmDemand.Model.DataDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class MaterialGroupRepository : BaseRepository<Material_Group> {
        #region Methods
        public List<Material_Group> GetMaterialGroups() {
            var mgs = (from mgDb in m_dbContext.Material_Group
                       select mgDb).ToList();

            if (mgs == null) {
                return null;
            }

            List<Material_Group> retMgs = new List<Material_Group>();
            foreach (var mg in mgs) {
                Material_Group retMg = new Material_Group();
                SetValues(mg, retMg);
                retMgs.Add(retMg);
            }

            return retMgs;
        }

        public int GetMaterialGroupId(string materialGroupName) {
            var mg = (from mgDb in m_dbContext.Material_Group
                      where mgDb.name.Trim().ToUpper() == materialGroupName.Trim().ToUpper()
                      select mgDb).FirstOrDefault();

            if (mg != null) {
                return mg.id;
            }

            int lastId = GetLastId();
            int newId = ++lastId;

            Material_Group materialGroup = new Material_Group();
            materialGroup.id = newId;
            materialGroup.name = materialGroupName;

            m_dbContext.Material_Group.Add(materialGroup);

            SaveChanges();

            return newId;
        }

        private int GetLastId() {

            var dbMatGroup = (from matGroupDb in m_dbContext.Material_Group
                                  orderby matGroupDb.id descending
                                  select matGroupDb).FirstOrDefault();

            if (dbMatGroup == null) {
                return -1;
            }

            return dbMatGroup.id;
        }

       
        #endregion
    }
}
