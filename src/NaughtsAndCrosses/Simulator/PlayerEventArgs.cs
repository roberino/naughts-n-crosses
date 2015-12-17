using System;

namespace NaughtsAndCrosses.Simulator
{
    public class PlayerEventArgs<T> : EventArgs
    {
        public Player Player { get; set; }

        public T ContextData { get; set; }
    }
}
