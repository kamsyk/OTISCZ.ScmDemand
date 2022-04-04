using OTISCZ.ScmDemand.Model.DataDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class UnitRepository : BaseRepository<Unit> {
        #region Methods
        public List<Unit> GetUnits() {
            var units = (from unitDb in m_dbContext.Unit
                           select unitDb).ToList();

            if (units == null) {
                return null;
            }

            List<Unit> retUnits = new List<Unit>();
            foreach (var unit in units) {
                Unit retUnit = new Unit();
                SetValues(unit, retUnit);
                retUnits.Add(retUnit);
            }

            return retUnits;
        }
        #endregion
    }
}
