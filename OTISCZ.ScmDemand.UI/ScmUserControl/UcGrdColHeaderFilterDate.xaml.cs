using OTISCZ.CommonDb;
using OTISCZ.ScmDemand.UI.Resource;
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
    /// Interaction logic for UcGrdColHeaderFilterDate.xaml
    /// </summary>
    public partial class UcGrdColHeaderFilterDate : UserControl, IView
    {
        #region Event
        public static readonly RoutedEvent FilterFromChangedEvent = EventManager.RegisterRoutedEvent(
            "FilterFromChangedEvent",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(UcGrdColHeaderFilterDate));


        public event RoutedEventHandler FilterFromChanged {
            add { AddHandler(FilterFromChangedEvent, value); }
            remove { RemoveHandler(FilterFromChangedEvent, value); }
        }

        public static readonly RoutedEvent FilterToChangedEvent = EventManager.RegisterRoutedEvent(
            "FilterToChangedEvent",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(UcGrdColHeaderFilterDate));


        public event RoutedEventHandler FilterToChanged {
            add { AddHandler(FilterToChangedEvent, value); }
            remove { RemoveHandler(FilterToChangedEvent, value); }
        }
        #endregion

        #region Properties
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(String),
            typeof(UcGrdColHeaderFilterDate),
            new UIPropertyMetadata("Title"));

        public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register(
            "FieldName",
            typeof(String),
            typeof(UcGrdColHeaderFilterDate),
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
        //    set { SetValue(SortedProperty, value); }
        //}

        public DateTime? DateFrom {
            get { return dtFrom.SelectedDate; }
            set { dtFrom.SelectedDate = value; }
        }

        public DateTime? DateTo {
            get {
                if (dtTo.SelectedDate != null) {
                    return ((DateTime)dtTo.SelectedDate).AddDays(1);
                }
                return dtTo.SelectedDate;
            }
            set { dtTo.SelectedDate = value; }
        }

        public string DateFromText {
            get {
                if (DateFrom == null) {
                    return null;
                }
                return ConvertData.ToDbDate((DateTime)DateFrom); }
        }

        public string DateToText {
            get {
                if (DateTo == null) {
                    return null;
                }
                return ConvertData.ToDbDate((DateTime)DateTo);
            }
        }
        #endregion

        #region Properties
        public UcGrdColHeaderFilterDate()
        {
            InitializeComponent();
            LocalizeUc();
        }
        #endregion

        #region Interface Methods
        public void LocalizeUc() {
            lblFrom.Content = ScmResource.FromToFrom;
            lblTo.Content = ScmResource.FromToTo;
        }

        public void SetLayout() {
            //throw new NotImplementedException();
        }
        #endregion

        private void DtTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {
            RaiseEvent(new RoutedEventArgs(FilterToChangedEvent));
        }

        private void DtFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {
            RaiseEvent(new RoutedEventArgs(FilterFromChangedEvent));
        }

        
    }
}
