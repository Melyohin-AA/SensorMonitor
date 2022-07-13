using System;
using System.Collections.Generic;
using System.Windows;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor
{
    public partial class SensorLogFormatsWindow : Window
    {
        private Core.SensorManager sensorManager;

        public SensorLogFormatsWindow(Core.SensorManager sensorManager)
        {
            this.sensorManager = sensorManager ?? throw new ArgumentNullException();
            InitializeComponent();
            InitLogFormatsListView();
        }
        private void InitLogFormatsListView()
        {
            foreach (SensorLogFormat logFormat in sensorManager.Table)
                logFormatsListView.Items.Add(logFormat.Name);
        }

        private void HandleLogFormatsListViewSelectionChangedEvent(object sender, RoutedEventArgs e)
        {
            FillLogFormatFieldsListView();
        }
        private void FillLogFormatFieldsListView()
        {
            string sensorName = (string)logFormatsListView.SelectedItem;
            if (sensorName == null)
            {
                logFormatFieldsListView.Items.Clear();
                return;
            }
            SensorLogFormat logFormat = sensorManager.Table[sensorName];
            logFormatFieldsListView.Items.Clear();
            foreach (KeyValuePair<string, FieldType> pair in logFormat)
            {
                string strFieldType = FieldTypeExtension.ToString(pair.Value);
                logFormatFieldsListView.Items.Add($"{strFieldType} '{pair.Key}'");
            }
        }

        private void HandleNewLogFormatButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            NewSensorLogFormatDialog dialog = new NewSensorLogFormatDialog(sensorManager.Table);
            if (dialog.ShowDialog() == true)
                AddLogFormat(dialog.Result);
        }
        private void AddLogFormat(SensorLogFormat logFormat)
        {
            sensorManager.AddLogFormat(logFormat);
            logFormatsListView.Items.Add(logFormat.Name);
            logFormatsListView.SelectedIndex = logFormatsListView.Items.Count - 1;
        }

        private void HandleDeleteLogFormatMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            string sensorName = (string)logFormatsListView.SelectedItem;
            if (sensorName == null) return;
            SensorLogFormat logFormat = sensorManager.Table[sensorName];
            sensorManager.DeleteLogFormat(logFormat);
            logFormatsListView.Items.RemoveAt(logFormatsListView.SelectedIndex);
        }
    }
}