using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class DemandNomenclatureExtend : Demand_Nomenclature {
        public string nomenclature_key {get; set;}
        public string name { get; set; }
        public string specification { get; set; }
        public int status_id { get; set; }
        public string status_text { get; set; }
        public string price_text { get; set; }
        public string price_text_orig { get; set; }
        public int? currency_id_orig { get; set; }
        public string img_status_path { get; set; }
        public bool is_locked { get; set; }
        public string currency_text { get; set; }
        public string remark { get; set; }
        public string modif_user_name { get; set; }
        public Visibility lock_visibility { get; set; }
        public Visibility edit_visibility { get; set; }
        public Visibility read_only_visibility { get; set; }
        public Visibility select_visibility { get; set; }
        public Visibility remove_visibility { get; set; }
        public bool is_selected { get; set; }
        public int source_id { get; set; }

        public List<DemandLight> other_demands { get; set; }
        
    }
}
