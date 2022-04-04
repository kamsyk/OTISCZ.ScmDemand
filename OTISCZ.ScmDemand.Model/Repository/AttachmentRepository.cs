using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class AttachmentRepository : BaseRepository<Attachment> {
        #region Constants
        public const int ATT_TYPE_MAIL_RECIPIENT = 0;
        public const int ATT_TYPE_GENERATED_DEMAND = 1;
        public const int ATT_TYPE_RECIPIENT = 10;
        public const int ATT_TYPE_SUPPLIER = 20;
        
        #endregion

        #region Methods
        public int AddAttachment(Attachment att, int userId) {
            int lastId = GetLastId();
            int newId = ++lastId;

            Attachment newAtt = new Attachment();
            SetValues(att, newAtt);

            newAtt.id = newId;
            newAtt.modif_user_id = userId;
            newAtt.modif_date = DateTime.Now;

            m_dbContext.Attachment.Add(newAtt);

            SaveChanges();

            return newId;
        }

        public AttachmentLight GetLightAttachmentById(int attId) {
            var attLight = (from attDb in m_dbContext.Attachment
                            where attDb.id == attId
                            select new AttachmentLight {
                                id =attDb.id,
                                file_icon = attDb.file_icon,
                                file_name = attDb.file_name,
                                modif_user_id = attDb.modif_user_id
                            }).FirstOrDefault();

            return attLight;
        }

        public byte[] GetAttachmentContent(int attId) {
            var attContent = (from attDb in m_dbContext.Attachment
                            where attDb.id == attId select attDb).Select(att => att.file_content).FirstOrDefault();
            
            return attContent;
        }

        private int GetLastId() {

            var att = (from attDb in m_dbContext.Attachment
                            orderby attDb.id descending
                            select attDb).FirstOrDefault();

            if (att == null) {
                return -1;
            }

            return att.id;
        }
        #endregion
    }
}
