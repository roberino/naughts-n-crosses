using System;
using System.Linq;

namespace NaughtsAndCrosses.Simulator
{
    public class RandomRobot : Robot
    {
        public RandomRobot(int id) : base(id, "Not so smart robot")
        {
        }

        public override void Play()
        {
            base.Play();
        }

        public override void SubmitGo()
        {
            if (game == null) throw new InvalidOperationException();

            var refs = game.Board.AvailableReferences.ToList();
            game.Board.SubmitGo(this, rand.Select(refs));
        }
    }
}