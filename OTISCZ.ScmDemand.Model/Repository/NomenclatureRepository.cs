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
    public class NomenclatureRepository : BaseRepository<Nomenclature> {
        #region Constants
        public const int NOM_STATUS_CANCELED = DemandRepository.DEM_STATUS_CANCELED;
        public const int NOM_STATUS_LOADED = 100;
        public const int NOM_STATUS_WO_DEMAND = DemandRepository.NOM_STATUS_WO_DEMAND;
        public const int NOM_STATUS_SENT_TO_SUPPLIER = DemandRepository.DEM_STATUS_SENT;
        public const int NOM_STATUS_SUPPLIER_REPLIED = DemandRepository.DEM_STATUS_SUPPLIER_REPLIED;
        public const int NOM_STATUS_WAIT_FOR_APPROVAL = DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL;
        public const int NOM_STATUS_APPROVED = DemandRepository.DEM_STATUS_APPROVED;
        public const int NOM_STATUS_REJECTED = DemandRepository.DEM_STATUS_REJECTED;
        public const int NOM_STATUS_PRICE_SET = DemandRepository.DEM_STATUS_PRICE_SET;
        public const int NOM_STATUS_PRICE_CONFIRMED = DemandRepository.DEM_STATUS_PRICE_CONFIRMED;
        public const int NOM_STATUS_CLOSED = DemandRepository.DEM_STATUS_CLOSED;
        public const int NOM_STATUS_UNKNOWN = DemandRepository.DEM_STATUS_UNKNOWN;
        #endregion

        #region Enum
        public enum Status {
            Loaded = NOM_STATUS_LOADED,
            WithoutDemand = NOM_STATUS_WO_DEMAND,
            SentToSupplier = NOM_STATUS_SENT_TO_SUPPLIER,
            SupplierReplied = NOM_STATUS_SUPPLIER_REPLIED,
            WaitForApproval = NOM_STATUS_WAIT_FOR_APPROVAL,
            Approved = NOM_STATUS_APPROVED,
            Rejected = NOM_STATUS_REJECTED,
            PriceSet = NOM_STATUS_PRICE_SET,
            PriceConfirmed = NOM_STATUS_PRICE_CONFIRMED

        }

        public enum NomSource {
            Prodis = 0,
            DNSSharePoint = 1,
            Both = 2,
            Custom = 3
        }
        #endregion

        #region Struct
        public struct NomStatus {
            public int NomId;
            public int StatusId;
            public decimal? Price;
            public int? CurrencyId;

            public NomStatus(int nomId, int statusId, decimal? price, int? currencyId) {
                NomId = nomId;
                StatusId = statusId;
                Price = price;
                CurrencyId = currencyId;
            }
        }
        #endregion

        #region Methods
        public int ImportNomenclature(Nomenclature lineNomenclature, int userId) {
            int nomId = -1;
            var dbNomenclature = (from nomenclatureDb in m_dbContext.Nomenclature
                                  where nomenclatureDb.nomenclature_key == lineNomenclature.nomenclature_key
                                  && nomenclatureDb.source_id != (int)NomSource.Custom
                                  orderby nomenclatureDb.version descending
                                  select nomenclatureDb).FirstOrDefault();


            if (dbNomenclature == null || dbNomenclature.source_id == (int)NomSource.Custom) {
                int lastId = GetLastId();
                int newId = ++lastId;
                nomId = newId;
                lineNomenclature.id = newId;
                lineNomenclature.version = 0;

                if (!String.IsNullOrWhiteSpace(lineNomenclature.name)) {
                    lineNomenclature.name_wo_dia = ConvertData.RemoveDiacritics(lineNomenclature.name);
                }

                if (!String.IsNullOrWhiteSpace(lineNomenclature.specification)) {
                    lineNomenclature.specification_wo_dia = ConvertData.RemoveDiacritics(lineNomenclature.specification);
                }

                lineNomenclature.status_id = NOM_STATUS_LOADED;
                lineNomenclature.import_date = DateTime.Now;
                lineNomenclature.last_status_modif_date = DateTime.Now;


                lineNomenclature.modif_date = DateTime.Now;
                lineNomenclature.modif_user_id = userId;
                lineNomenclature.last_version = true;
                lineNomenclature.is_active = true;

                m_dbContext.Nomenclature.Add(lineNomenclature);

                Active_Nomenclature activeNomenclature = new Active_Nomenclature();
                activeNomenclature.id = lineNomenclature.id;
                activeNomenclature.version = lineNomenclature.version;
                m_dbContext.Active_Nomenclature.Add(activeNomenclature);

                m_dbContext.SaveChanges();
            } else {
                nomId = dbNomenclature.id;
                if (dbNomenclature.source_id != (int)NomSource.Both && dbNomenclature.source_id != lineNomenclature.source_id) {
                    
                    var newNomVersion = GetNomenclatureNewVersion(dbNomenclature, userId, true);
                    newNomVersion.source_id = (int)NomSource.Both;
                    newNomVersion.last_version = true;
                    m_dbContext.Nomenclature.Add(newNomVersion);
                                        
                    dbNomenclature.last_version = false;

                    m_dbContext.SaveChanges();
                }
 
            }

            return nomId;
        }

        private int GetLastId() {

            var dbNomenclature = (from nomenclatureDb in m_dbContext.Nomenclature
                                  orderby nomenclatureDb.id descending
                                  select nomenclatureDb).FirstOrDefault();

            if (dbNomenclature == null) {
                return -1;
            }

            return dbNomenclature.id;
        }

        public List<NomenclatureExtend> GetNomenclatures(
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount) {

            bool isActiveOnly = false;
            string strFilterWhere = GetFilter(filter, out isActiveOnly);
            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT nd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            string sqlPureBody = GetPureBody(strFilterWhere, isActiveOnly);

            //Get Row count
            string selectCount = "SELECT COUNT(*) " + sqlPureBody;
            rowsCount = m_dbContext.Database.SqlQuery<int>(selectCount).Single();

            //Get Part Data
            string sqlPart = sqlPure + sqlPureBody;
            int partStart = pageSize * (pageNr - 1) + 1;
            int partStop = partStart + pageSize - 1;

            while (partStart > rowsCount) {
                partStart -= pageSize;
                partStart = partStart + pageSize - 1;
            }

            string sql = "SELECT * FROM(" + sqlPart + ") AS RegetPartData" +
                " WHERE RegetPartData.RowNum BETWEEN " + partStart + " AND " + partStop;

            var nomenclatures = m_dbContext.Database.SqlQuery<Nomenclature>(sql).ToList();

            return GetNomenclatures(nomenclatures, pageNr, pageSize);

        }

        public List<NomenclatureExtend> GetNomenclaturesReport(
            string filter,
            string sort) {
            bool isActiveOnly = false;
            string strFilterWhere = GetFilter(filter, out isActiveOnly);

            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT nd.* ";
            string sqlPureBody = GetPureBody(strFilterWhere, isActiveOnly);
            sqlPureBody += strOrder;

            string sql = sqlPure + sqlPureBody;

            var nomenclatures = m_dbContext.Database.SqlQuery<Nomenclature>(sql).ToList();

            return GetNomenclatures(nomenclatures);
        }

        private List<NomenclatureExtend> GetNomenclatures(List<Nomenclature> nomenclatures) {
            return GetNomenclatures(nomenclatures, 1, 1);
        }

        private List<NomenclatureExtend> GetNomenclatures(
            List<Nomenclature> nomenclatures,
            int pageNr,
            int pageSize) {

            List<NomenclatureExtend> nomenclaturesExtended = new List<NomenclatureExtend>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var nomenclature in nomenclatures) {
                Nomenclature dbNomenclature = (from dbNomBd in m_dbContext.Nomenclature
                                               where dbNomBd.id == nomenclature.id
                                               && dbNomBd.last_version == true
                                               select dbNomBd).FirstOrDefault();

                NomenclatureExtend nomenclatureExtended = new NomenclatureExtend();
                SetValues(nomenclature, nomenclatureExtended);
                nomenclatureExtended.row_index = rowIndex++;
                SetNomenclaturesExtProp(dbNomenclature, nomenclatureExtended);

                nomenclaturesExtended.Add(nomenclatureExtended);
            }

            return nomenclaturesExtended;
        }

        public NomenclatureDetailExtend GetNomenclatureById(int nomId) {

            Nomenclature dbNomenclature = (from dbNomBd in m_dbContext.Nomenclature
                                           where dbNomBd.id == nomId
                                           && dbNomBd.last_version == true
                                           select dbNomBd).FirstOrDefault();

            NomenclatureDetailExtend nomenclatureExtended = new NomenclatureDetailExtend();
            SetValues(dbNomenclature, nomenclatureExtended);
            SetNomenclaturesExtProp(dbNomenclature, nomenclatureExtended);

            

            var evaluationMethods = (from dbEval in m_dbContext.EvaluationMethod
                                     select dbEval).ToList();
            nomenclatureExtended.evaluation_methods = new List<EvaluationMethod>();
            foreach (var evaluationMethod in evaluationMethods) {
                EvaluationMethod evaluationMethodEx = new EvaluationMethod();
                SetValues(evaluationMethod, evaluationMethodEx);
                nomenclatureExtended.evaluation_methods.Add(evaluationMethodEx);
            }

            return nomenclatureExtended;
        }



        public void SaveNomenclature(NomenclatureExtend updNomenclature, int userId, bool isUpdateDemNoms, out int newVersion) {
            using (TransactionScope transaction = new TransactionScope()) {
                Nomenclature dbNomenclature = (from dbNomBd in m_dbContext.Nomenclature
                                               where dbNomBd.id == updNomenclature.id
                                               && dbNomBd.last_version == true
                                               select dbNomBd).FirstOrDefault();

                int origVersion = dbNomenclature.version;
                newVersion = ++origVersion;

                Nomenclature newNomeclature = new Nomenclature();
                SetValues(updNomenclature, newNomeclature);
                newNomeclature.version = newVersion;

                if (dbNomenclature.status_id != updNomenclature.status_id) {
                    newNomeclature.last_status_modif_date = DateTime.Now;
                }

                newNomeclature.last_version = true;
                newNomeclature.modif_user_id = userId;
                newNomeclature.modif_date = DateTime.Now;

                m_dbContext.Nomenclature.Add(newNomeclature);

                dbNomenclature.last_version = false;

                if (isUpdateDemNoms) {
                    var demNoms = (from demNomsDb in m_dbContext.Demand_Nomenclature
                                   where demNomsDb.nomenclature_id == updNomenclature.id
                                   select demNomsDb).ToList();

                    if (demNoms != null && demNoms.Count > 0) {
                        foreach (var demNom in demNoms) {
                            if (demNom.Demand.last_version) {
                                //demNom.nomenclature_version = newVersion;
                                Demand_Nomenclature newDemNom = new Demand_Nomenclature();
                                newDemNom.demand_id = demNom.demand_id;
                                newDemNom.demand_version = demNom.demand_version;
                                newDemNom.nomenclature_id = demNom.nomenclature_id;
                                newDemNom.nomenclature_version = newVersion;
                                m_dbContext.Demand_Nomenclature.Add(newDemNom);
                                m_dbContext.Demand_Nomenclature.Remove(demNom);
                            }

                        }
                    }
                }

                if (newNomeclature.status_id == (int)Status.WithoutDemand) {
                    var activeNoms = (from actNomDb in m_dbContext.Active_Nomenclature
                                      where actNomDb.id == newNomeclature.id
                                      select actNomDb).ToList();

                    if (activeNoms != null && activeNoms.Count > 0) {
                        foreach (var activeNom in activeNoms) {
                            m_dbContext.Active_Nomenclature.Remove(activeNom);
                        }
                    }
                }

                SaveChanges();

                transaction.Complete();
            }
        }

                

         public void SaveNomenclaturesStatus(
            int demandId,
            List<NomStatus> nomStatuses,
            int userId) {
            
            Hashtable htNomStatuses = new Hashtable();
            for (int i = 0; i < nomStatuses.Count; i++) {
                htNomStatuses.Add(nomStatuses[i].NomId, nomStatuses[i].StatusId);
            }

            using (TransactionScope transaction = new TransactionScope()) {
                DemandRepository demandRepository = new DemandRepository();

                int calcDemandStatus = -1;
                //var demand = demandRepository.GetDemandById(demandId);
                var demand = (from demandDb in m_dbContext.Demand
                              where demandDb.id == demandId
                              && demandDb.last_version == true
                              select demandDb).FirstOrDefault();

                var demNomsCurrVersion = (from demNomDb in demand.Demand_Nomenclature
                                          where demNomDb.demand_version == demand.version
                                          orderby demNomDb.id descending
                                          select demNomDb).ToList();

                List<int> usedNoms = new List<int>();
                bool isRequestorActive = false;
                bool isAppManActive = false;
                foreach (var demNom in demNomsCurrVersion) {

                    if (usedNoms.Contains(demNom.nomenclature_id)) {
                        continue;
                    }

                    usedNoms.Add(demNom.nomenclature_id);

                    int demNomStatus = demNom.status_id;
                    if (htNomStatuses.Contains(demNom.nomenclature_id)) {
                        demNomStatus = (int)htNomStatuses[demNom.nomenclature_id];
                    }

                    if (calcDemandStatus == -1) {
                        calcDemandStatus = demNomStatus;
                    }

                    isRequestorActive = isRequestorActive || IsRequestorActive(demNomStatus);
                    isAppManActive = isAppManActive || IsAppManActive(demNomStatus);

                    calcDemandStatus = GetDemandStatus(calcDemandStatus, demNomStatus);
                                        
                }

                int lastDemNomId = new DemandNomenclatureRepository().GetLastId();
                int newDemNomId = ++lastDemNomId;

                if (calcDemandStatus == demand.status_id) {
                    //update demand nomenclatures
                    foreach (var nomStatus in nomStatuses) {
                        
                        var demNom = (from demNomDb in demand.Demand_Nomenclature
                                      where demNomDb.nomenclature_id == nomStatus.NomId
                                      select demNomDb).FirstOrDefault();

                        if (demNom == null) {
                            continue;
                        }

                        int origDemNomStatus = demNom.status_id;

                        Demand_Nomenclature newDemNom = new Demand_Nomenclature();
                        SetValues(demNom, newDemNom);

                        int tmpStatus = (int)htNomStatuses[nomStatus.NomId];
                        newDemNom.id = newDemNomId;
                        newDemNomId++;
                        newDemNom.status_id = tmpStatus;
                        newDemNom.modif_date = DateTime.Now;
                        newDemNom.modif_user_id = userId;
                                                                      

                        UpdateNomenclatureStatus(m_dbContext, newDemNom, tmpStatus, userId, nomStatus.Price, nomStatus.CurrencyId);

                        //Lock Unlock
                        LockUnlockDemNom(
                                    newDemNom.demand_id,
                                    newDemNom.demand_version,
                                    newDemNom.nomenclature_id,
                                    origDemNomStatus,
                                    tmpStatus,
                                    m_dbContext);

                        m_dbContext.Demand_Nomenclature.Add(newDemNom);
                    }

                    demand.last_status_modif_date = DateTime.Now;
                } else {
                                        
                    int newDemandVersion = demand.version + 1;
                    var newDemVersion = demandRepository.GetNewDemandVersion(demand, newDemandVersion, userId);
                    newDemVersion.status_id = calcDemandStatus;
                    newDemVersion.modif_date = DateTime.Now;
                    newDemVersion.modif_user_id = userId;
                    newDemVersion.last_status_modif_date = DateTime.Now;

                    m_dbContext.Demand.Add(newDemVersion);

                    foreach (var nomStatus in nomStatuses) {
                        var demNom = (from demNomDb in newDemVersion.Demand_Nomenclature
                                      where demNomDb.nomenclature_id == nomStatus.NomId
                                      select demNomDb).FirstOrDefault();
                        int origDemNomStatus = demNom.status_id;

                        int tmpStatus = (int)htNomStatuses[nomStatus.NomId];
                        demNom.status_id = tmpStatus;

                        //m_dbContext.Demand.Add(newDemVersion);

                        UpdateNomenclatureStatus(m_dbContext, demNom, tmpStatus, userId, nomStatus.Price, nomStatus.CurrencyId);
                                               
                        //Lock Unlock
                        LockUnlockDemNom(
                                    //demNom.demand_id,
                                    // demand.version,//demNom.demand_version,
                                    newDemVersion,
                                    demNom.nomenclature_id,
                                    origDemNomStatus,
                                    tmpStatus,
                                    m_dbContext);
                       
                    }

                    //m_dbContext.Demand.Add(newDemVersion);
                    //SaveChanges();

                    if (calcDemandStatus == DemandRepository.DEM_STATUS_PRICE_SET || calcDemandStatus == DemandRepository.DEM_STATUS_PRICE_CONFIRMED) {
                        //delete from active nom
                        var actDem = (from actDemDB in m_dbContext.Active_Demand
                                      where actDemDB.id == demandId
                                      select actDemDB).FirstOrDefault();

                        if (actDem != null) {
                            m_dbContext.Active_Demand.Remove(actDem);
                        }
                    }

                    //m_dbContext.Demand.Add(newDemVersion);
                    
                }

                

                UpdateActiveDemands(
                    m_dbContext, 
                    demandId, 
                    demand.version,
                    calcDemandStatus, 
                    isRequestorActive, 
                    isAppManActive);

                SaveChanges();
                transaction.Complete();
            }
        }

        

        public static bool IsRequestorActive(int nomStat) {
            if (nomStat == NomenclatureRepository.NOM_STATUS_SENT_TO_SUPPLIER
                || nomStat == NomenclatureRepository.NOM_STATUS_SUPPLIER_REPLIED
                || nomStat == NomenclatureRepository.NOM_STATUS_APPROVED //waiting for PRODIS comfirmation
                || nomStat == NomenclatureRepository.NOM_STATUS_REJECTED) {
                return true;
            }

            return false;
        }

        public static bool IsAppManActive(int nomStat) {
            if (nomStat == NomenclatureRepository.NOM_STATUS_WAIT_FOR_APPROVAL) {
                return true;
            }

            return false;
        }

        public void UpdateActiveDemands(
            ScmDemandEntities dbContext, 
            int demandId, 
            int demandVersion, 
            int demandStatus, 
            bool isRequestorActive, 
            bool isAppManActive) {

            var activeDemand = (from actDemDb in dbContext.Active_Demand
                          where actDemDb.id == demandId
                          select actDemDb).FirstOrDefault();

            if (!isRequestorActive && !isAppManActive && activeDemand != null) {
                dbContext.Active_Demand.Remove(activeDemand);
            }

            if (isRequestorActive || isAppManActive) {
                if (activeDemand == null) {
                    activeDemand = new Active_Demand();
                    activeDemand.id = demandId;
                    activeDemand.version = demandVersion;
                    activeDemand.status_id = demandStatus;
                    activeDemand.is_requestor_acive = isRequestorActive;
                    activeDemand.is_app_man_active = isAppManActive;

                    dbContext.Active_Demand.Add(activeDemand);
                } else {
                    activeDemand.is_requestor_acive = isRequestorActive;
                    activeDemand.is_app_man_active = isAppManActive;
                }
            }
        }

        public void UpdateNomenclatureStatus(
           ScmDemandEntities dbContext,
           Demand_Nomenclature demNom,
           int statusId,
           int userId) {
            UpdateNomenclatureStatus(dbContext, demNom, statusId, userId, null, null);
        }

        public void UpdateNomenclatureStatus(
            ScmDemandEntities dbContext, 
            Demand_Nomenclature demNom, 
            int statusId, 
            int userId,
            decimal? price,
            int? currencyId) {

            var nom = (from nomDb in dbContext.Nomenclature
                       where nomDb.id == demNom.nomenclature_id
                       && nomDb.last_version == true
                       select nomDb).FirstOrDefault();

            int nomStatus = GetNomenclatureStatus(nom.id, demNom.demand_id, statusId);
            if (nom.status_id != nomStatus) {
                var newNomVersion = GetNomenclatureNewVersion(nom, userId);
                int newVersion = newNomVersion.version;
                SetValues(nom, newNomVersion);
                newNomVersion.version = newVersion;
                newNomVersion.status_id = nomStatus;

                if (price != null) {
                    newNomVersion.price = price;
                }

                if (currencyId != null) {
                    newNomVersion.currency_id = currencyId;
                }

                newNomVersion.modif_user_id = userId;
                newNomVersion.modif_date = DateTime.Now;
                newNomVersion.last_version = true;
                dbContext.Nomenclature.Add(newNomVersion);

                nom.last_version = false;

                demNom.nomenclature_version = newVersion;

                //if (demNom.status_id == NomenclatureRepository.NOM_STATUS_APPROVED) {
                //    DemandNomenclatureRepository.LockNomenclature(
                //        demNom.demand_id,
                //        demNom.demand_version,
                //        demNom.nomenclature_id, 
                //        m_dbContext);
                //}

                
            }

            //LockUnlockDemNom(
            //    demNom.demand_id,
            //    demNom.demand_version,
            //    demNom.nomenclature_id,
            //    demNom.status_id, 
            //    statusId,
            //    dbContext);

            dbContext.SaveChanges();


            if (statusId == NOM_STATUS_PRICE_SET || statusId == NOM_STATUS_PRICE_CONFIRMED) {
                //delete from active nom
                var actNom = (from actNomDB in dbContext.Active_Nomenclature
                              where actNomDB.id == nom.id
                              select actNomDB).FirstOrDefault();

                if (actNom != null) {
                    dbContext.Active_Nomenclature.Remove(actNom);
                }
            }
        }

        private void LockUnlockDemNom(
            int demandId,
            int demandVersion,
            int nomenclatureId,
            int origDemNomStatus,
            int currDemNomStatus,
            ScmDemandEntities dbContext) {

            LockUnlockDemNom(
                demandId,
                demandVersion,
                null,
                nomenclatureId,
                origDemNomStatus,
                currDemNomStatus,
                dbContext);
        }

        private void LockUnlockDemNom(
            Demand demand,
            int nomenclatureId,
            int origDemNomStatus,
            int currDemNomStatus,
            ScmDemandEntities dbContext) {

            //if new version of Demand is created this one must be called

            LockUnlockDemNom(
                -1,
                -1,
                demand,
                nomenclatureId,
                origDemNomStatus,
                currDemNomStatus,
                dbContext);
        }

        private void LockUnlockDemNom(
            int demandId,
            int demandVersion,
            Demand demand,
            int nomenclatureId,
            int origDemNomStatus, 
            int currDemNomStatus,
            ScmDemandEntities dbContext) {

            if (currDemNomStatus == DemandRepository.DEM_STATUS_APPROVED 
                || currDemNomStatus == DemandRepository.DEM_STATUS_PRICE_SET) {
                //|| currDemNomStatus == DemandRepository.DEM_STATUS_PRICE_CONFIRMED) {
                if (demand != null) {
                    DemandNomenclatureRepository.LockNomenclature(
                        demand,
                        nomenclatureId,
                        dbContext);
                } else {
                    DemandNomenclatureRepository.LockNomenclature(
                        demandId,
                        demandVersion,
                        nomenclatureId,
                        dbContext);
                }
            }

            if (currDemNomStatus == DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL && origDemNomStatus == DemandRepository.DEM_STATUS_APPROVED) {
                DemandNomenclatureRepository.UnlockLockNomenclature(nomenclatureId, dbContext);
            }
        }

        private int GetDemandStatus(int origNomStatus, int currNomStatus) {
            switch (currNomStatus) {
                case NOM_STATUS_LOADED:
                    //no impact to Demand Status
                    return origNomStatus;
                case NOM_STATUS_REJECTED:
                    return DemandRepository.DEM_STATUS_REJECTED;
                //case NOM_STATUS_APPROVED:
                //    switch (origNomStatus) {
                //        case DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL:
                //            return DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL;
                //    }
                //    break;
                case NOM_STATUS_SENT_TO_SUPPLIER:
                    return DemandRepository.DEM_STATUS_SENT;
                case NOM_STATUS_SUPPLIER_REPLIED:
                    switch (origNomStatus) {
                        case DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL:
                            return DemandRepository.DEM_STATUS_SUPPLIER_REPLIED;
                        case DemandRepository.DEM_STATUS_APPROVED:
                            return DemandRepository.DEM_STATUS_SUPPLIER_REPLIED; 
                    }
                    break;
                case NOM_STATUS_WAIT_FOR_APPROVAL:
                    switch (origNomStatus) {
                        case DemandRepository.DEM_STATUS_APPROVED:
                            return DemandRepository.DEM_STATUS_WAIT_FOR_APPROVAL;
                    }
                    break;
                case NOM_STATUS_PRICE_SET:
                    switch (origNomStatus) {
                        case DemandRepository.DEM_STATUS_PRICE_CONFIRMED:
                            return DemandRepository.DEM_STATUS_PRICE_SET;
                    }
                    break;
            }

            return origNomStatus;
        }

        public int GetNomenclatureStatus(int nomId, int demandId, int demNomStatusId) {

            //There is compared nomenclature status and demand nomenclature status
            //id Nomenclature Status is hogher than Demand Nomenclature status the staus remains

            var nom = (from nomDb in m_dbContext.Nomenclature
                       where nomDb.id == nomId
                       && nomDb.last_version
                       orderby nomDb.version descending
                       select nomDb).FirstOrDefault();

            if (demNomStatusId == NOM_STATUS_CANCELED) {
                var otherDemNoms = (from demNomDb in nom.Demand_Nomenclature
                                    join demDb in m_dbContext.Demand
                                    on demNomDb.demand_id equals demDb.id
                                      where demNomDb.nomenclature_id == nomId
                                      && demNomDb.demand_version == demDb.version
                                      && demDb.last_version == true
                                      && demDb.id != demandId
                                    orderby demNomDb.id descending
                                    select demNomDb).ToList();

                if (otherDemNoms == null || otherDemNoms.Count == 0) {
                    return NOM_STATUS_LOADED;
                } else {
                    List<int> tmpIds = new List<int>();
                    int tmpStatus = -1;
                    foreach (var demNom in otherDemNoms) {
                        if (tmpIds.Contains(demNom.nomenclature_id)) {
                            continue;
                        }

                        tmpIds.Add(demNom.nomenclature_id);

                        if (demNom.status_id > tmpStatus) {
                            tmpStatus = demNom.status_id;
                        }

                    }

                    return tmpStatus;
                }

            } else {
                switch (nom.status_id) {
                    case NOM_STATUS_CLOSED:
                        return NOM_STATUS_CLOSED;
                    case NOM_STATUS_PRICE_CONFIRMED:
                        switch (demNomStatusId) {
                            case NOM_STATUS_CLOSED:
                                return demNomStatusId;
                            default:
                                return nom.status_id;
                        }
                    case NOM_STATUS_PRICE_SET:
                        switch (demNomStatusId) {
                            case NOM_STATUS_CLOSED:
                            case NOM_STATUS_PRICE_CONFIRMED:
                                return demNomStatusId;
                            default:
                                return nom.status_id;
                        }
                    case NOM_STATUS_APPROVED:
                        switch (demNomStatusId) {
                            case NOM_STATUS_CLOSED:
                            case NOM_STATUS_PRICE_CONFIRMED:
                            case NOM_STATUS_PRICE_SET:
                                return demNomStatusId;
                            default:
                                return nom.status_id;
                        }
                    case NOM_STATUS_REJECTED:
                        switch (demNomStatusId) {
                            case NOM_STATUS_CLOSED:
                            case NOM_STATUS_PRICE_CONFIRMED:
                            case NOM_STATUS_PRICE_SET:
                            case NOM_STATUS_APPROVED:
                                return demNomStatusId;
                            default:
                                return nom.status_id;
                        }
                    case NOM_STATUS_WAIT_FOR_APPROVAL:
                        switch (demNomStatusId) {
                            case NOM_STATUS_CLOSED:
                            case NOM_STATUS_PRICE_CONFIRMED:
                            case NOM_STATUS_PRICE_SET:
                            case NOM_STATUS_APPROVED:
                            case NOM_STATUS_REJECTED:
                                return demNomStatusId;
                            default:
                                return nom.status_id;
                        }
                    case NOM_STATUS_SUPPLIER_REPLIED:
                        switch (demNomStatusId) {
                            case NOM_STATUS_CLOSED:
                            case NOM_STATUS_PRICE_CONFIRMED:
                            case NOM_STATUS_PRICE_SET:
                            case NOM_STATUS_APPROVED:
                            case NOM_STATUS_REJECTED:
                            case NOM_STATUS_WAIT_FOR_APPROVAL:
                                return demNomStatusId;
                            default:
                                return nom.status_id;
                        }
                    case NOM_STATUS_SENT_TO_SUPPLIER:
                        switch (demNomStatusId) {
                            case NOM_STATUS_CLOSED:
                            case NOM_STATUS_PRICE_CONFIRMED:
                            case NOM_STATUS_PRICE_SET:
                            case NOM_STATUS_APPROVED:
                            case NOM_STATUS_REJECTED:
                            case NOM_STATUS_WAIT_FOR_APPROVAL:
                            case NOM_STATUS_SUPPLIER_REPLIED:
                                return demNomStatusId;
                            default:
                                return nom.status_id;
                        }
                }
            }

            return demNomStatusId;

            //if (otherDemNoms == null || otherDemNoms.Count == 0) {
            //    return demNomStatusId;
            //} else {

            //    //int otherStatus = demNomStatusId;
            //    //List<int> tmpNomIds = new List<int>();
            //    //foreach (var demNom in otherDemNoms) {
            //    //    if (otherStatus == -1) {
            //    //        otherStatus = demNom.status_id;
            //    //    } else {
            //    //        switch (demNom.status_id) {
            //    //            case NOM_STATUS_CLOSED:
            //    //                break;
            //    //            case NOM_STATUS_PRICE_CONFIRMED:
            //    //                switch (demNom.status_id) {
            //    //                    case NOM_STATUS_CLOSED:
            //    //                    case NOM_STATUS_PRICE_CONFIRMED:
            //    //                        break;
            //    //                    default:
            //    //                        otherStatus = NOM_STATUS_PRICE_CONFIRMED;
            //    //                        break;
            //    //                }

            //    //                break;
            //    //            case NOM_STATUS_PRICE_SET:
            //    //                switch (otherStatus) {
            //    //                    case NOM_STATUS_CLOSED:
            //    //                    case NOM_STATUS_PRICE_CONFIRMED:
            //    //                    case NOM_STATUS_PRICE_SET:
            //    //                        break;
            //    //                    default:
            //    //                        otherStatus = NOM_STATUS_PRICE_SET;
            //    //                        break;
            //    //                }
            //    //                break;
            //    //        }
            //    //    }
            //    //}
            //}

            //return demNomStatusId;
        }

        public Nomenclature GetNomenclatureNewVersion(Nomenclature dbNomenclature, int userId) {
            return GetNomenclatureNewVersion(dbNomenclature, userId, false);
        }

        public Nomenclature GetNomenclatureNewVersion(Nomenclature dbNomenclature, int userId, bool isSetValues) {
            int origVersion = dbNomenclature.version;
            int newVersion = ++origVersion;

            Nomenclature newNomeclature = new Nomenclature();
            if (isSetValues) {
                SetValues(dbNomenclature, newNomeclature);
            }
            newNomeclature.version = newVersion;

            //if (dbNomenclature.status_id != updNomenclature.status_id) {
            //    newNomeclature.last_status_modif_date = DateTime.Now;
            //}

            newNomeclature.modif_user_id = userId;
            newNomeclature.modif_date = DateTime.Now;

            return newNomeclature;
        }

        public int GetPendingNomNumber() {
            var i = (from nomDb in m_dbContext.Nomenclature
                     join actNomDb in m_dbContext.Active_Nomenclature
                     on nomDb.id equals actNomDb.id
                     where nomDb.status_id == (int)Status.Loaded
                     && nomDb.last_version == true
                     && nomDb.is_active == true
                     select new { id = nomDb.id}).ToList().Count;

            return i;
        }

        public static string GetSourceText(int sourceId) {
            if (sourceId == (int)NomSource.Both) {
                return "Prodis, DMS";
            } else if (sourceId == (int)NomSource.DNSSharePoint) {
                return "DMS";
            } else if (sourceId == (int)NomSource.Custom) {
                return "Custom";
            }
            return "Prodis";

        }

        private void SetNomenclaturesExtProp(Nomenclature dbNomenclature, NomenclatureExtend nomenclatureExtended) {
            if (nomenclatureExtended.modif_date != null) {
                nomenclatureExtended.modif_date_text = ((DateTime)nomenclatureExtended.modif_date).ToString("dd.MM.yyyy HH:mm");
            }
            if (nomenclatureExtended.created_date != null) {
                nomenclatureExtended.created_date_text = ((DateTime)nomenclatureExtended.created_date).ToString("dd.MM.yyyy HH:mm");
            }
            if (nomenclatureExtended.import_date != null) {
                nomenclatureExtended.import_date_text = ((DateTime)nomenclatureExtended.import_date).ToString("dd.MM.yyyy HH:mm");
            }
            if (nomenclatureExtended.last_status_modif_date != null) {
                nomenclatureExtended.last_status_modif_date_text = ((DateTime)nomenclatureExtended.last_status_modif_date).ToString("dd.MM.yyyy HH:mm");
            }
            if (dbNomenclature.Material_Group != null) {
                nomenclatureExtended.material_group_text = dbNomenclature.Material_Group.name;
            }
            nomenclatureExtended.is_selected = false;
            nomenclatureExtended.days_in_status = (DateTime.Now - nomenclatureExtended.last_status_modif_date).Days;

            string impPath = DemandRepository.GetStatusImagePath(nomenclatureExtended.status_id);
            nomenclatureExtended.img_status_path = impPath;

            nomenclatureExtended.source_text = GetSourceText(dbNomenclature.source_id);
            

            //if (nomenclatureExtended.status_id == (int)Status.WaitForApproval) {
            //    nomenclatureExtended.img_status_path = "/OTISCZ.ScmDemand.UI;component/Images/Statuses/StatusWaitForApproval.png";
            //} else {
            //    nomenclatureExtended.img_status_path = "/OTISCZ.ScmDemand.UI;component/Images/Statuses/StatusLoaded.png";
            //}
        }

        private string GetFilter(string filter, out bool isActiveOnly) {
            isActiveOnly = false;
            string strFilterWhere = "";
            if (!String.IsNullOrEmpty(filter)) {
                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {
                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    strFilterWhere += " AND ";

                    string columnName = strItemProp[0].Trim().ToUpper();
                    if (columnName == NomenclatureData.NOMENCLATURE_KEY_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "nd." + NomenclatureData.NOMENCLATURE_KEY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    } else if (columnName == NomenclatureData.NAME_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "nd." + NomenclatureData.NAME_WO_DIA_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(strItemProp[1].Trim()) + "%'";
                    } else if (columnName == NomenclatureData.SPECIFICATION_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "nd." + NomenclatureData.SPECIFICATION_WO_DIA_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(strItemProp[1].Trim()) + "%'";
                    } else if (columnName == NomenclatureData.STATUS_ID_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "nd." + NomenclatureData.STATUS_ID_FIELD + "=" + strItemProp[1].Trim();
                    } else if (columnName == NomenclatureData.IMPORT_DATE_FIELD.Trim().ToUpper()) {
                        if (strItemProp.Length > 2 && strItemProp[2] == "FROM") {
                            strFilterWhere += "nd." + NomenclatureData.IMPORT_DATE_FIELD + ">=" + strItemProp[1].Trim();
                        } else if (strItemProp.Length > 2 && strItemProp[2] == "TO") {
                            strFilterWhere += "nd." + NomenclatureData.IMPORT_DATE_FIELD + "<=" + strItemProp[1].Trim();
                        } else {
                            strFilterWhere += "nd." + NomenclatureData.IMPORT_DATE_FIELD + "=" + strItemProp[1].Trim();
                        }
                    } else if (columnName == NomenclatureData.CREATED_DATE_FIELD.Trim().ToUpper()) {
                        if (strItemProp.Length > 2 && strItemProp[2] == "FROM") {
                            strFilterWhere += "nd." + NomenclatureData.CREATED_DATE_FIELD + ">=" + strItemProp[1].Trim();
                        } else if (strItemProp.Length > 2 && strItemProp[2] == "TO") {
                            strFilterWhere += "nd." + NomenclatureData.CREATED_DATE_FIELD + "<=" + strItemProp[1].Trim();
                        } else {
                            strFilterWhere += "nd." + NomenclatureData.CREATED_DATE_FIELD + "=" + strItemProp[1].Trim();
                        }
                    } else if (columnName.ToUpper() == NomenclatureData.IS_ACTIVE_FIELD.ToUpper()) {
                        if (strItemProp[1].Trim() == "1") {
                            isActiveOnly = true;
                        }
                        strFilterWhere += "nd." + NomenclatureData.IS_ACTIVE_FIELD + "=" + strItemProp[1].Trim();

                    } else if (columnName.ToUpper() == NomenclatureData.SOURCE_ID_FIELD.ToUpper()) {
                        strFilterWhere += "nd." + NomenclatureData.SOURCE_ID_FIELD + "=" + strItemProp[1].Trim();
                    } else if (columnName.ToUpper() == NomenclatureData.IS_PLNAKNAVRH_FIELD.ToUpper()) {
                        strFilterWhere += "nd." + NomenclatureData.IS_PLNAKNAVRH_FIELD + "=" + strItemProp[1].Trim();
                    } else if (columnName.ToUpper() == "MATERIAL_GROUP_TEXT") {
                        strFilterWhere += "mgd." + MaterialGroupData.NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    } 

                }
            }

            return strFilterWhere;
        }

        private string GetPureBody(string strFilterWhere, bool isActiveOnly) {
            string sqlPureBody = null;

            if (isActiveOnly) {
                sqlPureBody = " FROM " + NomenclatureData.TABLE_NAME + " nd"
                    + " INNER JOIN " + ActiveNomenclatureData.TABLE_NAME + " actnd"
                    + " ON nd." + NomenclatureData.ID_FIELD + "=actnd." + ActiveNomenclatureData.ID_FIELD
                    + " LEFT OUTER JOIN " + MaterialGroupData.TABLE_NAME + " mgd"
                    + " ON nd." + NomenclatureData.MATERIAL_GROUP_ID_FIELD + "=mgd." + MaterialGroupData.ID_FIELD
                    + " WHERE " + NomenclatureData.LAST_VERSION_FIELD + "=1";
            } else {
                sqlPureBody = " FROM " + NomenclatureData.TABLE_NAME + " nd"
                    + " LEFT OUTER JOIN " + MaterialGroupData.TABLE_NAME + " mgd"
                    + " ON nd." + NomenclatureData.MATERIAL_GROUP_ID_FIELD + "=mgd." + MaterialGroupData.ID_FIELD
                    + " WHERE " + NomenclatureData.LAST_VERSION_FIELD + "=1";
            }

            sqlPureBody += strFilterWhere;

            return sqlPureBody;
        }

        private string GetOrder(string sort) {
            string strOrder = "ORDER BY nd." + NomenclatureData.ID_FIELD;

            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    }

                    if (strItemProp[0] == NomenclatureData.NOMENCLATURE_KEY_FIELD) {
                        strOrder = "nd." + NomenclatureData.NOMENCLATURE_KEY_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == NomenclatureData.NAME_FIELD) {
                        strOrder += "nd." + NomenclatureData.NAME_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == NomenclatureData.SPECIFICATION_FIELD) {
                        strOrder += "nd." + NomenclatureData.SPECIFICATION_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "created_date_text") {
                        strOrder += "nd." + NomenclatureData.CREATED_DATE_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "source_text") {
                        strOrder += "nd." + NomenclatureData.SOURCE_ID_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "plnaknavrh_text") {
                        strOrder += "nd." + NomenclatureData.IS_PLNAKNAVRH_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "material_group_text") {
                        strOrder += "mgd." + MaterialGroupData.NAME_FIELD + " " + strItemProp[1];
                    }
                    
                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        public bool SetPrice(string nomenclatureKey, decimal price, int userId, int currencyId) {
            bool isWasFound = false;
            using (TransactionScope transaction = new TransactionScope()) {
                var noms = (from nomDB in m_dbContext.Nomenclature
                           where nomDB.nomenclature_key == nomenclatureKey
                           && nomDB.last_version == true
                           orderby nomDB.version descending
                           select nomDB).ToList();

                if (noms != null && noms.Count > 1) {
                    //fix wrong records - dobled last version item
                    for (int i = 1; i < noms.Count; i++) {
                        noms.ElementAt(i).last_version = false;
                    }
                }
                Nomenclature nom = null;
                if (noms != null && noms.Count > 0) {
                    nom = noms.ElementAt(0);
                }

                if (nom != null) {
                    isWasFound = true;

                    var demNoms = (from demNomDB in m_dbContext.Demand_Nomenclature
                                   where demNomDB.nomenclature_id == nom.id
                                   orderby demNomDB.id descending
                                   select demNomDB).ToList();

                    if (demNoms == null || demNoms.Count == 0) {
                        //Nomenclature Only
                        if (nom.status_id != NOM_STATUS_PRICE_CONFIRMED) {
                            var nomNewVersion = GetNomenclatureNewVersion(nom, userId);
                            int newVers = nomNewVersion.version;
                            SetValues(nom, nomNewVersion);
                            nomNewVersion.version = newVers;
                            nomNewVersion.price = price;
                            nomNewVersion.currency_id = currencyId;
                            nomNewVersion.status_id = NOM_STATUS_PRICE_CONFIRMED;
                            nomNewVersion.modif_user_id = userId;
                            nomNewVersion.modif_date = DateTime.Now;
                            nom.last_version = false;

                            m_dbContext.Nomenclature.Add(nomNewVersion);

                            //delete from active nom
                            var actNom = (from actNomDB in m_dbContext.Active_Nomenclature
                                          where actNomDB.id == nom.id
                                          select actNomDB).FirstOrDefault();

                            if (actNom != null) {
                                m_dbContext.Active_Nomenclature.Remove(actNom);
                            }


                        }
                    } else {
                        //Demand Nomeclature
                        Hashtable htProcessed = new Hashtable();

                        foreach (var demNom in demNoms) {
                            string demNomKey = GetDemNomKey(demNom.demand_id, demNom.nomenclature_id);
                            if (htProcessed.ContainsKey(demNomKey)) {
                                continue;
                            }

                            htProcessed.Add(demNomKey, null);

                            

                            if (demNom.status_id != NOM_STATUS_PRICE_CONFIRMED) {
                                NomStatus nomStatus = new NomStatus(
                                    demNom.nomenclature_id,
                                    NOM_STATUS_PRICE_CONFIRMED,
                                    price,
                                    currencyId);

                                List<NomStatus> nomStatuses = new List<NomStatus>() { nomStatus };

                                SaveNomenclaturesStatus(
                                    demNom.demand_id,
                                    nomStatuses,
                                    userId);
                            }
                        }
                    }

                   
                }

                SaveChanges();

                transaction.Complete();
            }

            return isWasFound;
        }

        public void DeactivateNomenclature(string nomKey) {
            var nom = (from nomDB in m_dbContext.Nomenclature
                       where nomDB.nomenclature_key == nomKey
                       && nomDB.last_version == true
                       orderby nomDB.version descending
                       select nomDB).FirstOrDefault();

            if (nom != null) {
                nom.is_active = false;
                nom.last_version = false;

                SaveChanges();
            }
        }

        public void SetPlNakNavrh(string nomKey) {
            var nom = (from nomDB in m_dbContext.Nomenclature
                       where nomDB.nomenclature_key == nomKey
                       && nomDB.last_version == true
                       select nomDB).FirstOrDefault();

            

            if (nom != null) {
                var newNom = GetNomenclatureNewVersion(nom, UserRepository.SYSTEM_USER_ID, true);
                newNom.is_plnaknavrh = true;
                newNom.last_version = true;
                m_dbContext.Nomenclature.Add(newNom);

                nom.last_version = false;

                SaveChanges();
            }
        }

        private string GetDemNomKey(int demandId, int nomId) {
            return demandId + "," + nomId;
        }

        private void SetPriceRemoveActiveDemands(int nomenclatureId) {
            var demNoms = (from demNomDb in m_dbContext.Demand_Nomenclature
                           where demNomDb.nomenclature_id == nomenclatureId
                           orderby demNomDb.demand_version descending, demNomDb.id descending
                           select demNomDb).ToList();

            if (demNoms == null) {
                return;
            }

            foreach (var demNom in demNoms) {
                if (demNom.Demand.last_version == false) {
                    continue;
                }

                var demand = (from demandDb in m_dbContext.Demand
                              where demandDb.id == demNom.demand_id
                              && demandDb.last_version == true
                              select demandDb).FirstOrDefault();

                if (demand.Active_Demand == null) {
                    continue;
                }

                bool isActive = false;
                foreach (var demDemNom in demand.Demand_Nomenclature) {
                    if (demDemNom.nomenclature_id == nomenclatureId) {
                        continue;
                    }

                    switch (demDemNom.status_id) {
                        case NOM_STATUS_SENT_TO_SUPPLIER:
                        case NOM_STATUS_SUPPLIER_REPLIED:
                        case NOM_STATUS_WAIT_FOR_APPROVAL:
                        case NOM_STATUS_REJECTED:
                            isActive = true;
                            break;
                    }

                    if (isActive) {
                        break;
                    }
                }

                if (!isActive) {
                    m_dbContext.Active_Demand.Remove(demand.Active_Demand);
                }
            }
        }

        public List<Nomenclature> GetNomenclaturesBySourceId(int sourceId) {
            var noms = (from nomDb in m_dbContext.Nomenclature
                        where nomDb.source_id == sourceId
                        select nomDb).ToList();

            return noms;
        }

        public List<Nomenclature> GetNomenclatureHistory(int id) {
            var noms = (from nomDb in m_dbContext.Nomenclature
                        where nomDb.id == id
                        orderby nomDb.version ascending
                        select nomDb).ToList();

            return noms;
        }

        public void DeleteNotUsedCustomNoms() {
            string sql = "SELECT nd.* FROM " + NomenclatureData.TABLE_NAME + " nd"
                + " LEFT OUTER JOIN " + DemandNomenclatureData.TABLE_NAME + " dnd"
                + " ON nd." + NomenclatureData.ID_FIELD + "=dnd." + DemandNomenclatureData.NOMENCLATURE_ID_FIELD 
                + " WHERE dnd." + DemandNomenclatureData.NOMENCLATURE_ID_FIELD + " IS NULL"
                + " AND nd." + NomenclatureData.SOURCE_ID_FIELD + "=" + (int)NomSource.Custom
                + " AND nd." + NomenclatureData.LAST_VERSION_FIELD + "=1" 
                + " AND nd." + NomenclatureData.LAST_STATUS_MODIF_DATE_FIELD + "<" + ConvertData.ToDbDate(DateTime.Now.AddHours(-2));

            var nomenclatures = m_dbContext.Database.SqlQuery<Nomenclature>(sql).ToList();

            foreach(var nom in nomenclatures) {
                var custNomDb = (from nomDb in m_dbContext.Nomenclature
                                 where nomDb.id == nom.id && nomDb.last_version == true
                                 select nomDb).FirstOrDefault();
                if (custNomDb != null) {
                    custNomDb.last_version = false;
                    SaveChanges();
                }
            }
        }
        #endregion
    }
}
