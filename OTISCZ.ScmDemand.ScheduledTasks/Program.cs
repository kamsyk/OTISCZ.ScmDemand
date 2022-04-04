using OTISCZ.ScmDemand.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.ScheduledTasks {
    public class Program {
        static void Main(string[] args) {
            Console.WriteLine("Delete Custom Noms ...");
            new NomenclatureImport().DeleteNotUsedCustomNoms();

            new NomenclatureImport().ImportData();
            new NomenclatureImport().ImportPriceData();

            Console.WriteLine("Loading suppliers...");
            new Concorde().ImportSuppliers(); 
        }
    }
}
