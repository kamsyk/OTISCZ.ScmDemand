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
    
    public partial class Active_Nomenclature
    {
        public int id { get; set; }
        public int version { get; set; }
        public string remark { get; set; }
    
        public virtual Nomenclature Nomenclature { get; set; }
    }
}
