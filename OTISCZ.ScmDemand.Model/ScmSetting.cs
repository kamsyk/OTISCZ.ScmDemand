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
    
    public partial class ScmSetting
    {
        public int id { get; set; }
        public string prodis_input_folder { get; set; }
        public Nullable<System.DateTime> last_import_date { get; set; }
        public string prodis_price_input_folder { get; set; }
        public string plnaknavrh_file_path { get; set; }
    }
}