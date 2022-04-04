
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class SupplierContactExtended  {
        public int id { get; set; }
        public int supplier_id { get; set; }
        public string first_name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string phone_nr { get; set; }
        public System.DateTime modif_date { get; set; }
        public int modif_user_id { get; set; }
        public string phone_nr2 { get; set; }
        public List<EntityError> entity_error { get; set; }
    }
}
