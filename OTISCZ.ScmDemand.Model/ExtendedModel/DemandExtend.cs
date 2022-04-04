using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class DemandExtend : Demand{
        public int row_index { get; set; }
        public string requestor_name { get; set; }
        public string app_man_name { get; set; }
        public string nomenclatures_text { get; set; }
        public string supplier_text { get; set; }
        public string created_date_text { get; set; }
        public string last_status_modif_date_text { get; set; }
        public string status_text { get; set; }
        public string img_status_path { get; set; }
        public int days_in_status { get; set; }
        
        public List<DemandNomenclatureExtend> demand_nomenclatures_extend { get; set; }
        public List<AttachmentExtend> recipient_attachments_extend { get; set; }
        public List<AttachmentExtend> supplier_attachments_extend { get; set; }

    }
}

