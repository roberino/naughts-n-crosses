using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace NandC.Engine.Simulator
{
    public class Persistence
    {
        public void Save<T>(IEnumerable<T> data, int id = 0)
        {
            using (var fs = File.OpenWrite(FileName(id)))
            {
                var sz = new BinaryFormatter();

                sz.Serialize(fs, data.ToList());
            }
        }

        public IEnumerable<T> Load<T>(int id = 0)
        {
            try
            {
                using (var fs = File.OpenRead(FileName(id)))
                {
                    var sz = new BinaryFormatter();

                    return (List<T>)sz.Deserialize(fs);
                }
            }
            catch
            {
                return Activator.CreateInstance<List<T>>();
            }
        }

        private static string FileName(int id)
        {
            return "game" + id + ".dat";
        }
    }
}
