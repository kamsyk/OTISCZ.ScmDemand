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
    
    public partial class User_Setting
    {
        public int user_id { get; set; }
        public string culture { get; set; }
    
        public virtual ScmUser ScmUser { get; set; }
    }
}