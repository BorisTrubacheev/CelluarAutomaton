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
using System.Windows.Controls.Primitives;

namespace CelluarAutomation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Lattice map;
        private bool isStarted;
        private bool isCoponentsInitiazed;

        public MainWindow()
        {
            isCoponentsInitiazed = false;
            InitializeComponent();
            InitializeTextBoxes();
            isCoponentsInitiazed = true;
            isStarted = false;
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (!isStarted)
            {
                InitializeLattice();
                btn.Content = "Следующий шаг";
                isStarted = true;
            }
            else
            {
                map.GetNextSteps(Int32.Parse(stepSize.Text));
            }
        }

        private void InitializeLattice()
        {
            map = new Lattice(Btm, Int32.Parse(sizeX.Text), Int32.Parse(sizeY.Text), double.Parse(Gn.Text),
                double.Parse(Cp.Text), double.Parse(defValue.Text), double.Parse(maxValue.Text), double.Parse(minValue.Text), true, useRandomMode.IsChecked.Value,
                Int32.Parse(stepSize.Text));
            map.BitMapLattice.Draw(map.CurrentLattice);
        }

        public Image GetImageSource()
        {
            return Btm;
        }

        private void AddPointCoordinateToMonitoring(object sender, RoutedEventArgs e)
        {
            if (isCoponentsInitiazed)
            {
                CheckAndAddPoint();
            }
        }

        private bool CheckAndAddPoint()
        {
            bool isXParsing = Int32.TryParse(monitoredPointX.Text, out int x);
            bool isYParsing = Int32.TryParse(monitoredPointY.Text, out int y);
            bool res = isXParsing && isYParsing;
            if (res)
            {
                Charts.AddGraphicToGraphicsList(new Chart(x, y));
                GraphicsStackPanel.Children.Add(Charts.Last.GetGraphic);

                monitoredPointX.Text = "";
                monitoredPointY.Text = "";

                TextBlock textBlock = new TextBlock();
                textBlock.Text = $"({x}, {y})";
                textBlock.TextAlignment = TextAlignment.Center;
                monitoredPoints.Children.Add(textBlock);
            }
            else
            {
                Popup pop = new Popup();
                TextBlock textBlock = new TextBlock();
                textBlock.Background = Brushes.OrangeRed;
                textBlock.Text = "Неправильный формат ввода координат.";
                pop.Child = textBlock;
                pop.Placement = PlacementMode.Mouse;
                pop.StaysOpen = false;
                pop.IsOpen = true;
            }

            return res;
        }

        private void InitializeTextBoxes()
        {
            sizeX.Text = DefaultValues.defaultSizeX.ToString();
            sizeY.Text = DefaultValues.defaultSizeY.ToString();
            Gn.Text = DefaultValues.defaultGn.ToString();
            Cp.Text = DefaultValues.defaultCp.ToString();
            defValue.Text = DefaultValues.defaultDefVulue.ToString();
            maxValue.Text = DefaultValues.defaultMaxValue.ToString();
            minValue.Text = DefaultValues.defaultMinValue.ToString();
            stepSize.Text = DefaultValues.defaultStepSize.ToString();
        }

        private void RebootButtonClick(object sender, RoutedEventArgs e)
        {
            InitializeTextBoxes();
            //InitializeLattice();
            isStarted = false;
            startButton.Content = "Старт";
            Charts.ClearChartsByRebootButton();
            GraphicsStackPanel.Children.Clear();
            ClearMonitoredPointsStackPanel();
        }

        private void ClearMonitoredPointsStackPanel()
        {
            monitoredPoints.Children.RemoveRange(3, monitoredPoints.Children.Count - 3);
        }
    }
}
