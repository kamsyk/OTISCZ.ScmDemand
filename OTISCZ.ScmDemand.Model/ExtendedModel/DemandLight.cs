using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class DemandLight {
        public string demand_nr { get; set; }
        public string img_status_path { get; set; }
        public string price_text { get; set; }
        public string status_text { get; set; }
        public string supplier_name { get; set; }
        public int status_id { get; set; }
    }
}
