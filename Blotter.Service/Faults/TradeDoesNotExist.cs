using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Blotter.Service.Faults
{
    [DataContract]
    public class TradeDoesNotExist
    {
        public string TradeId { get; private set; }

        public TradeDoesNotExist(string tradeId)
        {
            TradeId = tradeId;
        }
    }
}
