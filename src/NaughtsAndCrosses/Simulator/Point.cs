using System;

namespace NaughtsAndCrosses.Simulator
{
    public struct Point : IEquatable<Point>
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public bool Equals(Point other)
        {
            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Point ? Equals((Point)obj) : false;
        }

        public override int GetHashCode()
        {
            return Primes.GetPrime(X) * Primes.GetPrime(Y);
        }
    }
}
