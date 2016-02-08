using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NandC.Engine.Models
{
    public class Player : NPC, IEquatable<Player>
    {
        private static int counter;
        private readonly char[] symbols = new char[] { 'X', 'D', 'O', 'B' };

        private int wins;

        public Player(int id = 0, string name = null)
        {
            Contract.Assert(id > counter);

            Id = id > 0 ? id : ++counter;
            Name = name ?? "Player " + Id;
            Symbol = symbols[(Id - 1) % symbols.Length].ToString();
            Colour = GetColour(Id);
        }

        public static IEnumerable<Player> Create(int numberOfPlayers)
        {
            return Enumerable.Range(1, numberOfPlayers).Select(n => new Player(n)).ToList();
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public virtual string Symbol { get; private set; }

        public virtual bool Active { get; set; }

        public int Wins
        {
            get
            {
                return wins;
            }
            set
            {
                wins = value;
                NotifyPropertyChange("Wins");
            }
        }

        public string Colour { get; private set; }

        public virtual void Join(Game game) { }

        public virtual void Play()
        {
            if (!Active)
            {
                Active = true;
                NotifyPropertyChange("Active");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, Wins);
        }

        private string GetColour(int n)
        {
            switch (n)
            {
                case 1:
                    return "Red";
                case 2:
                    return "Black";
            }

            return "Green";
        }

        public bool Equals(Player other)
        {
            if (other == null) return false;

            if (ReferenceEquals(this, other)) return true;

            return (Id == other.Id && GetType() == other.GetType());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Player);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
