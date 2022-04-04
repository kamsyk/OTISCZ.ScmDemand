using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.ExtendedModel
{
    public class DemandReport : DemandExtend
    {
        public DemandNomenclatureExtend[] demand_nomenclatures_history { get; set; }
        public RemarkExtend[] remarks { get; set; }
    }
}
