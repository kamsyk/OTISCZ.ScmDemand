using OTISCZ.CommonDb;
using OTISCZ.ScmDemand.Model.DataDictionary;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class SupplierRepository : BaseRepository<Supplier> {
        #region Methods
        
        private int GetLastId() {

            var dbSupplier = (from supplierDb in m_dbContext.Supplier
                            orderby supplierDb.id descending
                            select supplierDb).FirstOrDefault();

            if (dbSupplier == null) {
                return -1;
            }

            return dbSupplier.id;
        }

        public Supplier GetSupplierBySupplierId(string supplierId) {
            var wsSupplier = (from supDb in m_dbContext.Supplier
                            where supDb.supplier_id == supplierId
                            select supDb).FirstOrDefault();
            if (wsSupplier == null) {
                return null;
            }

            Supplier supplier = new Supplier();
            SetValues(wsSupplier, supplier);

            return supplier;
        }

        public void DeactiveSuppliers(List<int> htActiveSuppliersIds, int userId) {
            if (htActiveSuppliersIds == null) {
                return;
            }

            var wsSuppliers = (from supDb in m_dbContext.Supplier
                              where supDb.active == true
                              select supDb).ToList();

            foreach (var supp in wsSuppliers) {
                if (!htActiveSuppliersIds.Contains(supp.id)) {
                    supp.active = false;
                    supp.is_approved = false;
                    supp.modif_user_id = userId;
                    supp.modif_date = DateTime.Now;
                    SaveChanges();
                }
            }
        }

        public SupplierExtend GetSupplierById(int id) {
            var wsSupplier = (from supDb in m_dbContext.Supplier
                              where supDb.id == id
                              select supDb).FirstOrDefault();
            if (wsSupplier == null) {
                return null;
            }

            SupplierExtend supplier = new SupplierExtend();
            SetValues(wsSupplier, supplier);

            supplier.supplier_contact_extended = new List<SupplierContactExtended>();
            if (wsSupplier.Supplier_Contact != null) {
                foreach (var contact in wsSupplier.Supplier_Contact) {
                    SupplierContactExtended suppCont = new SupplierContactExtended();
                    SetValues(contact, suppCont);
                    supplier.supplier_contact_extended.Add(suppCont);
                }
            }

            return supplier;
        }

        public int SaveSupplier(SupplierExtend supplierExtend, int userId, bool isContactUpdate, bool isImport) {
            int supplierId = supplierExtend.id;

            Supplier dbSupplier = null;
            if (isImport) {
                dbSupplier = (from supplierDb in m_dbContext.Supplier
                              where supplierDb.supplier_id.ToLower().Trim() == supplierExtend.supplier_id.ToLower().Trim()
                              select supplierDb).FirstOrDefault();
            } else {
                dbSupplier = (from supplierDb in m_dbContext.Supplier
                              where supplierDb.id == supplierExtend.id
                              select supplierDb).FirstOrDefault();
            }


            if (dbSupplier != null) {
                bool isModified = false;

                if (dbSupplier.supp_name != supplierExtend.supp_name) {
                    isModified = true;
                    dbSupplier.supp_name = supplierExtend.supp_name;

                }

                if (dbSupplier.street_part1 != supplierExtend.street_part1) {
                    isModified = true;
                    dbSupplier.street_part1 = supplierExtend.street_part1;

                }

                if (dbSupplier.street_part2 != supplierExtend.street_part2) {
                    isModified = true;
                    dbSupplier.street_part2 = supplierExtend.street_part2;

                }

                if (dbSupplier.city != supplierExtend.city) {
                    isModified = true;
                    dbSupplier.city = supplierExtend.city;

                }

                if (dbSupplier.zip != supplierExtend.zip) {
                    isModified = true;
                    dbSupplier.zip = supplierExtend.zip;

                }

                if (dbSupplier.country != supplierExtend.country) {
                    isModified = true;
                    dbSupplier.country = supplierExtend.country;
                    
                }

                bool isConactModified = false;
                if (isContactUpdate) {
                    isConactModified = UpdateSupplierContacts(supplierExtend.supplier_contact_extended, dbSupplier.Supplier_Contact, userId, dbSupplier.id);
                }

                if (isModified || isConactModified) {
                    SetWoDiaTexts(dbSupplier);

                    //dbSupplier.is_approved = true;

                    m_dbContext.Entry(dbSupplier).State = EntityState.Modified;
                    SaveChanges();
                }
            } else {
                int lastId = GetLastId();
                int newId = ++lastId;
                supplierExtend.id = newId;
                supplierId = newId;
                //supplier.active = true;
                //supplier.is_approved = true;

                if (isContactUpdate) {
                    UpdateSupplierContacts(supplierExtend.supplier_contact_extended, null, userId, supplierId);
                }

                Supplier newSupplier = new Supplier();
                SetValues(supplierExtend, newSupplier);

                newSupplier.modif_date = DateTime.Now;
                newSupplier.modif_user_id = userId;

                SetWoDiaTexts(newSupplier);

                m_dbContext.Supplier.Add(newSupplier);
                SaveChanges();
            }

            return supplierId;
        }

        private void SetWoDiaTexts(Supplier dbSupplier) {
            if (String.IsNullOrWhiteSpace(dbSupplier.street_part1) && String.IsNullOrWhiteSpace(dbSupplier.street_part2)) {

            } else if (String.IsNullOrWhiteSpace(dbSupplier.street_part1) && !String.IsNullOrWhiteSpace(dbSupplier.street_part2)) {
                dbSupplier.street_part1 = dbSupplier.street_part2;
            } else if (!String.IsNullOrWhiteSpace(dbSupplier.street_part1) && String.IsNullOrWhiteSpace(dbSupplier.street_part2)) {
                dbSupplier.street_part1 = dbSupplier.street_part1;
            } else if (!String.IsNullOrWhiteSpace(dbSupplier.street_part1) && !String.IsNullOrWhiteSpace(dbSupplier.street_part2)) {
                dbSupplier.street_part1 = dbSupplier.street_part1 + ", " + dbSupplier.street_part2;
            }

            dbSupplier.supp_name_wo_dia = ConvertData.RemoveDiacritics(dbSupplier.supp_name);
            dbSupplier.street_part1_wo_dia = ConvertData.RemoveDiacritics(dbSupplier.street_part1);
            dbSupplier.street_part2_wo_dia = ConvertData.RemoveDiacritics(dbSupplier.street_part2);
            dbSupplier.city_wo_dia = ConvertData.RemoveDiacritics(dbSupplier.city);
            dbSupplier.country_wo_dia = ConvertData.RemoveDiacritics(dbSupplier.country);
        }

        //public int SaveSupplier(SupplierExtend supplierExt, int userId) {
        //    int supplierId = supplierExt.id;

        //    bool isModified = false;

        //    using (TransactionScope transaction = new TransactionScope()) {
        //        var dbSupplier = (from supplierDb in m_dbContext.Supplier
        //                          where supplierDb.id == supplierExt.id
        //                          select supplierDb).FirstOrDefault();
        //        if (dbSupplier != null) {
        //            dbSupplier.supp_name = supplierExt.supp_name;
        //            dbSupplier.supp_name_wo_dia = ConvertData.RemoveDiacritics(dbSupplier.supp_name);

        //            bool isConactModified =  UpdateSupplierContacts(supplierExt.supplier_contact_extended, dbSupplier.Supplier_Contact, userId, dbSupplier.id);

        //            dbSupplier.modif_date = DateTime.Now;
        //            dbSupplier.modif_user_id = userId;
        //        } else {
        //            isModified = true;
        //            dbSupplier = new Supplier();

        //            SetValues(supplierExt, dbSupplier);

        //            int lastId = GetLastId();
        //            int newId = ++lastId;
        //            supplierId = newId;
        //            dbSupplier.id = newId;
        //            dbSupplier.active = true;

        //            dbSupplier.supp_name_wo_dia = ConvertData.RemoveDiacritics(dbSupplier.supp_name);
        //            UpdateSupplierContacts(supplierExt.supplier_contact_extended, dbSupplier.Supplier_Contact, userId, newId);

        //            dbSupplier.modif_date = DateTime.Now;
        //            dbSupplier.modif_user_id = userId;

        //            m_dbContext.Supplier.Add(dbSupplier);
        //        }

        //        if (isModified) {
        //            SaveChanges();
        //        }

        //        transaction.Complete();
        //    }

        //    return supplierId;
        //}

        private bool UpdateSupplierContacts(ICollection<SupplierContactExtended> updContacts, ICollection<Supplier_Contact> dbContacts, int userId, int suppId) {
            bool isModified = false;

            //Delete
            if (dbContacts != null) {
                for (int i = dbContacts.Count - 1; i >= 0; i--) {
                    var fupdContacts = (from contDb in updContacts
                                        where contDb.id == dbContacts.ElementAt(i).id
                                        select contDb).FirstOrDefault();
                    if (fupdContacts == null) {
                        m_dbContext.Supplier_Contact.Remove(dbContacts.ElementAt(i));
                        isModified = true;
                    }
                }
            }

            //Update
            if (updContacts != null) {
                int lastId = -1;
                foreach (var updCont in updContacts) {
                    
                    if (updCont.id >= 0) {
                        //update existing
                        var fupdContact = (from contDb in dbContacts
                                            where contDb.id == updCont.id
                                            select contDb).FirstOrDefault();

                        if (updCont.first_name != fupdContact.first_name) {
                            isModified = true;
                        }

                        if (updCont.surname != fupdContact.surname) {
                            isModified = true;
                        }

                        if (updCont.email != fupdContact.email) {
                            isModified = true;
                        }

                        if (updCont.phone_nr != fupdContact.phone_nr) {
                            isModified = true;
                        }

                        if (updCont.phone_nr2 != fupdContact.phone_nr2) {
                            isModified = true;
                        }

                        if (isModified) {
                            SetValues(updCont, fupdContact);
                            fupdContact.modif_user_id = userId;
                            fupdContact.modif_date = DateTime.Now;
                            //SaveChanges();
                        }
                    } else {
                        //new contacts
                        Supplier_Contact newSupplierContact = new Supplier_Contact();
                        SetValues(updCont, newSupplierContact);

                        if (lastId < 0) {
                            lastId = new SupplierContactRepository().GetLastId();
                        } else {
                            lastId++;
                        }
                        int newId = ++lastId;

                        newSupplierContact.id = newId;
                        newSupplierContact.supplier_id = suppId;
                        newSupplierContact.modif_user_id = userId;
                        newSupplierContact.modif_date = DateTime.Now;
                        
                        m_dbContext.Supplier_Contact.Add(newSupplierContact);
                        isModified = true;
                    }
                }
            }

            return isModified;
        }

        //private int GetContactLastId() {

        //    var att = (from attDb in m_dbContext.Supplier_Contact
        //               orderby attDb.id descending
        //               select attDb).FirstOrDefault();

        //    if (att == null) {
        //        return -1;
        //    }

        //    return att.id;
        //}

        public List<SupplierExtend> GetSuppliers(
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount) {

            string strFilterWhere = GetFilter(filter);
            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT sd.*, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            string sqlPureBody = GetPureBody(strFilterWhere);

            //Get Row count
            string selectCount = "SELECT COUNT(*) " + sqlPureBody;
            rowsCount = m_dbContext.Database.SqlQuery<int>(selectCount).Single();

            //Get Part Data
            string sqlPart = sqlPure + sqlPureBody;
            int partStart = pageSize * (pageNr - 1) + 1;
            int partStop = partStart + pageSize - 1;

            while (partStart > rowsCount) {
                partStart -= pageSize;
                partStart = partStart + pageSize - 1;
            }

            string sql = "SELECT * FROM(" + sqlPart + ") AS RegetPartData" +
                " WHERE RegetPartData.RowNum BETWEEN " + partStart + " AND " + partStop;

            var suppliers = m_dbContext.Database.SqlQuery<Supplier>(sql).ToList();

            return GetSuppliers(suppliers, pageNr, pageSize);

        }

        public List<SupplierExtend> GetSuppliersReport(
            string filter,
            string sort) {
            
            string strFilterWhere = GetFilter(filter);

            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT sd.* ";
            string sqlPureBody = GetPureBody(strFilterWhere);
            sqlPureBody += strOrder;

            string sql = sqlPure + sqlPureBody;

            var suppliers = m_dbContext.Database.SqlQuery<Supplier>(sql).ToList();

            return GetSuppliers(suppliers);
        }

        private string GetOrder(string sort) {
            string strOrder = "ORDER BY sd." + SupplierData.ID_FIELD;

            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    }

                            if (strItemProp[0] == SupplierData.SUPP_NAME_FIELD) {
                                strOrder = "sd." + SupplierData.SUPP_NAME_FIELD + " " + strItemProp[1];
                    //        } else if (strItemProp[0] == NomenclatureData.NAME_FIELD) {
                    //            strOrder += "nd." + NomenclatureData.NAME_FIELD + " " + strItemProp[1];
                    //        } else if (strItemProp[0] == "material_group_text") {
                    //            strOrder += "mgd." + MaterialGroupData.NAME_FIELD + " " + strItemProp[1];
                            }

                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        private string GetFilter(string filter) {

            string strFilterWhere = "";
            if (!String.IsNullOrEmpty(filter)) {
                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {
                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    strFilterWhere += " AND ";

                    string columnName = strItemProp[0].Trim().ToUpper();
                    if (columnName == SupplierData.SUPPLIER_ID_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.SUPPLIER_ID_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    } else if (columnName == SupplierData.SUPP_NAME_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.SUPP_NAME_WO_DIA_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(strItemProp[1].Trim()) + "%'";
                    } else if (columnName == SupplierData.STREET_PART1_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.STREET_PART1_WO_DIA_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(strItemProp[1].Trim()) + "%'";
                    } else if (columnName == SupplierData.CITY_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.CITY_WO_DIA_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(strItemProp[1].Trim()) + "%'";
                    } else if (columnName == SupplierData.ZIP_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.ZIP_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    } else if (columnName == SupplierData.COUNTRY_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "sd." + SupplierData.COUNTRY_WO_DIA_FIELD + " LIKE '%" + ConvertData.RemoveDiacritics(strItemProp[1].Trim()) + "%'";
                    }

                }
            }

            return strFilterWhere;
        }

        private string GetPureBody(string strFilterWhere) {
            string sqlPureBody = " FROM " + SupplierData.TABLE_NAME + " sd"
                //+ " LEFT OUTER JOIN " + MaterialGroupData.TABLE_NAME + " mgd"
                //+ " ON nd." + NomenclatureData.MATERIAL_GROUP_ID_FIELD + "=mgd." + MaterialGroupData.ID_FIELD
                + " WHERE 1=1";

            sqlPureBody += strFilterWhere;

            return sqlPureBody;
        }

        private List<SupplierExtend> GetSuppliers(List<Supplier> suppliers) {
            return GetSuppliers(suppliers, 1, 1);
        }

        public List<SupplierExtend> GetSuppliers(
            List<Supplier> suppliers,
            int pageNr,
            int pageSize) {

            List<SupplierExtend> suppliersExtend = new List<SupplierExtend>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var supplier in suppliers) {
                Supplier dbSupplier = (from dbSupBd in m_dbContext.Supplier
                                               where dbSupBd.id == supplier.id
                                               select dbSupBd).FirstOrDefault();

                SupplierExtend supplierExtend = new SupplierExtend();
                SetValues(supplier, supplierExtend);
                if (!String.IsNullOrWhiteSpace(supplierExtend.country)) {
                    supplierExtend.country = supplierExtend.country.Trim();
                }
                supplierExtend.row_index = rowIndex++;
                supplierExtend.img_approved_path = "/OTISCZ.ScmDemand.UI;component/Images/Grid/Checked.png";

                suppliersExtend.Add(supplierExtend);
            }

            return suppliersExtend;
        }
        #endregion
    }
}
