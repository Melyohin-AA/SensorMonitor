using System;
using System.Windows;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor
{
    public partial class SensorsWindow : Window
    {
        private Core.SensorManager sensorManager;

        public event Action<SensorIdentifier> DeleteLogsEvent;

        public SensorsWindow(Core.SensorManager sensorManager)
        {
            this.sensorManager = sensorManager ?? throw new ArgumentNullException();
            InitializeComponent();
            InitLogSeqViewList();
        }
        private void InitLogSeqViewList()
        {
            foreach (var logChronoFile in sensorManager.LogChronoFiles)
                logSeqListView.Items.Add(logChronoFile.SensorIdentifier);
        }

        private void HandleAddNewSensorButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            SensorLogFormat defaultLogFormat = (logSeqListView.Items.Count == 0) ? null :
                ((SensorIdentifier)logSeqListView.Items[logSeqListView.Items.Count - 1]).logFormat;
            NewSensorDialog dialog = new NewSensorDialog(sensorManager, defaultLogFormat);
            if (dialog.ShowDialog() == true)
                AddSensor(dialog.Result);
        }
        private void AddSensor(SensorIdentifier id)
        {
            sensorManager.AddSensor(id);
            logSeqListView.Items.Add(id);
            logSeqListView.SelectedIndex = logSeqListView.Items.Count - 1;
        }

        private void HandleDeleteLogSeqMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            if (logSeqListView.SelectedItem == null) return;
            SensorIdentifier sensorId = (SensorIdentifier)logSeqListView.SelectedItem;
            DeleteLogsEvent?.Invoke(sensorId);
            sensorManager.DeleteSensor(sensorId);
            logSeqListView.Items.RemoveAt(logSeqListView.SelectedIndex);
        }

        private void HandleClearLogSeqMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            if (logSeqListView.SelectedItem == null) return;
            SensorIdentifier sensorId = (SensorIdentifier)logSeqListView.SelectedItem;
            DeleteLogsEvent?.Invoke(sensorId);
            sensorManager.OpenLogChronoFileAsNew(sensorId);
            sensorManager[sensorId].Close();
        }

        private void HandleDefragmentLogSeqFileMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            if (logSeqListView.SelectedItem == null) return;
            SensorIdentifier sensorId = (SensorIdentifier)logSeqListView.SelectedItem;
            sensorManager.DefragmentLogChronoFile(sensorId);
        }
    }
}