using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CelluarAutomation
{
    class NeuralMap
    {
        public SmartBitmap bitMapLattice;

        public int sizeX;
        public int sizeY;

        public double defval;
        public double Gn, Cp;
        public double MaxVal;
        public double MinVal;
        public bool initRandom;
        private double r; 

        public double[][] currentLattice;
        public double[][] lastLattice;

        public int time;
        public int stepSize;
        public bool ActivatePointsGraph;
        public Hashtable pastConfigs;

        Random rnd = new Random();
        public NeuralMap()
        {
            sizeX = 512;
            sizeY = 512;
            Gn = 500;
            Cp = 1;
            MaxVal = 1;
            MinVal = 0;
            defval = 0.1;
            initRandom = false;
            ActivatePointsGraph = true;
            r = 4 * Gn / 1000;


            currentLattice = new double[sizeX][];
            for (int i = 0; i < sizeX; i++)
                currentLattice[i] = new double[sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    currentLattice[i][j] = rnd.NextDouble();
            lastLattice = currentLattice;
            time = 0;
            if (ActivatePointsGraph)
            {
                pastConfigs = new Hashtable();
                pastConfigs[time] = currentLattice;
            }
        }



        public NeuralMap(Image img, int sizeX, int sizeY, double Gn, double Cp, double defaultvalue, double
        MaxVal, double MinVal, bool ActivateGraph, bool initRandom, int stepSize)
        {
            bitMapLattice = new SmartBitmap(img, sizeX, sizeY, MaxVal, MinVal, currentLattice);
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.Gn = Gn;
            this.Cp = Cp;
            defval = defaultvalue;
            this.MaxVal = MaxVal;
            this.MinVal = MinVal;
            this.stepSize = stepSize;
            ActivatePointsGraph = ActivateGraph;
            this.initRandom = initRandom;
            r = 4 * Gn / 1000;
            currentLattice = new double[this.sizeX][];
            for (int i = 0; i < this.sizeX; i++)
                currentLattice[i] = new double[this.sizeY];
            Random rand = new Random();
            for (int i = 0; i < this.sizeX; i++)
                for (int j = 0; j < this.sizeY; j++)
                    if (initRandom)
                        currentLattice[i][j] = Math.Round(rand.NextDouble(), 2);
                    else currentLattice[i][j] = defval;
            
            Copy(currentLattice, ref lastLattice);
            time = 0;
            if (ActivatePointsGraph)
            {
                pastConfigs = new Hashtable();
                pastConfigs[time] = currentLattice;
            }
        }

        public void GetNextSteps(int stepsCount)
        {
            for(int i = 0; i < stepsCount; i++)
            {
                NextStep();
                bitMapLattice.Draw(currentLattice);
            }
        }


        public void NextStep()
        {
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    double stepval = F(lastLattice[i][j]) + C(i, j, lastLattice[i][j]);
                    if (lastLattice[i][j] <= 0.5)
                    if (stepval > 1)
                        stepval = 1;
                    if (stepval < 0)
                        stepval = 0;
                    currentLattice[i][j] = stepval;
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

        public void SetValue(int x, int y, double v)
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
