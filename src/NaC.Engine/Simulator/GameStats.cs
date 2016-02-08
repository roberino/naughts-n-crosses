using NandC.Engine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NandC.Engine.Simulator
{
    public class GameStats
    {
        private readonly HashSet<GameState> gameData;
        private readonly Player player;

        int maxGameLength;

        public GameStats(Player player)
        {
            gameData = new HashSet<GameState>();

            this.player = player;
        }

        public bool IsOwner(Player player)
        {
            return this.player.Equals(player);
        }

        public void Save()
        {
            var persistance = new Persistence();

            persistance.Save(gameData, player.Id);
        }

        public void Load()
        {
            var persistance = new Persistence();

            var data = persistance.Load<GameState>(player.Id);

            foreach (var d in data)
            {
                if (!gameData.Contains(d)) gameData.Add(d);
            }
        }

        public void Register(Game game)
        {
            if (!game.Board.IsComplete)
            {
                throw new InvalidOperationException();
            }

            if (!game.Board.HasWinner)
            {
                return; // Ignore for now
            }

            var isWin = game.Board.Winner.Id == player.Id;

            Register(game.Board.CurrentState(player), isWin);
        }

        public void Register(byte[] state, bool isWin)
        {
            var s = new GameState()
            {
                State = state,
                IsWin = isWin
            };

            if (!gameData.Contains(s)) gameData.Add(s);

            if (state.Length > maxGameLength)
            {
                maxGameLength = state.Length;
            }
        }

        public IEnumerable<GameState> FindMatches(byte[] state)
        {
            var n = state[state.Length - 1];
            var sum = state.Select(x => (int)x).Sum();
            var s1 = new GameState() { State = state };

            return gameData.Where(k => k.IsLike(state));

            //return gameData.Where(k => 
            //    k.State.Length >= state.Length && 
            //    k.State[state.Length - 1] == n && 
            //    k.State.Take(state.Length).Select(x => (int)x).Sum() == sum &&
            //    k.Key.StartsWith(s1.Key));
        }

        public float? ScoreMove(byte[] state, bool wins = true)
        {
            var matches = FindMatches(state).ToList();
            var winning = matches.Where(m => m.IsWin == wins).ToList();
            var losing = matches.Where(m => m.IsWin != wins);
            var penalty = losing.Count() > winning.Count() ? 1 : 0;

            return !winning.Any() ? new float?() : winning.Min(w => w.State.Length) - state.Length + penalty;
        }

        [Serializable]
        public class GameState : IEquatable<GameState>
        {
            public byte[] State { get; set; }

            public bool IsWin { get; set; }

            public string Key
            {
                get
                {
                    return Aggr(State) + '=' + IsWin;
                }
            }

            public bool IsLike(byte[] state)
            {
                return State.Length >= state.Length &&
                    State[state.Length - 1] == state[state.Length - 1] &&
                    Aggr(State.Take(state.Length - 1).ToArray()) == Aggr(state.Take(state.Length - 1).ToArray());
            }

            public override string ToString()
            {
                return Key;
            }

            public override int GetHashCode()
            {
                return Key.GetHashCode();
            }

            private string Aggr(byte[] state)
            {
                return state == null ? string.Empty : state
                        .Aggregate(
                            new StringBuilder(),
                                (s, n) => s.Append('/').Append(n)).ToString();
            }

            public bool Equals(GameState other)
            {
                if (other == null) return false;

                if (ReferenceEquals(this, other)) return true;

                return string.Equals(Key, other.Key);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as GameState);
            }
        }
    }
}
