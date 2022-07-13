using System;
using System.Windows;
using SensorMonitor.Charting;

namespace SensorMonitor
{
    public partial class SeriesLoadingWindow : Window
    {
        private ChartManager chartManager;

        public SeriesLoadingWindow(ChartManager chartManager)
        {
            this.chartManager = chartManager ?? throw new ArgumentNullException();
            InitializeComponent();
            chartManager.SeriesValuesUpdatingAsyncEndedEvent += OnSeriesValuesUpdatingAsyncEnded;
        }

        private void OnSeriesValuesUpdatingAsyncEnded()
        {
            Dispatcher.Invoke(Close);
        }

        private void HandleHaltButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            chartManager.HaltSeriesValuesIpdatingAsync();
        }
    }
}