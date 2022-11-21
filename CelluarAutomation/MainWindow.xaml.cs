using System;
using System.Collections.Generic;
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
        private NeuralMap map;
        private bool isStarted;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGraph();
            isStarted = false;
        }

        /*private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NextLattice();
        }*/

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
                map.GetNextSteps(Int32.Parse(stepSize.Text));
                AddPointToGraph();
            }
        }

        /*public void NextLattice()
        {
            Btm.Source = BitmapToImageSource(bitmap.map);
            bitmap.Draw();
            bitmap.neuralmap.GetNextSteps(Int32.Parse(stepSize.Text));
        }*/



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
            graph.Series[0].Values.Add(new ObservablePoint(map.Time, map.CurrentLattice[0][0]));
        }

        private void InitializeNeuralBitmap()
        {
            map = new NeuralMap(Btm, Int32.Parse(sizeX.Text), Int32.Parse(SizeY.Text), double.Parse(Gn.Text),
                double.Parse(Cp.Text), double.Parse(defValue.Text), double.Parse(maxValue.Text), double.Parse(minValue.Text), true, true,
                Int32.Parse(stepSize.Text));
            map.BitMapLattice.Draw(map.CurrentLattice);
            /*bitmap = new SmartBitmap(this);
            bitmap.Draw();
            Btm.Source = BitmapToImageSource(bitmap.map);*/
        }

        public Image GetImageSource()
        {
            return Btm;
        }
    }
}
