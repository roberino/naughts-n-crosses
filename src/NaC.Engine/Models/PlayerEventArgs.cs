using System;

namespace NandC.Engine.Models
{
    public class PlayerEventArgs<T> : EventArgs
    {
        public Player Player { get; set; }

        public T ContextData { get; set; }
    }
}
