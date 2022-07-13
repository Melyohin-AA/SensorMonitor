using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using SensorMonitor.Core.SensorLogModel;
using SensorMonitor.Charting;
using LiveCharts.Wpf;

namespace SensorMonitor
{
    public partial class MainWindow : Window
    {
        private Core.SensorManager sensorManager;
        private ChartManager chartManager;
        private Window subwindow;

        private Dictionary<string, ChartManager.SeriesFormat> seriesFormatOptions;
        private Dictionary<string, long> logPeriodOptions;
        private Dictionary<string, Axis> axesStyle;

        public MainWindow()
        {
            sensorManager = CreateSensorManager();
            InitializeComponent();
            chartManager = new ChartManager(chart, sensorManager);
            InitSeriesFormatComboBox();
            InitLogPeriodComboBox();
            InitDTPickers();
            InitAxesStyles();
            InitAxesStyleComboBox();
            ApplyTimePeriod(false);
        }
        private Core.SensorManager CreateSensorManager()
        {
            string envDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string logFilesDir = System.IO.Path.Combine(envDir, "SensorLogs");
            return new Core.SensorManager(logFilesDir);
        }
        private void InitSeriesFormatComboBox()
        {
            seriesFormatOptions = new Dictionary<string, ChartManager.SeriesFormat>()
            {
                { "Ломаная", ChartManager.SeriesFormat.Lines },
                { "Кривая", ChartManager.SeriesFormat.Curve },
                { "Точки", ChartManager.SeriesFormat.Points },
                { "Ступенчатый график", ChartManager.SeriesFormat.Steps },
            };
            foreach (string strSeriesFormat in seriesFormatOptions.Keys)
                seriesFormatComboBox.Items.Add(strSeriesFormat);
        }
        private void InitLogPeriodComboBox()
        {
            logPeriodOptions = new Dictionary<string, long>()
            {
                { "Без осреднения", 0 },
                { "Минута", new DateTime().AddMinutes(1).Ticks },
                { "5 минут", new DateTime().AddMinutes(5).Ticks },
                { "15 минут", new DateTime().AddMinutes(15).Ticks },
                { "Полчаса", new DateTime().AddMinutes(30).Ticks },
                { "Час", new DateTime().AddHours(1).Ticks },
                { "3 часа", new DateTime().AddHours(3).Ticks },
                { "6 часов", new DateTime().AddHours(6).Ticks },
                { "12 часов", new DateTime().AddHours(12).Ticks },
                { "Сутки", new DateTime().AddDays(1).Ticks },
            };
            foreach (string strLogPeriod in logPeriodOptions.Keys)
                logPeriodComboBox.Items.Add(strLogPeriod);
            logPeriodComboBox.SelectedIndex = 0;
        }
        private void InitDTPickers()
        {
            fromDTPicker.Value = DateTime.Today.Add(new TimeSpan(9, 0, 0));
            untilDTPicker.Value = DateTime.Today.Add(new TimeSpan(17, 1, 0));
        }
        private void InitAxesStyles()
        {
            axesStyle = new Dictionary<string, Axis>()
            {
                { "F1", new Axis
                    {
                        LabelFormatter = value => value.ToString("F1", new System.Globalization.CultureInfo("en-UK")),
                        Foreground = Brushes.Black, FontSize = 14,
                    }
                },
                { "F4", new Axis
                    {
                        LabelFormatter = value => value.ToString("F4", new System.Globalization.CultureInfo("en-UK")),
                        Foreground = Brushes.Black, FontSize = 14,
                    }
                },
                { "CO2", new Axis {
                    MinValue = 0, MaxValue = 8000, Sections = new SectionsCollection()
                        {
                            new AxisSection() { Value = 0, SectionWidth = 500, Label = "Отлично",
                                Fill = new SolidColorBrush(Color.FromArgb(96, 11, 93, 25)) },
                            new AxisSection() { Value = 500, SectionWidth = 500, Label = "Нормально",
                                Fill = new SolidColorBrush(Color.FromArgb(96, 112, 173, 71)) },
                            new AxisSection() { Value = 1000, SectionWidth = 500, Label = "Допустимо",
                                Fill = new SolidColorBrush(Color.FromArgb(96, 255, 255, 0)) },
                            new AxisSection() { Value = 1500, SectionWidth = 500, Label = "Плохо",
                                Fill = new SolidColorBrush(Color.FromArgb(96, 255, 153, 0)) },
                            new AxisSection() { Value = 2000, SectionWidth = 6000, Label = "Опасно",
                                Fill = new SolidColorBrush(Color.FromArgb(96, 255, 0, 0)) },
                        },
                        Foreground = Brushes.Black, FontSize = 14,
                    }
                },
            };
        }
        private void InitAxesStyleComboBox()
        {
            foreach (string styleName in axesStyle.Keys)
                axesStyleComboBox.Items.Add(styleName);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            subwindow?.Close();
        }

        private void HandleAxesStyleComboBoxSelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            if (chart.AxisY.Count > 0) chart.AxisY.RemoveAt(chart.AxisY.Count - 1);
            chart.AxisY.Add(axesStyle[(string)axesStyleComboBox.SelectedItem]);
        }

        private void HandleSensorsMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            var win = new SensorsWindow(sensorManager);
            win.DeleteLogsEvent += (id) =>
            {
                for (int i = 0; i < chartManager.SeriesList.Count; i++)
                {
                    ChartManager.SeriesBase series = chartManager.SeriesList[i];
                    if (series.LogFieldPath.sensorId == id) DeleteSeries(i--);
                }
            };
            ShowSensorManagerSubwindow(win);
        }
        private void HandleSensorLogFormatsMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            ShowSensorManagerSubwindow(new SensorLogFormatsWindow(sensorManager));
        }
        private void ShowSensorManagerSubwindow(Window subwindow)
        {
            this.subwindow = subwindow;
            sensorManagerMenu.IsEnabled = false;
            subwindow.Closed += (sender, e) => sensorManagerMenu.IsEnabled = true;
            subwindow.Show();
        }

        private void HandleImportLogsMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "JSON files (*.json)|*.json|CSV files (*.csv)|*.csv" };
            if (dialog.ShowDialog() == false) return;
            //ImportLogs(dialog.FileName);
            try
            {
                ImportLogs(dialog.FileName);
            }
            catch (Exception exception)
            {
                string msg = "Файл повреждён!\nУбедитесь, что файл записан в кодировке UTF-8\n" + exception.Message;
                MessageBox.Show(msg, "Ошибка чтения");
            }
        }
        private void ImportLogs(string path)
        {
            string ext = new System.IO.FileInfo(path).Extension.ToLower();
            var source = new System.IO.StreamReader(path);
            switch (ext)
            {
                case ".csv": sensorManager.LoadCsv(source); break;
                case ".json": sensorManager.LoadJson(source); break;
                default: throw new Exception();
            }
            source.Close();
        }

        private void HandleChartScaleXSliderPreviewMouseUpEvent(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Released) return;
            SetChartScaleX();
        }
        private void SetChartScaleX()
        {
            double scaleExp = chartScaleXSlider.Value;
            chartManager.ScaleX = Math.Pow(2.0, scaleExp);
            double part = chartScrollXScrollBar.Value / chartScrollXScrollBar.Maximum;
            if (double.IsNaN(part)) part = 0.5;
            chartScrollXScrollBar.Maximum = chartManager.NativeRangeWidth - chartManager.VisibleRangeWidth;
            chartScrollXScrollBar.Value = chartScrollXScrollBar.Maximum * part;
        }
        private void ResetScaleX()
        {
            chartScaleXSlider.Value = 0.0;
            chartScrollXScrollBar.Maximum = 0.0;
        }

        private void ChartScrollXScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            long visRangeWidth = chartManager.VisibleRangeWidth;
            DateTime left = new DateTime((long)e.NewValue + chartManager.From.Ticks);
            DateTime right = new DateTime(left.Ticks + visRangeWidth);
            chartManager.UpdateVisibleRange(left, right);
        }

        private void HandleApplyTimePeriodButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            ApplyTimePeriod(true);
        }
        private void ApplyTimePeriod(bool async)
        {
            if (!fromDTPicker.Value.HasValue || !untilDTPicker.Value.HasValue)
                throw new InvalidOperationException();
            DateTime from = fromDTPicker.Value.Value, until = untilDTPicker.Value.Value;
            if (from >= until)
            {
                MessageBox.Show("Начало периода опережает или совпадает с концом!");
                return;
            }
            ResetScaleX();
            long newLogPeriod = logPeriodOptions[(string)logPeriodComboBox.SelectedItem];
            chartManager.IsUpdatingSeriesValuesFreezed = true;
            chartManager.UpdateChronoRange(from, until);
            chartManager.LogPeriod = newLogPeriod;
            chartManager.IsUpdatingSeriesValuesFreezed = false;
            if (async)
            {
                var win = new SeriesLoadingWindow(chartManager);
                chartManager.UpdateAllSeriesAsync();
                win.ShowDialog();
            }
            else chartManager.UpdateAllSeriesValues();
        }

        private void HandleFromDTPickerValueChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!fromDTPicker.Value.HasValue)
                fromDTPicker.Value = new DateTime(2021, 1, 1);
        }
        private void HandleUntilDTPickerValueChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!untilDTPicker.Value.HasValue)
                untilDTPicker.Value = new DateTime(2021, 2, 1);
        }

        private void HandleSeriesFormatComboBoxSelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            string strSeriesFormat = (string)seriesFormatComboBox.SelectedItem;
            chartManager.Format = seriesFormatOptions[strSeriesFormat];
        }

        private void HandleAddSeriesButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            SensorLogFieldSeriesInitDialog dialog = new SensorLogFieldSeriesInitDialog(sensorManager);
            if (dialog.ShowDialog() == true)
                AddSeriesOfLogField(dialog.ResultLogFieldPath, dialog.ResultMaxMinDayMode);
        }
        private void AddSeriesOfLogField(SensorLogFieldPath logFieldPath, bool maxMinDayMode)
        {
            FieldType fieldType = logFieldPath.sensorId.logFormat[logFieldPath.logFieldName];
            if (!ChartManager.CanDisplayField(fieldType))
            {
                MessageBox.Show("Тип данных выбранного поля не позволяет отображать данные поля на графике!");
                return;
            }
            string title = logFieldPath.ToString();
            ChartManager.SeriesBase series = maxMinDayMode ?
                (ChartManager.SeriesBase)new ChartManager.DayMaxMinSeries(chartManager, title, logFieldPath) :
                (ChartManager.SeriesBase)new ChartManager.PeriodAverageSeries(chartManager, title, logFieldPath);
            try
            {
                chartManager.AddSeries(series);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Выбранное поле уже используется для построения графика!");
                return;
            }
            TextBlock logFieldPathTextBlock = new TextBlockWithObject<SensorLogFieldPath> {  obj = logFieldPath };
            string strPath = logFieldPath.ToString();
            logFieldPathTextBlock.Text = maxMinDayMode ? $"Max/Min '{strPath}'" : strPath;
            logFieldPathTextBlock.Foreground = Brushes.Gray;
            seriesListView.Items.Add(logFieldPathTextBlock);
        }

        private void HandleShowHideSeriesMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            if (seriesListView.SelectedItem == null) return;
            ChartManager.SeriesBase series = chartManager.SeriesList[seriesListView.SelectedIndex];
            series.ChangeVisibility();
            TextBlock logFieldPathTextBlock = (TextBlock)seriesListView.SelectedItem;
            if (series.IsVisible) logFieldPathTextBlock.Foreground = Brushes.Black;
            else logFieldPathTextBlock.Foreground = Brushes.Gray;
        }

        private void HandleDeleteSeriesMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            if (seriesListView.SelectedItem == null) return;
            DeleteSeries(seriesListView.SelectedIndex);
        }
        private void HandleDeleteAllSeriesMenuItemClickedEvent(object sender, RoutedEventArgs e)
        {
            for (int i = seriesListView.Items.Count - 1; i >= 0; i--)
                DeleteSeries(i);
        }
        private void DeleteSeries(int index)
        {
            chartManager.RemoveSeries(index);
            seriesListView.Items.RemoveAt(index);
        }

        private class TextBlockWithObject<T> : TextBlock
        {
            public T obj;
        }
    }
}