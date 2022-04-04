using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class NomenclatureExtend : Nomenclature{
        public int row_index { get; set; }
        public string material_group_text { get; set; }
        public bool is_selected { get; set; }
        public string modif_date_text { get; set; }
        public string created_date_text { get; set; }
        public string import_date_text { get; set; }
        public string last_status_modif_date_text { get; set; }
        public string status_text { get; set; }
        public int days_in_status { get; set; }
        public string img_status_path { get; set; }
        public string new_demand_text { get; set; }
        public string source_text { get; set; }
        public string plnaknavrh_text { get; set; }
    }
}
