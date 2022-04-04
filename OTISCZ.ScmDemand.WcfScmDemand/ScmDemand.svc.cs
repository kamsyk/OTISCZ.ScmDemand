using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.Model.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;

namespace OTISCZ.ScmDemand.WcfScmDemand {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ScmDemand" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ScmDemand.svc or ScmDemand.svc.cs at the Solution Explorer and start debugging.
    public class ScmDemand : IScmDemand {
        #region Constructor
        //public ScmDemand(string password) {

        //}
        #endregion

        #region Methods
        public ScmUser GetUserData(string userName) {
            return new UserRepository().GetUserByUserNameWs(userName);
        }

        public string GetUserMail(int userId) {
            return new UserRepository().GetUserUserMail(userId);
        }

        public string GetUserName(int userId) {
            return new UserRepository().GetUserUserName(userId);
        }

        public void SetUserCulture(int userId, string culture) {
            new UserRepository().SetUserCulture(userId, culture);
        }

        public ScmSetting GetScmSetting() {
            return new SettingRepository().GetScmSetting();
        }

        public void SetImportFolder(string folder) {
            new SettingRepository().SetImportFolder(folder);
        }

        public int ImportNomenclature(Nomenclature lineNomenclature, int userId) {
            return new NomenclatureRepository().ImportNomenclature(lineNomenclature, userId);
        }

        public List<Unit> GetUnits() {
            return new UnitRepository().GetUnits();
        }

        public List<Material_Group> GetMaterialGroups() {
            return new MaterialGroupRepository().GetMaterialGroups();
        }

        public List<NomenclatureExtend> GetNomenclatures(
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount) {

            return new NomenclatureRepository().GetNomenclatures(
                filter,
                sort,
                pageSize,
                pageNr,
                out rowsCount);
        }

        public NomenclatureDetailExtend GetNomenclatureById(int nomenclatureId) {
            return new NomenclatureRepository().GetNomenclatureById(nomenclatureId);
        }

        public void SaveNomenclature(NomenclatureExtend nomenclature, int userId) {
            int nomVersion = -1;
            new NomenclatureRepository().SaveNomenclature(nomenclature, userId, true, out nomVersion);
        }

        public void SaveDemandNomenclature(DemandNomenclatureExtend demNom, int userId) {
            new DemandNomenclatureRepository().SaveDemandNomenclature(demNom, userId);
        }

        public List<NomenclatureExtend> GetNomenclaturesReport(
            string filter,
            string sort) {

            return new NomenclatureRepository().GetNomenclaturesReport(
                filter,
                sort);
        }

        public List<SupplierExtend> GetSuppliersReport(
            string filter,
            string sort) {

            return new SupplierRepository().GetSuppliersReport(
                filter,
                sort);
        }

        public Supplier GetSupplierBySupplierId(string supplierId) {
            return new SupplierRepository().GetSupplierBySupplierId(supplierId);
        }

        //public void UpdateSupplier(Supplier supplier, int userId) {
        //    new SupplierRepository().UpdateSupplier(supplier, userId);
        //}

        public void DeactiveSuppliers(List<int> htActivesuppliersIds, int userId) {
            new SupplierRepository().DeactiveSuppliers(htActivesuppliersIds, userId);
        }

        public List<SupplierExtend> GetSuppliers(
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount) {

            return new SupplierRepository().GetSuppliers(
                filter,
                sort,
                pageSize,
                pageNr,
                out rowsCount);
        }

        public int SaveDemand(DemandExtend demand, int userId) {
            return new DemandRepository().SaveDemand(demand, userId, false);
        }

        public int SaveDemandWasSent(DemandExtend demand, int userId) {
            return new DemandRepository().SaveDemand(demand, userId, true);
        }

        public string GetNewDemandRequestNr() {
            int lastInex = new DemandRepository().GetNewNumberIndex();

            string nbr = lastInex.ToString();
            while (nbr.Length < 4) {
                nbr = "0" + nbr;
            }

            string demandNr = "RFQ-" + DateTime.Now.Year.ToString().Substring(2) + "-" + nbr;

            return demandNr;
        }


        public List<DemandExtend> GetDemands(
            List<WcfFilterField> filterFields,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount) {

            return new DemandRepository().GetDemands(
                filterFields,
                sort,
                pageSize,
                pageNr,
                out rowsCount);
        }

        public List<DemandExtend> GetDemandsReport(
            List<WcfFilterField> filterFields,
            string sort) {
            return new DemandRepository().GetDemandsReport(
                filterFields,
                sort);
        }

        public int GetPendingNomenclaturesNumber() {
            return new NomenclatureRepository().GetPendingNomNumber();
        }

        public int GetPendingDemandsNumber(int userId) {
            return new DemandRepository().GetPendingDemNumber(userId);
        }

        public DemandExtend GetDemandDetail(int demandId, int userId) {
            var demandExtend = new DemandRepository().GetDemandById(demandId, userId);

            return demandExtend;
        }

        public Byte[] GetAttachmentContent(int attId) {
            byte[] attContent = new AttachmentRepository().GetAttachmentContent(attId);
            return attContent;
        }

        public int AddAttachment(int demandId, string fileName, byte[] fileContent, byte[] fileIcon, int attType, int userId) {
            return new DemandRepository().AddAttachment(demandId, fileName, fileContent, fileIcon, attType, userId);
        }

        public void DeleteAttachment(int demandId, int attachmentId, int userId) {
            new DemandRepository().DeleteAttachment(demandId, attachmentId, userId);
        }

        public List<ScmUserExtend> GetActiveAppMen() {
            return new UserRepository().GetActiveAppMen();
        }

        public List<Currency> GetActiveCurrencies() {
            return new CurrencyRepository().GetAtiveCurrencies();
        }

        //public void SaveDemandNomenclature(DemandNomenclatureExtend demNom, int userId) {
        //    new DemandNomenclatureRepository().SaveDemandNomenclature(demNom, userId);
        //}

        public int GetMaterialGroupId(string materialGroupName) {
            return new MaterialGroupRepository().GetMaterialGroupId(materialGroupName);
        }

        public int GetDemandPreviousStatus(int demandId) {
            return new DemandRepository().GetDemandPreviousStatus(demandId);
        }

        public string GetLastImportDateText() {
            return new LastImportRepository().GetLastImportDate();
        }

        public void SetLastImportDate() {
            new LastImportRepository().SetLastImportDate();
        }

        public SupplierExtend GetSupplierById(int id) {
            return new SupplierRepository().GetSupplierById(id);
        }

        public int SaveSupplier(SupplierExtend supplier, int userId, bool isContactUpdate, bool isImport) {
            return new SupplierRepository().SaveSupplier(supplier, userId, isContactUpdate, isImport);

        }

        public int SaveSupplierContact(SupplierContactExtended supplierContactExtended, int userId) {
            return new SupplierContactRepository().SaveSupplierContact(supplierContactExtended, userId);
        }

        public void SaveImportInfo(string fileName, DateTime lastModifDate, bool isError, int userId) {
            new SourceFileRepository().SaveImportInfo(fileName, lastModifDate, isError, userId);
        }

        public bool IsFileLoaded(string fileName, DateTime lastModifDate) {
            var sourceFile = new SourceFileRepository().GetSourceFile(fileName, lastModifDate);
            if (sourceFile == null) {
                return false;
            }

            if (sourceFile.status != SourceFileRepository.STATUS_SUCCESS) {
                return false;
            }

            return true;
        }

        public void SaveNomenclaturesStatus(int demandId, int[] nomIds, int[] statusesId, int userId) {
            
            List<NomenclatureRepository.NomStatus> nomStatuses = new List<NomenclatureRepository.NomStatus>();
            for (int i = 0; i < nomIds.Length; i++) {
                NomenclatureRepository.NomStatus nomStatus = new NomenclatureRepository.NomStatus(
                                    nomIds[i],
                                    statusesId[i],
                                    null,
                                    null);
                nomStatuses.Add(nomStatus);
            }

            new NomenclatureRepository().SaveNomenclaturesStatus(demandId, nomStatuses, userId);
        }

        public void RemoveDemandNomenclature(int demandId, int nomenclatureId, int userId) {
            new DemandNomenclatureRepository().RemoveDemandNomenclature(demandId, nomenclatureId, userId);
        }

        public bool SetPrice(string strNomenclature, decimal dPrice, int userId, int currencyId) {
            return new NomenclatureRepository().SetPrice(strNomenclature, dPrice, userId, currencyId);
        }

        public void AddRemark(int demandId, int demandVersion, int authorId, string strRemark) {
            new RemarkRepository().AddRemark(demandId, demandVersion, authorId, strRemark);
        }

        public List<RemarkExtend> GetRemarks(int demandId) {
            return new RemarkRepository().GetRemarks(demandId);
        }

        public List<DemandNomenclatureExtend> GetDemandNomHistory(int demandId) {
            return new DemandNomenclatureRepository().GetDemandNomHistory(demandId);
        }

        public int GetDemandNomPrevStatus(int demandId, int nomId) {
            return new DemandNomenclatureRepository().GetDemandNomPrevStatus(demandId, nomId);
        }

        public List<Demand_Status> GetDemandStatuses() {
            return new DemandStatusRepository().GetDemandStatusWs();
        }

        public List<Nomenclature_Source> GetNomenclatureSources() {
            return new NomenclatureSourceRepository().GetNomenclatureSourcesWs();
        }

        public void DeactivateNomenclature(string strNomKey) {
            new NomenclatureRepository().DeactivateNomenclature(strNomKey);
        }

        public void SetPlNakNavrh(string strNomKey) {
            new NomenclatureRepository().SetPlNakNavrh(strNomKey);
        }

        public void DeleteNotUsedCustomNoms() {
            new NomenclatureRepository().DeleteNotUsedCustomNoms();
        }
        #endregion
    }
}
