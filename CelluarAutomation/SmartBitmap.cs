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
        private Bitmap latticeBitmap;
        private WindowImage image;
        private double maxValInCell;
        private double minValInCell;

        public Bitmap LatticeBitmap => latticeBitmap;

        public SmartBitmap(WindowImage img, int sizeX, int sizeY, double max, double min, double[][] latticeArray)
        {
            latticeBitmap = new Bitmap(sizeX, sizeY);
            image = img;
            image.Source = BitmapToImageSource(latticeBitmap);
            maxValInCell = max;
            minValInCell = min;
        }

        public void Draw(double[][] lattticeArray)
        {
            for (int i = 0; i < latticeBitmap.Width; i++)
                for (int j = 0; j < latticeBitmap.Height; j++)
                {
                    Color c = GetCellColor(lattticeArray[i][j]);
                    latticeBitmap.SetPixel(i, j, c);
                }
            image.Source = BitmapToImageSource(latticeBitmap);
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
