using System;
using System.Windows;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor
{
    public partial class NewSensorDialog : Window
    {
        private Core.SensorManager sensorManager;

        public SensorIdentifier Result { get; private set; }

        public NewSensorDialog(Core.SensorManager sensorManager, SensorLogFormat defaultLogFormat = null)
        {
            this.sensorManager = sensorManager ?? throw new ArgumentNullException();
            InitializeComponent();
            InitLogFormatsComboBox(defaultLogFormat);
        }
        private void InitLogFormatsComboBox(SensorLogFormat defaultLogFormat)
        {
            int defaultLogFormatIndex = 0, i = 0;
            foreach (SensorLogFormat logFormat in sensorManager.Table)
            {
                logFormatsComboBox.Items.Add(logFormat.Name);
                if (logFormat == defaultLogFormat) defaultLogFormatIndex = i;
                i++;
            }
            logFormatsComboBox.SelectedIndex = defaultLogFormatIndex;
        }

        private void HandleSubmitButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            string sensorName = (string)logFormatsComboBox.SelectedItem;
            SensorLogFormat logFormat = sensorManager.Table[sensorName];
            string sensorSerial = serialTextBox.Text;
            SensorIdentifier id = new SensorIdentifier(logFormat, sensorSerial);
            bool exists = sensorManager.DoesSensorExist(id);
            if (exists)
            {
                MessageBox.Show("Прибор с заданными форматом и serial уже существует!");
                return;
            }
            Result = id;
            DialogResult = true;
            Close();
        }
    }
}