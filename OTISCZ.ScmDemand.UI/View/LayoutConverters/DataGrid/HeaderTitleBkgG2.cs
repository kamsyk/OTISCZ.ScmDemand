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
    public class HeaderTitleBkgG2 : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Pink) {
                return (Color)ColorConverter.ConvertFromString("#F8BBD0");
                //return (SolidColorBrush)(new BrushConverter().ConvertFrom("#F8BBD0"));
            } else if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Black) {
                return (Color)ColorConverter.ConvertFromString("#8E24AA");
                //return (SolidColorBrush)(new BrushConverter().ConvertFrom("#F8BBD0"));
            }

            return (Color)ColorConverter.ConvertFromString("#e0ecff");
            //return (SolidColorBrush)(new BrushConverter().ConvertFrom("#e0ecff"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
