using OTISCZ.ScmDemand.Model.DataDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class SettingRepository : BaseRepository<ScmSetting> {
        #region Methods
        public ScmSetting GetScmSetting() {
            var setting = (from settingDb in m_dbContext.ScmSetting
                           select settingDb).FirstOrDefault();

            if (setting == null) {
                ScmSetting newSet = new ScmSetting();
                newSet.id = 0;
                newSet.prodis_input_folder = "";
                m_dbContext.ScmSetting.Add(newSet);
                m_dbContext.SaveChanges();

                setting = (from settingDb in m_dbContext.ScmSetting
                           select settingDb).FirstOrDefault();
            }

            return setting;
        }

        public void SetImportFolder(string folder) {
            ScmSetting smcSetting = GetScmSetting();
            smcSetting.prodis_input_folder = folder;
            m_dbContext.SaveChanges();
        }
        #endregion
    }
}
