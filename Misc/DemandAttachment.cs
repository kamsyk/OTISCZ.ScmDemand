using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.Repository;

namespace Misc {
    public class DemandAttachment {
        public void FixGeneratedDemandAttType() {
            DemandAttachmentRepository demandAttachmentRepository = new DemandAttachmentRepository();
            var demAtts = demandAttachmentRepository.GetAllReferentMailDemandAtt();
            
            foreach (var demAtt in demAtts) {
                if (demAtt.att_type == AttachmentRepository.ATT_TYPE_MAIL_RECIPIENT) {
                    string fileName = demAtt.Attachment.file_name;
                    if ((fileName.StartsWith("PoptavkaRFQ") && fileName.EndsWith(".xlsx"))
                        || (fileName.StartsWith("DemandRFQ") && fileName.EndsWith(".xlsx"))) {

                        demandAttachmentRepository.UpdateAttType(demAtt.demand_id, demAtt.demand_version, demAtt.attachment_id, AttachmentRepository.ATT_TYPE_GENERATED_DEMAND);

                        Console.WriteLine(fileName);
                    }
                    
                }
            }
        }
    }
}
