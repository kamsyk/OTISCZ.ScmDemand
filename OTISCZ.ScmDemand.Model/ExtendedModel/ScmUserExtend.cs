using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class ScmUserExtend : ScmUser {
        
        public string name_surname_first {
            get { return surname + " " + first_name; }
        }
        
    }
}
