using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OTISCZ.ScmDemand.UI.View
{
    public class DataGridUtil {
        public static bool GetIsSortDisabled(DependencyObject obj) {
            return (bool)obj.GetValue(IsSortDisabledProperty);
        }

        public static void SetIsSortDisabled(DependencyObject obj, bool value) {
            obj.SetValue(IsSortDisabledProperty, value);
        }

        public static readonly DependencyProperty IsSortDisabledProperty = DependencyProperty.RegisterAttached(
            "IsSortDisabled", 
            typeof(bool), 
            typeof(DataGridUtil), new UIPropertyMetadata(false));

        public static string GetScmColumnName(DependencyObject obj) {
            return (string)obj.GetValue(ScmColumnName);
        }

        public static void SetScmColumnName(DependencyObject obj, string value) {
            obj.SetValue(ScmColumnName, value);
        }

        public static readonly DependencyProperty ScmColumnName = DependencyProperty.RegisterAttached(
            "ScmColumnName",
            typeof(string),
            typeof(DataGridUtil), new UIPropertyMetadata(null));
    }
}
