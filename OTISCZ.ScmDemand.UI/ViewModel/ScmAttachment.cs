using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class ScmAttachment {
        public int id { get; set; }
        public int parent_id { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }
        public BitmapSource icon { get; set; }
        public byte[] file_content { get; set; }
        public ScmMail mail { get; set; }
        public int att_type { get; set; }
        public string added_by { get; set; }
        public Visibility deletebtn_visibility { get; set; }
    }
}
