using System;
using System.Collections.Generic;
using System.Linq;

namespace NaughtsAndCrosses.Simulator
{
    public class Board
    {
        private readonly int boardSize;
        private IDictionary<int, int> boardState;
        private IList<Tuple<int, Player>> boardSequence;
        
        public Board(int boardSize = 9)
        {
            this.boardSize = boardSize;
            Reset();
        }

        public void Reset()
        {
            boardState = Enumerable.Range(1, 9).ToDictionary(n => n, p => 0);
            boardSequence = new List<Tuple<int, Player>>(boardSize);

            var ev = BoardReset;

            if (ev != null)
            {
                ev.Invoke(this, EventArgs.Empty);
            }
        }

        public Point GridReference(int reference)
        {
            var r = reference - 1;
            var w = (int)Math.Sqrt(boardSize);
            return new Point(r / w + 1, r % w + 1);
        }

        public event EventHandler<PlayerEventArgs<Point>> Updated;
        public event EventHandler BoardReset;
        public event EventHandler Completed;

        public void SubmitGo(Player p, Point gridRef)
        {
            var r = gridRef.Y + (gridRef.X - 1) * (int)Math.Sqrt(boardSize);

            SubmitGo(p, r);
        }
        public void SubmitGo(Player p, int reference)
        {
            if (!p.Active)
            {
                throw new InvalidOperationException("Inactive player");
            }

            if (IsComplete)
            {
                throw new InvalidOperationException("Game finished");
            }

            if (boardSequence.Any() && boardSequence.Last().Item2.Id == p.Id)
            {
                throw new InvalidOperationException("Not your turn mofo");
            }

            if(boardState[reference] > 0)
            {
                throw new ArgumentException("Position taken " + reference);
            }

            boardState[reference] = p.Id;
            boardSequence.Add(new Tuple<int, Player>(reference, p));

            var ev = Updated;

            if (ev != null)
            {
                ev.Invoke(this, new PlayerEventArgs<Point>()
                {
                    Player = p,
                    ContextData = GridReference(reference)
                });
            }

            if (IsComplete)
            {
                if (HasWinner)
                {
                    Winner.Wins++;
                }

                var evw = Completed;

                if (evw != null) evw.Invoke(this, EventArgs.Empty);
            }
        }

        public IEnumerable<Point> GridReferences
        {
            get
            {
                return Enumerable.Range(1, boardSize).Select(r => GridReference(r));
            }
        }

        public IEnumerable<int> AvailableReferences
        {
            get
            {
                return Enumerable.Range(1, boardSize).Except(boardSequence.Select(s => s.Item1));
            }
        }

        public bool IsFirstGo
        {
            get
            {
                return !boardSequence.Any();
            }
        }

        public byte[] CurrentState(Player attacker)
        {
            return boardSequence
                                .Select(s => Hasher.Hash(s.Item1, s.Item2.Id == attacker.Id))
                                .ToArray();
        }

        public byte[] PreviewState(int reference, Player attacker, bool forOpponent)
        {
            return PreviewState(CurrentState(attacker), reference, forOpponent);
        }

        public byte[] PreviewState(byte[] currentState, int reference, bool forOpponent)
        {
            var newState = new byte[currentState.Length + 1];
            var newPoint = Hasher.Hash(reference, !forOpponent);
            Array.Copy(currentState, newState, currentState.Length);
            newState[newState.Length - 1] = newPoint;
            return newState;
        }

        public bool HasWinner
        {
            get
            {
                if (IsPlayerSequence(new[] { 1, 2, 3 }) ||
                    IsPlayerSequence(new[] { 1, 4, 7 }) ||
                    IsPlayerSequence(new[] { 4, 5, 6 }) ||
                    IsPlayerSequence(new[] { 7, 8, 9 }) ||
                    IsPlayerSequence(new[] { 2, 5, 8 }) ||
                    IsPlayerSequence(new[] { 3, 6, 9 }) ||
                    IsPlayerSequence(new[] { 1, 5, 9 }) ||
                    IsPlayerSequence(new[] { 7, 5, 3 }))
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsComplete
        {
            get
            {
                return HasWinner || !AvailableReferences.Any();
            }
        }

        public int TurnNumber
        {
            get
            {
                return boardSequence.Count;
            }
        }

        private bool IsPlayerSequence(int[] refs)
        {
            var m = boardSequence.Where(x => refs.Contains(x.Item1)).ToList();

            if (m.Count == refs.Length && m.Select(x => x.Item2.Id).Distinct().Count() == 1)
            {
                return true;
            }

            return false;
        }

        public Player Winner
        {
            get
            {
                if(HasWinner)
                    return boardSequence.Last().Item2;
                return null;
            }
        }

        public Player LastGo()
        {
            var p = boardSequence.LastOrDefault();

            if (p != null) return p.Item2;

            return null;
        }
    }
}
