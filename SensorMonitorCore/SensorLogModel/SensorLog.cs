using System;
using System.Collections;
using System.Collections.Generic;

namespace SensorMonitor.Core.SensorLogModel
{
    public class SensorLog : IEnumerable<KeyValuePair<string, object>>
    {
        private Dictionary<string, object> fieldValues;

        public SensorLogInfo Info { get; }
        public SensorLogFormat Format { get; }

        public string SensorName { get { return Format.Name; } }
        public SensorIdentifier Identifier { get { return new SensorIdentifier(Format, Info.serial); } }
        
        public SensorLog(SensorLogInfo info, SensorLogFormat format)
        {
            Info = info;
            Format = format ?? throw new ArgumentNullException();
            InitFieldValues(format, new DefaulInitializer());
        }
        public SensorLog(SensorLogInfo info, SensorLogFormat format, IFieldInitializer initializer)
        {
            if ((format == null) || (initializer == null)) throw new ArgumentNullException();
            Info = info;
            Format = format;
            InitFieldValues(format, initializer);
        }
        private void InitFieldValues(SensorLogFormat format, IFieldInitializer initializer)
        {
            fieldValues = new Dictionary<string, object>(format.FieldCount);
            foreach (var pair in format)
            {
                object value = initializer.GetFieldValue(pair.Key);
                this[pair.Key] = value;
            }
        }

        public object this[string fieldName]
        {
            get { return fieldValues[fieldName]; }
            set
            {
                if (value != null)
                {
                    Type validType = Format[fieldName].GetAppropriateDataType(), valueType = value.GetType();
                    if (valueType != validType) throw new ArgumentException();
                }
                fieldValues[fieldName] = value;
            }
        }
        public int FieldCount { get { return fieldValues.Count; } }

        public Field GetFieldByName(string fieldName)
        {
            FieldType type = Format[fieldName];
            object value = fieldValues[fieldName];
            return new Field(fieldName, type, value);
        }
        public IEnumerable<Field> Fields
        {
            get
            {
                foreach (KeyValuePair<string, object> pair in fieldValues)
                {
                    FieldType type = Format[pair.Key];
                    yield return new Field(pair.Key, type, pair.Value);
                }
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (KeyValuePair<string, object> pair in fieldValues)
                yield return pair;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Field
        {
            public readonly string name;
            public readonly FieldType type;
            public readonly object value;

            public Field(string name, FieldType type, object value)
            {
                this.name = name;
                this.type = type;
                this.value = value;
            }
        }

        public interface IFieldInitializer
        {
            object GetFieldValue(string fieldName);
        }

        private class DefaulInitializer : IFieldInitializer
        {
            public object GetFieldValue(string fieldName)
            {
                return null;
            }
        }
    }
}