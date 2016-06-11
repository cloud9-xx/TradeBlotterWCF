using Blotter.Model;
using System.Collections.Generic;
using System.ServiceModel;

namespace Blotter.Service
{
    [ServiceContract]
    public interface IReferenceDataService
    {
        [OperationContract]
        List<Counterparty> GetCounterparties();

        [OperationContract]
        List<Security> GetSecurities();
    }
}
