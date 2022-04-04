using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class EntityError {
        public int id { get; set; }
        public List<string> errors { get; set; }
    }
}
