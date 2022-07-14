using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log.Csv
{
    public class CsvLogSeqConverter : ITextStreamLogSeqConverter
    {
        private SensorLogFormatTable table;
        private IFieldValueTextParser fieldValueParser;

        public CsvLogSeqConverter(SensorLogFormatTable table, IFieldValueTextParser fieldValueParser)
        {
            if ((table == null) || (fieldValueParser == null)) throw new ArgumentNullException();
            this.table = table;
            this.fieldValueParser = fieldValueParser;
        }

        public IEnumerable<SensorLog> Convert(TextReader csvLogSeqReader)
        {
            Inner inner = new Inner(this, csvLogSeqReader);
            inner.Convert();
            return inner.Result;
        }

        private class Inner
        {
            private CsvLogSeqConverter host;
            private TextReader csvReader;
            private SensorIdentifier sensorId;
            private SensorLogFormat format;
            private int dateFieldIndex;
            private string[] fieldNames;
            private List<SensorLog> logs = new List<SensorLog>();

            public ReadOnlyCollection<SensorLog> Result { get; private set; }

            public Inner(CsvLogSeqConverter host, TextReader csvReader)
            {
                if ((host == null) || (csvReader == null)) throw new ArgumentNullException();
                this.host = host;
                this.csvReader = csvReader;
            }

            public void Convert()
            {
                ReadHeader();
                format = host.table[sensorId.logFormat.Name];
                ReadFieldNames();
                while (true)
                {
                    string lineFieldValues = csvReader.ReadLine();
                    if ((lineFieldValues == null) || (lineFieldValues.Length == 0)) break;
                    SensorLog log = ParseLog(lineFieldValues);
                    logs.Add(log);
                }
                logs.TrimExcess();
                Result = logs.AsReadOnly();
            }
            private void ReadHeader()
            {
                string header = csvReader.ReadLine();
                int nameAndSerialOrigin = header.IndexOf(';') + 1;
                int nameAndSerialEnd = header.IndexOf(';', nameAndSerialOrigin);
                int nameAndSerialLength = nameAndSerialEnd - nameAndSerialOrigin;
                string nameAndSerial = header.Substring(nameAndSerialOrigin, nameAndSerialLength);
                sensorId = new SensorIdentifier(nameAndSerial, host.table);
            }
            private void ReadFieldNames()
            {
                string fieldNamesLine = csvReader.ReadLine();
                fieldNamesLine = fieldNamesLine.Remove(fieldNamesLine.Length - 1);
                fieldNames = fieldNamesLine.Split(';');
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (fieldNames[i] == "Date")
                    {
                        dateFieldIndex = i;
                        break;
                    }
                }
            }
            private SensorLog ParseLog(string lineFieldValues)
            {
                lineFieldValues = lineFieldValues.Remove(lineFieldValues.Length - 1);
                string[] strFieldValues = lineFieldValues.Split(';');
                if (fieldNames.Length != strFieldValues.Length) throw new FormatException();
                var parser = host.fieldValueParser;
                var strFields = new Dictionary<string, string>();
                SensorLogInfo info = default(SensorLogInfo);
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (i == dateFieldIndex)
                    {
                        DateTime dt = (DateTime)parser.Parse(FieldType.DateTime, strFieldValues[i]);
                        info = new SensorLogInfo(dt, sensorId.serial);
                        continue;
                    }
                    strFields.Add(fieldNames[i], strFieldValues[i]);
                }
                var initializer = new LogFieldFromStringValuesInitializer(strFields, parser, format);
                return new SensorLog(info, format, initializer);
            }
        }
    }
}