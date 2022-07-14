using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log
{
    public class LogFieldFromStringValuesInitializer : SensorLog.IFieldInitializer
    {
        private Dictionary<string, string> strFields;
        private SensorLogFormat format;
        private IFieldValueTextParser fieldValueParser;

        public LogFieldFromStringValuesInitializer(Dictionary<string, string> strFields,
            IFieldValueTextParser fieldValueParser, SensorLogFormat format)
        {
            if ((strFields == null) || (fieldValueParser == null) || (format == null))
                throw new ArgumentNullException();
            this.strFields = strFields;
            this.format = format;
            this.fieldValueParser = fieldValueParser;
        }

        public object GetFieldValue(string fieldName)
        {
            string strValue = strFields[fieldName];
            FieldType type = format[fieldName];
            return fieldValueParser.Parse(type, strValue);
        }
    }
}