using OTISCZ.CommonDb;
using OTISCZ.ScmDemand.Model.DataDictionary;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class NomenclatureSourceRepository : BaseRepository<Nomenclature_Source> {
        #region Methods
        
        public List<Nomenclature_Source> GetNomenclatureSourcesWs() {

            var nomenclatureSources = (from nomSourceDb in m_dbContext.Nomenclature_Source
                                select nomSourceDb).ToList();

            var retNomSources = new List<Nomenclature_Source>();
            foreach (var nomenclatureSource in nomenclatureSources) {
                Nomenclature_Source tmpNomSource = new Nomenclature_Source();
                SetValues(nomenclatureSource, tmpNomSource);

                retNomSources.Add(tmpNomSource);
            }

            return retNomSources;

        }

        
        #endregion
    }
}
