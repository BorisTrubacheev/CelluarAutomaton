using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelluarAutomation
{
    static class Charts
    {
        private static List<Chart> chartsList = new List<Chart>();

        public static int ChartsCount => chartsList.Count();
        public static Chart Last => chartsList.Last();

        public static void AddChartToChartsList(Chart chart)
        {
            chartsList.Add(chart);
        }

        public static void AddPointsToCharts(int time, double[][] arr)
        {
            foreach (Chart chrt in chartsList)
            {
                ObservablePoint point = new ObservablePoint(time, arr[chrt.MonitoredPointX][chrt.MonitoredPointY]);
                chrt.GetChart.Series[0].Values.Add(point);
            }
        }

        public static void ClearChartsByRebootButton()
        {
            chartsList.Clear();
        }
    }
}
