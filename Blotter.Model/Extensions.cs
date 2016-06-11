using System;
using System.Collections.Generic;
using System.Linq;

namespace Blotter.Model
{
    public static class Extensions
    {
        private static Random gen = new Random();

        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }

        public static T PickRandom<T>(this T[] source)
        {
            return source[(gen).Next(source.Count())];
        }

        public static T PickRandom<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(gen.Next(v.Length));
        }

        public static DateTime RandomDay(DateTime min, DateTime max)
        {
            DateTime start = min;
            int range = (max - start).Days;
            return start.AddDays(gen.Next(range));
        }

        /// <summary>
        /// Generates a Check Digit for a unfinished ISIN
        /// 
        /// Modified from http://www.extensionmethod.net/csharp/string/isisin
        /// </summary>
        public static int CalculateCheckDigit(this string isin)
        {
            var digits = new int[22];
            int index = 0;
            for (int i = 0; i < 11; i++)
            {
                char c = isin[i];
                if (c >= '0' && c <= '9')
                {
                    digits[index++] = c - '0';
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    int n = c - 'A' + 10;
                    int tens = n / 10;
                    if (tens != 0)
                    {
                        digits[index++] = tens;
                    }
                    digits[index++] = n % 10;
                }
                else
                {
                    // Not a digit or upper-case letter.
                    throw new ArgumentOutOfRangeException("isin", "Got an invalid character '" + c + "' in the ISIN " + isin);
                }
            }
            int sum = 0;
            for (int i = 0; i < index; i++)
            {
                int digit = digits[index - 1 - i];
                if (i % 2 == 0)
                {
                    digit *= 2;
                }
                sum += digit / 10;
                sum += digit % 10;
            }

            return (sum % 10 == 0) ? 0 : ((sum / 10) + 1) * 10 - sum;
        }

    }
}
