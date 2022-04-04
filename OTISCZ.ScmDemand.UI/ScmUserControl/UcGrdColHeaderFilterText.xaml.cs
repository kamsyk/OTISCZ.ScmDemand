using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for UcGrdColHeaderFilterText.xaml
    /// </summary>
    public partial class UcGrdColHeaderFilterText : UserControl {
        #region Event
        public static readonly RoutedEvent FilterTextChangedEvent = EventManager.RegisterRoutedEvent(
            "FilterTextChangedEvent",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(UcGrdColHeaderFilterText));
        

        public event RoutedEventHandler FilterTextChanged {
            add { AddHandler(FilterTextChangedEvent, value); }
            remove { RemoveHandler(FilterTextChangedEvent, value); }
        }

        #endregion

        #region Properties
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(String),
            typeof(UcGrdColHeaderFilterText),
            new UIPropertyMetadata("Title"));

        public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register(
            "FieldName",
            typeof(String),
            typeof(UcGrdColHeaderFilterText),
            new UIPropertyMetadata("FieldName"));

        //public static readonly DependencyProperty SortedProperty = DependencyProperty.Register(
        //    "Sorted",
        //    typeof(ListSortDirection),
        //    typeof(UcGrdColHeaderFilterText),
        //    new UIPropertyMetadata(null));

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string FieldName {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }

        //public ListSortDirection Sorted {
        //    get { return (ListSortDirection)GetValue(SortedProperty); }
        //    set {
        //        SetValue(SortedProperty, value);
        //        imgSortAsc.Visibility = Visibility.Collapsed;
        //    }
        //}

        public string FilterText {
            get { return txtFilter.Text; }
        }
        #endregion

        #region Constructor
        public UcGrdColHeaderFilterText() {
            InitializeComponent();
        }
        #endregion

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e) {
            RaiseEvent(new RoutedEventArgs(FilterTextChangedEvent));
        }
    }
}
