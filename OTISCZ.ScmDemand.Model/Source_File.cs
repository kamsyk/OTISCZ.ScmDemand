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
    
    public partial class Source_File
    {
        public int id { get; set; }
        public string name { get; set; }
        public System.DateTime last_modification_date { get; set; }
        public int status { get; set; }
        public System.DateTime import_date { get; set; }
        public int import_user_id { get; set; }
    
        public virtual ScmUser ScmUser { get; set; }
    }
}
