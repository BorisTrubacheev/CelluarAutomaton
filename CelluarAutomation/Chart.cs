using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CelluarAutomation
{
    class Chart
    {
        private CartesianChart chart;
        private readonly int monitoredPointX;
        private readonly int monitoredPointY;

        public CartesianChart GetChart => chart;
        public int MonitoredPointX => monitoredPointX;
        public int MonitoredPointY => monitoredPointY;

        public Chart(int coordX, int coordY)
        {
            monitoredPointX = coordX;
            monitoredPointY = coordY;
            InitializeChart();
        }

        private void InitializeChart()
        {
            chart = new CartesianChart();
            chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>{},
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 2,
                    StrokeThickness = 1,
                    Title = $"Value in ({MonitoredPointX}, {monitoredPointY}) "
                }
            };
            chart.LegendLocation = LegendLocation.Right;
            chart.MaxHeight = 200;
            chart.MinHeight = 100;
            chart.VerticalAlignment = System.Windows.VerticalAlignment.Top;
        }
    }
}
