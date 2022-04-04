using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ScmUserControl;
using OTISCZ.ScmDemand.UI.ViewModel;
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

using OTISCZ.ScmDemand.Model.ExtendedModel;

namespace OTISCZ.ScmDemand.UI.View {
    /// <summary>
    /// Interaction logic for SupplierView.xaml
    /// </summary>
    public partial class SupplierView : UserControl, IView {
        #region Properties
        private VmSupplier m_ViewModel {
            get { return ((VmSupplier)DataContext); }
        }
        #endregion

        #region Constructor
        public SupplierView() {
            InitializeComponent();
            LocalizeUc();
        }
        #endregion

        #region Interface Methods
        public void LocalizeUc() {
            lblSupplierName.Content = ScmResource.NameIt;
            lblSupplierIco.Content = ScmResource.SupplierId;

            txtGridColContacts.Text = ScmResource.Contacts;

            VmBaseGrid.LocalizeContactGridColumnsLabels(grdContact);
        }

       
        public void SetLayout() {
            
        }

        #endregion

        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            //var err = Validation.GetHasError(grdContact);

            m_ViewModel.SaveSupplier(true);
            
        }

        private void GrdContact_AddingNewItem(object sender, AddingNewItemEventArgs e) {
            
            e.NewItem = new SupplierContactExtended {
                id = -1,
                supplier_id = m_ViewModel.SupplierExtend.id
            };
            
        }

        private void GrdContact_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) {
            SupplierContactExtended supplierContactExtended = (SupplierContactExtended)e.Row.DataContext;
            if (supplierContactExtended.id < 0
                && String.IsNullOrWhiteSpace(supplierContactExtended.first_name)
                && String.IsNullOrWhiteSpace(supplierContactExtended.surname)
                && String.IsNullOrWhiteSpace(supplierContactExtended.email)
                && String.IsNullOrWhiteSpace(supplierContactExtended.phone_nr)
                && String.IsNullOrWhiteSpace(supplierContactExtended.phone_nr2)) {
                e.Cancel = true;
                //m_ViewModel.SaveSupplier();
            } else {
                if (supplierContactExtended.entity_error == null || supplierContactExtended.entity_error.Count == 0) {
                    m_ViewModel.SaveSupplierContact(supplierContactExtended);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) {
            m_ViewModel.CloseWindow();
        }

        private void GrdContact_SourceUpdated(object sender, DataTransferEventArgs e) {
            //m_ViewModel.SaveSupplier();
        }

        private void GrdContact_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            //m_ViewModel.SaveSupplier();
        }

        private void GrdContact_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //m_ViewModel.SaveSupplier();
        }

        private void GrdContact_LostFocus(object sender, RoutedEventArgs e) {
            //m_ViewModel.SaveSupplier();
        }
    }

    public class SupplierContactValidationRule : ValidationRule {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo) {
            BindingExpression be = (BindingExpression)value;
            var strPropName = be.ResolvedSourcePropertyName;

            if (strPropName == "email") {
                
                if (be.DataItem is SupplierContactExtended) {
                    
                    SupplierContactExtended sc = (SupplierContactExtended)be.DataItem;

                    if (VmSupplier.IsMailAddressValid(sc.email)) {
                        if (sc.entity_error != null) {
                            int id = sc.id;
                            var selEnt = (from erDb in sc.entity_error
                                          where erDb.id == id
                                          select erDb).FirstOrDefault();
                            if (selEnt != null) {
                                sc.entity_error.Remove(selEnt);
                            }
                        }
                    } else { 

                        if (sc.entity_error == null) {
                            sc.entity_error = new List<EntityError>();
                        }

                        int id = sc.id;
                        var selEnt = (from erDb in sc.entity_error
                                      where erDb.id == id
                                      select erDb).FirstOrDefault();
                        if (selEnt == null) {
                            EntityError ee = new EntityError {
                                id = sc.id,
                                errors = new List<string>()
                            };
                            ee.errors.Add(ScmResource.MailFormatError);
                            sc.entity_error.Add(ee);
                        }

                        MessageBox.Show(ScmResource.MailFormatError, ScmResource.MailFormatError, MessageBoxButton.OK, MessageBoxImage.Exclamation);

                        return new ValidationResult(false, ScmResource.MailFormatError);
                    }
                }

                return ValidationResult.ValidResult;
            } else {
                return ValidationResult.ValidResult;
            }
            
            
        }
    }
}
