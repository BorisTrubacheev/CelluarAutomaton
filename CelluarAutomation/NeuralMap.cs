using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelluarAutomation
{
    class NeuralMap
    {
        public int sizeX;
        public int sizeY;

        public double defval;
        public double Gn, Cp;
        public double MaxVal;
        public double MinVal;
        public bool initRandom;
        private double r; 

        public double[][] _CURconfig;
        public double[][] _LASTconfig;

        public int _CURtime;
        public int stepSize;
        public bool ActivatePointsGraph;
        public Hashtable PastConfigs;

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


            _CURconfig = new double[sizeX][];
            for (int i = 0; i < sizeX; i++)
                _CURconfig[i] = new double[sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    _CURconfig[i][j] = rnd.NextDouble();
            _LASTconfig = _CURconfig;
            _CURtime = 0;
            if (ActivatePointsGraph)
            {
                PastConfigs = new Hashtable();
                PastConfigs[_CURtime] = _CURconfig;
            }
        }



        public NeuralMap(int sizeX, int sizeY, double Gn, double Cp, double defaultvalue, double
        MaxVal, double MinVal, bool ActivateGraph, bool initRandom, int stepSize)
        {
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
            _CURconfig = new double[this.sizeX][];
            for (int i = 0; i < this.sizeX; i++)
                _CURconfig[i] = new double[this.sizeY];
            Random rand = new Random();
            for (int i = 0; i < this.sizeX; i++)
                for (int j = 0; j < this.sizeY; j++)
                    if (initRandom)
                        _CURconfig[i][j] = Math.Round(rand.NextDouble(), 2);
                    else _CURconfig[i][j] = defval;
            
            Copy(_CURconfig, ref _LASTconfig);
            _CURtime = 0;
            if (ActivatePointsGraph)
            {
                PastConfigs = new Hashtable();
                PastConfigs[_CURtime] = _CURconfig;
            }
        }

        public void GetNextSteps(int stepsCount)
        {
            for(int i = 0; i < stepsCount; i++)
            {
                NextStep();
            }
        }


        public void NextStep()
        {
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    double stepval = F(_LASTconfig[i][j]) + C(i, j, _LASTconfig[i][j]);
                    if (_LASTconfig[i][j] <= 0.5)
                    if (stepval > 1)
                        stepval = 1;
                    if (stepval < 0)
                        stepval = 0;
                    _CURconfig[i][j] = stepval;
                }
            _LASTconfig = _CURconfig;
            _CURtime++;
            if (ActivatePointsGraph)
            {
                double[][] src = new double[sizeX][];
                for (int i = 0; i < sizeX; i++)
                    src[i] = new double[sizeY];
                for (int i = 0; i < sizeX; i++)
                    for (int j = 0; j < sizeY; j++)
                        src[i][j] = _CURconfig[i][j];
                PastConfigs[_CURtime] = src;
            }
        }

        public void SetValue(int x, int y, double v)
        {
            _CURconfig[x][y] = v;
            _LASTconfig[x][y] = v;
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
            return _LASTconfig[x][y];
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
