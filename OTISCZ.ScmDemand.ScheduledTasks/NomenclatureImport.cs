using OTISCZ.OtisUser;
using OTISCZ.ScmDemand.Interface;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OTISCZ.ScmDemand.Interface.Prodis;

namespace OTISCZ.ScmDemand.ScheduledTasks {
    public class NomenclatureImport {
        #region Properties
#if RELEASE
        private WsScmDemand.ScmDemand m_WsScm = null;

        protected WsScmDemand.ScmDemand WsScm {
            get {
                if (m_WsScm == null) {
                    m_WsScm = new WsScmDemand.ScmDemand();
                    m_WsScm.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }

                return m_WsScm;
            }
        }
#else
        private WsScmDemandDebug.ScmDemand m_WsScm = null;

        protected WsScmDemandDebug.ScmDemand WsScm {
            get {
                if (m_WsScm == null) {
                    m_WsScm = new WsScmDemandDebug.ScmDemand();

                }

                return m_WsScm;
            }
        }
#endif
        #endregion

        #region Methods
        public void ImportData() {
            var errList = ImportData(UserRepository.SYSTEM_USER_ID);
        }

        public void ImportPriceData() {
            var errList = ImportPriceData(UserRepository.SYSTEM_USER_ID);
        }

        private List<ImportError> ImportData(
           int userId) {

            Prodis prodis = new Prodis();
            string sourceFolder = prodis.ProdisFolder;

            List<Material_Group> mgs = prodis.GetMaterialGroups();
            Hashtable htMsg = new Hashtable();
            foreach (var mg in mgs) {
                htMsg.Add(mg.name, mg.id);
            }

            List<ImportError> importErrors = null;

#if DEBUG
            UserClass userClass = new UserClass();
            var userContext = userClass.ImpersonateUser("autobom", "OT", "Heslo38.");
#endif

            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files) {
                FileInfo fi = new FileInfo(file);
                if (fi.Name.StartsWith("Bez_Ceny") && fi.Name.EndsWith("txt")) {

                    prodis.ImportFile(
                        fi.FullName,
                        htMsg,
                        userId,
                        null,
                        null,
                        out importErrors);
                }
            }

#if DEBUG
            userContext.Undo();
#endif

            //SharePoint import
            new SharePoint().ImportServiceCentrumNoms(
                htMsg,
                userId,
                null,
                null,
                out importErrors);


            WsScm.SetLastImportDate();

            //plnaknavr.txt
            if (ImportPlNakNavrh() == false) {
                ImportError importError = new ImportError();
                importError.ErrorMsg = "ImportPlNakNavrh failed";
                importErrors.Add(importError);
            }

            Console.WriteLine("Finished Successfully");
            //Thread.Sleep(3000);

            return importErrors;
        }

        private bool ImportPlNakNavrh() {
            Console.WriteLine("Loading plnaknavr.txt ...");
            try {
                Prodis prodis = new Prodis();
                string plNakNavrhFile = prodis.ProdisPlNakNavrhFile;
                using (StreamReader sr = new StreamReader(@"\\Otis.com\cz-dfs\DATA\Data_as\Nakup\PUR\Prenosy\plnaknavr.txt")) {
                    string strLine;
                    bool isHeaderSkipped = false;
                    while ((strLine = sr.ReadLine()) != null) {
                        if (!isHeaderSkipped) {
                            isHeaderSkipped = true;
                            continue;
                        }
                        string[] lineItems = strLine.Split('\t');
                        string nomKey = lineItems[1].Trim();
                        string strPrice = lineItems[9].Trim();
                        if (strPrice == "0.00") {
                            Console.WriteLine("Loading plnaknavr.txt " + nomKey + "...");
                            WsScm.SetPlNakNavrh(nomKey);
                        }
                    }
                }

                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        
        private List<ImportError> ImportPriceData(
           int userId) {


            Prodis prodis = new Prodis();
            string sourceFolder = prodis.ProdisPriceFolder;

#if DEBUG
            UserClass userClass = new UserClass();
            var userContext = userClass.ImpersonateUser("autobom", "OT", "Heslo38.");
#endif            
            List<ImportError> importErrors = null;
            string[] files = Directory.GetFiles(sourceFolder);

            foreach (string file in files) {
                FileInfo fi = new FileInfo(file);
                if (fi.Name.StartsWith("Nove_Ceny") && fi.Name.EndsWith("txt")) {

                    prodis.ImportPriceFile(
                        fi.FullName,
                        userId,
                        null,
                        null,
                        out importErrors);
                }
            }

#if DEBUG
            userContext.Undo();
#endif

            WsScm.SetLastImportDate();

            Console.WriteLine("Finished Successfully");
            Thread.Sleep(3000);

            return importErrors;
        }

        public void DeleteNotUsedCustomNoms() {
            WsScm.DeleteNotUsedCustomNoms();
        }
        #endregion
    }
}
