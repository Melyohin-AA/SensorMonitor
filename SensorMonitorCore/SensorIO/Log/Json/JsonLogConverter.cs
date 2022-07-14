using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log.Json
{
    public interface IJsonLogConverter
    {
        SensorLog Convert(string jsonLog);
    }

    public class JsonLogConverter : IJsonLogConverter
    {
        private IJsonParser jsonParser;
        private IFieldValueTextParser fieldValueParser;
        private SensorLogFormatTable table;

        public JsonLogConverter(IJsonParser jsonParser, IFieldValueTextParser fieldValueParser,
            SensorLogFormatTable table)
        {
            if ((jsonParser == null) || (fieldValueParser == null) || (table == null))
                throw new ArgumentNullException();
            this.jsonParser = jsonParser;
            this.fieldValueParser = fieldValueParser;
            this.table = table;
        }

        public SensorLog Convert(string jsonLog)
        {
            var parsedJsonLog = jsonParser.Parse(jsonLog);
            SensorLogInfo info = ParseLogInfo(parsedJsonLog, out string sensorName);
            SensorLogFormat format = table[sensorName];
            var strFields = jsonParser.Parse(parsedJsonLog["data"]);
            var initializer = new LogFieldFromStringValuesInitializer(strFields, fieldValueParser, format);
            return new SensorLog(info, format, initializer);
        }
        private SensorLogInfo ParseLogInfo(Dictionary<string, string> parsedJsonLog, out string sensorName)
        {
            DateTime dt = (DateTime)fieldValueParser.Parse(FieldType.DateTime, parsedJsonLog["Date"]);
            sensorName = parsedJsonLog["uName"];
            string serial = parsedJsonLog["serial"];
            return new SensorLogInfo(dt, serial);
        }
    }
}