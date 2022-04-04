
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class SupplierExtend : Supplier {
        public int row_index { get; set; }
        public string img_approved_path { get; set; }
        public bool is_selected { get; set; }

        public Visibility approved_visibility { get; set; }
        //public Visibility supplier_edit_visibility { get; set; }
        public string tooltip_approved { get; set; }
        public List<SupplierContactExtended> supplier_contact_extended { get; set; }
    }

    
}
