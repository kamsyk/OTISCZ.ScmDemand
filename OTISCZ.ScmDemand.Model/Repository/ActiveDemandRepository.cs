using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class ActiveDemandRepository : BaseRepository<Active_Demand> {

        #region Methods
        public void RemoveActiveDemands(int demandId, ScmDemandEntities dbContext) {
            List<Active_Demand> ads = (from adDb in dbContext.Active_Demand
                                       where adDb.id == demandId
                                       select adDb).ToList();
            if (ads != null && ads.Count > 0) {
                foreach (var ad in ads) {
                    dbContext.Active_Demand.Remove(ad);
                }
            }
        }
        #endregion
    }
}
