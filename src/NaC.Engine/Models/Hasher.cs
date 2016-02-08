using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NandC.Engine.Models
{
    public static class Hasher
    {
        public static int UnorderedHash<T>(IEnumerable<T> items, Func<T, string> selector)
        {
            return items
                .Select(selector)
                .OrderBy(s => s)
                .Aggregate(new StringBuilder(), (s, b) => s.Append(b).Append(';'))
                .GetHashCode();
        }

        public static byte Hash(int x0, bool x1)
        {
            return (byte)Primes.GetPrime((x0 + (x1 ? 1 : 10)));
        }
    }
}
