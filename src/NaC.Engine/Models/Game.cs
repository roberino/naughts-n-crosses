using System.Collections.Generic;
using System.Linq;

namespace NandC.Engine.Models
{
    public class Game
    {
        private readonly IList<Player> players;
        private readonly Board board;

        public Game(int numberOfPlayers = 2, int boardSize = 9) : this(Player.Create(numberOfPlayers), boardSize)
        {
        }

        public Game(IEnumerable<Player> players, int boardSize = 9)
        {
            board = new Board(9);
            this.players = players.ToList();

            foreach (var player in players) player.Join(this);
        }

        public static Game Create(params Player[] players)
        {
            return new Game(players);
        }

        public void Reset()
        {
            board.Reset();
        }

        public IEnumerable<Player> Players
        {
            get
            {
                return players;
            }
        }

        public Board Board
        {
            get
            {
                return board;
            }
        }
    }
}
