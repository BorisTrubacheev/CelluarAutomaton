using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using Brushes = System.Windows.Media.Brushes;

namespace CelluarAutomation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NeuralBitmap bitmap;
        private bool isStarted;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGraph();
            isStarted = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NextLattice();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (!isStarted)
            {
                InitializeNeuralBitmap();
                btn.Content = "Следующий шаг";
                isStarted = true;
            }
            else
            {
                NextLattice();
                AddPointToGraph();
            }
        }

        public void NextLattice()
        {
            Btm.Source = BitmapToImageSource(bitmap.map);
            bitmap.Draw();
            bitmap.neuralmap.NextStep();
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
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

        public void InitializeGraph()
        {
            graph.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>{},
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 1,
                    StrokeThickness = 1,
                    Title = "Значение в точке"
                }
            };
        }

        public void AddPointToGraph()
        {
            graph.Series[0].Values.Add(new ObservablePoint(bitmap.neuralmap._CURtime, bitmap.neuralmap._CURconfig[0][0]));
        }

        public void InitializeNeuralBitmap()
        {
            bitmap = new NeuralBitmap(this);
            bitmap.Draw();
            Btm.Source = BitmapToImageSource(bitmap.map);
        }
    }
}
