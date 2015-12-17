using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NaughtsAndCrosses.Simulator
{
    public class GameStats
    {
        private readonly List<GameState> gameData;
        private readonly Player player;

        int maxGameLength;

        public GameStats(Player player)
        {
            gameData = new List<GameState>();

            this.player = player;
        }

        public void Save()
        {
            var persistance = new Persistence();

            persistance.Save(gameData, player.Id);
        }

        public void Load()
        {
            var persistance = new Persistence();

            var data = persistance.Load<List<GameState>>(player.Id);

            gameData.AddRange(data);
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

        internal void Register(byte[] state, bool isWin)
        {
            var s = new GameState()
            {
                State = state,
                IsWin = isWin
            };

            gameData.Add(s);

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

            return gameData.Where(k => 
                k.State.Length >= state.Length && 
                k.State[state.Length - 1] == n && 
                k.State.Take(state.Length).Select(x => (int)x).Sum() == sum &&
                k.Key.StartsWith(s1.Key));
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
        public class GameState
        {
            public byte[] State { get; set; }

            public bool IsWin { get; set; }

            public string Key
            {
                get
                {
                    return State
                        .Aggregate(
                            new StringBuilder(),
                                (s, n) => s.Append('/').Append(n)).ToString();
                }
            }

            public override string ToString()
            {
                return Key + '=' + IsWin;
            }
        }
    }
}
