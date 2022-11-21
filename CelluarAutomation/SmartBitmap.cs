using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WindowImage = System.Windows.Controls.Image;

namespace CelluarAutomation
{
    class SmartBitmap
    {
        private Bitmap lattice;
        private WindowImage image;
        private double maxValInCell;
        private double minValInCell;

        public SmartBitmap(WindowImage img, int sizeX, int sizeY, double max, double min, double[][] latticeArray)
        {
            lattice = new Bitmap(sizeX, sizeY);
            image = img;
            image.Source = BitmapToImageSource(lattice);
            maxValInCell = max;
            minValInCell = min;
        }

        /*public SmartBitmap(int sizeX, int sizeY, double Gn, double Cp, double defaultvalue, double MaxVal, double MinVal, bool ActivateGraph, bool initRandom, int stepSize)
        {
            neuralmap = new NeuralMap(sizeX, sizeY, Gn, Cp, defaultvalue, MaxVal, MinVal, ActivateGraph, initRandom, stepSize);

            map = new Bitmap(sizeX, sizeY);

        }*/

        public void Draw(double[][] lattticeArray)
        {
            for (int i = 0; i < lattice.Width; i++)
                for (int j = 0; j < lattice.Height; j++)
                {
                    Color c = GetCellColor(lattticeArray[i][j]);
                    lattice.SetPixel(i, j, c);
                }
            image.Source = BitmapToImageSource(lattice);
        }

        private Color GetCellColor(double value)
        {
            double interval = (maxValInCell - minValInCell) / 256;
            double iterator = minValInCell;
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

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
