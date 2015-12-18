using System;
using System.Linq;

namespace NaughtsAndCrosses.Simulator
{
    public class SmartRobot : Robot
    {
        private readonly GameStats brain;
        private Trainer trainer;

        public SmartRobot(int id, GameStats brain = null) : base(id, "Adaptive robot")
        {
            this.brain = brain ?? new GameStats(this);

            if (!this.brain.IsOwner(this)) throw new InvalidOperationException();
        }

        public bool AutoSave { get; set; }

        public override void Join(Game game)
        {
            base.Join(game);

            game.Board.Completed += BoardCompleted;

            trainer = new Trainer(game);
        }

        public override void Play()
        {
            base.Play();
        }

        public void Save()
        {
            brain.Save();
        }

        public void Load()
        {
            brain.Load();
        }

        public void Learn(int iterations = 100)
        {
            if (game == null) throw new InvalidOperationException();

            trainer.Train(brain, iterations);
        }

        public override void SubmitGo()
        {
            if (game == null) throw new InvalidOperationException();

            var availableRefs = game.Board.AvailableReferences.ToList();

            var scoreAggr = new Score();

            if (!game.Board.IsFirstGo)
            {
                foreach (var r in availableRefs)
                {
                    var hypo1 = game.Board.PreviewState(r, this, false);
                    var score = brain.ScoreMove(hypo1);

                    scoreAggr.Update(score, r);

                    foreach (var r2 in availableRefs.Where(x => x != r))
                    {
                        var scoreOp = brain.ScoreMove(game.Board.PreviewState(hypo1, r2, true), false);

                        //if (scoreOp.HasValue)
                        //{
                        //    Console.WriteLine("ya");
                        //}

                        scoreAggr.Update(scoreOp, r2, 0.5f);
                    }
                }
            }

            if (!scoreAggr.ScoreValue.HasValue)
            {
                var refs = game.Board.AvailableReferences.ToList();
                scoreAggr.Update(0, rand.Select(refs));
            }

            game.Board.SubmitGo(this, scoreAggr.Reference);
        }

        private void BoardCompleted(object sender, EventArgs e)
        {
            brain.Register(game);

            if (AutoSave)
            {
                var sv = new Action(Save);
                sv.BeginInvoke(a => sv.EndInvoke(a), null);
            }
        }

        private class Score
        {
            public float? ScoreValue { get; private set; }

            public int Reference { get; set; }

            public void Update(float? newScore, int reference, float offset = 0)
            {
                if (newScore.HasValue)
                {
                    newScore += offset;

                    if(!ScoreValue.HasValue || newScore < ScoreValue)
                    {
                        ScoreValue = newScore;
                        Reference = reference;
                    }
                }
            }
        }
    }
}
