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

namespace OTISCZ.ScmDemand.UI.ScmUserControl {
    /// <summary>
    /// Interaction logic for UcDataGridHeader.xaml
    /// </summary>
    public partial class UcDataGridHeader : UserControl, IView {
        #region Properties
        public static readonly DependencyProperty GrdTitleProperty = DependencyProperty.Register(
            "GrdTitle",
            typeof(String),
            typeof(UcDataGridHeader),
            new UIPropertyMetadata("GrdTitle"));

        public string GrdTitle {
            get { return (string)GetValue(GrdTitleProperty); }
            set {
                SetValue(GrdTitleProperty, value);
                txtGridColHeader.Text = value;
            }
        }

        private MainWindow m_MainWindow {
            get { return (MainWindow)Application.Current.MainWindow; }
        }
        #endregion

        #region Constructor
        public UcDataGridHeader() {
            InitializeComponent();

            
        }
        #endregion

        #region Methods
        public void SetLayout() {
            MainWindow.RefreshStyles(dpHeader);
            //dpHeader.Style = m_MainWindow.GetLayoutStyle("DpGrdHeader");
        }

        public void LocalizeUc() {
            throw new NotImplementedException();
        }
        #endregion
    }
}
