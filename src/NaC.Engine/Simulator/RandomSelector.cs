using System;
using System.Collections.Generic;

namespace NandC.Engine.Simulator
{
    public class RandomSelector
    {
        private readonly Random rnd;

        public RandomSelector()
        {
            rnd = new Random(DateTime.Now.Millisecond);
        }

        public T Select<T>(IList<T> items, T defaultValue = default(T))
        {
            return items.Count > 0 ? items[rnd.Next(items.Count)] : defaultValue;
        }
    }
}
