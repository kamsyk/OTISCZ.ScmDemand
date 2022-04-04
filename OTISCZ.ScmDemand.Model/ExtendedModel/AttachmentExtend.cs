using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OTISCZ.ScmDemand.Model.ExtendedModel {
    public class AttachmentExtend : Attachment {
        public int att_type { get; set; }
        public string added_by { get; set; }
        //public BitmapSource icon_bitmap { get; set; }
    }
}
