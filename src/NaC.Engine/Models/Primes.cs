using System.Collections.Generic;
using System.Linq;

namespace NandC.Engine.Models
{
    public static class Primes
    {
        private static IList<int> primes;

        static Primes()
        {
            primes = Enumerable.Range(3, 100).Where(IsPrime).ToList();
        }

        public static int GetPrime(int index)
        {
            return primes[index];
        }

        public static bool IsPrime(int n)
        {
            return !Factors(n).Any();
        }

        public static IEnumerable<int> Factors(int n)
        {
            return (n == 2 || n == 1) ? Enumerable.Empty<int>() : Enumerable.Range(2, n - 2).Where(d => n % d == 0);
        }
    }
}
