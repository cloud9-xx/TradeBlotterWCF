using Blotter.Model;
using Blotter.Service.Faults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Blotter.Service
{
    public class BlotterContext
    {
        #region Singleton Bits

        private static readonly BlotterContext instance = new BlotterContext();

        static BlotterContext() { }

        public static BlotterContext Instance { get { return instance; } }
        #endregion

        private Dictionary<string, List<Trade>> trades = new Dictionary<string, List<Trade>>();
        private List<Counterparty> counterparties = new List<Counterparty>();
        private List<Security> securities = new List<Security>();

        public static event TradeUpdateHandler TradeUpdateEvent;
        public delegate void TradeUpdateHandler(Trade trade);
        
        public IReadOnlyCollection<Trade> Trades { get { return new ReadOnlyCollection<Trade>(trades.Select((k) => k.Value.OrderByDescending(v => v.Version).First()).ToList()); } }
        public IReadOnlyCollection<Counterparty> Counterparties { get { return new ReadOnlyCollection<Counterparty>(counterparties); } }
        public IReadOnlyCollection<Security> Securities { get { return new ReadOnlyCollection<Security>(securities); } }

        private BlotterContext()
        {
            //Produce Mock Data
            for(var i = 0; i < 100; i++)
            {
                securities.Add(Security.RandomSecurity());
            }

            for (var i = 0; i < 50; i++)
            {
                counterparties.Add(Counterparty.RandomCounterparty());
            }

            for (var i = 0; i < 100; i++)
            {
                Trade.RandomTrade(securities, counterparties).ForEach(AddUpdateTrade);
            }
        }

        public void Subscribe(TradeUpdateHandler handler)
        {
            TradeUpdateEvent += handler;
        }

        public void Unsubscribe(TradeUpdateHandler handler)
        {
            TradeUpdateEvent -= handler;
        }

        /// <summary>
        /// Add or Update a Trade. Ensure the Trade is based upon the correct previous version (it should supply the previous version, that it is updating)
        /// </summary>
        /// <param name="t">The new (version of the) trade</param>
        public void AddUpdateTrade(Trade t)
        {
            lock (trades)
            {
                //Add or Update?
                if (trades.ContainsKey(t.Id))
                {
                    var history = trades[t.Id];
                    int latestVersion = history.Select(h => h.Version).Max();

                    //Check for update collisions
                    if (t.Version != latestVersion)
                    {
                        throw new FaultException<AttemptingToUpdateOldVersionOfTrade>(
                            new AttemptingToUpdateOldVersionOfTrade(t, history.Where(h => h.Version == latestVersion).Single()));
                    }

                    t.Version = latestVersion + 1;
                    if (t.Version > 1 && t.State == State.New)
                    {
                        t.State = State.Amended;
                    }
                    history.Add(t);
                }
                else
                {
                    t.Version = 1;
                    trades.Add(t.Id, new List<Trade>() { t });
                }
            }

            TradeUpdateEvent?.Invoke(t);
        }

        public IReadOnlyCollection<Trade> GetHistoryForTrade(string tradeId)
        {
            if (trades.ContainsKey(tradeId))
            {
                return new ReadOnlyCollection<Trade>(trades[tradeId]);
            }

            throw new FaultException<TradeDoesNotExist>(new TradeDoesNotExist(tradeId));
        }
    }
}
