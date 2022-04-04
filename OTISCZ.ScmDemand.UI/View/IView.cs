using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.UI.ScmUserControl {
    interface IView {
        void LocalizeUc();
        void SetLayout();
        //void AdjustWidth(double dWidth);
    }
}
