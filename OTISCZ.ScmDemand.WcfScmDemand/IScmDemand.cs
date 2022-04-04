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

namespace OTISCZ.ScmDemand.WcfScmDemand {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IScmDemand" in both code and config file together.
    [ServiceContract]
    public interface IScmDemand {
        [OperationContract]
        ScmUser GetUserData(string userName);

        [OperationContract]
        string GetUserMail(int userId);

        [OperationContract]
        string GetUserName(int userId);

        [OperationContract]
        void SetUserCulture(int userId, string culture);

        [OperationContract]
        ScmSetting GetScmSetting();

        [OperationContract]
        void SetImportFolder(string folder);

        [OperationContract]
        int ImportNomenclature(Nomenclature lineNomenclature, int userId);

        [OperationContract]
        List<Unit> GetUnits();

        [OperationContract]
        List<Material_Group> GetMaterialGroups();

        [OperationContract]
        List<NomenclatureExtend> GetNomenclatures(
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount);

        [OperationContract]
        NomenclatureDetailExtend GetNomenclatureById(int nomenclatureId);

        [OperationContract]
        void SaveNomenclature(NomenclatureExtend nomenclature, int userId);

        [OperationContract]
        List<NomenclatureExtend> GetNomenclaturesReport(
            string filter,
            string sort);

        [OperationContract]
        Supplier GetSupplierBySupplierId(string supplierId);

        //[OperationContract]
        //void UpdateSupplier(Supplier supplier, int userId);

        [OperationContract]
        void DeactiveSuppliers(List<int> htActivesuppliersIds, int userId);

        [OperationContract]
        List<SupplierExtend> GetSuppliers(
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount);

        [OperationContract]
        int SaveDemand(DemandExtend demand, int userId);

        [OperationContract]
        int SaveDemandWasSent(DemandExtend demand, int userId);

        [OperationContract]
        string GetNewDemandRequestNr();

        [OperationContract]
        List<DemandExtend> GetDemands(
            List<WcfFilterField> filterFields,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount);

        [OperationContract]
        List<DemandExtend> GetDemandsReport(
            List<WcfFilterField> filterFields,
            string sort);

        [OperationContract]
        int GetPendingNomenclaturesNumber();

        [OperationContract]
        int GetPendingDemandsNumber(int userId);

        [OperationContract]
        DemandExtend GetDemandDetail(int demandId, int userId);

        [OperationContract]
        Byte[] GetAttachmentContent(int attId);

        [OperationContract]
        int AddAttachment(int demandId, string fileName, byte[] fileContent, byte[] fileIcon, int attType, int userId);

        [OperationContract]
        void DeleteAttachment(int demandId, int attachmentId, int userId);

        [OperationContract]
        List<ScmUserExtend> GetActiveAppMen();

        [OperationContract]
        List<Currency> GetActiveCurrencies();

        [OperationContract]
        void SaveDemandNomenclature(DemandNomenclatureExtend demNom, int userId);

        [OperationContract]
        int GetMaterialGroupId(string materialGroupName);

        [OperationContract]
        int GetDemandPreviousStatus(int demandId);

        [OperationContract]
        string GetLastImportDateText();

        [OperationContract]
        void SetLastImportDate();

        [OperationContract]
        SupplierExtend GetSupplierById(int id);

        [OperationContract]
        int SaveSupplier(SupplierExtend supplier, int userId, bool isContactUpdate, bool isImport);

        [OperationContract]
        int SaveSupplierContact(SupplierContactExtended supplierContactExtended, int userId);

        [OperationContract]
        void SaveImportInfo(string fileName, DateTime lastModifDate, bool isError, int userId);

        [OperationContract]
        bool IsFileLoaded(string fileName, DateTime lastModifDate);

        [OperationContract]
        List<SupplierExtend> GetSuppliersReport(string filter, string sort);

        [OperationContract]
        void SaveNomenclaturesStatus(int demandId, int[] nomIds, int[] statusesId, int userId);

        [OperationContract]
        void RemoveDemandNomenclature(int demandId, int nomenclatureId, int userId);

        [OperationContract]
        bool SetPrice(string strNomenclature, decimal dPrice, int userId, int currencyId);

        [OperationContract]
        void AddRemark(int demandId, int demandVersion, int authorId, string strRemark);

        [OperationContract]
        List<RemarkExtend> GetRemarks(int demandId);

        [OperationContract]
        List<DemandNomenclatureExtend> GetDemandNomHistory(int demandId);

        [OperationContract]
        int GetDemandNomPrevStatus(int demandId, int nomId);

        [OperationContract]
        List<Demand_Status> GetDemandStatuses();

        [OperationContract]
        List<Nomenclature_Source> GetNomenclatureSources();

        [OperationContract]
        void DeactivateNomenclature(string strNomKey);

        [OperationContract]
        void SetPlNakNavrh(string strNomKey);

        [OperationContract]
        void DeleteNotUsedCustomNoms();
    }
}
