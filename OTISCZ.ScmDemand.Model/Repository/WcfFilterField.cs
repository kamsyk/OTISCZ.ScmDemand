using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class WcfFilterField {
        #region Enums
        //public enum FilterFromTo {
        //    No,
        //    From,
        //    To
        //}
        #endregion

        public string FieldName { get; set; }
        public string FilterText { get; set; }
        public int FromTo { get; set; }
        public string SqlFilter { get; set; }

        //public WcfFilterField(string fieldName, string filterText, int fromTo, string sqlFilter) {
        //    FieldName = fieldName;
        //    FilterText = filterText;
        //    FromTo = fromTo;
        //    SqlFilter = sqlFilter;
        //}
    }
}
