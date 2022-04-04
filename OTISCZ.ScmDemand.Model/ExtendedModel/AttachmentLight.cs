using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class AttachmentLight {
        public int id { get; set; }
        //public int att_type { get; set; }
        public string file_name { get; set; }
        public byte[] file_icon { get; set; }
        public System.DateTime modif_date { get; set; }
        public int modif_user_id { get; set; }
    }
}
