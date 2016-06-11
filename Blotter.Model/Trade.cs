using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blotter.Model
{
    public enum BuySell
    {
        Buy,
        Sell
    }

    public enum State
    {
        New,
        Matched,
        Error,
        Cancelled,
        Amended
    }

    [DataContract]
    public class Trade
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public int Version { get; set; }

        //Actual Trade Data
        [DataMember]
        public Counterparty Counterparty { get; set; }

        [DataMember]
        public Security Security { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public int Units { get; set; }

        [DataMember]
        public BuySell BuySell { get; set; }

        [DataMember]
        public State State { get; set; }

        //Audit Data
        [DataMember]
        public DateTime Created { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime LastUpdated { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }

        #region Random Data Generation

        public Trade Copy()
        {
            return new Trade()
            {
                Id = Id,
                Version = Version,
                Counterparty = Counterparty,
                Security = Security,
                BuySell = BuySell,
                State = State,
                Price = Price,
                Units = Units,
                Created = Created,
                CreatedBy = CreatedBy,
                LastUpdated = LastUpdated,
                LastUpdatedBy = LastUpdatedBy
            };
        }

        public static string GenerateTradeId()
        {
            return "BLM" + Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper();
        }

        private static DateTime DateTimeHelper = DateTime.Now.AddHours(-2);
        private static Random r = new Random();
        public static List<Trade> RandomTrade(List<Security> securities, List<Counterparty> counterparties)
        {
            var tradesCreated = new List<Trade>();

            DateTimeHelper = DateTimeHelper.AddSeconds(r.Next(30));

            Trade t = new Trade()
            {
                Id = GenerateTradeId(),
                Version = 1,
                Counterparty = counterparties.PickRandom(),
                Security = securities.PickRandom(),
                BuySell = Extensions.PickRandom<BuySell>(),
                State = State.New,
                Price = r.Next(1, 1000) / r.Next(1, 15),
                Units = r.Next(1000000),
                CreatedBy = "sparkl",
                Created = DateTimeHelper,
                LastUpdated = DateTimeHelper,
                LastUpdatedBy = "sparkl"
            };

            tradesCreated.Add(t);
       
            //Randomly add Extra Rows
            int v = r.Next(0, 10);
            if (v > 5 && v < 8)
            {
                Trade c = t.Copy();
                c.State = State.Amended;
                c.Price += r.Next(5);
                tradesCreated.Add(c);
            }
            else if (v < 2)
            {
                Trade c = t.Copy();
                c.State = State.Cancelled;
                tradesCreated.Add(c);
            }
            else if( v >= 2 && v <= 5)
            {
                Trade c = t.Copy();
                c.State = State.Matched;
                tradesCreated.Add(c);
            }

            return tradesCreated;
        }

        #endregion
    }
}
