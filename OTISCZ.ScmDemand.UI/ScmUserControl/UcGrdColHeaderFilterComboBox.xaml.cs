using OTISCZ.ScmDemand.Interface.WsScmDemand;
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
    /// Interaction logic for UcGrdColHeaderFilterComboBox.xaml
    /// </summary>
    public partial class UcGrdColHeaderFilterComboBox : UserControl {
        #region Event
        public static readonly RoutedEvent FilterComboBoxChangedEvent = EventManager.RegisterRoutedEvent(
            "FilterComboBoxChangedEvent",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(UcGrdColHeaderFilterComboBox));


        public event RoutedEventHandler FilterComboBoxChanged {
            add { AddHandler(FilterComboBoxChangedEvent, value); }
            remove { RemoveHandler(FilterComboBoxChangedEvent, value); }
        }

        #endregion

        #region Properties
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(String),
            typeof(UcGrdColHeaderFilterComboBox),
            new UIPropertyMetadata("Title"));

        public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register(
            "FieldName",
            typeof(String),
            typeof(UcGrdColHeaderFilterComboBox),
            new UIPropertyMetadata("FieldName"));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "CmbItemsSource",
            typeof(System.Collections.IEnumerable),
            typeof(UcGrdColHeaderFilterComboBox),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
           "CmbDisplayMemberPath",
           typeof(String),
           typeof(UcGrdColHeaderFilterComboBox),
           new UIPropertyMetadata(null));

        public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register(
           "CmbSelectedValuePath",
           typeof(String),
           typeof(UcGrdColHeaderFilterComboBox),
           new UIPropertyMetadata(null));

        //private static Demand_Status[] GetDemandStatuses() {
        //    Demand_Status[] demSats = new Demand_Status[2];
        //    Demand_Status ds = new Demand_Status();
        //    ds.name = "dsasdasas fd";
        //    demSats[0] = ds;

        //    ds = new Demand_Status();
        //    ds.name = "ss3w4dsasdasas fd";
        //    demSats[1] = ds;

        //    return demSats;
        //}

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string FieldName {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }

        public System.Collections.IEnumerable CmbItemsSource {
            get {
                return (System.Collections.IEnumerable)GetValue(ItemsSourceProperty);
                //return cmbFilter.ItemsSource;
            }
            set {
                //SetValue(ItemsSourceProperty, null);
                SetValue(ItemsSourceProperty, value);
                //cmbFilter.ItemsSource = null;
                //cmbFilter.ItemsSource = value;
            }
        }

        public string CmbDisplayMemberPath {
            get { return (string)GetValue(DisplayMemberPathProperty);  }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
        
        public string CmbSelectedValuePath {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public object CmbSelectedValue {
            get { return cmbFilter.SelectedValue; }
            
        }
        #endregion

        #region Constructor
        public UcGrdColHeaderFilterComboBox() {
            InitializeComponent();
        }
        #endregion

        private void CmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            RaiseEvent(new RoutedEventArgs(FilterComboBoxChangedEvent));
        }
    }
}
