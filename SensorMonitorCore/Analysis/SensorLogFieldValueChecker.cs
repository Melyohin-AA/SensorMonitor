using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Analysis
{
    public class SensorLogFieldValueChecker : ISensorLogFieldValueChecker
    {
        private IValueChecker valueChecker;

        public FieldType ValuesType { get; }
        public SensorLogFieldPath ValuesField { get; }

        public SensorLogFieldValueChecker(SensorLogFieldPath valuesField, IValueChecker valueChecker)
        {
            this.valueChecker = valueChecker ?? throw new ArgumentNullException();
            ValuesField = valuesField;
            ValuesType = valuesField.sensorId.logFormat[valuesField.logFieldName];
        }

        public ValueIssue CheckSingleLog(SensorLog log)
        {
            double value = Convert.ToDouble(log[ValuesField.logFieldName]);
            return valueChecker.CheckSingleValue(value);
        }

        public IEnumerable<ValueIssue> CheckLogSeq(IEnumerable<SensorLog> logs)
        {
            var values = SelectFieldChronoValues(logs);
            return valueChecker.CheckValueSeq(values);
        }
        private IEnumerable<ChronoValue> SelectFieldChronoValues(IEnumerable<SensorLog> logs)
        {
            foreach (SensorLog log in logs)
            {
                CheckId(log);
                double value = Convert.ToDouble(log[ValuesField.logFieldName]);
                long ticks = log.Info.dt.Ticks;
                yield return new ChronoValue(value, ticks);
            }
        }

        private void CheckId(SensorLog log)
        {
            if (log.Identifier != ValuesField.sensorId) throw new ArgumentException();
        }
    }
}