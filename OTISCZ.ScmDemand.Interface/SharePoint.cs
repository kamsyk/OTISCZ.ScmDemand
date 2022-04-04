using Microsoft.SharePoint.Client;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OTISCZ.ScmDemand.Interface.Prodis;

namespace OTISCZ.ScmDemand.Interface {
    public class SharePoint : NomenclatureSource {
        #region Methods
        public void ImportServiceCentrumNoms(
            Hashtable htMgs,
            int userId,
            DlgLoadFileInfo dlgLoadfileInfo,
            DlgLoadProgressInfo dlgLoadProgressInfo,
            out List<ImportError> importErrors) {

            Console.WriteLine("Loading data from Share Point Service Centrum ...");

            importErrors = null;

            //string siteUrl = "http://portaldms/ACE/SCM/Lists/Poptvky/";
            string siteUrl = "http://portaldms/ACE/SCM/";

            ClientContext clientContext = new ClientContext(siteUrl);

            Web oWebsite = clientContext.Web;
            ListCollection collList = oWebsite.Lists;
            clientContext.Load(collList);

            clientContext.ExecuteQuery();

            List lstPoptavky = null;
            foreach (var slist in collList) {
                string s = slist.Title;
                //Console.WriteLine(s);
                if (s == "Poptávky") {
                    lstPoptavky = slist;
                    break;
                }
            }

            //List list = clientContext.Web.Lists.GetByTitle("Všechny položky");

            CamlQuery camlQuery = new CamlQuery();
            //camlQuery.ViewXml = "<View></View>";
            ListItemCollection nomItems = lstPoptavky.GetItems(camlQuery);

            clientContext.Load(nomItems);
            clientContext.ExecuteQuery();

            //ContentTypeCollection contentTypeColl = lstPoptavky.ContentTypes;
            //clientContext.Load(contentTypeColl);
            //clientContext.ExecuteQuery();
            //foreach (ContentType ct in contentTypeColl) {
            //    var df = ct.Name;
            //}

            //find filed name

            List<string> htPriceWasSet = new List<string>();

            List<Currency> activeCurrencies = new List<Currency>();
            var activeCurrenciesWs = WsScm.GetActiveCurrencies();
            foreach (var activeCurrencieWs in activeCurrenciesWs) {
                Currency currency = new Currency();
                SetValues(activeCurrencieWs, currency);
                activeCurrencies.Add(currency);
            }

            Console.WriteLine("Loading data from Share Point Service Centrum - Price Check ...");
            foreach (var nomItem in nomItems) {
                try {
                    string strCena = GetFieldValue(nomItem["Cena"]);
                    string strNomId = GetFieldValue(nomItem["Title"]);

                    if (!String.IsNullOrWhiteSpace(strCena)) {
                        Console.WriteLine("Loading data from Share Point Service Centrum - " + strNomId + " + Price Was Set");

                        if (!htPriceWasSet.Contains(strNomId.ToUpper())) {
                            htPriceWasSet.Add(strNomId.ToUpper());
                            var strPriceItems = strCena.Split(' ');
                            string strCurrency = strPriceItems[0].Trim().ToUpper();
                            string strPrice = strPriceItems[1].Trim();

                            decimal dPrice = Decimal.MinValue;
                            bool isDecimal = Decimal.TryParse(strPrice, out dPrice);

                            if (isDecimal) {
                                var curr = (from currDb in activeCurrencies
                                            where currDb.currency_code.ToUpper() == strCurrency
                                            select currDb).FirstOrDefault();

                                if (curr == null) {
                                    throw new Exception(strCurrency + " was not found among active Currencies");
                                }

                                bool isFound;
                                bool isFoundSpecified;
                                WsScm.SetPrice(strNomId, dPrice, true, userId, true, curr.id, true, out isFound, out isFoundSpecified);
                            }

                            
                        }
                        
                    }
                } catch (Exception ex) {
                    if (importErrors == null) {
                        importErrors = new List<ImportError>();
                    }

                    var errItem = new ImportError();
                    errItem.ErrorMsg = ex.Message;
                    importErrors.Add(errItem);
                }
            }

            Console.WriteLine("Loading data from Share Point Service Centrum ...");
            Hashtable htImportNom = new Hashtable();
            foreach (var nomItem in nomItems) {
                try {
                    string strNomId = GetFieldValue(nomItem["Title"]); //Nomenclature
                    if (htPriceWasSet.Contains(strNomId.ToUpper())) {
                        continue;
                    }
                    string strId = GetFieldValue(nomItem["ID"]); //Material group

                    string strProjekt = GetFieldValue(nomItem["Projekt"]);  //specifikace
                    string strAttachments = GetFieldValue(nomItem["Attachments"]);
                    DateTime dtCreated = GetDateTimeValue(nomItem["Created"]);
                    string strApprovalStatus = GetFieldValue(nomItem["_ModerationStatus"]);
                                        
                    if (dtCreated < new DateTime(2020, 1, 1)) {
                        continue;
                    }

                    bool isRejected = (strApprovalStatus == "1") ? true : false;
                    if (isRejected) {
#if DEBUG
                        //WsScm.DeactivateNomenclature(strNomId);
#endif
                        continue;
                    }

                    string strCommodity = null;
                    var lfCommodity = nomItem["Lookup"] as FieldLookupValue;
                    if (lfCommodity != null) {
                        strCommodity = lfCommodity.LookupValue; //name
                    }

                    Nomenclature nomenclature = new Nomenclature();

                    string strMaterialGroup = strId;
                    int materialGroupId = GetMaterialGroupId(strMaterialGroup, htMgs);
                    if (materialGroupId > -1) {
                        nomenclature.material_group_id = materialGroupId;
                    } else {
                        throw new Exception("Material Group '" + strMaterialGroup + "' was not found");
                    }


                    nomenclature.nomenclature_key = strNomId;
                    nomenclature.specification = strProjekt;
                    nomenclature.name = strCommodity;
                    nomenclature.created_date = dtCreated;
                    nomenclature.source_id = (int)NomenclatureRepository.NomSource.DNSSharePoint;
                    nomenclature.is_plnaknavrh = false;

                    if (htImportNom.ContainsKey(strNomId.ToUpper())) {
                        var lastNom = htImportNom[strNomId.ToUpper()] as Nomenclature;
                        if (lastNom.created_date < nomenclature.created_date) {
                            htImportNom[strNomId.ToUpper()] = nomenclature;
                        }
                    } else {
                        htImportNom.Add(strNomId.ToUpper(), nomenclature);
                    }



                    //string ctId = GetFieldValue(nomItem["ContentTypeId"]); 

                    //ContentType ct = clientContext.Web.ContentTypes.GetById(ctId);

                    //clientContext.Load(ct);

                    //clientContext.ExecuteQuery();


                    //var s = ct.Name;

                    //string strLookVal = lookUp.Name;
                    //var ctfId = nomItem["ContentTypeId"] as FieldLink;
                    //var dfvdf = ctfId.Name;

                    //ContentTypeId documentCTypeId = new ContentTypeId();

                    //SPContentType documentCType = web.AvailableContentTypes[documentCTypeId];
                } catch (Exception ex) {
                    if (importErrors == null) {
                        importErrors = new List<ImportError>();
                    }

                    var errItem = new ImportError();
                    errItem.ErrorMsg = ex.Message;
                    importErrors.Add(errItem);
                }
            }


            IDictionaryEnumerator iEnum = htImportNom.GetEnumerator();
            while (iEnum.MoveNext()) {
                try {
                    Nomenclature nomenclature = iEnum.Value as Nomenclature;
                    Console.WriteLine("Loading data from Share Point Service Centrum - Saving " + nomenclature.nomenclature_key + " ...");
#if DEBUG
                    WsScmDemandDebug.Nomenclature wsNomenclature = new WsScmDemandDebug.Nomenclature();
#else
                    WsScmDemand.Nomenclature wsNomenclature = new WsScmDemand.Nomenclature();
#endif
                    
                    SetValues(nomenclature, wsNomenclature);

                    int nomId = -1;
                    bool isNomIdSpec = false;
                    WsScm.ImportNomenclature(wsNomenclature, userId, true, out nomId, out isNomIdSpec);
                    //WsScm.ImportNomenclature(wsNomenclature, userId, true);
                    
                } catch (Exception ex) {
                    if (importErrors == null) {
                        importErrors = new List<ImportError>();
                    }

                    var errItem = new ImportError();
                    errItem.ErrorMsg = ex.Message;
                    importErrors.Add(errItem);
                }
            }
            

            //var t = resList.Count;


            //********************************************************

            //siteUrl = "http://portaldms/ACE/Oddeleni/QTY/";
            //clientContext = new ClientContext(siteUrl);
            //list = clientContext.Web.Lists.GetByTitle("eBench_CZSK_PC_Log");

            //camlQuery = new CamlQuery();
            //camlQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Datum' /><Value Type='DateTime'><Today/></Value></Eq></Where></Query></View>";
            //resList = list.GetItems(camlQuery);

            //clientContext.Load(resList);
            //clientContext.ExecuteQuery();

            //var i = resList.Count;

            //************************************************************************            

        }

        private string GetFieldValue(object fieldValue) {
            if (fieldValue == null) {
                return null;
            }

            return fieldValue.ToString();
        }

        private DateTime GetDateTimeValue(object fieldValue) {
            if (fieldValue == null) {
                return DateTime.MinValue;
            }

            return (DateTime) fieldValue;
        }

        private Nomenclature GetScmNomenclatureFromLine(string nomKey, string nomSpec, string nomMaterialGroup, Hashtable htMgs) {
            Nomenclature nomenclature = new Nomenclature();
            nomenclature.nomenclature_key = nomKey;
            nomenclature.specification = nomSpec;

            
            int materialGroupId = GetMaterialGroupId(nomMaterialGroup, htMgs);
            if (materialGroupId > -1) {
                nomenclature.material_group_id = materialGroupId;
            } else {
                throw new Exception("Material Group '" + nomMaterialGroup + "' was not found");
            }

            //string strDate = null;
            //if (lineItems.Length > 4) {
            //    strDate = GetLineItemValue(lineItems[5]);
            //}

            //if (lineItems.Length > 5) {
            //    strDate += " " + GetLineItemValue(lineItems[6]);
            //}

            //DateTime createDate;
            //bool isDate = DateTime.TryParseExact(
            //    strDate,
            //    "dd/MM/yy HH:mm:ss",
            //    CultureInfo.InvariantCulture,
            //    DateTimeStyles.None,
            //    out createDate);
            //if (isDate) {
            //    nomenclature.created_date = createDate;
            //}

            

            nomenclature.source_id = (int)NomenclatureRepository.NomSource.DNSSharePoint;

            return nomenclature;
        }
        #endregion
    }
}
