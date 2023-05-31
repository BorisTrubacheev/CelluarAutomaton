using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace CelluarAutomation
{
    class Lattice
    {
        #region fields
        private SmartBitmap bitMapLattice;
        private int stepsCount;

        private LatticeHashTable previousLattices;

        private int sizeX;
        private int sizeY;

        private double defval;
        private double r, Cp;

        private double[][] currentLattice;
        private double[][] lastLattice;

        private int time;
        private int firstCollisionTime;

        private bool haveCollision;
        private int periodSize;

        private Random rnd = new Random();
        #endregion

        #region properties
        public SmartBitmap BitMapLattice => bitMapLattice;
        public double[][] CurrentLattice => currentLattice;
        public int Time => time;
        #endregion

        #region constructor
        public Lattice(Image img, int sizeX, int sizeY, double r, double Cp, double defaultvalue, double
        MaxVal, double MinVal, bool initRandom, int stepSize)
        {
            bitMapLattice = new SmartBitmap(img, sizeX, sizeY, MaxVal, MinVal, CurrentLattice);

            previousLattices = new LatticeHashTable();

            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.r = r;
            this.Cp = Cp;
            defval = defaultvalue;
            time = 0;
            firstCollisionTime = 0;
            haveCollision = false;
            periodSize = 0;

            currentLattice = new double[this.sizeX][];
            for (int i = 0; i < this.sizeX; i++)
                CurrentLattice[i] = new double[this.sizeY];

            if (initRandom)
                FillLatticeWithRandomValues();
            else
                FillLatticeWithDefValues();

            Copy(currentLattice, ref lastLattice);
        }

        void FillLatticeWithRandomValues()
        {
            for (int i = 0; i < this.sizeX; i++)
                for (int j = 0; j < this.sizeY; j++)
                    currentLattice[i][j] = Math.Round(rnd.NextDouble(), 2);
        }

        void FillLatticeWithDefValues()
        {
            for (int i = 0; i < this.sizeX; i++)
                for (int j = 0; j < this.sizeY; j++)
                    currentLattice[i][j] = defval;
        }
        #endregion

        #region methods

        public (bool, int, int) GetNextSteps(int count)
        {
            stepsCount = count;
            /*NextStep();
            bitMapLattice.Draw(CurrentLattice);
            Charts.AddPointsToCharts(time, currentLattice);
            int period = previousLattices.TryAddValueToTable(bitMapLattice.LatticeBitmap, currentLattice, time);
            if(period != 0)
            {
                firstCollisionTime = haveCollision ? firstCollisionTime : time;
                periodSize = haveCollision ? periodSize : period;
                haveCollision = true;
            }*/
            int period = 0;
            for(int i = 0; i < stepsCount; i++)
            {
                NextStep();
                bitMapLattice.Draw(CurrentLattice);
                Charts.AddPointsToCharts(time, currentLattice);
                period = previousLattices.TryAddValueToTable(bitMapLattice.LatticeBitmap, currentLattice, time);
                if (period != 0)
                {
                    firstCollisionTime = haveCollision ? firstCollisionTime : time;
                    periodSize = haveCollision ? periodSize : period;
                    haveCollision = true;
                }
            }
            return (haveCollision, firstCollisionTime, periodSize);
        }


        private void NextStep()
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    double value = F(lastLattice[i][j]) + C(i, j, lastLattice[i][j]);
                    if (value > 1) value = 1;
                    if (value < 0) value = 0;

                    CurrentLattice[i][j] = value;
                }
            }
            Copy(currentLattice, ref lastLattice);
            time++;
        }

        private double C(int x, int y, double point)
        {
            return Cp * (DelthaX(x, y) - point);
        }

        private double F(double x)
        {
            if (x <= 0.5)
                return 2 * r * x;

            return 2 * r * (1 - x);
        }

        private double DelthaX(int x, int y)
        {
            return (GetPoint(x, y - 1) + GetPoint(x, y + 1) + GetPoint(x - 1, y) + GetPoint(x + 1, y)) / 4;
        }

        private double GetPoint(int x, int y)
        {
            if ((x < 0) || (x >= sizeX) || (y < 0) || (y >= sizeY))
                return 0;
            return lastLattice[x][y];
        }

        private void Copy(double[][] src, ref double[][] res)
        {
            if (src == null)
            {
                res = null;
                return;
            }
            res = new double[src.Length][];
            for (int i = 0; i < src.Length; i++)
                res[i] = new double[src[i].Length];
            for (int i = 0; i < src.Length; i++)
                for (int j = 0; j < src[i].Length; j++)
                    res[i][j] = src[i][j];
        }

        public string GetLatticeInfo()
        {
            int maxValuePointsCount = GetMaxValuePointsCount();
            int minValuePointsCount = GetMinValuePointsCount();
            double summ = Math.Round(GetLatticeSumm(), 3);
            double average = Math.Round(summ / (sizeX * sizeY), 3);

            string info = $"Count of max values in lattice: {maxValuePointsCount}\n" +
                $"Count of min values in lattice: {minValuePointsCount}\n" +
                $"Lattice amount: {summ}\n" +
                $"Average value in cells: {average}\n" +
                $"Collisions count: {previousLattices.CollisionCount}";

            return info;
        }

        private int GetMaxValuePointsCount()
        {
            int result = 0;
            foreach (double[] str in currentLattice)
            {
                result += str.Count((x) => x == 1);
            }
            return result;
        }

        private int GetMinValuePointsCount()
        {
            int result = 0;
            foreach (double[] str in currentLattice)
            {
                result += str.Count((x) => x == 0);
            }
            return result;
        }

        private double GetLatticeSumm()
        {
            double result = 0;
            foreach (double[] str in currentLattice)
            {
                result += str.Sum();
            }
            return result;
        }
        #endregion
    }
}
