using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Interface {
    public class Prodis : NomenclatureSource  {
        #region Delegates
        public delegate void DlgLoadProgressInfo(int lineNumber);
        public delegate void DlgLoadFileInfo(string fileName, int lineCount);
        #endregion

        #region Struct
        public struct ImportError {
            public string Line;
            public string ErrorMsg;

            public ImportError(string line, string errorMsg) {
                Line = line;
                ErrorMsg = errorMsg;
            }
        }
        #endregion


        #region Properties
        public string ProdisFolder {
            get {
                return ScmSetting.prodis_input_folder;
            }
        }

        public string ProdisPriceFolder {
            get {
                return ScmSetting.prodis_price_input_folder;
            }
        }

        public string ProdisPlNakNavrhFile {
            get {
                return ScmSetting.plnaknavrh_file_path;
            }
        }
        #endregion

        #region Methods
        public Nomenclature GetScmNomenclatureFromLine(string strLine, Hashtable htMgs) {
            string[] lineItems = strLine.Split(';');
            Nomenclature nomenclature = new Nomenclature();
            nomenclature.nomenclature_key = GetLineItemValue(lineItems[0]);
            nomenclature.name = GetLineItemValue(lineItems[1]);

            string strMaterialGroup = GetLineItemValue(lineItems[2]);
            int materialGroupId = GetMaterialGroupId(strMaterialGroup, htMgs);
            if (materialGroupId > -1) {
                nomenclature.material_group_id = materialGroupId;
            } else {
                throw new Exception("Material Group '" + strMaterialGroup + "' was not found");
            }

            string strDate = null;
            if (lineItems.Length > 4) {
                strDate = GetLineItemValue(lineItems[5]);
            }

            if (lineItems.Length > 5) {
                strDate += " " + GetLineItemValue(lineItems[6]);
            }

            DateTime createDate;
            bool isDate = DateTime.TryParseExact(
                strDate,
                "dd/MM/yy HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out createDate);
            if (isDate) {
                nomenclature.created_date = createDate;
            }

            if (lineItems.Length > 8) {
                nomenclature.specification = GetLineItemValue(lineItems[9]);
            }

            nomenclature.source_id = (int)NomenclatureRepository.NomSource.Prodis;
            nomenclature.is_plnaknavrh = false;

            return nomenclature;
        }

        private string GetLineItemValue(string rawValue) {
            if (rawValue == null) {
                return null;
            }

            if (rawValue == "") {
                return null;
            }

            string retValue = rawValue;
            if (retValue.StartsWith("\"")) {
                retValue = retValue.Substring(1);
            }
            if (retValue.EndsWith("\"")) {
                retValue = retValue.Substring(0, retValue.Length - 1);
            }

            return retValue.Trim();
        }

        

        public List<Material_Group> GetMaterialGroups() {
            var wsMaterialGroups = WsScm.GetMaterialGroups();

            var materialGroups = new List<Material_Group>();
            foreach (var wsMat in wsMaterialGroups) {
                Material_Group mg = new Material_Group();
                SetValues(wsMat, mg);
                materialGroups.Add(mg);
            }

            return materialGroups;

           
        }

        public void ImportFile(
            string strFile,
            Hashtable htMgs,
            int userId,
            DlgLoadFileInfo dlgLoadfileInfo,
            DlgLoadProgressInfo dlgLoadProgressInfo,
            //Dispatcher dispatcher,
            out List<ImportError> importErrors) {

            importErrors = null;

            int lineCount = 1;

            FileInfo fi = new FileInfo(strFile);

            bool isLoaded  = false;
            bool isLoadedSpec = false;

            if (dlgLoadfileInfo == null) {
                Console.WriteLine(fi.FullName);
            }


            try {
                
                WsScm.IsFileLoaded(fi.FullName, fi.LastWriteTime, true, out isLoaded, out isLoadedSpec);
                if (isLoaded) {
                    return;
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                throw ex;
            }

            

            using (StreamReader sr = new StreamReader(strFile)) {
                string strLine;
                while ((strLine = sr.ReadLine()) != null) {
                    lineCount++;
                }
            }

            string fileName = fi.Name;
            if (dlgLoadfileInfo != null) {
                dlgLoadfileInfo(fileName, lineCount);
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

#if DEBUG
                            WsScmDemandDebug.Nomenclature wsNomenclature = new WsScmDemandDebug.Nomenclature();
#else
                            WsScmDemand.Nomenclature wsNomenclature = new WsScmDemand.Nomenclature();
#endif
                            SetValues(nomenclature, wsNomenclature);

                            int nomId = -1;
                            bool isNomIdSpec = false;
                            WsScm.ImportNomenclature(wsNomenclature, userId, true, out nomId, out isNomIdSpec);

                        } catch (Exception ex) {
                            if (importErrors == null) {
                                importErrors = new List<ImportError>();
                            }
                            importErrors.Add(new ImportError(strLine, ex.ToString()));
                            isError = true;
                        }

                        if (dlgLoadProgressInfo != null) {
                            dlgLoadProgressInfo(lineNr);
                        } else {
                            Console.WriteLine(fi.FullName + " loading " + lineNr + " out of " + lineCount);
                        }
                        lineNr++;
                    }
                }

                SaveImportInfo(fi.FullName, fi.LastWriteTime, isError, userId);
            } catch (Exception ex) {
                //VmBase.HandleError(ex, dispatcher);
                if (dlgLoadProgressInfo == null) {
                    Console.WriteLine(ex.ToString());
                }
                throw ex;
            } 
        }

        public void ImportPriceFile(
           string strFile,
           int userId,
           DlgLoadFileInfo dlgLoadfileInfo,
           DlgLoadProgressInfo dlgLoadProgressInfo,
           out List<ImportError> importErrors) {

            importErrors = null;

            int lineCount = 1;

            FileInfo fi = new FileInfo(strFile);

            bool isLoaded = false;
            bool isLoadedSpec = false;

            if (dlgLoadfileInfo == null) {
                Console.WriteLine(fi.FullName);
            }


            try {

                WsScm.IsFileLoaded(fi.FullName, fi.LastWriteTime, true, out isLoaded, out isLoadedSpec);
                if (isLoaded) {
                    return;
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                throw ex;
            }



            using (StreamReader sr = new StreamReader(strFile)) {
                string strLine;
                while ((strLine = sr.ReadLine()) != null) {
                    lineCount++;
                }
            }

            string fileName = fi.Name;
            if (dlgLoadfileInfo != null) {
                dlgLoadfileInfo(fileName, lineCount);
            }



            bool isError = false;

            try {
                NomenclatureRepository demandRepository = new NomenclatureRepository();

                var currencies = WsScm.GetActiveCurrencies();
                var currCz = (from currDb in currencies
                            where currDb.currency_code == "CZK"
                            select currDb).FirstOrDefault();

                using (StreamReader sr = new StreamReader(strFile, Encoding.GetEncoding(28592))) {
                    string strLine;
                    int lineNr = 1;
                    bool isHeaderSkiped = false;

                    Prodis prodis = new Prodis();

                    NumberFormatInfo nfi = new CultureInfo(CultureInfo.CurrentCulture.Name, false).NumberFormat;
                    NomenclatureRepository nomenclatureRepository = new NomenclatureRepository();
                    while ((strLine = sr.ReadLine()) != null) {
                        if (!isHeaderSkiped) {
                            isHeaderSkiped = true;
                            continue;
                        }
                        try {
                            bool isFound = false;
                            bool isFoundSpecified = false;
                            string[] lineItems = strLine.Split(';');
                            string strNomenclature = lineItems[0].Trim();
                            string strPrice = lineItems[8].Trim().Replace(".", nfi.NumberDecimalSeparator);
                            
                            decimal dPrice = Decimal.MinValue;
                            bool isDecimal = Decimal.TryParse(strPrice, out dPrice);

                            if (isDecimal) {
                                WsScm.SetPrice(strNomenclature, dPrice, true, userId, true, currCz.id, true, out isFound, out isFoundSpecified);
                            }

                            if (dlgLoadProgressInfo == null) {
                                if (isFound) {
                                    Console.WriteLine(strNomenclature + " ******************** Cena Nastavena *****************");
                                } else {
                                    Console.WriteLine(strNomenclature + " Nenalezena");
                                }
                            }

                            //Console.WriteLine(strNomenclature + " " + ((isFound) ? " Cena Nastavena" : "Nenalezena"));
                        } catch (Exception ex) {
                            if (importErrors == null) {
                                importErrors = new List<ImportError>();
                            }
                            importErrors.Add(new ImportError(strLine, ex.ToString()));
                            isError = true;
                        }

                        if (dlgLoadProgressInfo != null) {
                            dlgLoadProgressInfo(lineNr);
                        } else {
                            Console.WriteLine(fi.FullName + " loading " + lineNr + " out of " + lineCount);
                        }
                        lineNr++;
                    }
                }

                SaveImportInfo(fi.FullName, fi.LastWriteTime, isError, userId);
            } catch (Exception ex) {
                //VmBase.HandleError(ex, dispatcher);
                if (dlgLoadProgressInfo == null) {
                    Console.WriteLine(ex.ToString());
                }
                throw ex;
            }
        }

        private void SaveImportInfo(string fileName, DateTime lastModifDate, bool isError, int userId) {
            WsScm.SaveImportInfo(fileName, lastModifDate, true, isError, true, userId, true);
        }

        
#endregion

    }
}
