using System;
using System.IO;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log.Binary
{
    public class LogFieldsBinaryWriter : Tools.BinaryIOFormat.BinaryFormatWriter<SensorLog>, ILogFieldsWriter
    {
        public const byte ActualBinaryFormatVersion = 1;

        private IFieldValueBinaryWriter fieldValueBinaryWriter;

        static LogFieldsBinaryWriter()
        {
            AddInnerCreationFunc(0, (dest) => new InnerV0(dest));
            AddInnerCreationFunc(1, (dest) => new InnerV1(dest));
        }

        public LogFieldsBinaryWriter(IFieldValueBinaryWriter fieldValueBinaryWriter,
            byte binaryFormatVersion = ActualBinaryFormatVersion) : base(binaryFormatVersion)
        {
            this.fieldValueBinaryWriter = fieldValueBinaryWriter ?? throw new ArgumentNullException();
        }

        public void WriteFields(Stream dest, SensorLog log)
        {
            ThisInner inner = (ThisInner)appropriateInnerCreationFunc(dest);
            inner.Host = this;
            inner.Write(log);
        }

        private abstract class ThisInner : Inner
        {
            public LogFieldsBinaryWriter Host { get; set; }

            public ThisInner(Stream dest) : base(dest) { }
        }

        private class InnerV0 : ThisInner
        {
            public InnerV0(Stream dest) : base(dest) { }

            public override void Write(SensorLog log)
            {
                foreach (SensorLog.Field field in log.Fields)
                {
                    writer.Write(field.name);
                    Host.fieldValueBinaryWriter.Write(writer, field.type, field.value);
                }
            }
        }

        private class InnerV1 : ThisInner
        {
            public InnerV1(Stream dest) : base(dest) { }

            public override void Write(SensorLog log)
            {
                foreach (SensorLog.Field field in log.Fields)
                    Host.fieldValueBinaryWriter.Write(writer, field.type, field.value);
            }
        }
    }
}