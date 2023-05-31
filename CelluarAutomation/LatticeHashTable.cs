using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelluarAutomation
{
    class LatticeHashTable
    {
        private Dictionary<Int64, (int, double[][])> table;
        private int collisionCount;

        public LatticeHashTable()
        {
            table = new Dictionary<Int64, (int, double[][])>();
            collisionCount = 0;
        }

        public int CollisionCount => collisionCount;

        public int TryAddValueToTable(Bitmap btm, double[][] arr, int step)
        {
            Int64 hash = GetHash(btm);
            if (table.ContainsKey(hash) && Enumerable.SequenceEqual(table[hash].Item2, arr))
            {
                return step - table[hash].Item1;
            }
            else if(table.ContainsKey(hash) && !Enumerable.SequenceEqual(table[hash].Item2, arr))
            {
                collisionCount++;
            }
            AddValue(hash, arr, step);
            return 0;
        }

        private void AddValue(Int64 hash, double[][] arr, int step)
        {
            table.Add(hash, (step, arr));
        }

        private Int64 GetHash(Bitmap btm)
        {
            Int64 hash = Int64.MinValue;
            for(int x = 1; x < btm.Size.Width; x++)
            {
                for (int y = 1; y < btm.Size.Height; y++)
                {
                    try
                    {
                        hash += x * y * btm.GetPixel(x, y).ToArgb();
                    }
                    catch
                    {
                        hash = 0;
                        hash += x * y * btm.GetPixel(x, y).ToArgb();
                    }
                }
            }
            return hash;
        }
    }
}
