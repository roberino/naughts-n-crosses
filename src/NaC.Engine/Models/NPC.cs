using System.ComponentModel;

namespace NandC.Engine.Models
{
    public abstract class NPC : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChange(string name)
        {
            var ev = PropertyChanged;

            if (ev != null)
            {
                ev.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
