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
        private Dictionary<Int64, double[][]> table;

        public LatticeHashTable()
        {
            table = new Dictionary<Int64, double[][]>();
        }

        public bool TryAddValueToTable(Bitmap btm, double[][] arr)
        {
            Int64 hash = GetHash(btm);
            if (table.ContainsKey(hash) && table.ContainsValue(arr))
            {
                return false;
            }
            AddValue(hash, arr);
            return true;
        }

        private void AddValue(Int64 hash, double[][] arr)
        {
            table.Add(hash, arr);
        }

        private Int64 GetHash(Bitmap btm)
        {
            Int64 hash = Int64.MinValue;
            for(int x = 1; x < btm.Size.Width; x++)
            {
                for (int y = 1; y < btm.Size.Width; y++)
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
