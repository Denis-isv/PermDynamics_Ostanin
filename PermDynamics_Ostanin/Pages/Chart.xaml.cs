using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PermDynamics_Ostanin.Pages
{
    public partial class Chart : Page
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private double actualHeightCanvas = 200; // высота Canvas
        private double maxValue = 0;
        private double averageValue = 0;
        private Line averageLine;
        private MainWindow mainWindow;

        public Chart(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            actualHeightCanvas = canvas.Height; // берём высоту из XAML

            timer.Interval = TimeSpan.FromSeconds(2); // по заданию раз в 2 секунды
            timer.Tick += Timer_Tick;
            timer.Start();

            CreateChart();
            ColorChart();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            double lastValue = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;

            // отклонение не более 50%: множитель от 0.5 до 1.5
            double newValue = lastValue * (0.5 + random.NextDouble());
            mainWindow.pointsInfo.Add(new Classes.PointInfo(newValue));

            ControlCreateChart();
        }

        public void CreateChart()
        {
            canvas.Children.Clear();
            averageLine = null;

            // пересчитываем максимум заново
            maxValue = 0;
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
            {
                if (mainWindow.pointsInfo[i].value > maxValue)
                    maxValue = mainWindow.pointsInfo[i].value;
            }
            if (maxValue == 0) maxValue = 1; // защита от деления на ноль

            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
            {
                Line line = new Line();
                line.X1 = i * 20;
                line.X2 = (i + 1) * 20;

                if (i == 0)
                    line.Y1 = actualHeightCanvas;
                else
                    line.Y1 = actualHeightCanvas - ((mainWindow.pointsInfo[i - 1].value / maxValue) * actualHeightCanvas);

                line.Y2 = actualHeightCanvas - ((mainWindow.pointsInfo[i].value / maxValue) * actualHeightCanvas);
                line.StrokeThickness = 2;
                mainWindow.pointsInfo[i].line = line;
                canvas.Children.Add(line);
            }

            // среднее значение
            averageValue = 0;
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                averageValue += mainWindow.pointsInfo[i].value;
            if (mainWindow.pointsInfo.Count > 0)
                averageValue /= mainWindow.pointsInfo.Count;

            DrawAverageLine();
        }

        public void CreatePoint()
        {
            // добавляем только одну новую линию
            Line line = new Line();
            line.X1 = (mainWindow.pointsInfo.Count - 1) * 20;
            line.X2 = mainWindow.pointsInfo.Count * 20;
            line.Y1 = actualHeightCanvas - ((mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 2].value / maxValue) * actualHeightCanvas);
            line.Y2 = actualHeightCanvas - ((mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value / maxValue) * actualHeightCanvas);
            line.StrokeThickness = 2;
            mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].line = line;
            canvas.Children.Add(line);

            // пересчёт среднего
            averageValue = 0;
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                averageValue += mainWindow.pointsInfo[i].value;
            averageValue /= mainWindow.pointsInfo.Count;

            DrawAverageLine();
        }

        public void ControlCreateChart()
        {
            double lastValue = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;

            if (lastValue < maxValue)
                CreatePoint();
            else
                CreateChart();

            ColorChart();
        }

        public void ColorChart()
        {
            double lastValue = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;

            // пересчитываем среднее (на всякий случай)
            averageValue = 0;
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                averageValue += mainWindow.pointsInfo[i].value;
            averageValue /= mainWindow.pointsInfo.Count;

            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
            {
                if (lastValue < averageValue)
                    mainWindow.pointsInfo[i].line.Stroke = Brushes.Red;
                else
                    mainWindow.pointsInfo[i].line.Stroke = Brushes.Green;
            }

            canvas.Width = mainWindow.pointsInfo.Count * 20 + 300;
            scroll.ScrollToHorizontalOffset(canvas.Width);

            current_value.Content = "Тек. знач: " + Math.Round(lastValue, 2);
            average_value.Content = "Сред. знач: " + Math.Round(averageValue, 2);
        }

        public void DrawAverageLine()
        {
            if (averageLine != null)
                canvas.Children.Remove(averageLine);

            if (maxValue == 0) return;

            averageLine = new Line();
            averageLine.X1 = 0;
            averageLine.X2 = mainWindow.pointsInfo.Count * 20;

            double averageY = actualHeightCanvas - ((averageValue / maxValue) * actualHeightCanvas);
            averageY = Math.Max(0, Math.Min(actualHeightCanvas, averageY)); // не выходим за пределы

            averageLine.Y1 = averageY;
            averageLine.Y2 = averageY;
            averageLine.StrokeThickness = 2;
            averageLine.Stroke = Brushes.Pink;
            averageLine.StrokeDashArray = new DoubleCollection { 5, 5 };

            canvas.Children.Add(averageLine);
        }

        public void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mainWindow.pointsInfo.Count > 0)
            {
                CreateChart();
                ColorChart();
            }
        }
    }
}