//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OTISCZ.ScmDemand.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Supplier_Contact
    {
        public int id { get; set; }
        public int supplier_id { get; set; }
        public string first_name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string phone_nr { get; set; }
        public System.DateTime modif_date { get; set; }
        public int modif_user_id { get; set; }
        public string phone_nr2 { get; set; }
    
        public virtual Supplier Supplier { get; set; }
        public virtual ScmUser ScmUser { get; set; }
    }
}
