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

namespace OTISCZ.ScmDemand.Model.Repository {
    public class DemandNomenclatureRepository : BaseRepository<Demand_Nomenclature> {
        #region Methods
        public void SaveDemandNomenclature(DemandNomenclatureExtend updDemandNomenclature, int userId) {

            using (TransactionScope transaction = new TransactionScope()) {
                var dbDemandNom = (from demandNomDb in m_dbContext.Demand_Nomenclature
                                   where demandNomDb.demand_id == updDemandNomenclature.demand_id
                                   && demandNomDb.demand_version == updDemandNomenclature.demand_version
                                   && demandNomDb.nomenclature_id == updDemandNomenclature.nomenclature_id
                                   && demandNomDb.nomenclature_version == updDemandNomenclature.nomenclature_version
                                   orderby demandNomDb.id descending
                                   select demandNomDb).FirstOrDefault();

                if (String.IsNullOrWhiteSpace(updDemandNomenclature.price_text)) {
                    dbDemandNom.price = null;
                } else {
                    dbDemandNom.price = ConvertData.ToDecimal(updDemandNomenclature.price_text);
                }

                dbDemandNom.currency_id = updDemandNomenclature.currency_id;
                dbDemandNom.modif_user_id = userId;
                dbDemandNom.modif_date = DateTime.Now;

                SaveChanges();

                transaction.Complete();
            }


        }

        public static void LockNomenclature(int demId, int demVersion, int nomId, ScmDemandEntities dbContext) {
            var demandNomeclatures = (from demNomDb in dbContext.Demand_Nomenclature
                                      join demDb in dbContext.Demand
                                      on demNomDb.demand_id equals demDb.id
                                      where demNomDb.nomenclature_id == nomId
                                      && demNomDb.demand_version == demDb.version
                                      && demDb.last_version == true
                                      && demDb.id != demId
                                      select demNomDb).ToList();
            if (demandNomeclatures != null) {
                var demNomsOrder = (from demNomDb in demandNomeclatures
                                    orderby demNomDb.id descending
                                    select demNomDb).ToList();
                List<int> usedNomIds = new List<int>();
                foreach (var demandNomeclature in demNomsOrder) {
                    if (usedNomIds.Contains(demandNomeclature.nomenclature_id)) {
                        continue;
                    }

                    usedNomIds.Add(demandNomeclature.nomenclature_id);

                    demandNomeclature.locked_by_demand_id = demId;
                    demandNomeclature.locked_by_demand_version = demVersion;
                }
            }
        }

        public static void LockNomenclature(Demand demand, int nomId, ScmDemandEntities dbContext) {
            var demandNomeclatures = (from demNomDb in dbContext.Demand_Nomenclature
                                      join demDb in dbContext.Demand
                                      on demNomDb.demand_id equals demDb.id
                                      where demNomDb.nomenclature_id == nomId
                                      && demNomDb.demand_version == demDb.version
                                      && demDb.last_version == true
                                      && demDb.id != demand.id
                                      select demNomDb).ToList();
            if (demandNomeclatures != null) {
                var demNomsOrder = (from demNomDb in demandNomeclatures
                                    orderby demNomDb.id descending
                                    select demNomDb).ToList();
                List<int> usedNomIds = new List<int>();
                foreach (var demandNomeclature in demNomsOrder) {
                    if (usedNomIds.Contains(demandNomeclature.nomenclature_id)) {
                        continue;
                    }

                    usedNomIds.Add(demandNomeclature.nomenclature_id);

                    demand.DemandNomenclatureLock.Add(demandNomeclature);
                }
            }
        }

        public static void UnlockLockNomenclature(int nomId, ScmDemandEntities dbContext) {
            var demandNomeclatures = (from demNomDb in dbContext.Demand_Nomenclature
                                      where demNomDb.nomenclature_id == nomId
                                      select demNomDb).ToList();

            if (demandNomeclatures != null) {
                foreach (var demandNomeclature in demandNomeclatures) {
                    demandNomeclature.locked_by_demand_id = null;
                    demandNomeclature.locked_by_demand_version = null;
                }
            }
        }

        public void RemoveDemandNomenclature(int demandId, int nomenclatureId, int userId) {
            using (TransactionScope transaction = new TransactionScope()) {
                var demand = (from demandDb in m_dbContext.Demand
                              where demandDb.id == demandId
                              && demandDb.last_version == true
                              select demandDb).FirstOrDefault();

                int newDemandVersion = demand.version + 1;
                var newDemVersion = new DemandRepository().GetNewDemandVersion(demand, newDemandVersion, userId);

                //delete demNom
                var demNomRemove = (from demNomDb in newDemVersion.Demand_Nomenclature
                                    where demNomDb.nomenclature_id == nomenclatureId
                                    orderby demNomDb.id descending
                                    select demNomDb).FirstOrDefault();

                newDemVersion.Demand_Nomenclature.Remove(demNomRemove);

                int calcDemandStatus = CalculateDemandStatus(newDemVersion);
                newDemVersion.status_id = calcDemandStatus;

                //foreach (var nomId in nomIds) {
                //    var demNom = (from demNomDb in newDomVersion.Demand_Nomenclature
                //                  where demNomDb.nomenclature_id == nomId
                //                  select demNomDb).FirstOrDefault();
                //    int tmpStatus = (int)htNomStatuses[nomId];
                //    demNom.status_id = tmpStatus;

                //    UpdateNomenclatureStatus(m_dbContext, demNom, tmpStatus, userId);

                //}

                m_dbContext.Demand.Add(newDemVersion);

                //var demNomsCurrVersion = (from demNomDb in demand.Demand_Nomenclature
                //                          where demNomDb.demand_version == demand.version
                //                          orderby demNomDb.nomenclature_version descending
                //                          select demNomDb).ToList();

                //NomenclatureRepository nomenclatureRepository = new NomenclatureRepository();

                List<int> usedNoms = new List<int>();
                bool isRequestorActive = false;
                bool isAppManActive = false;
                foreach (var demNom in newDemVersion.Demand_Nomenclature) {
                    if (demNom.nomenclature_id == nomenclatureId) {
                        continue;
                    }

                    if (usedNoms.Contains(demNom.nomenclature_id)) {
                        continue;
                    }

                    usedNoms.Add(demNom.nomenclature_id);

                    isRequestorActive = isRequestorActive || NomenclatureRepository.IsRequestorActive(demNom.status_id);
                    isAppManActive = isAppManActive || NomenclatureRepository.IsAppManActive(demNom.status_id);

                    Demand_Nomenclature newDemNom = new Demand_Nomenclature();
                    SetValues(demNom, newDemNom);

                    newDemNom.modif_date = DateTime.Now;
                    newDemNom.modif_user_id = userId;

                    //newDomVersion.Demand_Nomenclature.Add(newDemNom);
                }

                //Update Nomenclature Status
                UpdateNomenclaureStatusAfterRemoval(demandId, nomenclatureId, userId);

                new NomenclatureRepository().UpdateActiveDemands(
                    m_dbContext,
                    demandId,
                    demand.version,
                    newDemVersion.status_id,
                    isRequestorActive,
                    isAppManActive);

                SaveChanges();

                transaction.Complete();
            }
        }

        private int CalculateDemandStatus(Demand newDemand) {
            if (newDemand.Demand_Nomenclature.Count == 0) {
                return DemandRepository.DEM_STATUS_CANCELED;
            }

            int calcStatus = -1;
            foreach (var demNom in newDemand.Demand_Nomenclature) {
                if (calcStatus < demNom.status_id) {
                    calcStatus = demNom.status_id;
                }
            }

            return calcStatus;
        }

        private void UpdateNomenclaureStatusAfterRemoval(int demandId, int nomenclatureId, int userId) {
            var dbNomenclature = (from nomDb in m_dbContext.Nomenclature
                                  where nomDb.id == nomenclatureId
                                  orderby nomDb.version descending
                                  select nomDb).FirstOrDefault();

            int nomStatus = -1;

            var otherDemNoms = (from demNomDb in dbNomenclature.Demand_Nomenclature
                                join demDb in m_dbContext.Demand
                                on demNomDb.demand_id equals demDb.id
                                where demNomDb.nomenclature_id == nomenclatureId
                                && demNomDb.demand_version == demDb.version
                                && demDb.last_version == true
                                && demDb.id != demandId
                                orderby demNomDb.id descending
                                select demNomDb).ToList();

            List<int> tmpIds = new List<int>();

            foreach (var demNom in otherDemNoms) {
                if (tmpIds.Contains(demNom.nomenclature_id)) {
                    continue;
                }

                if (nomStatus < demNom.status_id) {
                    nomStatus = demNom.status_id;
                }
            }

            if (dbNomenclature.status_id != nomStatus) {
                Nomenclature nomNewVersion = new NomenclatureRepository().GetNomenclatureNewVersion(dbNomenclature, userId);
                int newVersion = nomNewVersion.version;
                SetValues(dbNomenclature, nomNewVersion);
                nomNewVersion.version = newVersion;
                nomNewVersion.modif_user_id = userId;
                nomNewVersion.modif_date = DateTime.Now;
                nomNewVersion.status_id = (int)NomenclatureRepository.Status.SentToSupplier;

                m_dbContext.Nomenclature.Add(nomNewVersion);

                dbNomenclature.last_version = false;


            }
        }

        public int GetLastId() {

            var dbDemNom = (from demNomDb in m_dbContext.Demand_Nomenclature
                            orderby demNomDb.id descending
                            select demNomDb).FirstOrDefault();

            if (dbDemNom == null) {
                return -1;
            }

            return dbDemNom.id;
        }

        public List<DemandNomenclatureExtend> GetDemandNomHistory(int demandId) {
            var demanNomHistory = (from demanNomHistoryDb in m_dbContext.Demand_Nomenclature
                                   where demanNomHistoryDb.demand_id == demandId
                                   orderby demanNomHistoryDb.nomenclature_id, demanNomHistoryDb.modif_date
                                   select demanNomHistoryDb).ToList();
                        
            List<DemandNomenclatureExtend> demNomExds = new List<DemandNomenclatureExtend>();

            //Add nom import date - date when it was uploaded from Prodis
            Hashtable htNom = new Hashtable();
            foreach (var demNom in demanNomHistory) {
                if (!htNom.ContainsKey(demNom.nomenclature_id)) {
                    htNom.Add(demNom.nomenclature_id, null);
                    var nom = (from nomDb in m_dbContext.Nomenclature
                               where nomDb.id == demNom.nomenclature_id
                               orderby nomDb.version ascending
                               select nomDb).FirstOrDefault();

                    DemandNomenclatureExtend demNomExd = new DemandNomenclatureExtend();
                    SetValues(demNom, demNomExd);
                    demNomExd.status_id = NomenclatureRepository.NOM_STATUS_LOADED;
                    demNomExd.modif_date = nom.import_date;
                    demNomExd.modif_user_id = nom.modif_user_id;
                    var scmUser = (from scmUserDb in m_dbContext.ScmUser
                                   where scmUserDb.id == nom.modif_user_id
                                   select scmUserDb).FirstOrDefault(); ;
                    demNomExd.modif_user_name = scmUser.first_name + " " + scmUser.surname;
                    demNomExds.Add(demNomExd);
                }
            }
            
            //Load Dem nom history data
            int lastStatus = DataNulls.INT_NULL;
            int lastNomId = DataNulls.INT_NULL;
            foreach (var demNom in demanNomHistory) {
                if (lastStatus == demNom.status_id && lastNomId == demNom.nomenclature_id) {
                    continue;
                }
                DemandNomenclatureExtend demNomExd = new DemandNomenclatureExtend();
                SetValues(demNom, demNomExd);
                demNomExd.modif_user_name = demNom.ScmUser.first_name + " " + demNom.ScmUser.surname;
                demNomExds.Add(demNomExd);
                lastStatus = demNom.status_id;
                lastNomId = demNom.nomenclature_id;
            }

            return demNomExds;
        }

        public int GetDemandNomPrevStatus(int demandId, int nomId) {
            var demNoms = (from demNomDb in m_dbContext.Demand_Nomenclature
                       where demNomDb.demand_id == demandId
                       && demNomDb.nomenclature_id == nomId
                       orderby demNomDb.modif_date descending
                       select demNomDb).ToList();

            if (demNoms == null || demNoms.Count == 0) {
                return NomenclatureRepository.NOM_STATUS_UNKNOWN;
            }

            int currStatus = demNoms.ElementAt(0).status_id;

            foreach (var demNom in demNoms) {
                if (demNom.status_id != currStatus) {
                    return demNom.status_id;
                }
            }

            return NomenclatureRepository.NOM_STATUS_UNKNOWN;
        }
        #endregion
    }
}
