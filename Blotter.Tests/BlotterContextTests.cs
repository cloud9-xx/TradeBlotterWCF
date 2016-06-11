using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blotter.Service;
using Blotter.Model;
using System.Linq;
using System.ServiceModel;
using Blotter.Service.Faults;

namespace Blotter.Tests
{
    [TestClass]
    public class BlotterContextTests
    {
        [ExpectedException(typeof(FaultException<AttemptingToUpdateOldVersionOfTrade>))]
        [TestMethod]
        public void Test_AddUpdateTrades_Throws_AttemptingToUpdateOldVersionOfTrade()
        {
            BlotterContext context = BlotterContext.Instance;

            var newTrade = Trade.RandomTrade(context.Securities.ToList(), context.Counterparties.ToList()).First();
            var update1 = newTrade.Copy();
            var update2 = newTrade.Copy();

            Assert.AreEqual(newTrade.Version, update1.Version);
            Assert.AreEqual(newTrade.Version, update2.Version);

            //Put in version 1 of the trade
            context.AddUpdateTrade(newTrade);

            //Update this first version creating a version 2
            context.AddUpdateTrade(update1);

            //Try to update version 1 again, triggering a FaultException<AttemptingToUpdateOldVersionOfTrade>
            context.AddUpdateTrade(update2);
        }

        [TestMethod]
        public void Test_AddUpdateTrades_Does_Not_Throw_AttemptingToUpdateOldVersionOfTrade()
        {
            BlotterContext context = BlotterContext.Instance;

            var newTrade = Trade.RandomTrade(context.Securities.ToList(), context.Counterparties.ToList()).First();
            var update1 = newTrade.Copy();
            var update2 = newTrade.Copy();

            update2.Version = update1.Version + 1;

            Assert.AreEqual(newTrade.Version, update1.Version);
            Assert.AreEqual(update1.Version + 1, update2.Version);

            //Put in version 1 of the trade
            context.AddUpdateTrade(newTrade);

            //Update this first version creating a version 2
            context.AddUpdateTrade(update1);

            //Now we are updating version 2 -> version 3
            context.AddUpdateTrade(update2);

            var finalTrade = context.GetHistoryForTrade(newTrade.Id).Where(t => t.Version == 3).FirstOrDefault();

            Assert.IsNotNull(finalTrade);
            Assert.AreEqual(3, finalTrade.Version);
        }
    }
}
