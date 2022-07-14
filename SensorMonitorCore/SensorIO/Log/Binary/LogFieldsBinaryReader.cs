using System;
using System.IO;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log.Binary
{
    public class LogFieldsBinaryReader : Tools.BinaryIOFormat.BinaryFormatReader<SensorLog.IFieldInitializer>,
        ILogFieldsReader
    {
        public const byte ActualBinaryFormatVersion = 1;

        private IFieldValueBinaryReader fieldValueBinaryReader;

        static LogFieldsBinaryReader()
        {
            AddInnerCreationFunc(0, (source) => new InnerV0(source));
            AddInnerCreationFunc(1, (source) => new InnerV1(source));
        }

        public LogFieldsBinaryReader(IFieldValueBinaryReader fieldValueBinaryReader,
            byte binaryFormatVersion = ActualBinaryFormatVersion) : base(binaryFormatVersion)
        {
            this.fieldValueBinaryReader = fieldValueBinaryReader ?? throw new ArgumentNullException();
        }

        public SensorLog.IFieldInitializer Read(Stream source, SensorLogFormat format)
        {
            ThisInner inner = (ThisInner)appropriateInnerCreationFunc(source);
            inner.Host = this;
            inner.LogFormat = format;
            return inner.Read();
        }

        private abstract class ThisInner : Inner
        {
            public LogFieldsBinaryReader Host { get; set; }
            public SensorLogFormat LogFormat { get; set; }

            public ThisInner(Stream source) : base(source) { }
        }

        private class InnerV0 : ThisInner
        {
            public InnerV0(Stream source) : base(source) { }

            public override SensorLog.IFieldInitializer Read()
            {
                var fieldValues = new Dictionary<string, object>(LogFormat.FieldCount);
                for (short i = 0; i < LogFormat.FieldCount; i++)
                {
                    string name = reader.ReadString();
                    FieldType type = LogFormat[name];
                    object value = Host.fieldValueBinaryReader.Read(reader, type);
                    fieldValues.Add(name, value);
                }
                return new Tools.LogFieldFromDictInitializer(fieldValues);
            }
        }

        private class InnerV1 : ThisInner
        {
            public InnerV1(Stream source) : base(source) { }

            public override SensorLog.IFieldInitializer Read()
            {
                var fieldValues = new Dictionary<string, object>(LogFormat.FieldCount);
                foreach (KeyValuePair<string, FieldType> pair in LogFormat)
                {
                    FieldType type = pair.Value;
                    object value = Host.fieldValueBinaryReader.Read(reader, type);
                    fieldValues.Add(pair.Key, value);
                }
                return new Tools.LogFieldFromDictInitializer(fieldValues);
            }
        }
    }
}