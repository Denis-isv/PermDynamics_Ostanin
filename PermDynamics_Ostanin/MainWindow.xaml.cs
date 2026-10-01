using System.Collections.Generic;
using System.Windows;
using PermDynamics_Ostanin.Pages;

namespace PermDynamics_Ostanin
{
    public partial class MainWindow : Window
    {
        public List<Classes.PointInfo> pointsInfo = new List<Classes.PointInfo>();

        public MainWindow()
        {
            InitializeComponent();
            OpenPages(pages.main);
        }

        public enum pages
        {
            main,
            chart
        }

        public void OpenPages(pages _pages)
        {
            if (_pages == pages.main)
                frame.Navigate(new Main(this));
            else if (_pages == pages.chart)
                frame.Navigate(new Chart(this));
        }
    }
}