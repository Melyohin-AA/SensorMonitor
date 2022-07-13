using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using SensorMonitor.Core.SensorLogModel;
using SensorMonitor.Core.SensorIO.Log.Binary;

namespace SensorMonitor.Charting
{
    public class ChartManager
    {
        private static readonly long xAxisPeriod = new DateTime().AddMinutes(30).Ticks;
        private DateTime XAxisValueToDT(double xAxisValue)
        {
            double dTicks = xAxisValue * xAxisPeriod;
            if (dTicks > long.MaxValue) return DateTime.MaxValue;
            if (dTicks < long.MinValue) return DateTime.MinValue;
            return From.AddTicks((long)dTicks);
        }
        private double TicksToXAxisValue(long ticks)
        {
            return (ticks - From.Ticks) / (double)xAxisPeriod;
        }

        public static bool CanDisplayField(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.UInt64:
                case FieldType.Int64:
                case FieldType.UInt32:
                case FieldType.Int32:
                case FieldType.UInt16:
                case FieldType.Int16:
                case FieldType.UInt8:
                case FieldType.Int8:
                case FieldType.Float:
                case FieldType.Double:
                case FieldType.Boolean:
                    return true;
            }
            return false;
        }

        private CartesianChart chart;
        private Core.SensorManager sensorManager;

        public List<SeriesBase> SeriesList { get; }

        public bool IsUpdatingSeriesValuesFreezed { get; set; }

        public event Action SeriesValuesUpdatingAsyncEndedEvent;

        private bool haltSeriesValuesUpdatingAsync;

        private SeriesFormat format = SeriesFormat.Lines;
        public SeriesFormat Format
        {
            get { return format; }
            set
            {
                if (value == format) return;
                format = value;
                foreach (SeriesBase series in SeriesList)
                    series.Update();
            }
        }

        private long logPeriod;
        public long LogPeriod
        {
            get { return logPeriod; }
            set
            {
                if (logPeriod == value) return;
                if (value < 0) throw new ArgumentOutOfRangeException();
                logPeriod = value;
                UpdateAllSeriesValues();
            }
        }

        public long NativeRangeWidth { get { return Until.Ticks - From.Ticks; } }
        public long VisibleRangeWidth { get { return right.Ticks - left.Ticks; } }
        public long VisibleMiddle { get { return (left.Ticks + right.Ticks) >> 1; } }

        public double ScaleX
        {
            get { return NativeRangeWidth / (double)VisibleRangeWidth; }
            set
            {
                if (value < 1.0) throw new ArgumentOutOfRangeException();
                long newVisRangeWidth = (long)(NativeRangeWidth / value);
                if (newVisRangeWidth >= NativeRangeWidth)
                {
                    left = From;
                    right = Until;
                    UpdateVisibleRange();
                    return;
                }
                long newLeft = VisibleMiddle - (newVisRangeWidth >> 1);
                long newRight = VisibleMiddle + (newVisRangeWidth >> 1);
                long delta;
                if (newLeft < From.Ticks) delta = From.Ticks - newLeft;
                else if (newRight > Until.Ticks) delta = Until.Ticks - newRight;
                else delta = 0;
                left = new DateTime(newLeft + delta);
                right = new DateTime(newRight + delta);
                UpdateVisibleRange();
            }
        }

        private DateTime left, right;
        public DateTime Left
        {
            get { return left; }
            set
            {
                left = value;
                UpdateVisibleRange();
            }
        }
        public DateTime Right
        {
            get { return right; }
            set
            {
                right = value;
                UpdateVisibleRange();
            }
        }

        public DateTime From { get; private set; }
        public DateTime Until { get; private set; }

        public ChartManager(CartesianChart chart, Core.SensorManager sensorManager)
        {
            if ((chart == null) || (sensorManager == null)) throw new ArgumentNullException();
            this.chart = chart;
            this.sensorManager = sensorManager;
            SeriesList = new List<SeriesBase>();
            chart.FontSize = 14;
        }

        public bool DoesContainSeries(SensorLogFieldPath logFieldPath, bool maxMinDayMode)
        {
            return FindSeriesByLogFieldPath(logFieldPath, maxMinDayMode) != null;
        }
        public SeriesBase FindSeriesByLogFieldPath(SensorLogFieldPath logFieldPath, bool maxMinDayMode)
        {
            foreach (SeriesBase series in SeriesList)
            {
                if (series.LogFieldPath != logFieldPath) continue;
                if (maxMinDayMode && (series is DayMaxMinSeries)) return series;
                if (!maxMinDayMode && (series is PeriodAverageSeries)) return series;
            }
            return null;
        }

        public void AddSeries(SeriesBase series)
        {
            if (series == null) throw new ArgumentNullException();
            bool maxMinDayMode = series is DayMaxMinSeries;
            if (DoesContainSeries(series.LogFieldPath, maxMinDayMode)) throw new ArgumentException();
            SensorIdentifier id = series.LogFieldPath.sensorId;
            if (!DoesContainSeriesOfSensor(id))
            {
                LogChronoFile chronoFile = sensorManager[id];
                if (chronoFile.IsClosed) chronoFile.Open();
            }
            SeriesList.Add(series);
            series.UpdateValues();
        }
        public void RemoveSeries(int index)
        {
            SeriesBase  series = SeriesList[index];
            SeriesList.RemoveAt(index);
            series.RemoveFromChart();
            SensorIdentifier id = series.LogFieldPath.sensorId;
            if (!DoesContainSeriesOfSensor(id))
            {
                LogChronoFile chronoFile = sensorManager[id];
                if (chronoFile.IsOpened) chronoFile.Close();
            }
        }
        private bool DoesContainSeriesOfSensor(SensorIdentifier id)
        {
            foreach (SeriesBase series in SeriesList)
                if (series.LogFieldPath.sensorId == id) return true;
            return false;
        }

        public void UpdateVisibleRange(DateTime newLeft, DateTime newRight)
        {
            if (newLeft >= newRight) throw new ArgumentException();
            left = newLeft;
            right = newRight;
            UpdateVisibleRange();
        }
        private void UpdateVisibleRange()
        {
            if (chart.AxisX.Count > 0) chart.AxisX.RemoveAt(chart.AxisX.Count - 1);
            chart.AxisX.Add(new Axis
            {
                //MinValue = left.Ticks,
                //MaxValue = right.Ticks,
                //LabelFormatter = value => new DateTime((long)value).ToString("yyyy-MM-dd HH:mm:ss"),
                MinValue = TicksToXAxisValue(left.Ticks),
                MaxValue = TicksToXAxisValue(right.Ticks),
                //LabelFormatter = value => XAxisValueToDT(value).ToString("HH:mm:ss"),
                LabelFormatter = value => XAxisValueToDT(value).ToString("yyyy-MM-dd HH:mm:ss"),
                //LabelsRotation = -90.0,
                Foreground = Brushes.Black, FontSize = 14,
            });
        }

        public void UpdateChronoRange(DateTime from, DateTime until)
        {
            From = from;
            Until = until;
            UpdateVisibleRange(from, until);
            UpdateAllSeriesValues();
        }

        public void UpdateAllSeriesValues()
        {
            if (IsUpdatingSeriesValuesFreezed) return;
            foreach (SeriesBase series in SeriesList)
                series.UpdateValues();
        }
        public void UpdateAllSeriesAsync()
        {
            if (IsUpdatingSeriesValuesFreezed) return;
            ThreadPool.QueueUserWorkItem((state) =>
            {
                foreach (SeriesBase series in SeriesList)
                {
                    if (haltSeriesValuesUpdatingAsync) break;
                    series.UpdateValues();
                }
                haltSeriesValuesUpdatingAsync = false;
                SeriesValuesUpdatingAsyncEndedEvent?.Invoke();
            });
        }

        public void HaltSeriesValuesIpdatingAsync()
        {
            haltSeriesValuesUpdatingAsync = true;
        }

        public enum SeriesFormat
        {
            Lines,
            Curve,
            Steps,
            Points,
        }

        public class PeriodAverageSeries : SeriesBase
        {
            private ISeriesView inner;
            private ChartValues<ObservablePoint> values;

            public PeriodAverageSeries(ChartManager host, string title, SensorLogFieldPath logFieldPath) :
                base(host, title, logFieldPath) { }

            public override void Update()
            {
                RemoveFromChart();
                if (!IsVisible) return;
                inner = CreateSeriesView();
                host.chart.Series.Add(inner);
            }
            private ISeriesView CreateSeriesView()
            {
                switch (host.format)
                {
                    case SeriesFormat.Lines:
                        return new LineSeries
                        {
                            Title = Title,
                            Values = values,
                            PointGeometry = null,
                            Fill = Brushes.Transparent,
                            LineSmoothness = 0,
                        };
                    case SeriesFormat.Curve:
                        return new LineSeries
                        {
                            Title = Title,
                            Values = values,
                            PointGeometry = null,
                            Fill = Brushes.Transparent,
                        };
                    case SeriesFormat.Steps:
                        return new StepLineSeries
                        {
                            Title = Title,
                            Values = values,
                            PointGeometry = null,
                            Fill = Brushes.Transparent,
                        };
                    case SeriesFormat.Points:
                        return new LineSeries
                        {
                            Title = Title,
                            Values = values,
                            PointForeground = new SolidColorBrush(host.chart.GetNextDefaultColor()),
                            Stroke = Brushes.Transparent,
                            Fill = Brushes.Transparent,
                            LineSmoothness = 0,
                        };
                }
                throw new Exception();
            }

            public override void RemoveFromChart()
            {
                if (inner == null) return;
                host.chart.Series.Remove(inner);
                inner = null;
            }
            /*
            protected override void SetValues(LogChronoFile.Iterator from, LogChronoFile.Iterator until)
            {
                var newValues = new ChartValues<ObservablePoint>();
                var trnsformer = new Core.Analysis.Transform.PeriodAverageTransformer(host.logPeriod);
                var values = SelectValuesFromLogs(from, until);
                foreach (var chronoValue in trnsformer.Transform(values))
                    newValues.Add(new ObservablePoint(chronoValue.ticks, chronoValue.value));
                this.values = newValues;
                TrySetInnerValues(inner, newValues);
            }
            private IEnumerable<Core.Analysis.ChronoValue> SelectValuesFromLogs(LogChronoFile.Iterator from,
                LogChronoFile.Iterator until)
            {
                while (from < until)
                {
                    SensorLog log = from.ReadCurrentLog();
                    yield return new Core.Analysis.ChronoValue(log, LogFieldPath.logFieldName);
                    from.MoveForward();
                }
            }
            //*/

            //*
            protected override void SetValues(LogChronoFile.Iterator from, LogChronoFile.Iterator until)
            {
                if (host.logPeriod == 0)
                {
                    SetValuesForZeroPeriod(from, until);
                    return;
                }
                var newValues = new ChartValues<ObservablePoint>();
                double avgY = 0.0;
                long periodOrigin = 0;
                uint logInPeriod = 0;
                SetValuesCarcass(from, until,
                (dt) => {
                    avgY = 0.0;
                    logInPeriod = 0;
                    periodOrigin = dt.Ticks;
                }, (log) => {
                    double value = Convert.ToDouble(log[LogFieldPath.logFieldName]);
                    logInPeriod++;
                    double part = 1.0 / logInPeriod;
                    avgY = (1.0 - part) * avgY + part * value;
                }, (dt) => dt.Ticks >= periodOrigin + host.logPeriod,
                () => AddPoint(newValues, periodOrigin, avgY));
                values = newValues;
                TrySetInnerValues(inner, newValues);
            }
            private void SetValuesForZeroPeriod(LogChronoFile.Iterator from, LogChronoFile.Iterator until)
            {
                var newValues = new ChartValues<ObservablePoint>();
                while (from < until)
                {
                    if (host.haltSeriesValuesUpdatingAsync) return;
                    SensorLog log = from.ReadCurrentLog();
                    long x = from.GetCurrentLogDT().Ticks;
                    double y = Convert.ToDouble(log[LogFieldPath.logFieldName]);
                    AddPoint(newValues, x, y);
                    if (from.IsAtEnd) break;
                    else from.MoveForward();
                }
                values = newValues;
                TrySetInnerValues(inner, newValues);
            }
            //*/
        }

        public class DayMaxMinSeries : SeriesBase
        {
            private ISeriesView maxInner, minInner;
            private ChartValues<ObservablePoint> maxValues, minValues;

            public DayMaxMinSeries(ChartManager host, string title, SensorLogFieldPath logFieldPath) :
                base(host, title, logFieldPath) { }

            public override void Update()
            {
                RemoveFromChart();
                if (!IsVisible) return;
                Brush color = new SolidColorBrush(host.chart.GetNextDefaultColor());
                minInner = CreateSeriesView($"Min '{Title}'", minValues, color);
                maxInner = CreateSeriesView($"Max '{Title}'", maxValues, color);
                host.chart.Series.Add(maxInner);
                host.chart.Series.Add(minInner);
            }
            private ISeriesView CreateSeriesView(string title, ChartValues<ObservablePoint> values, Brush color)
            {
                return new StepLineSeries
                {
                    Title = title,
                    Values = values,
                    PointGeometry = null,
                    Stroke = color,
                    AlternativeStroke = color,
                };
            }

            public override void RemoveFromChart()
            {
                if (maxInner != null)
                {
                    host.chart.Series.Remove(maxInner);
                    maxInner = null;
                }
                if (minInner != null)
                {
                    host.chart.Series.Remove(minInner);
                    minInner = null;
                }
            }

            protected override void SetValues(LogChronoFile.Iterator from, LogChronoFile.Iterator until)
            {
                var newMaxValues = new ChartValues<ObservablePoint>();
                var newMinValues = new ChartValues<ObservablePoint>();
                double maxY = 0.0, minY = 0.0;
                DateTime dayOrigin = default(DateTime), dayEnd = default(DateTime);
                SetValuesCarcass(from, until,
                (dt) => {
                    maxY = double.MinValue;
                    minY = double.MaxValue;
                    dayOrigin = dt.Date;
                    dayEnd = dayOrigin.AddDays(1);
                }, (log) => {
                    double value = Convert.ToDouble(log[LogFieldPath.logFieldName]);
                    if (value > maxY) maxY = value;
                    if (value < minY) minY = value;
                }, (dt) => dt >= dayEnd,
                () => {
                    AddPoint(newMaxValues, dayOrigin.Ticks, maxY);
                    AddPoint(newMinValues, dayOrigin.Ticks, minY);
                });
                if (newMaxValues.Count > 0)
                {
                    AddPoint(newMaxValues, dayEnd.Ticks, maxY);
                    AddPoint(newMinValues, dayEnd.Ticks, minY);
                }
                maxValues = newMaxValues;
                minValues = newMinValues;
                TrySetInnerValues(maxInner, newMaxValues);
                TrySetInnerValues(minInner, newMinValues);
            }
        }

        public abstract class SeriesBase
        {
            protected ChartManager host;

            public string Title { get; }
            public bool IsVisible { get; private set; }
            public SensorLogFieldPath LogFieldPath { get; }

            public SeriesBase(ChartManager host, string title, SensorLogFieldPath logFieldPath)
            {
                if ((host == null) || (title == null))
                    throw new ArgumentNullException();
                if (!CanDisplayField(logFieldPath.sensorId.logFormat[logFieldPath.logFieldName]))
                    throw new ArgumentException("Specified log field couldn't be displayed!");
                this.host = host;
                Title = title;
                LogFieldPath = logFieldPath;
            }

            public void ChangeVisibility()
            {
                IsVisible = !IsVisible;
                Update();
            }

            public abstract void Update();

            public abstract void RemoveFromChart();

            public void UpdateValues()
            {
                var file = host.sensorManager[LogFieldPath.sensorId];
                if (file.IsClosed) file.Open();
                var fromIter = file.GetIteratorOnOrJustAfter(host.From);
                var untilIter = file.GetIteratorOnOrJustAfter(host.Until);
                SetValues(fromIter, untilIter);
            }
            protected abstract void SetValues(LogChronoFile.Iterator from, LogChronoFile.Iterator until);
            protected void SetValuesCarcass(LogChronoFile.Iterator from, LogChronoFile.Iterator until,
                Action<DateTime> resetVars, Action<SensorLog> updateVars,
                Func<DateTime, bool> isToAddPoint, Action addPoint)
            {
                bool atEnd;
                SensorLog log;
                reset:
                resetVars(from.GetCurrentLogDT());
                read:
                if (host.haltSeriesValuesUpdatingAsync) return;
                log = from.ReadCurrentLog();
                updateVars(log);
                atEnd = from.IsAtEnd;
                if (!from.IsAtEnd) from.MoveForward();
                if (from >= until) atEnd = true;
                if (atEnd || isToAddPoint(from.GetCurrentLogDT()))
                {
                    addPoint();
                    if (!atEnd) goto reset;
                }
                if (!atEnd) goto read;
            }
            protected void TrySetInnerValues(ISeriesView inner, ChartValues<ObservablePoint> values)
            {
                if (inner != null) host.chart.Dispatcher.Invoke(() => inner.Values = values);
            }

            protected void AddPoint(ChartValues<ObservablePoint> values, long x, double y)
            {
                values.Add(new ObservablePoint(host.TicksToXAxisValue(x), y));
            }
        }
    }
}