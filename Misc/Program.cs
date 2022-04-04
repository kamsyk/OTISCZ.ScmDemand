using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc {
    class Program {
        static void Main(string[] args) {
            //new DemandNomenclature().SetDemNomStatus();
            //new DemandNomenclature().SetIdKey();
            //new DemandNomenclature().FixDeactivated();
            new DemandAttachment().FixGeneratedDemandAttType();
        }
    }
}
