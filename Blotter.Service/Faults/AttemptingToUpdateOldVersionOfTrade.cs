using Blotter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Blotter.Service.Faults
{
    [DataContract]
    public class AttemptingToUpdateOldVersionOfTrade
    {
        public Trade LatestVersion { get; private set; }
        public Trade UpdateRequested { get; private set; }

        public AttemptingToUpdateOldVersionOfTrade(Trade latest, Trade request)
        {
            LatestVersion = latest;
            UpdateRequested = request;
        }
    }
}
