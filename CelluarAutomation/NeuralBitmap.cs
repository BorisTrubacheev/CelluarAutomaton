using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelluarAutomation
{
    class NeuralBitmap
    {
        public Bitmap map;
        public NeuralMap neuralmap;
        public MainWindow window;

        public NeuralBitmap(MainWindow wind)
        {
            window = wind;
            neuralmap = new NeuralMap(Int32.Parse(window.sizeX.Text), Int32.Parse(window.SizeY.Text), double.Parse(window.Gn.Text),
                double.Parse(window.Cp.Text), double.Parse(window.defValue.Text), double.Parse(window.maxValue.Text), double.Parse(window.minValue.Text), true, true, 
                Int32.Parse(window.stepSize.Text));
            map = new Bitmap(neuralmap.sizeX, neuralmap.sizeY);
        }

        public NeuralBitmap(int sizeX, int sizeY, double Gn, double Cp, double defaultvalue, double MaxVal, double MinVal, bool ActivateGraph, bool initRandom, int stepSize)
        {
            neuralmap = new NeuralMap(sizeX, sizeY, Gn, Cp, defaultvalue, MaxVal, MinVal, ActivateGraph, initRandom, stepSize);

            map = new Bitmap(sizeX, sizeY);

        }

        public void Draw()
        {
            for (int i = 0; i < neuralmap.sizeX; i++)
                for (int j = 0; j < neuralmap.sizeY; j++)
                {
                    Color c = GetCellColor(neuralmap._CURconfig[i][j]);
                    map.SetPixel(i, j, c);
                }
        }

        private Color GetCellColor(double value)
        {
            double interval = (neuralmap.MaxVal - neuralmap.MinVal) / 256;
            double iterator = neuralmap.MinVal;
            if (value <= iterator)
            {
                return Color.Black;
            }
            int i = 0;
            while (iterator <= value)
            {
                i++;
                if (i > 255)
                {
                    return Color.White;
                }
                iterator += interval;
            }
            return Color.FromArgb(0, 0, i);
        }
    }
}
