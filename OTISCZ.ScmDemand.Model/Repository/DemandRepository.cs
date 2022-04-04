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
    public class DemandRepository : BaseRepository<Demand> {
        #region Constants
        public const int DEM_STATUS_CANCELED = 50;
        public const int DEM_STATUS_DRAFT = 100;
        public const int DEM_STATUS_SENT = 200;
        public const int DEM_STATUS_SUPPLIER_REPLIED = 300;
        public const int DEM_STATUS_WAIT_FOR_APPROVAL = 400;
        public const int DEM_STATUS_APPROVED = 500;
        public const int DEM_STATUS_REJECTED = 600;
        public const int DEM_STATUS_PRICE_SET = 700;
        public const int DEM_STATUS_PRICE_CONFIRMED = 800;
        public const int DEM_STATUS_CLOSED = 1000;
        public const int DEM_STATUS_UNKNOWN = -1;
        public const int NOM_STATUS_WO_DEMAND = 150;

        public const string LOCK_REASON_OTHERDEMAND = "LROD:";
        public const string LOCK_REASON_PRICE_SET = "LRPS:";
        #endregion

        #region Enum
        public enum Status {
            WithoutDemand = NOM_STATUS_WO_DEMAND,
            Canceled = DEM_STATUS_CANCELED,
            Draft = DEM_STATUS_DRAFT,
            Sent = DEM_STATUS_SENT,
            Replied = DEM_STATUS_SUPPLIER_REPLIED,
            WaitForApproval = DEM_STATUS_WAIT_FOR_APPROVAL,
            Approved = DEM_STATUS_APPROVED,
            Rejected = DEM_STATUS_REJECTED,
            PriceSet = DEM_STATUS_PRICE_SET,
            PriceConfirmed = DEM_STATUS_PRICE_CONFIRMED,
            Closed = DEM_STATUS_CLOSED
        }
        #endregion

        #region Methods
        public int SaveDemand(DemandExtend updDemand, int userId, bool isWasSent) {
            
            using (TransactionScope transaction = new TransactionScope()) {
                var dbDemand = (from demandDb in m_dbContext.Demand
                                where demandDb.id == updDemand.id
                                && demandDb.last_version == true
                                select demandDb).FirstOrDefault();

                if (dbDemand != null) {
                    //Existing demand

                    int lastVersion = dbDemand.version;
                    int newVersion = ++lastVersion;

                    bool isOrigDemandClosed = (dbDemand.status_id == DemandRepository.DEM_STATUS_CLOSED);

                    Demand newDemandVersion = GetNewDemandVersion(dbDemand, newVersion, userId);
                    SetValues(updDemand, newDemandVersion);
                    newDemandVersion.version = newVersion;
                    SetDemandSysItems(newDemandVersion, userId);
                                       
                    if (updDemand.status_id == (int)Status.Canceled) {
                        CancelDemand(updDemand, dbDemand, newDemandVersion, userId);
                    }

                    if (updDemand.status_id == (int)Status.Closed) {
                        var activeDemand = (from actDemDb in m_dbContext.Active_Demand
                                            where actDemDb.id == dbDemand.id
                                            select actDemDb).FirstOrDefault();
                        if (activeDemand != null) {
                            m_dbContext.Active_Demand.Remove(activeDemand);
                        }
                    }

                    //Check active demands
                    if (isOrigDemandClosed) {
                        if (updDemand.status_id == (int)Status.Sent
                            || updDemand.status_id == (int)Status.Replied
                            || updDemand.status_id == (int)Status.WaitForApproval
                            || updDemand.status_id == (int)Status.Rejected
                            || updDemand.status_id == (int)Status.Approved) {
                            var activeDemand = (from actDemDb in m_dbContext.Active_Demand
                                                where actDemDb.id == dbDemand.id
                                                select actDemDb).FirstOrDefault();
                            if (activeDemand == null) {
                                bool isReqActive = false;
                                bool isAppManActive = false;
                                ActiveDemandActivity(updDemand.status_id, out isReqActive, out isAppManActive);

                                Active_Demand newActDemand = new Active_Demand();
                                newActDemand.id = updDemand.id;
                                newActDemand.status_id = updDemand.status_id;
                                newActDemand.is_requestor_acive = isReqActive;
                                newActDemand.is_app_man_active = isAppManActive;

                                m_dbContext.Active_Demand.Add(newActDemand);
                            }
                        }
                    }

                    newDemandVersion.last_version = updDemand.last_version;

                    m_dbContext.Demand.Add(newDemandVersion);
                                                            
                } else {
                    //New Demand
                    SaveNewDemand(updDemand, userId, isWasSent);
                }
                

                SaveChanges();

                transaction.Complete();
            }

            return updDemand.id;
        }

        private void CancelDemand(DemandExtend updDemand, Demand dbDemand, Demand newDemandVersion, int userId) {
            DemandNomenclatureRepository demNomRep = new DemandNomenclatureRepository();
            NomenclatureRepository nomRep = new NomenclatureRepository();

            foreach (var demNom in dbDemand.Demand_Nomenclature) {
                if (demNom.status_id == (int)Status.WaitForApproval
                    || demNom.status_id == (int)Status.Approved
                    || demNom.status_id == (int)Status.PriceSet) {
                    DemandNomenclatureRepository.UnlockLockNomenclature(demNom.nomenclature_id, m_dbContext);
                }

            }

            foreach (var demNom in newDemandVersion.Demand_Nomenclature) {
                demNom.status_id = updDemand.status_id;
                nomRep.UpdateNomenclatureStatus(m_dbContext, demNom, updDemand.status_id, userId);
            }

            var activeDemand = (from actDemDb in m_dbContext.Active_Demand
                                where actDemDb.id == dbDemand.id
                                select actDemDb).FirstOrDefault();
            if (activeDemand != null) {
                m_dbContext.Active_Demand.Remove(activeDemand);
            }
        }

        //private void SaveDemandWaitForApproval(DemandExtend updDemand, Demand newDemandVersion) {
        //    DemandNomenclatureRepository demandNomenclatureRepository = new DemandNomenclatureRepository();
        //    foreach (var demNom in updDemand.demand_nomenclatures_extend) {
        //        demandNomenclatureRepository.LockNomenclature(newDemandVersion.id, newDemandVersion.version, demNom.nomenclature_id, m_dbContext);
        //    }
        //}

        private void SaveNewDemand(DemandExtend updDemand, int userId, bool isWasSent) {
            Demand newDemandVersion = new Demand();
            SetValues(updDemand, newDemandVersion);

            int lastId = GetLastId();
            int newId = ++lastId;
            newDemandVersion.id = newId;
            newDemandVersion.version = 0;
            newDemandVersion.created_date = DateTime.Now;
            newDemandVersion.last_status_modif_date = DateTime.Now;

            int lastDemNomId = new DemandNomenclatureRepository().GetLastId();
            int newDemNomId = ++lastDemNomId;

            //Demand nomenclatures
            newDemandVersion.Demand_Nomenclature = new List<Demand_Nomenclature>();
            foreach (var demNom in updDemand.Demand_Nomenclature) {
                Demand_Nomenclature newDemNom = new Demand_Nomenclature();
                newDemNom.id = newDemNomId;
                newDemNomId++;
                newDemNom.demand_id = newDemandVersion.id;
                newDemNom.demand_version = newDemandVersion.version;
                newDemNom.nomenclature_id = demNom.nomenclature_id;
                newDemNom.nomenclature_version = demNom.nomenclature_version;
                newDemNom.status_id = (int)NomenclatureRepository.Status.SentToSupplier;
                newDemNom.modif_user_id = userId;
                newDemNom.modif_date = DateTime.Now;
                newDemandVersion.Demand_Nomenclature.Add(newDemNom);

                //new NomenclatureRepository().UpdateNomenclatureStatus(m_dbContext, newDemNom, (int)NomenclatureRepository.Status.SentToSupplier, userId);
            }

            

            if (updDemand.recipient_attachments_extend != null) {
                newDemandVersion.Demand_Attachment = new List<Demand_Attachment>();
                AttachmentRepository attachmentRepository = new AttachmentRepository();
                foreach (var att in updDemand.recipient_attachments_extend) {
                    int attId = attachmentRepository.AddAttachment(att, userId);
                    Demand_Attachment da = new Demand_Attachment();
                    da.demand_id = newId;
                    da.demand_version = 0;
                    da.attachment_id = attId;
                    da.att_type = att.att_type;
                    newDemandVersion.Demand_Attachment.Add(da);
                }
            }

            
            if (isWasSent && newDemandVersion.Demand_Nomenclature != null) {
                NomenclatureRepository nomenclatureRepository = new NomenclatureRepository();
                foreach (var demNom in newDemandVersion.Demand_Nomenclature) {
                    //var dbNomenclature = nomenclatureRepository.GetNomenclatureById(demNom.nomenclature_id);
                    var dbNomenclature = (from nomDb in m_dbContext.Nomenclature
                                          where nomDb.id == demNom.nomenclature_id
                                          && nomDb.last_version == true
                                          select nomDb).FirstOrDefault();
                    if (dbNomenclature.status_id == (int)NomenclatureRepository.Status.Loaded) {
                        var nomNewVersion = nomenclatureRepository.GetNomenclatureNewVersion(dbNomenclature, userId);
                        int newVersion = nomNewVersion.version;
                        SetValues(dbNomenclature, nomNewVersion);
                        nomNewVersion.version = newVersion;
                        nomNewVersion.modif_user_id = userId;
                        nomNewVersion.modif_date = DateTime.Now;
                        nomNewVersion.status_id = (int)NomenclatureRepository.Status.SentToSupplier;

                        m_dbContext.Nomenclature.Add(nomNewVersion);

                        dbNomenclature.last_version = false;

                        //demNom.nomenclature_version = newVersion;
                    }

                    new ActiveNomenclatureRepository().RemoveActiveNoms(demNom.nomenclature_id);
                }


            }

            SetDemandSysItems(newDemandVersion, userId);

            m_dbContext.Demand.Add(newDemandVersion);

            //active demands
            Active_Demand activeDemand = new Active_Demand();
            activeDemand.id = newDemandVersion.id;
            activeDemand.version = newDemandVersion.version;
            activeDemand.status_id = newDemandVersion.status_id;
            activeDemand.is_requestor_acive = true;

            m_dbContext.Active_Demand.Add(activeDemand);
        }

        private void SetDemandSysItems(Demand demand, int userId) {
            

            demand.modif_date = DateTime.Now;
            demand.modif_user_id = userId;
            demand.last_version = true;
        }

        public int GetNewNumberIndex() {
            var dbDemandLatNr = (from demandDb in m_dbContext.Demand_LastNr
                            where demandDb.year == DateTime.Now.Year
                                 select demandDb).FirstOrDefault();

            if (dbDemandLatNr == null) {
                int lastId = GetLastNrId();
                int newId = ++lastId;
                Demand_LastNr demand_LastNr = new Demand_LastNr();
                demand_LastNr.id = newId;
                demand_LastNr.year = DateTime.Now.Year;
                demand_LastNr.last_nr = 1;

                m_dbContext.Demand_LastNr.Add(demand_LastNr);
                SaveChanges();

                return 1;
            }

            int lastNr = dbDemandLatNr.last_nr;
            int newNr = ++lastNr;
            dbDemandLatNr.last_nr = newNr;
            SaveChanges();

            return newNr;
        }

        private int GetLastNrId() {

            var dbDemand = (from demandDb in m_dbContext.Demand_LastNr
                            orderby demandDb.id descending
                            select demandDb).FirstOrDefault();

            if (dbDemand == null) {
                return -1;
            }

            return dbDemand.id;
        }

        private int GetLastId() {

            var dbDemand = (from demandDb in m_dbContext.Demand
                                  orderby demandDb.id descending
                                  select demandDb).FirstOrDefault();

            if (dbDemand == null) {
                return -1;
            }

            return dbDemand.id;
        }

        public List<DemandExtend> GetDemands(
            List<WcfFilterField> filterFields,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount) {

            bool isActiveOnly = false;
            bool isNomenclatureFilter = false;
            string strNomeclatreKeyFilter = null;
            bool isSupplierFilter = false;
            bool isRequestorFilter = false;
            bool isAppManFilter = false;

            string strFilterWhere = GetFilter(
                filterFields, 
                out isActiveOnly, 
                out isNomenclatureFilter, 
                out strNomeclatreKeyFilter, 
                out isSupplierFilter,
                out isRequestorFilter,
                out isAppManFilter);

            bool isSupplierSort = false;
            bool isRequestorSort = false;
            bool isAppManSort = false;

            string strOrder = GetOrder(
                sort, 
                out isSupplierSort,
                out isRequestorSort,
                out isAppManSort);

            string sqlPure = "SELECT dd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";
            //if (isNomenclatureFilter) {
            //    //syka TODO - order by does not work if there is a DISTINCT
            //    sqlPure = "SELECT DISTINCT dd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";
            //}

            string sqlPureBody = GetPureBody(
                strFilterWhere, 
                isActiveOnly, 
                isNomenclatureFilter,
                strNomeclatreKeyFilter,
                isSupplierFilter,
                isRequestorFilter,
                isAppManFilter,
                isSupplierSort,
                isRequestorSort,
                isAppManSort);

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

            string sql = "SELECT * FROM(" + sqlPart + ") AS ScmDemandPartData" +
                " WHERE ScmDemandPartData.RowNum BETWEEN " + partStart + " AND " + partStop;
            //if (isNomenclatureFilter) {
            //    //syka TODO - order by does not work if there is a DISTINCT
            //    sql = "SELECT DISTINCT * FROM(" + sqlPart + ") AS ScmDemandPartData" +
            //    " WHERE ScmDemandPartData.RowNum BETWEEN " + partStart + " AND " + partStop;
            //}

            var demands = m_dbContext.Database.SqlQuery<Demand>(sql).ToList();

            return GetDemands(demands, pageNr, pageSize);

        }

        public List<DemandExtend> GetDemandsReport(
            List<WcfFilterField> filterFields,
            string sort) {

            bool isActiveOnly = false;
            bool isNomenclatureFilter = false;
            string strNomeclatreKeyFilter = null;
            bool isSupplierFilter = false;
            bool isRequestorFilter = false;
            bool isAppManFilter = false;

            string strFilterWhere = GetFilter(
                    filterFields, 
                    out isActiveOnly, 
                    out isNomenclatureFilter, 
                    out strNomeclatreKeyFilter,
                    out isSupplierFilter,
                    out isRequestorFilter,
                    out isAppManFilter);

            bool isSupplierSort = false;
            bool isRequestorSort = false;
            bool isAppManSort = false;

            string strOrder = GetOrder(
                sort, 
                out isSupplierSort,
                out isRequestorSort,
                out isAppManSort);

            string sqlPure = "SELECT dd.* ";
            string sqlPureBody = GetPureBody(
                strFilterWhere, 
                isActiveOnly, 
                isNomenclatureFilter,
                strNomeclatreKeyFilter,
                isSupplierFilter,
                isRequestorFilter,
                isAppManFilter,
                isSupplierSort,
                isRequestorSort,
                isAppManSort);

            sqlPureBody += strOrder;

            string sql = sqlPure + sqlPureBody;

            var demands = m_dbContext.Database.SqlQuery<Demand>(sql).ToList();

            return GetDemands(demands);
        }

        private List<DemandExtend> GetDemands(List<Demand> demands) {
            return GetDemands(demands, 1, 1);
        }

        private List<DemandExtend> GetDemands(
            List<Demand> demands,
            int pageNr,
            int pageSize) {

            List<DemandExtend> demandsExtend = new List<DemandExtend>();
            int rowIndex = (pageNr - 1) * pageSize + 1;

            

            foreach (var demand in demands) {
                
                DemandExtend demandExtend = new DemandExtend();
                SetValues(demand, demandExtend);
                demandExtend.row_index = rowIndex++;

                Demand dbDemand = (from demDb in m_dbContext.Demand
                                               where demDb.id == demand.id 
                                               && demDb.last_version == true
                                   select demDb).FirstOrDefault();

                //requestor
                demandExtend.requestor_name = dbDemand.Requestor.surname + " " + dbDemand.Requestor.first_name;

                //app man
                if (demandExtend.app_man_id != null) {
                    demandExtend.app_man_name = dbDemand.AppMan.surname + " " + dbDemand.AppMan.first_name;
                }

                //created date
                if (demandExtend.created_date != null) {
                    demandExtend.created_date_text = demandExtend.created_date.ToString("dd.MM.yyyy HH:mm");
                }
                if (demandExtend.last_status_modif_date != null) {
                    demandExtend.last_status_modif_date_text = demandExtend.last_status_modif_date.ToString("dd.MM.yyyy HH:mm");
                }

                //nomenclature
                string strNomList = "";

                var demNomsCurrVersion = (from demNomDb in demand.Demand_Nomenclature
                                          where demNomDb.demand_version == demand.version
                                          orderby demNomDb.id descending
                                          select demNomDb).ToList();

                List<int> usedNoms = new List<int>();

                
                foreach (var demNom in dbDemand.Demand_Nomenclature) {
                    if (usedNoms.Contains(demNom.nomenclature_id)) {
                        continue;
                    }

                    usedNoms.Add(demNom.nomenclature_id);


                    if (strNomList.Length > 0) {
                        strNomList += ", ";
                    }
                    strNomList += demNom.Nomenclature.nomenclature_key;
                }
                demandExtend.nomenclatures_text = strNomList;

                //supplier
                if (dbDemand.supplier_id != null) {
                    demandExtend.supplier_text = dbDemand.Supplier.supp_name;
                    if (!String.IsNullOrWhiteSpace(dbDemand.Supplier.supplier_id)) {
                        demandExtend.supplier_text += " (" + dbDemand.Supplier.supplier_id + ")";
                    }
                }

                //status
                demandExtend.img_status_path = GetStatusImagePath(demandExtend.status_id);

                //days in status
                demandExtend.days_in_status = (DateTime.Now - demandExtend.last_status_modif_date).Days;


                demandsExtend.Add(demandExtend);
            }

            return demandsExtend;
        }

        public static string GetStatusImagePath(int statusId) {
            if (statusId == (int)Status.Draft) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Status08.png";
            } else if (statusId == (int)Status.Sent) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Status28.png";
            } else if (statusId == (int)Status.Replied) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Status48.png";
            } else if (statusId == (int)Status.WaitForApproval) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Status68.png";
            } else if (statusId == (int)Status.Approved) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Status78.png";
            } else if (statusId == (int)Status.PriceSet) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Status88.png";
            } else if (statusId == (int)Status.Canceled) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Canceled.png";
            } else if (statusId == (int)Status.Rejected) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/StatusReject.png";
            } else if (statusId == (int)Status.PriceConfirmed) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/PriceConfirmed.png";
            } else if (statusId == (int)Status.Closed) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Status88Grey.png";
            } else if (statusId == (int)Status.WithoutDemand) {
                return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Status88Grey.png";
            }

            return "/OTISCZ.ScmDemand.UI;component/Images/Statuses/Status08.png";
        }

        private string GetPureBody(
            string strFilterWhere, 
            bool isActiveOnly, 
            bool isNomenclatureFiltered,
            string strNomeclatreKeyFilter,
            bool isSupplierFilter,
            bool isRequestorFilter,
            bool isAppManFilter,
            bool isSupplierSort,
            bool isRequestorSort,
            bool isAppManSort) {
            //string sqlPureBody = null;

            string sqlPureBody = " FROM " + DemandData.TABLE_NAME + " dd";
            if (isActiveOnly) {
                sqlPureBody += " INNER JOIN " + ActiveDemandData.TABLE_NAME + " actdd"
                    + " ON dd." + DemandData.ID_FIELD + "=actdd." + ActiveDemandData.ID_FIELD;
            }

            if (isNomenclatureFiltered) {
                //sqlPureBody += " INNER JOIN " + DemandNomenclatureData.TABLE_NAME + " dnd"
                //    + " ON dd." + DemandData.ID_FIELD + "=dnd." + DemandNomenclatureData.DEMAND_ID_FIELD
                //    + " AND dd." + DemandData.VERSION_FIELD + "=dnd." + DemandNomenclatureData.DEMAND_VERSION_FIELD
                //    + " INNER JOIN " + NomenclatureData.TABLE_NAME + " nd" 
                //    + " ON nd." + NomenclatureData.ID_FIELD + "=dnd." + DemandNomenclatureData.NOMENCLATURE_ID_FIELD
                //    + " AND nd." + NomenclatureData.VERSION_FIELD + "=dnd." + DemandNomenclatureData.NOMENCLATURE_VERSION_FIELD;
                sqlPureBody += " INNER JOIN"
                            + " ("
                            + " SELECT DISTINCT axdd." + DemandData.ID_FIELD + " FROM " + DemandData.TABLE_NAME + " axdd"
                            + " INNER JOIN " + DemandNomenclatureData.TABLE_NAME + " dnd ON axdd." + DemandData.ID_FIELD + "= dnd." + DemandNomenclatureData.DEMAND_ID_FIELD 
                            + " AND axdd." + DemandData.VERSION_FIELD + "= dnd." + DemandNomenclatureData.DEMAND_VERSION_FIELD
                            + " INNER JOIN " + NomenclatureData.TABLE_NAME + " nd ON nd." + NomenclatureData.ID_FIELD + "= dnd." + DemandNomenclatureData.NOMENCLATURE_ID_FIELD
                            + " AND nd." + NomenclatureData.VERSION_FIELD + "= dnd." + DemandNomenclatureData.NOMENCLATURE_VERSION_FIELD
                            + " WHERE axdd." + DemandData.LAST_VERSION_FIELD + "= 1 AND axdd." + DemandData.IS_ACTIVE_FIELD + "= 1 "
                            + " AND nd." + NomenclatureData.NOMENCLATURE_KEY_FIELD + " LIKE '%" + strNomeclatreKeyFilter + "%'"
                            + ") axdd"
                            + " ON dd.id = axdd.id";
            }

            if (isSupplierFilter) {
                sqlPureBody += " INNER JOIN " + SupplierData.TABLE_NAME + " sd"
                    + " ON sd." + SupplierData.ID_FIELD + "=dd." + DemandData.SUPPLIER_ID_FIELD;
            } else if (isSupplierSort) {
                sqlPureBody += " LEFT OUTER JOIN " + SupplierData.TABLE_NAME + " sd"
                    + " ON sd." + SupplierData.ID_FIELD + "=dd." + DemandData.SUPPLIER_ID_FIELD;
            }

            if (isRequestorFilter) {
                sqlPureBody += " INNER JOIN " + ScmuserData.TABLE_NAME + " urd"
                    + " ON urd." + ScmuserData.ID_FIELD + "=dd." + DemandData.REQUESTOR_ID_FIELD;
            } else if (isRequestorSort) {
                sqlPureBody += " LEFT OUTER JOIN " + ScmuserData.TABLE_NAME + " urd"
                    + " ON urd." + ScmuserData.ID_FIELD + "=dd." + DemandData.REQUESTOR_ID_FIELD;
            }

            if (isAppManFilter) {
                sqlPureBody += " INNER JOIN " + ScmuserData.TABLE_NAME + " uad"
                    + " ON uad." + ScmuserData.ID_FIELD + "=dd." + DemandData.APP_MAN_ID_FIELD;
            } else if(isAppManSort) {
                sqlPureBody += " LEFT OUTER JOIN " + ScmuserData.TABLE_NAME + " uad"
                    + " ON uad." + ScmuserData.ID_FIELD + "=dd." + DemandData.APP_MAN_ID_FIELD;
            }

            sqlPureBody += " WHERE dd." + DemandData.LAST_VERSION_FIELD + "=1";
        

            sqlPureBody += strFilterWhere;

            return sqlPureBody;
        }

        private string GetFilter(
            List<WcfFilterField> filterFields,
            out bool isActiveOnly, 
            out bool isNomeclatreFilter,
            out string strNomeclatreKeyFilter,
            out bool isSupplierFilter,
            out bool isRequestorFilter,
            out bool isAppManFilter) {

            isActiveOnly = false;
            isNomeclatreFilter = false;
            strNomeclatreKeyFilter = null;
            isSupplierFilter = false;
            isRequestorFilter = false;
            isAppManFilter = false;


            string strFilterWhere = "";
            if (filterFields != null) {
                
                foreach (var filterField in filterFields) {

                    if (!String.IsNullOrWhiteSpace(filterField.SqlFilter)) {
                        strFilterWhere += " AND " + filterField.SqlFilter;
                    } else {
                        //string[] strItemProp = null;//filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                        if (String.IsNullOrWhiteSpace(filterField.FieldName)) {
                            continue;
                        }
                        if (String.IsNullOrWhiteSpace(filterField.FilterText)) {
                            continue;
                        }

                        strFilterWhere += " AND ";

                        string columnName = filterField.FieldName.Trim().ToUpper();
                        if (columnName == DemandData.DEMAND_NR_FIELD.Trim().ToUpper()) {
                            strFilterWhere += "dd." + DemandData.DEMAND_NR_FIELD + " LIKE '%" + filterField.FilterText.Trim() + "%'";
                        } else if (columnName == DemandData.REQUESTOR_ID_FIELD.Trim().ToUpper()) {
                            strFilterWhere += "dd." + DemandData.REQUESTOR_ID_FIELD + "=" + filterField.FilterText.Trim();
                        } else if (columnName == DemandData.STATUS_ID_FIELD.Trim().ToUpper()) {
                            strFilterWhere += "dd." + DemandData.STATUS_ID_FIELD + "=" + filterField.FilterText.Trim();
                        } else if (columnName == DemandData.CREATED_DATE_FIELD.Trim().ToUpper()) {
                            if (filterField.FromTo == (int)FilterFromTo.From) {
                                strFilterWhere += "dd." + DemandData.CREATED_DATE_FIELD + ">=" + filterField.FilterText.Trim();
                            } else if (filterField.FromTo == (int)FilterFromTo.To) {
                                strFilterWhere += "dd." + DemandData.CREATED_DATE_FIELD + "<=" + filterField.FilterText.Trim();
                            } else {
                                strFilterWhere += "dd." + DemandData.CREATED_DATE_FIELD + "=" + filterField.FilterText.Trim();
                            }
                        } else if (columnName == DemandData.LAST_STATUS_MODIF_DATE_FIELD.Trim().ToUpper()) {
                            if (filterField.FromTo == (int)FilterFromTo.From) {
                                strFilterWhere += "dd." + DemandData.LAST_STATUS_MODIF_DATE_FIELD + ">=" + filterField.FilterText.Trim();
                            } else if (filterField.FromTo == (int)FilterFromTo.To) {
                                strFilterWhere += "dd." + DemandData.LAST_STATUS_MODIF_DATE_FIELD + "<=" + filterField.FilterText.Trim();
                            } else {
                                strFilterWhere += "dd." + DemandData.LAST_STATUS_MODIF_DATE_FIELD + "=" + filterField.FilterText.Trim();
                            }
                        } else if (columnName == DemandData.IS_ACTIVE_FIELD.Trim().ToUpper()) {
                            if (filterField.FilterText.Trim().ToUpper() == "1") {
                                isActiveOnly = true;
                            }
                            strFilterWhere += "dd." + DemandData.IS_ACTIVE_FIELD + "=" + filterField.FilterText.Trim();

                        } else if (columnName.ToLower() == "nomenclatures_text") {
                            isNomeclatreFilter = true;
                            strNomeclatreKeyFilter = filterField.FilterText.Trim();
                            //strFilterWhere += "nd." + NomenclatureData.NOMENCLATURE_KEY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                            strFilterWhere += "1=1";
                        } else if (columnName.ToLower() == SupplierData.SUPP_NAME_FIELD) {
                            isSupplierFilter = true;

                            strFilterWhere += "(sd." + SupplierData.SUPP_NAME_WO_DIA_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(filterField.FilterText.Trim()) + "%' OR sd."
                                + SupplierData.SUPPLIER_ID_FIELD + " LIKE '%" + filterField.FilterText.Trim() + "%')";

                        } else if (columnName.ToLower() == "requestor_name") {
                            isRequestorFilter = true;
                            strFilterWhere += "(urd." + ScmuserData.SURNAME_WO_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(filterField.FilterText.Trim()) + "%' OR urd."
                                + ScmuserData.FIRST_NAME_WO_DIA_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(filterField.FilterText.Trim()) + "%')";
                        } else if (columnName.ToLower() == "app_man_name") {
                            isAppManFilter = true;
                            strFilterWhere += "(uad." + ScmuserData.SURNAME_WO_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(filterField.FilterText.Trim()) + "%' OR uad."
                                + ScmuserData.FIRST_NAME_WO_DIA_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(filterField.FilterText.Trim()) + "%')";
                        }
                    }

                }
            }

            return strFilterWhere;
        }

        private string GetOrder(
            string sort, 
            out bool isSupplierSort,
            out bool isRequestorSort,
            out bool isAppManSort) {

            isSupplierSort = false;
            isRequestorSort = false;
            isAppManSort = false;

            string strOrder = "ORDER BY dd." + DemandData.ID_FIELD;

            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    } else if (strItemProp[0] == DemandData.DEMAND_NR_FIELD) {
                        strOrder = "dd." + DemandData.DEMAND_NR_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0].ToUpper() == DemandData.STATUS_ID_FIELD.ToUpper() || strItemProp[0].ToUpper() == "STATUS_TEXT") {
                        strOrder = "dd." + DemandData.STATUS_ID_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0] == "last_status_modif_date_text") {
                        strOrder = "dd." + DemandData.LAST_STATUS_MODIF_DATE_FIELD + " " + strItemProp[1];
                    } else if (strItemProp[0].ToLower() == "supplier_text") {
                        strOrder = "sd." + SupplierData.SUPP_NAME_FIELD + " " + strItemProp[1];
                        isSupplierSort = true;
                    } else if (strItemProp[0].ToLower() == "requestor_name") {
                        strOrder = "urd." + ScmuserData.SURNAME_FIELD + " " + strItemProp[1] + "," + "urd." + ScmuserData.FIRST_NAME_FIELD + " " + strItemProp[1];
                        isRequestorSort = true;
                    } else if (strItemProp[0].ToLower() == "app_man_name") {
                        strOrder = "uad." + ScmuserData.SURNAME_FIELD + " " + strItemProp[1] + "," + "uad." + ScmuserData.FIRST_NAME_FIELD + " " + strItemProp[1];
                        isAppManSort = true;
                    }
                }

                if (string.IsNullOrWhiteSpace(strOrder)) {
                    strOrder = "dd." + DemandData.ID_FIELD;
                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        public int GetPendingDemNumber(int userId) {

            if (userId > -1) {
                var i = (from demDb in m_dbContext.Demand
                         join actDemDb in m_dbContext.Active_Demand
                         on demDb.id equals actDemDb.id
                         where ((demDb.requestor_id == userId && actDemDb.is_requestor_acive == true)
                         || (demDb.app_man_id == userId && actDemDb.is_app_man_active == true))
                         && demDb.last_version == true
                         && demDb.is_active == true
                         select new { id = demDb.id }).ToList().Count;

                return i;
            } else {
                var i = (from demDb in m_dbContext.Demand
                         join actDemDb in m_dbContext.Active_Demand
                         on demDb.id equals actDemDb.id
                         where demDb.last_version == true
                         && demDb.is_active == true
                         select new { id = demDb.id }).ToList().Count;

                return i;
            }
        }

        public DemandExtend GetDemandById(int id, int currentUserId) {
            var dbDemand = (from demandDb in m_dbContext.Demand
                          where demandDb.id == id 
                          && demandDb.last_version == true
                          select demandDb).FirstOrDefault();

            DemandExtend demandExtend = new DemandExtend();
            SetValues(dbDemand, demandExtend);
            demandExtend.requestor_name = dbDemand.Requestor.surname + " " + dbDemand.Requestor.first_name;
            demandExtend.supplier_text = dbDemand.Supplier.supp_name + " (" + dbDemand.Supplier.supplier_id + ")";
            if (demandExtend.created_date != null) {
                demandExtend.created_date_text = demandExtend.created_date.ToString("dd.MM.yyyy HH:mm");
            }

            demandExtend.img_status_path = GetStatusImagePath(demandExtend.status_id);

            //Nomenclatures
            demandExtend.demand_nomenclatures_extend = new List<DemandNomenclatureExtend>();
            NomenclatureRepository nomRep = new NomenclatureRepository();

            var demNoms = (from nomDb in dbDemand.Demand_Nomenclature
                        where nomDb.demand_version == demandExtend.version
                        orderby nomDb.id descending
                        select nomDb).ToList();

            Hashtable htNomIds = new Hashtable();

            foreach (var demNom in demNoms) {
                if (htNomIds.ContainsKey(demNom.nomenclature_id)) {
                    continue;
                }

                htNomIds.Add(demNom.nomenclature_id, null);

                //var nom = nomRep.GetNomenclatureById(demNom.nomenclature_id);
                DemandNomenclatureExtend demandNomenclatureExtend = new DemandNomenclatureExtend();
                SetValues(demNom, demandNomenclatureExtend);
                Nomenclature nom = (from nomDb in m_dbContext.Nomenclature
                                    where nomDb.id == demNom.Nomenclature.id
                                    && nomDb.last_version == true
                                    select nomDb).FirstOrDefault();

                if (nom == null) {
                    //this is a workaroud, it sometimes happens that nimenclature does not have a last version
                    //there should be found out why there are nomenclatures without last version flag
                    var nomFix = (from nomDb in m_dbContext.Nomenclature
                                        where nomDb.id == demNom.Nomenclature.id
                                        orderby nomDb.version descending
                                        select nomDb).ToList();
                    if (nomFix != null && nomFix.Count > 0) {
                        nomFix.ElementAt(0).last_version = true;
                        m_dbContext.SaveChanges();
                    }

                    nom = (from nomDb in m_dbContext.Nomenclature
                           where nomDb.id == demNom.Nomenclature.id
                           && nomDb.last_version == true
                           select nomDb).FirstOrDefault();
                }

                demandNomenclatureExtend.nomenclature_key = nom.nomenclature_key;
                demandNomenclatureExtend.name = demNom.Nomenclature.name;
                demandNomenclatureExtend.specification = demNom.Nomenclature.specification;
                demandNomenclatureExtend.status_id = demNom.status_id;
                demandNomenclatureExtend.source_id = nom.source_id;


                demandNomenclatureExtend.img_status_path = GetStatusImagePath(demNom.status_id);
                if (demandNomenclatureExtend.price != null) {
                    demandNomenclatureExtend.price_text = ConvertData.FormatNumber((decimal)demandNomenclatureExtend.price);
                }
                demandNomenclatureExtend.price_text_orig = demandNomenclatureExtend.price_text;
                demandNomenclatureExtend.currency_id_orig = demandNomenclatureExtend.currency_id;
                if (demNom.Nomenclature.status_id == DemandRepository.DEM_STATUS_PRICE_CONFIRMED) {
                    demandNomenclatureExtend.lock_visibility = Visibility.Visible;
                    demandNomenclatureExtend.is_locked = true;
                    demandNomenclatureExtend.read_only_visibility = Visibility.Visible;
                    demandNomenclatureExtend.edit_visibility = Visibility.Collapsed;
                    //demandNomenclatureExtend.select_visibility = Visibility.Collapsed;
                    demandNomenclatureExtend.remark = LOCK_REASON_PRICE_SET;
                    if (demNom.Nomenclature.price != null) {
                        demandNomenclatureExtend.remark += demNom.Nomenclature.price.ToString();
                    }
                    if (demNom.Nomenclature.Currency != null) {
                        demandNomenclatureExtend.remark += " " + demNom.Nomenclature.Currency.currency_code;
                    }
                } else if (demNom.locked_by_demand_id != null) {
                    demandNomenclatureExtend.lock_visibility = Visibility.Visible;
                    demandNomenclatureExtend.read_only_visibility = Visibility.Visible;
                    demandNomenclatureExtend.edit_visibility = Visibility.Collapsed;
                    //demandNomenclatureExtend.select_visibility = Visibility.Collapsed;
                    demandNomenclatureExtend.remark = LOCK_REASON_OTHERDEMAND + demNom.DemandLock.demand_nr;
                } else {
                    if (demandExtend.requestor_id == currentUserId
                    && (demandNomenclatureExtend.status_id == NomenclatureRepository.NOM_STATUS_SENT_TO_SUPPLIER
                    || demandNomenclatureExtend.status_id == NomenclatureRepository.NOM_STATUS_SUPPLIER_REPLIED)) {
                        demandNomenclatureExtend.lock_visibility = Visibility.Collapsed;
                        demandNomenclatureExtend.read_only_visibility = Visibility.Collapsed;
                        demandNomenclatureExtend.edit_visibility = Visibility.Visible;
                    } else {
                        demandNomenclatureExtend.lock_visibility = Visibility.Collapsed;
                        demandNomenclatureExtend.read_only_visibility = Visibility.Visible;
                        demandNomenclatureExtend.edit_visibility = Visibility.Collapsed;
                    }
                }

                if (demNom.currency_id != null) {
                    demandNomenclatureExtend.currency_text = demNom.Currency.currency_code;
                }

                //other demands
                var otherDemands = (from demandDb in m_dbContext.Demand
                                   join demNomDb in m_dbContext.Demand_Nomenclature
                                   on demandDb.id equals demNomDb.demand_id
                                   where demandDb.version == demNomDb.demand_version 
                                   && demNomDb.nomenclature_id == demNom.nomenclature_id
                                   //&& demNomDb.nomenclature_version == demNom.nomenclature_version
                                   && demandDb.last_version == true
                                   && demNomDb.demand_id != demNom.demand_id
                                   select demandDb).ToList();
                demandNomenclatureExtend.other_demands = new List<DemandLight>();
                if (otherDemands != null) {
                    foreach (var otherDemand in otherDemands) {
                        DemandLight newDemandLight = new DemandLight();
                        newDemandLight.demand_nr = otherDemand.demand_nr;
                        newDemandLight.img_status_path = GetStatusImagePath(otherDemand.status_id);
                        newDemandLight.status_id = otherDemand.status_id;

                        if (otherDemand.supplier_id != null) {
                            newDemandLight.supplier_name = otherDemand.Supplier.supp_name + " (" + otherDemand.Supplier.supplier_id + ")";
                        }

                        var otherDemNom = (from otherDemandNomDb in otherDemand.Demand_Nomenclature
                                           where otherDemandNomDb.nomenclature_id == demNom.nomenclature_id
                                           select otherDemandNomDb).FirstOrDefault();
                        string priceText = "";
                        if (otherDemNom.price != null) {
                            priceText = ConvertData.ToString((decimal)otherDemNom.price, 2);
                        }
                        if (otherDemNom.currency_id != null) {
                            priceText += " " + otherDemNom.Currency.currency_code;
                        }
                        newDemandLight.price_text = priceText;
                        demandNomenclatureExtend.other_demands.Add(newDemandLight);
                    }
                }

                bool isRequestor = (dbDemand.requestor_id == currentUserId);
                bool isAppMan = (dbDemand.app_man_id == currentUserId);
                SetSelectedVisibility(demandNomenclatureExtend, demNom.Nomenclature.status_id, isRequestor, isAppMan);
                SetRemoveVisibility(demandNomenclatureExtend, demNom.Nomenclature.status_id, currentUserId, dbDemand.requestor_id);
                
                demandExtend.demand_nomenclatures_extend.Add(demandNomenclatureExtend);
                demandExtend.demand_nomenclatures_extend = demandExtend.demand_nomenclatures_extend.OrderBy(x => x.nomenclature_key).ToList();
            }

            demandExtend.recipient_attachments_extend = new List<AttachmentExtend>();
            demandExtend.supplier_attachments_extend = new List<AttachmentExtend>();
            AttachmentRepository attRep = new AttachmentRepository();
            var atts = (from attDb in dbDemand.Demand_Attachment
                        where attDb.demand_version == demandExtend.version
                        select attDb).ToList();

            foreach (var demAtt in atts) {
                
                var attLight = attRep.GetLightAttachmentById(demAtt.attachment_id);
                AttachmentExtend attExtend = new AttachmentExtend();
                SetValues(attLight, attExtend);
                var user = new UserRepository().GetUserById(attLight.modif_user_id);
                attExtend.added_by = "( " + user.surname + " " + user.first_name + " )";

                attExtend.att_type = demAtt.att_type;
                if (demAtt.att_type == AttachmentRepository.ATT_TYPE_MAIL_RECIPIENT) {
                    demandExtend.recipient_attachments_extend.Add(attExtend);
                } else if (demAtt.att_type == AttachmentRepository.ATT_TYPE_GENERATED_DEMAND) {
                    demandExtend.recipient_attachments_extend.Add(attExtend);
                } else if (demAtt.att_type == AttachmentRepository.ATT_TYPE_RECIPIENT) {
                    demandExtend.recipient_attachments_extend.Add(attExtend);
                } else if (demAtt.att_type == AttachmentRepository.ATT_TYPE_SUPPLIER) {
                    demandExtend.supplier_attachments_extend.Add(attExtend);
                }
                
            }
                        
            return demandExtend;
        }

        private void SetSelectedVisibility(
            DemandNomenclatureExtend demandNomenclatureExtend, 
            int nomenclatureStatus,
            bool isReqestor, 
            bool isAppMan) {

            if (nomenclatureStatus == NomenclatureRepository.NOM_STATUS_PRICE_CONFIRMED) {
                demandNomenclatureExtend.select_visibility = Visibility.Collapsed;
                return;
            }

            if (!isReqestor && !isAppMan) {
                demandNomenclatureExtend.select_visibility = Visibility.Collapsed;
                return;
            }

            if (isReqestor) {
                switch (demandNomenclatureExtend.status_id) {
                    case NomenclatureRepository.NOM_STATUS_SENT_TO_SUPPLIER:
                    case NomenclatureRepository.NOM_STATUS_SUPPLIER_REPLIED:
                    case NomenclatureRepository.NOM_STATUS_WAIT_FOR_APPROVAL:
                    case NomenclatureRepository.NOM_STATUS_APPROVED:
                    case NomenclatureRepository.NOM_STATUS_REJECTED:
                    case NomenclatureRepository.NOM_STATUS_PRICE_SET:
                        demandNomenclatureExtend.select_visibility = Visibility.Visible;
                        break;
                    default:
                        demandNomenclatureExtend.select_visibility = Visibility.Collapsed;
                        break;
                }
            }

            if (isAppMan) {
                switch (demandNomenclatureExtend.status_id) {
                    case NomenclatureRepository.NOM_STATUS_WAIT_FOR_APPROVAL:
                    case NomenclatureRepository.NOM_STATUS_APPROVED:
                    case NomenclatureRepository.NOM_STATUS_REJECTED:
                        demandNomenclatureExtend.select_visibility = Visibility.Visible;
                        break;
                    default:
                        demandNomenclatureExtend.select_visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void SetRemoveVisibility(
            DemandNomenclatureExtend demandNomenclatureExtend,
            int nomenclatureStatus,
            int currentUser,
            int requestorId) {

            if (currentUser != requestorId) {
                demandNomenclatureExtend.remove_visibility = Visibility.Collapsed;
                return;
            }

        }

            public int AddAttachment(int demandId, string fileName, byte[] fileContent, byte[] fileIcon, int attType, int userId) {
            using (TransactionScope transaction = new TransactionScope()) {
                var dbDemand = (from demandDb in m_dbContext.Demand
                                where demandDb.id == demandId
                                && demandDb.last_version == true
                                select demandDb).FirstOrDefault();

                Attachment attachment = new Attachment();
                attachment.file_name = fileName;
                attachment.file_content = fileContent;
                attachment.file_icon = fileIcon;
                attachment.modif_user_id = userId;
                attachment.modif_date = DateTime.Now;
                int attId = new AttachmentRepository().AddAttachment(attachment, userId);

                if (attType == AttachmentRepository.ATT_TYPE_SUPPLIER && dbDemand.status_id == (int)Status.Sent) {
                    int lastVersion = dbDemand.version;
                    int newVersion = ++lastVersion;

                    var newDemandVersion = GetNewDemandVersion(dbDemand, newVersion, userId);
                    SetDemandSysItems(newDemandVersion, userId);

                    newDemandVersion.status_id = (int)Status.Replied;

                    Demand_Attachment da = new Demand_Attachment();
                    da.demand_id = newDemandVersion.id;
                    da.demand_version = newDemandVersion.version;
                    da.attachment_id = attId;
                    da.att_type = attType;
                    if (newDemandVersion.Demand_Attachment == null) {
                        newDemandVersion.Demand_Attachment = new List<Demand_Attachment>();
                    }
                    newDemandVersion.Demand_Attachment.Add(da);

                    //Update Demand Nomenclatures
                    NomenclatureRepository nomenclatureRepository = new NomenclatureRepository();
                    foreach (var demNom in newDemandVersion.Demand_Nomenclature) {
                        if (demNom.status_id == (int)Status.Sent) {
                            demNom.status_id = (int)Status.Replied;
                        }
                        nomenclatureRepository.UpdateNomenclatureStatus(m_dbContext, demNom, (int)Status.Replied, userId);
                    }
                    m_dbContext.SaveChanges();


                    m_dbContext.Demand.Add(newDemandVersion);
                } else {


                    Demand_Attachment da = new Demand_Attachment();
                    da.demand_id = demandId;
                    da.demand_version = dbDemand.version;
                    da.attachment_id = attId;
                    da.att_type = attType;
                    if (dbDemand.Demand_Attachment == null) {
                        dbDemand.Demand_Attachment = new List<Demand_Attachment>();
                    }
                    dbDemand.Demand_Attachment.Add(da);
                }

                
                SaveChanges();

                transaction.Complete();

                return attId;
            }
        }

        //public void UpdateNomenclatureStatus(Demand_Nomenclature demNom, int statusId, int userId) {
        //    NomenclatureRepository nomenclatureRepository = new NomenclatureRepository();

        //    var nom = (from nomDb in m_dbContext.Nomenclature
        //               where nomDb.id == demNom.nomenclature_id
        //               && nomDb.last_version == true
        //               select nomDb).FirstOrDefault();

        //    int nomStatus = nomenclatureRepository.GetNomenclatureStatus(nom, demNom.demand_id, statusId);
        //    if (nom.status_id != nomStatus) {
        //        var newNomVersion = nomenclatureRepository.GetNomenclatureNewVersion(nom, userId);
        //        int newVersion = newNomVersion.version;
        //        SetValues(nom, newNomVersion);
        //        newNomVersion.status_id = statusId;
        //        newNomVersion.version = newVersion;
        //        newNomVersion.modif_user_id = userId;
        //        newNomVersion.modif_date = DateTime.Now;
        //        newNomVersion.last_version = true;
        //        m_dbContext.Nomenclature.Add(newNomVersion);

        //        nom.last_version = false;

        //        demNom.nomenclature_version = newVersion;
        //    }
        //}

        public void DeleteAttachment(int demandId, int attachmentId, int userId) {
            using (TransactionScope transaction = new TransactionScope()) {
                var dbDemand = (from demandDb in m_dbContext.Demand
                                where demandDb.id == demandId
                                && demandDb.last_version == true
                                select demandDb).FirstOrDefault();

                int lastVersion = dbDemand.version;
                int newVersion = ++lastVersion;
                var newDemandVersion = GetNewDemandVersion(dbDemand, newVersion, userId);
                SetDemandSysItems(newDemandVersion, userId);

                var demAtt = (from demAttDb in newDemandVersion.Demand_Attachment
                              where demAttDb.attachment_id == attachmentId
                              select demAttDb).FirstOrDefault();
                newDemandVersion.Demand_Attachment.Remove(demAtt);

                m_dbContext.Demand.Add(newDemandVersion);

                SaveChanges();

                transaction.Complete();

            }
        }

        public Demand GetNewDemandVersion(Demand origDemand, int newVersion, int userId) {
            int oldVersion = origDemand.version;
            
            Demand newDemandVersion = new Demand();
            SetValues(origDemand, newDemandVersion);

            origDemand.last_version = false;


            SetDemandSysItems(newDemandVersion, userId);
            newDemandVersion.version = newVersion;

            int lastDemNomId = new DemandNomenclatureRepository().GetLastId();
            int newDemNomId = ++lastDemNomId;

            //Nomenclatures
            List<int> processedNomIds = new List<int>();
            newDemandVersion.Demand_Nomenclature = new List<Demand_Nomenclature>();
            var demNoms = (from nomDb in origDemand.Demand_Nomenclature
                        where nomDb.demand_version == oldVersion
                        orderby nomDb.id descending
                        select nomDb).ToList();
            foreach (var demNom in demNoms) {
                if (processedNomIds.Contains(demNom.nomenclature_id)) {
                    continue;
                }

                processedNomIds.Add(demNom.nomenclature_id);

                Demand_Nomenclature newDemNom = new Demand_Nomenclature();
                SetValues(demNom, newDemNom);
                newDemNom.id = newDemNomId;
                newDemNomId++;
                //demNom.demand_id = newDemandVersion.id;
                newDemNom.demand_version = newVersion;
                newDemNom.modif_date = DateTime.Now;
                newDemNom.modif_user_id = userId;
                //demNom.nomenclature_id = nom.nomenclature_id;
                newDemandVersion.Demand_Nomenclature.Add(newDemNom);
            }

            //Attachments
            newDemandVersion.Demand_Attachment = new List<Demand_Attachment>();
            var demAtts = (from attDb in origDemand.Demand_Attachment
                        where attDb.demand_version == oldVersion
                        select attDb).ToList();
            foreach (var demAtt in demAtts) {
                Demand_Attachment newDemAtt = new Demand_Attachment();
                SetValues(demAtt, newDemAtt);
                //demAtt.demand_id = newDemandVersion.id;
                newDemAtt.demand_version = newVersion;
                //demAtt.attachment_id = att.attachment_id;
                newDemandVersion.Demand_Attachment.Add(newDemAtt);
            }

            ////Active Demands
            //var aciveDemands = (from actDem in origDemand.Active_Demand
            //               where actDem.demand_version == oldVersion
            //               select actDem).ToList();
            //foreach (var demAtt in demAtts) {
            //    Demand_Attachment newDemAtt = new Demand_Attachment();
            //    SetValues(demAtt, newDemAtt);
            //    //demAtt.demand_id = newDemandVersion.id;
            //    newDemAtt.demand_version = newVersion;
            //    //demAtt.attachment_id = att.attachment_id;
            //    newDemandVersion.Demand_Attachment.Add(newDemAtt);
            //}

            return newDemandVersion;
        }

        public int GetDemandPreviousStatus(int demandId) {
            var dbDemands = (from demandDb in m_dbContext.Demand
                            where demandDb.id == demandId
                            orderby demandDb.version descending
                            select demandDb).ToList();

            int iStatus = dbDemands.ElementAt(0).status_id;
            for (int i = 1; i < dbDemands.Count; i++) {
                if (dbDemands.ElementAt(i).status_id != iStatus) {
                    return dbDemands.ElementAt(i).status_id;
                }
            }

            return iStatus;
        }

        public void PopulateDemandNomStatusDemands() {
            var demands = (from demandDb in m_dbContext.Demand
                           where demandDb.last_version == true
                           select demandDb).ToList();

            foreach (var demand in demands) {
                if (demand.Demand_Nomenclature == null || demand.Demand_Nomenclature.Count == 0) {
                    continue;
                }
                foreach (var demNom in demand.Demand_Nomenclature) {
                    Console.WriteLine(demand.id);
                    demNom.status_id = demand.status_id;

                    bool isReqActive = false;
                    bool isAppManActive = false;
                    //switch (demand.status_id) {
                    //    case (DEM_STATUS_SENT):
                    //    case (DEM_STATUS_SUPPLIER_REPLIED):
                    //    case (DEM_STATUS_REJECTED):
                    //        isReqActive = true;
                    //        isAppManActive = false;
                    //        break;
                    //    case (DEM_STATUS_WAIT_FOR_APPROVAL):
                    //        isReqActive = false;
                    //        isAppManActive = true;
                    //        break;
                    //}

                    ActiveDemandActivity(demand.status_id, out isReqActive, out isAppManActive);

                    if (isReqActive || isAppManActive) {
                        var actDemand = (from actDemDb in m_dbContext.Active_Demand
                                         where actDemDb.id == demand.id
                                         select actDemDb).FirstOrDefault();

                        if (actDemand == null) {
                            Active_Demand actDem = new Active_Demand();
                            actDem.id = demand.id;
                            actDem.is_app_man_active = isAppManActive;
                            actDem.is_requestor_acive = isReqActive;
                            actDem.status_id = demand.status_id;
                        } else {
                            actDemand.is_app_man_active = isAppManActive;
                            actDemand.is_requestor_acive = isReqActive;
                        }
                    }

                    //SaveChanges(); //- to prevent run it in PROD again
                }
            }
        }

        private void ActiveDemandActivity(int statusId, out bool isReqActive, out bool isAppManActive) {
            isReqActive = false;
            isAppManActive = false;
            switch (statusId) {
                case (DEM_STATUS_SENT):
                case (DEM_STATUS_SUPPLIER_REPLIED):
                case (DEM_STATUS_REJECTED):
                    isReqActive = true;
                    isAppManActive = false;
                    break;
                case (DEM_STATUS_WAIT_FOR_APPROVAL):
                    isReqActive = false;
                    isAppManActive = true;
                    break;
            }
        }

        public void SetIdKey() {
            var demNoms = (from demandDb in m_dbContext.Demand_Nomenclature
                           select demandDb).ToList();

            for (int i=0; i< demNoms.Count; i++) {
                Console.WriteLine(i);

                demNoms.ElementAt(i).id = i;

                    //SaveChanges(); //- to prevent run it in PROD again
                
            }
        }

        
        #endregion
    }
}
