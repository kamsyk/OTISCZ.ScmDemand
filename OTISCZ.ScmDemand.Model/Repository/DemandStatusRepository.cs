using OTISCZ.CommonDb;
using OTISCZ.ScmDemand.Model.DataDictionary;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class DemandStatusRepository : BaseRepository<Demand_Status> {
        #region Methods
        
        public List<Demand_Status> GetDemandStatusWs() {

            var demandStatus = (from demandStatusDb in m_dbContext.Demand_Status
                                select demandStatusDb).ToList();

            var retStatuses = new List<Demand_Status>();
            foreach (var demStat in demandStatus) {
                Demand_Status tmpDemStat = new Demand_Status();
                SetValues(demStat, tmpDemStat);

                retStatuses.Add(tmpDemStat);
            }

            return retStatuses;

        }

        
        #endregion
    }
}
