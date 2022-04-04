using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class NomenclatureDetailExtend : NomenclatureExtend{
        public List<EvaluationMethod> evaluation_methods { get; set; }
    }
}
