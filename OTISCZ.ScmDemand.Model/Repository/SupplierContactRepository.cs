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
    public class SupplierContactRepository : BaseRepository<Supplier_Contact> {
        #region Methods
        
        public int GetLastId() {

            var dbSupplierContact = (from supplierCDb in m_dbContext.Supplier_Contact
                            orderby supplierCDb.id descending
                            select supplierCDb).FirstOrDefault();

            if (dbSupplierContact == null) {
                return -1;
            }

            return dbSupplierContact.id;
        }

        
        public int SaveSupplierContact(SupplierContactExtended supplierContactExtended, int userId) {
            int suppcontactId = supplierContactExtended.id;
            var dbSupplierContact = (from supplierContactDb in m_dbContext.Supplier_Contact
                                     where supplierContactDb.id == supplierContactExtended.id
                                     select supplierContactDb).FirstOrDefault();

            if (dbSupplierContact == null || suppcontactId < 0) {
                int lastId = GetLastId();
                int newId = ++lastId;
                suppcontactId = newId;

                Supplier_Contact newSuppContact = new Supplier_Contact();
                
                SetValues(supplierContactExtended, newSuppContact);

                newSuppContact.id = newId;
                newSuppContact.modif_date = DateTime.Now;
                newSuppContact.modif_user_id = userId;
                m_dbContext.Supplier_Contact.Add(newSuppContact);

                SaveChanges();
            } else {
                bool isModified = false;
                if (supplierContactExtended.first_name != dbSupplierContact.first_name) {
                    isModified = true;
                }

                if (supplierContactExtended.surname != dbSupplierContact.surname) {
                    isModified = true;
                }

                if (supplierContactExtended.email != dbSupplierContact.email) {
                    isModified = true;
                }

                if (supplierContactExtended.phone_nr != dbSupplierContact.phone_nr) {
                    isModified = true;
                }

                if (supplierContactExtended.phone_nr2 != dbSupplierContact.phone_nr2) {
                    isModified = true;
                }

                if (isModified) {
                    SetValues(supplierContactExtended, dbSupplierContact);
                    dbSupplierContact.modif_date = DateTime.Now;
                    dbSupplierContact.modif_user_id = userId;

                    SaveChanges();
                }
            }

            

            return suppcontactId;
        }

        #endregion
    }
}
