using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class RemarkExtend : Remark {
        public string user_firstname { get; set; }
        public string user_surname { get; set; }
    }
}
