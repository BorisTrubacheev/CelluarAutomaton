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
        private Timer drawTimer;
        private int stepsCount;

        private int sizeX;
        private int sizeY;

        private double defval;
        private double r, Cp;
        private double MaxVal;
        private double MinVal;
        private bool initRandom;

        private double[][] currentLattice;
        private double[][] lastLattice;

        private int time;
        private int stepSize;
        private bool ActivatePointsGraph;
        private Hashtable pastConfigs;

        private Random rnd = new Random();
        #endregion

        #region properties
        public SmartBitmap BitMapLattice => bitMapLattice;
        public double[][] CurrentLattice => currentLattice;
        public int Time => time;
        #endregion

        public Lattice(Image img, int sizeX, int sizeY, double r, double Cp, double defaultvalue, double
        MaxVal, double MinVal, bool ActivateGraph, bool initRandom, int stepSize)
        {
            bitMapLattice = new SmartBitmap(img, sizeX, sizeY, MaxVal, MinVal, CurrentLattice);

            drawTimer = new Timer(1000);
            drawTimer.Elapsed += DrawTimerElapsed;

            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.r = r;
            this.Cp = Cp;
            defval = defaultvalue;
            this.MaxVal = MaxVal;
            this.MinVal = MinVal;
            this.stepSize = stepSize;
            ActivatePointsGraph = ActivateGraph;
            this.initRandom = initRandom;
            currentLattice = new double[this.sizeX][];
            for (int i = 0; i < this.sizeX; i++)
                CurrentLattice[i] = new double[this.sizeY];
            Random rand = new Random();
            for (int i = 0; i < this.sizeX; i++)
                for (int j = 0; j < this.sizeY; j++)
                    if (initRandom)
                        currentLattice[i][j] = Math.Round(rand.NextDouble(), 2);
                    else 
                        currentLattice[i][j] = defval;
            
            Copy(currentLattice, ref lastLattice);
            time = 0;
            if (ActivatePointsGraph)
            {
                pastConfigs = new Hashtable();
                pastConfigs[time] = CurrentLattice;
            }
        }

        private void DrawTimerElapsed(object sender, ElapsedEventArgs e)
        {
            GetNextSteps(--stepsCount);
        }

        public void GetNextSteps(int count)
        {
            stepsCount = count;
            NextStep();
            bitMapLattice.Draw(CurrentLattice);
            Charts.AddPointsToCharts(time, currentLattice);
            /*drawTimer.Start();*/
            for(int i = 0; i < stepsCount - 1; i++)
            {
                NextStep();
                bitMapLattice.Draw(CurrentLattice);
                Charts.AddPointsToCharts(time, currentLattice);
            }
        }


        private void NextStep()
        {
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    double stepval = F(lastLattice[i][j]) + C(i, j, lastLattice[i][j]);
                    if (lastLattice[i][j] <= 0.5) //????
                    if (stepval > 1)
                        stepval = 1;
                    if (stepval < 0)
                        stepval = 0;
                    CurrentLattice[i][j] = stepval;
                }
            lastLattice = currentLattice;
            time++;
            if (ActivatePointsGraph)
            {
                double[][] src = new double[sizeX][];
                for (int i = 0; i < sizeX; i++)
                    src[i] = new double[sizeY];
                for (int i = 0; i < sizeX; i++)
                    for (int j = 0; j < sizeY; j++)
                        src[i][j] = currentLattice[i][j];
                pastConfigs[time] = src;
            }
        }

        private void SetValue(int x, int y, double v)
        {
            currentLattice[x][y] = v;
            lastLattice[x][y] = v;
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
    }
}
