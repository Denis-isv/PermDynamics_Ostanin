using System;
using System.Windows;
using System.Windows.Controls;

namespace PermDynamics_Ostanin.Pages
{
    public partial class Main : Page
    {
        public MainWindow mainWindow;

        public Main(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void OpenPageChart(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(tb_value.Text, out double value))
            {
                MessageBox.Show("Введите корректное число!");
                return;
            }

            mainWindow.pointsInfo.Add(new Classes.PointInfo(value));
            mainWindow.OpenPages(MainWindow.pages.chart);
        }
    }
}