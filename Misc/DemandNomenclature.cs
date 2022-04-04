using OTISCZ.ScmDemand.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc {
    public class DemandNomenclature {
        public void SetDemNomStatus() {
            DemandRepository demandRepository = new DemandRepository();
            demandRepository.PopulateDemandNomStatusDemands();
        }

        public void SetIdKey() {
            DemandRepository demandRepository = new DemandRepository();
            demandRepository.SetIdKey();
        }

        public void FixDeactivated() {
            NomenclatureRepository nomenclatureRepository = new NomenclatureRepository();
            var noms = nomenclatureRepository.GetNomenclaturesBySourceId(1);

            foreach (var nom in noms) {
                var nomVers = nomenclatureRepository.GetNomenclatureHistory(nom.id);
                if(nomVers.Last().is_active == false && nomVers.Count > 1) {
                   // Console.WriteLine(nomVers.Last().nomenclature_key);
                    
                    for (int i = 0; i < nomVers.Count; i++) {
                        if (nomVers.ElementAt(i).is_active == false) {
                            if (nomVers.ElementAt(i).modif_user_id == -1) {
                                Console.WriteLine("FIX IT:" + nomVers.ElementAt(0).id);
                                Console.ReadLine();
                            }
                            break;
                        }   
                    }
                
                }
            }
        }
    }
}
