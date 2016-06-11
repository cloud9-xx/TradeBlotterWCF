using System;
using System.Runtime.Serialization;
using System.Text;

namespace Blotter.Model
{
    [DataContract]
    public class Counterparty
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Counterparty;
            if (other == null) return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name + " (" + Id.ToString().Substring(0, 7).ToUpper() + ")";
        }

        #region Random Data Generation

        private static readonly string[] NAMES_1 = new string[] { "Neowin", "Cloud", "Barclays", "Morgan", "Stanley"  };
        private static readonly string[] NAMES_2 = new string[] { "Credit", "Retail", "Investment", "Partners" };
        private static readonly string[] ORG_TYPE = new string[] { "Bank", "Fund", "Insurance" };
        private static readonly string[] LEGAL_TYPE = new string[] { "Llp", "Ltd.", "Llc", "Plc", "Ips", "Rc" };

        public static Counterparty RandomCounterparty()
        {
            StringBuilder builder = new StringBuilder()
                .Append(NAMES_1.PickRandom()).Append(" ")
                .Append(NAMES_2.PickRandom()).Append(" ")
                .Append(ORG_TYPE.PickRandom()).Append(" ")
                .Append(LEGAL_TYPE.PickRandom());
            return new Counterparty() { Id = Guid.NewGuid(), Name = builder.ToString() };
        }

        #endregion
    }
}
