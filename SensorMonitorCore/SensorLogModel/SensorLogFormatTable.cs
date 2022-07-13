using System;
using System.Collections;
using System.Collections.Generic;

namespace SensorMonitor.Core.SensorLogModel
{
    public class SensorLogFormatTable : IEnumerable<SensorLogFormat>
    {
        private Dictionary<string, SensorLogFormat> logFormats;

        public SensorLogFormatTable()
        {
            logFormats = new Dictionary<string, SensorLogFormat>();
        }
        public SensorLogFormatTable(IEnumerable<SensorLogFormat> logFormats)
        {
            if (logFormats == null) throw new ArgumentNullException();
            this.logFormats = new Dictionary<string, SensorLogFormat>();
            foreach (SensorLogFormat format in logFormats)
                this.logFormats.Add(format.Name, format);
        }

        public SensorLogFormat this[string sensorName] { get { return logFormats[sensorName]; } }
        public int LogFormatCount { get { return logFormats.Count; } }

        public bool Contains(string sensorName)
        {
            return logFormats.ContainsKey(sensorName);
        }
        public bool Contains(SensorLogFormat logFormat)
        {
            return logFormats.ContainsValue(logFormat);
        }

        public void AddLogFormat(SensorLogFormat newFormat)
        {
            if (newFormat == null) throw new ArgumentNullException();
            logFormats.Add(newFormat.Name, newFormat);
        }
        public void RemoveLogFormat(SensorLogFormat format)
        {
            if (format == null) throw new ArgumentNullException();
            logFormats.Remove(format.Name);
        }

        public IEnumerator<SensorLogFormat> GetEnumerator()
        {
            foreach (var pair in logFormats)
                yield return pair.Value;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}