using System;
using System.Collections;
using System.Collections.Generic;

namespace SensorMonitor.Core.SensorLogModel
{
    public class SensorLogFormat : IEnumerable<KeyValuePair<string, FieldType>>
    {
        private Dictionary<string, FieldType> fieldTypes;

        public string Name { get; }

        public SensorLogFormat(string name, Dictionary<string, FieldType> fieldTypes)
        {
            if ((name == null) || (fieldTypes == null)) throw new ArgumentNullException();
            Name = name;
            this.fieldTypes = fieldTypes;
        }

        public FieldType this[string fieldName] { get { return fieldTypes[fieldName]; } }
        public int FieldCount { get { return fieldTypes.Count; } }

        public IEnumerator<KeyValuePair<string, FieldType>> GetEnumerator()
        {
            foreach (KeyValuePair<string, FieldType> pair in fieldTypes)
                yield return pair;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}