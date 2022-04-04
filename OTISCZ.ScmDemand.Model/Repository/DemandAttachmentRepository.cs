using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class DemandAttachmentRepository : BaseRepository<Demand_Attachment> {

        #region Methods
        public List<Demand_Attachment> GetAllReferentMailDemandAtt() {
            var demAtt = (from demAttDb in m_dbContext.Demand_Attachment
                          where demAttDb.att_type == AttachmentRepository.ATT_TYPE_MAIL_RECIPIENT
                          select demAttDb).ToList();

            return demAtt;
        }

        public void UpdateAttType(int demandId, int demandVersion, int attId, int attType) {
            var demAtt = (from demAttDb in m_dbContext.Demand_Attachment
                          where demAttDb.demand_id == demandId
                          && demAttDb.demand_version == demandVersion
                           && demAttDb.attachment_id == attId
                          select demAttDb).FirstOrDefault();
            demAtt.att_type = attType;
            SaveChanges();
        }
        #endregion
    }
}
