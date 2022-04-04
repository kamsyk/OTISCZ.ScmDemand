using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace OTISCZ.ScmDemand.UI.View.LayoutConverters.DataGrid
{
    public class HeaderTitleBkgG1 : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Pink) {
                return (Color)ColorConverter.ConvertFromString("#FCE4EC");
                //return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FCE4EC"));
            } else if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Black) {
                return (Color)ColorConverter.ConvertFromString("#6A1B9A");
                //return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FCE4EC"));
            }

            return (Color)ColorConverter.ConvertFromString("#eff5ff");
            //return (SolidColorBrush)(new BrushConverter().ConvertFrom("#eff5ff"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
