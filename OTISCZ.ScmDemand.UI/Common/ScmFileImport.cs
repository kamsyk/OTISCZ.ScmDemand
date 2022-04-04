using OTISCZ.ScmDemand.Interface;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using static OTISCZ.ScmDemand.Interface.Prodis;

namespace OTISCZ.ScmDemand.UI.Common {
    public class ScmFileImport {
        //#region Delegates
        //public delegate void DlgLoadProgressInfo(int lineNumber);
        //public delegate void DlgLoadFileInfo(string fileName, int lineCount);
        //#endregion

        //#region Struct
        //public struct ImportError {
        //    public string Line;
        //    public string ErrorMsg;

        //    public ImportError(string line, string errorMsg) {
        //        Line = line;
        //        ErrorMsg = errorMsg;
        //    }
        //}
        //#endregion

        #region Properties
        //#if RELEASE
        //#else
        //        private WsScmDemandDebug.ScmDemand m_WsScm = null;


        //        protected WsScmDemandDebug.ScmDemand WsScm {
        //            get {
        //                if (m_WsScm == null) {
        //                    m_WsScm = new WsScmDemandDebug.ScmDemand();
        //                }
        //                return m_WsScm;
        //            }
        //        }
        //#endif

#if RELEASE
        private WcfScmDemand.ScmDemandClient m_WcfScm = null;
        protected WcfScmDemand.ScmDemandClient WcfScm {
            get {
                if (m_WcfScm == null) {
                    m_WcfScm = new WcfScmDemand.ScmDemandClient();

                }

                return m_WcfScm;
            }
        }
#else
        private WcfScmDemandDebug.ScmDemandClient m_WcfScm = null;

        protected WcfScmDemandDebug.ScmDemandClient WcfScm {
            get {
                if (m_WcfScm == null) {
                    m_WcfScm = new WcfScmDemandDebug.ScmDemandClient();

                }

                return m_WcfScm;
            }
        }
#endif
        #endregion

        #region Methods
        public List<ImportError> ImportData(
            string sourceFolder, 
            int userId,
            DlgLoadFileInfo dlgLoadfileInfo, 
            DlgLoadProgressInfo dlgLoadProgressInfo,
            Dispatcher dispatcher) {

            Prodis prodis = new Prodis();
            List<Material_Group> mgs = prodis.GetMaterialGroups();
            Hashtable htMsg = new Hashtable();
            foreach (var mg in mgs) {
                htMsg.Add(mg.name, mg.id);
            }

            List<ImportError> importErrors = null;
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files) {
                FileInfo fi = new FileInfo(file);
                if (fi.Name.StartsWith("Bez_Ceny") && fi.Name.EndsWith("txt")) {
                    try {
                        prodis.ImportFile(
                            fi.FullName,
                            htMsg,
                            userId,
                            dlgLoadfileInfo,
                            dlgLoadProgressInfo,
                            //dispatcher,
                            out importErrors);
                    } catch(Exception ex) {
                        VmBase.HandleError(ex, dispatcher);
                    }
                }
            }

            WcfScm.SetLastImportDate();

            return importErrors;
        }

        public List<ImportError> ImportPriceData(
            string sourceFolder,
            int userId,
            DlgLoadFileInfo dlgLoadfileInfo,
            DlgLoadProgressInfo dlgLoadProgressInfo,
            Dispatcher dispatcher) {

            Prodis prodis = new Prodis();
          
            List<ImportError> importErrors = null;
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files) {
                FileInfo fi = new FileInfo(file);
                if (fi.Name.StartsWith("Nove_Ceny") && fi.Name.EndsWith("txt")) {
                    try {
                        prodis.ImportPriceFile(
                            fi.FullName,
                            userId,
                            dlgLoadfileInfo,
                            dlgLoadProgressInfo,
                            out importErrors);
                    } catch (Exception ex) {
                        VmBase.HandleError(ex, dispatcher);
                    }
                }
            }

            //WcfScm.SetLastImportDate();

            return importErrors;
        }

        private Unit[] GetUnits() {
            var units = WcfScm.GetUnits();

            return units;

            //List<Unit> units = new List<Unit>();
            //if (wsUnits == null) {
            //    return units;
            //}

            //foreach(var wsUnit in wsUnits) {
            //    Unit unit = new Unit();
            //    VmBase.SetValues(wsUnit, unit);
            //    units.Add(unit);
            //}

            //return units;
        }

        //private Material_Group[] GetMaterialGroups() {
        //    var materialGroups = WcfScm.GetMaterialGroups();

        //    return materialGroups;

        //    //List<Material_Group> mgs = new List<Material_Group>();
        //    //if (wsMgs == null) {
        //    //    return mgs;
        //    //}

        //    //foreach (var wsMg in wsMgs) {
        //    //    Material_Group mg = new Material_Group();
        //    //    VmBase.SetValues(wsMg, mg);
        //    //    mgs.Add(mg);
        //    //}

        //    //return mgs;
        //}

        private void ImportFile(
            string strFile,
            Hashtable htMgs,
            int userId,
            DlgLoadFileInfo dlgLoadfileInfo,
            DlgLoadProgressInfo dlgLoadProgressInfo,
            Dispatcher dispatcher,
            out List<ImportError> importErrors) {

            importErrors = null;
            int lineCount = 1;
            using (StreamReader sr = new StreamReader(strFile)) {
                string strLine;
                while ((strLine = sr.ReadLine()) != null) {
                    lineCount++;
                }
            }



            FileInfo fi = new FileInfo(strFile);
            string fileName = fi.Name;
            dlgLoadfileInfo(fileName, lineCount);

            if (WcfScm.IsFileLoaded(fi.FullName, fi.LastWriteTime)) {
                return;
            }

            bool isError = false;

            try {
                NomenclatureRepository demandRepository = new NomenclatureRepository();

                using (StreamReader sr = new StreamReader(strFile, Encoding.GetEncoding(28592))) {
                    string strLine;
                    int lineNr = 1;
                    bool isHeaderSkiped = false;

                    Prodis prodis = new Prodis();

                    while ((strLine = sr.ReadLine()) != null) {
                        if (!isHeaderSkiped) {
                            isHeaderSkiped = true;
                            continue;
                        }
                        try {
                            Nomenclature nomenclature = prodis.GetScmNomenclatureFromLine(strLine, htMgs);
                            WcfScm.ImportNomenclature(nomenclature, userId);

                        } catch (Exception ex) {
                            if (importErrors == null) {
                                importErrors = new List<ImportError>();
                            }
                            importErrors.Add(new ImportError(strLine, ex.ToString()));
                            isError = true;
                        }

                        dlgLoadProgressInfo(lineNr);
                        lineNr++;
                    }
                }
            } catch (Exception ex) {
                VmBase.HandleError(ex, dispatcher);
            } finally {
                SaveImportInfo(fi.FullName, fi.LastWriteTime, isError, userId);
            }
        }

        private void SaveImportInfo(string fileName, DateTime lastModifDate, bool isError, int userId) {
            WcfScm.SaveImportInfo(fileName, lastModifDate, isError, userId);
        }

        //private Nomenclature GetScmNomenclatureFromLine(string strLine, Hashtable htMgs) {
        //    string[] lineItems = strLine.Split(';');
        //    Nomenclature nomenclature = new Nomenclature();
        //    nomenclature.nomenclature_key = GetLineItemValue(lineItems[0]);
        //    nomenclature.name = GetLineItemValue(lineItems[1]);

        //    string strMaterialGroup = GetLineItemValue(lineItems[2]);
        //    int materialGroupId = GetMaterialGroupId(strMaterialGroup, htMgs);
        //    if (materialGroupId > -1) {
        //        nomenclature.material_group_id = materialGroupId;
        //    } else {
        //        throw new Exception("Material Group '" + strMaterialGroup + "' was not found");
        //    }

        //    string strDate = null;
        //    if (lineItems.Length > 4) {
        //        strDate = GetLineItemValue(lineItems[5]);
        //    }

        //    if (lineItems.Length > 5) {
        //        strDate += " " + GetLineItemValue(lineItems[6]);
        //    }

        //    DateTime createDate;
        //    bool isDate = DateTime.TryParseExact(
        //        strDate,
        //        "dd/MM/yy HH:mm:ss",
        //        CultureInfo.InvariantCulture,
        //        DateTimeStyles.None,
        //        out createDate);
        //    if (isDate) {
        //        nomenclature.created_date = createDate;
        //    }

        //    if (lineItems.Length > 8) {
        //        nomenclature.specification = GetLineItemValue(lineItems[9]);
        //    }

        //    return nomenclature;
        //}

        //private int GetMaterialGroupId(string strMaterialGroupName, Hashtable htMg) {
        //    //var mg = (from mgDb in materialGroups
        //    //            where mgDb.name.ToLower().Trim() == strMaterialGroup.ToLower().Trim()
        //    //            select mgDb).FirstOrDefault();

        //    //if (mg == null) {
        //    //    return -1;
        //    //    materialGroups
        //    //}

        //    //return mg.id;
        //    if (htMg.ContainsKey(strMaterialGroupName)) {
        //        return (int)htMg[strMaterialGroupName];
        //    } else {
        //        int mgId = WcfScm.GetMaterialGroupId(strMaterialGroupName);
        //        htMg.Add(strMaterialGroupName, mgId);
        //        return mgId;
        //    }
        //}

        private int GetUnitId(string strUnit, List<Unit> units) {
            var unit = (from unitsDb in units
                        where unitsDb.code.ToLower().Trim() == strUnit.ToLower().Trim()
                        select unitsDb).FirstOrDefault();

            if (unit == null) {
                return -1;
            }

            return unit.id;
        }

        //private string GetLineItemValue(string rawValue) {
        //    if (rawValue == null) {
        //        return null;
        //    }

        //    if (rawValue == "") {
        //        return null;
        //    }

        //    string retValue = rawValue;
        //    if (retValue.StartsWith("\"")) {
        //        retValue = retValue.Substring(1);
        //    }
        //    if (retValue.EndsWith("\"")) {
        //        retValue = retValue.Substring(0, retValue.Length - 1);
        //    }

        //    return retValue.Trim();
        //}

#if RELEASE

#else
        //private WsScmDemandDebug.Nomenclature GetWsNomenclature(Nomenclature nomenclature) {
        //    WsScmDemandDebug.Nomenclature wsNomenclature = new WsScmDemandDebug.Nomenclature();
        //    VmBase.SetValues(nomenclature, wsNomenclature);

        //    return wsNomenclature;
        //}
#endif
        #endregion
    }
}
