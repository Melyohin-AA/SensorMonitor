using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor
{
    public partial class NewSensorLogFormatDialog : Window
    {
        private SensorLogFormatTable sensorTable;

        public SensorLogFormat Result { get; private set; }

        public NewSensorLogFormatDialog(SensorLogFormatTable sensorTable)
        {
            this.sensorTable = sensorTable ?? throw new ArgumentNullException();
            InitializeComponent();
        }

        private void HandleAddLogFieldButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            var item = CreateLogFieldsListViewItem();
            logFieldsListView.Items.Add(item);
        }
        private object CreateLogFieldsListViewItem()
        {
            ComboBox fieldTypeComboBox = new ComboBox();
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.UInt8);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.Int8);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.UInt16);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.Int16);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.UInt32);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.Int32);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.UInt64);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.Int64);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.Float);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.Double);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.String);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.Boolean);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.DateTime);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.Date);
            AddFieldTypeToComboBox(fieldTypeComboBox, FieldType.Time);
            SelectFieldTypeInComboBox(fieldTypeComboBox);
            fieldTypeComboBox.HorizontalAlignment = HorizontalAlignment.Right;
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            TextBox sensorNameTextBox = new TextBox();
            sensorNameTextBox.MinWidth = 50;
            stackPanel.Children.Add(sensorNameTextBox);
            stackPanel.Children.Add(fieldTypeComboBox);
            return stackPanel;
        }
        private void AddFieldTypeToComboBox(ComboBox comboBox, FieldType type)
        {
            string strType = FieldTypeExtension.ToString(type);
            comboBox.Items.Add(strType);
        }
        private void SelectFieldTypeInComboBox(ComboBox fieldTypeComboBox)
        {
            if (logFieldsListView.Items.Count == 0)
            {
                fieldTypeComboBox.SelectedIndex = 0;
                return;
            }
            var stackPanel = (StackPanel)logFieldsListView.Items[logFieldsListView.Items.Count - 1];
            ComboBox prev = (ComboBox)stackPanel.Children[1];
            fieldTypeComboBox.SelectedIndex = prev.SelectedIndex;
        }

        private void HandleSubmitButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            string sensorName = sensorNameTextBox.Text;
            if (sensorTable.Contains(sensorName))
            {
                MessageBox.Show("Формат с заданным названием уже существует!");
                return;
            }
            Dictionary<string, FieldType> filedTypes;
            try
            {
                filedTypes = FormFieldTypes();
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Совпадение имён некоторых полей!");
                return;
            }
            Result = new SensorLogFormat(sensorName, filedTypes);
            DialogResult = true;
            Close();
        }
        private Dictionary<string, FieldType> FormFieldTypes()
        {
            var fieldTypes = new Dictionary<string, FieldType>();
            for (int i = 0; i < logFieldsListView.Items.Count; i++)
            {
                var stackPanel = (StackPanel)logFieldsListView.Items[i];
                TextBox sensorNameTextBox = (TextBox)stackPanel.Children[0];
                ComboBox fieldTypeComboBox = (ComboBox)stackPanel.Children[1];
                string fieldName = sensorNameTextBox.Text;
                string strFieldType = (string)fieldTypeComboBox.SelectedValue;
                FieldType fieldType = FieldTypeExtension.FromString(strFieldType);
                fieldTypes.Add(fieldName, fieldType);
            }
            return fieldTypes;
        }

        private void HandleDeleteLogFieldMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            int index = logFieldsListView.SelectedIndex;
            if (index == -1) return;
            logFieldsListView.Items.RemoveAt(index);
        }
    }
}