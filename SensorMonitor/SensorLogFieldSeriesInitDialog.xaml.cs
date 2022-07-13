using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor
{
    public partial class SensorLogFieldSeriesInitDialog : Window
    {
        private static SensorIdentifier prevSelectedId;

        public SensorLogFieldPath ResultLogFieldPath { get; private set; }
        public bool ResultMaxMinDayMode { get; private set; }

        private Core.SensorManager sensorManager;

        public SensorLogFieldSeriesInitDialog(Core.SensorManager sensorManager)
        {
            this.sensorManager = sensorManager ?? throw new ArgumentNullException();
            InitializeComponent();
            InitSensorComboBox();
        }
        private void InitSensorComboBox()
        {
            foreach (SensorIdentifier id in sensorManager.LogChronoFilesIds)
            {
                sensorComboBox.Items.Add(id);
                if (id == prevSelectedId)
                    sensorComboBox.SelectedIndex = sensorComboBox.Items.Count - 1;
            }
        }

        private void HandleSensorComboBoxSelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            logFieldComboBox.Items.Clear();
            if (sensorComboBox.SelectedItem == null) return;
            SensorIdentifier id = (SensorIdentifier)sensorComboBox.SelectedItem;
            foreach (KeyValuePair<string, FieldType> pair in id.logFormat)
                logFieldComboBox.Items.Add(pair.Key);
        }

        private void HandleSubmitButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            if ((sensorComboBox.SelectedItem == null) || (logFieldComboBox.SelectedItem == null))
            {
                MessageBox.Show("Не выбрано поле записей прибора!");
                return;
            }
            prevSelectedId = (SensorIdentifier)sensorComboBox.SelectedItem;
            string logFieldName = (string)logFieldComboBox.SelectedItem;
            ResultLogFieldPath = new SensorLogFieldPath(prevSelectedId, logFieldName);
            ResultMaxMinDayMode = maxMinDayModeCheckBox.IsChecked == true;
            DialogResult = true;
            Close();
        }
    }
}