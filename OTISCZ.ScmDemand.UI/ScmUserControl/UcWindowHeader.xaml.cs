using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OTISCZ.ScmDemand.UI.ScmUserControl
{
    /// <summary>
    /// Interaction logic for UcWindowHeader.xaml
    /// </summary>
    public partial class UcWindowHeader : UserControl
    {
        #region Properties
        public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(
            "WindowTitle",
            typeof(String),
            typeof(UcWindowHeader),
            new UIPropertyMetadata("Window Title"));

        public string WindowTitle {
            get { return (string)GetValue(WindowTitleProperty); }
            set {
                SetValue(WindowTitleProperty, value);
                txtWindowTitle.Text = value;
            }
        }

        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }

        //public Style LayoutStyle {
        //    get { return dpHeader.Style; }
        //    set { dpHeader.Style = value; }
        //}
        #endregion

        #region Constructor
        public UcWindowHeader()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void CloseUc() {
            var parent = this.Parent;
            while (parent != null) {
                if (parent is MainWindow) {
                    MainWindow mw = parent as MainWindow;
                    mw.ClearChildren();
                    break;
                }

                parent = LogicalTreeHelper.GetParent(parent);
            }
        }

        public void SetLayout() {
            //dpHeader.Style = m_MainWindow.GetLayoutStyle("DpWinHeader");
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e) {
            CloseUc();
        }
    }
}
