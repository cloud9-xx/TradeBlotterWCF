using System;
using System.Runtime.Serialization;
using System.Text;

namespace Blotter.Model
{
    public enum Currency
    {
        GBP,
        USD,
        EUR,
        JPY,
        SEK,
        NOK,
        GEL
    }

    [DataContract]
    public class Security
    {
        private const string SHORT_DATE_FORMAT = "yyyy-MM-dd";

        [DataMember]
        public string Isin { get; private set; }

        [DataMember]
        public DateTime IssueDate { get; private set; }

        [DataMember]
        public DateTime MaturityDate { get; private set; }

        [DataMember]
        public decimal Coupon { get; private set; }

        [DataMember]
        public int UnitSize { get; private set; }

        [DataMember]
        public Currency Currency { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as Security;
            if (other == null) return false;

            return Isin == other.Isin;
        }

        public override int GetHashCode()
        {
            return Isin.GetHashCode();
        }

        public override string ToString()
        {
            return Isin + " (" + Currency + "|" + IssueDate.ToString(SHORT_DATE_FORMAT) + "|" + MaturityDate.ToString(SHORT_DATE_FORMAT) + "|" + Coupon + "|" + UnitSize + ")";
        }

        #region Random Data Generation

        public static Security RandomSecurity()
        {
            Random r = new Random();

            return new Security
            {
                Isin = GetRandomIsin(),
                Currency = Extensions.PickRandom<Currency>(),
                IssueDate = Extensions.RandomDay(new DateTime(2010, 01, 01), DateTime.Today),
                MaturityDate = Extensions.RandomDay(DateTime.Today, new DateTime(2030, 01, 01)),
                Coupon = r.Next() / r.Next(1, 50),
                UnitSize = r.Next(1000) * 100
            };
        }

        private static readonly string[] ISO_CODES = new string[] { "GB", "US", "JP", "XS" };
        private static readonly char[] CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        private static string GetRandomIsin()
        {
            StringBuilder isin = new StringBuilder(ISO_CODES.PickRandom());
            for (int x = 0; x < 9; x++)
            {
                isin.Append(CHARS.PickRandom());
            }
            isin.Append(isin.ToString().CalculateCheckDigit());
            return isin.ToString();
        }

        #endregion
    }
}
