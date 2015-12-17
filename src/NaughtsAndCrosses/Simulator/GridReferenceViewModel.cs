using System.ComponentModel;

namespace NaughtsAndCrosses.Simulator
{
    public class GridReferenceViewModel : NPC
    {
        private Player player;

        public GridReferenceViewModel(Point reference)
        {
            GridReference = reference;
        }

        public Point GridReference { get; private set; }

        public Player Player
        {
            get
            {
                return player;
            }
            set
            {
                player = value;
                NotifyPropertyChange("Player");
            }
        }
    }
}
