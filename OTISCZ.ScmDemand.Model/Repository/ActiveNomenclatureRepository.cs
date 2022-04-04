using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class ActiveNomenclatureRepository : BaseRepository<Active_Nomenclature> {

        #region Methods
        public void RemoveActiveNoms(int nomId) {
            List<Active_Nomenclature> anoms = (from adDb in m_dbContext.Active_Nomenclature
                                               where adDb.id == nomId
                                               select adDb).ToList();
            if (anoms != null && anoms.Count > 0) {
                foreach (var anom in anoms) {
                    m_dbContext.Active_Nomenclature.Remove(anom);
                }
            }
        }
                
        #endregion
    }
}
