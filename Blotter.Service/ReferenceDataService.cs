using System.Collections.Generic;
using System.Linq;
using Blotter.Model;

namespace Blotter.Service
{
    public class ReferenceDataService : IReferenceDataService
    {
        public List<Counterparty> GetCounterparties()
        {
            return BlotterContext.Instance.Counterparties.ToList();
        }

        public List<Security> GetSecurities()
        {
            return BlotterContext.Instance.Securities.ToList();
        }
    }
}
