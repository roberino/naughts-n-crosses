using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NaughtsAndCrosses.Simulator
{
    public class Persistence
    {
        public void Save<T>(T data, int id = 0)
        {
            using (var fs = File.OpenWrite(FileName(id)))
            {
                var sz = new BinaryFormatter();

                sz.Serialize(fs, data);
            }
        }

        public T Load<T>(int id = 0)
        {
            using (var fs = File.OpenRead(FileName(id)))
            {
                var sz = new BinaryFormatter();

                return (T)sz.Deserialize(fs);
            }
        }

        private static string FileName(int id)
        {
            return "game" + id + ".dat";
        }
    }
}
