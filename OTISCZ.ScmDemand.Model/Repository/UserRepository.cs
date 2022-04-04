using OTISCZ.ScmDemand.Model.DataDictionary;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class UserRepository : BaseRepository<ScmUser> {
        #region User Role
        public const int USER_ROLE_RO = 100;
        public const int USER_ROLE_REFERNT = 100;
        public const int USER_ROLE_APP_MAN = 200;
        public const int USER_ROLE_ADMINISTRATOR = 1000;

        public const int SYSTEM_USER_ID = -1;
        #endregion

        #region Methods
        public ScmUser GetUserByUserNameWs(string userName) {
            //m_dbContext.Configuration.LazyLoadingEnabled = false;
            //m_dbContext.Configuration.ProxyCreationEnabled = false;
            try
            {
                var user = (from userDb in m_dbContext.ScmUser
                            where userDb.user_name.ToLower().Trim() == userName.ToLower().Trim()
                            select userDb).FirstOrDefault();

                ScmUser retUser = new ScmUser();
                SetValues(user, retUser);

                retUser.Role = new List<Role>();
                foreach (var role in user.Role)
                {
                    Role retRole = new Role();
                    SetValues(role, retRole, new List<string> { RoleData.ID_FIELD, RoleData.ROLE_NAME_FIELD });
                    retUser.Role.Add(retRole);
                }

                if (user.User_Setting != null)
                {
                    User_Setting userSetting = new User_Setting();
                    SetValues(user.User_Setting, userSetting);
                    retUser.User_Setting = userSetting;
                }

                return retUser;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public string GetUserUserMail(int userId) {
           
            try {
                var user = (from userDb in m_dbContext.ScmUser
                            where userDb.id == userId
                            select userDb).FirstOrDefault();

                return user.email;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public string GetUserUserName(int userId) {

            try {
                var user = (from userDb in m_dbContext.ScmUser
                            where userDb.id == userId
                            select userDb).FirstOrDefault();

                return user.surname + " " + user.first_name;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public void SetUserCulture(int userId, string culture) {
            var user = (from userDb in m_dbContext.ScmUser
                        where userDb.id == userId
                        select userDb).FirstOrDefault();

            if (user.User_Setting == null) {
                user.User_Setting = new User_Setting();
                user.User_Setting.user_id = userId;
                user.User_Setting.culture = culture;
                m_dbContext.SaveChanges();
            } else {
                if (user.User_Setting.culture != culture) {
                    user.User_Setting.culture = culture;
                    m_dbContext.SaveChanges();
                }
            }
                        
        }

        public void RemoveUserSettings(int userId) {
            var csmUser = (from scmUserDb in m_dbContext.ScmUser
                           where scmUserDb.id == userId
                           select scmUserDb).FirstOrDefault();

            if (csmUser.User_Setting == null) {
                return;
            }

            csmUser.User_Setting = null;

            m_dbContext.SaveChanges();
        }

        public ScmUser GetUserById(int userId) {
            var csmUser = (from scmUserDb in m_dbContext.ScmUser
                           where scmUserDb.id == userId
                           select scmUserDb).FirstOrDefault();

            return csmUser;
        }

        public List<ScmUserExtend> GetActiveAppMen() {
            var scmUsers = (from scmUserDb in m_dbContext.ScmUser
                            where scmUserDb.is_active == true
                            orderby scmUserDb.surname, scmUserDb.first_name
                            select scmUserDb).ToArray();


            List<ScmUserExtend> appMen = new List<ScmUserExtend>();
            if (scmUsers != null) {
                foreach (var scmUser in scmUsers) {
                    var appRole = (from roleDb in scmUser.Role
                                   where roleDb.id == USER_ROLE_APP_MAN
                                   select roleDb).FirstOrDefault();
                    if (appRole != null) {
                        ScmUserExtend appUser = new ScmUserExtend();
                        SetValues(scmUser, appUser);
                        appMen.Add(appUser);
                    }
                }
            }



            return appMen;
        }
        #endregion
    }
}
