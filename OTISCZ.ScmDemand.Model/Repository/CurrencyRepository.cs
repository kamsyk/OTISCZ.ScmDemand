using OTISCZ.ScmDemand.Model.DataDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.Model.Repository {
    public class CurrencyRepository : BaseRepository<Currency> {
        #region Methods
        public List<Currency> GetAtiveCurrencies() {
            
            List<Currency> currencies = (from currencyDb in m_dbContext.Currency
                              where currencyDb.is_active
                              select currencyDb).ToList();

            if (currencies == null) {
                return null;
            }

            List<Currency> retCurrencies = new List<Currency>();
            foreach (var currency in currencies) {
                Currency retCurrency = new Currency();
                SetValues(currency, retCurrency);
                retCurrencies.Add(retCurrency);
            }

            return retCurrencies;
        }
        #endregion
    }
}
