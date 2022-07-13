using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Tools
{
    public class LogFieldFromDictInitializer : SensorLog.IFieldInitializer
    {
        private Dictionary<string, object> fieldValues;

        public LogFieldFromDictInitializer(Dictionary<string, object> fieldValues)
        {
            this.fieldValues = fieldValues ?? throw new ArgumentNullException();
        }

        public object GetFieldValue(string fieldName)
        {
            return fieldValues[fieldName];
        }
    }
}