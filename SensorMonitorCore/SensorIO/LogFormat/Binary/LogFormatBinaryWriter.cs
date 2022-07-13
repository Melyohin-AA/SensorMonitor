using System;
using System.IO;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.LogFormat.Binary
{
    public class LogFormatBinaryWriter : Tools.BinaryIOFormat.BinaryFormatWriter<SensorLogFormat>, ILogFormatWriter
    {
        public const byte ActualBinaryFormatVersion = 0;

        static LogFormatBinaryWriter()
        {
            AddInnerCreationFunc(0, (dest) => new InnerV0(dest));
        }

        public LogFormatBinaryWriter(byte binaryFormatVersion = ActualBinaryFormatVersion) :
            base(binaryFormatVersion) { }

        public void Write(Stream dest, SensorLogFormat logFormat)
        {
            Inner inner = appropriateInnerCreationFunc(dest);
            inner.Write(logFormat);
        }

        private class InnerV0 : Inner
        {
            public InnerV0(Stream dest) : base(dest) { }

            public override void Write(SensorLogFormat obj)
            {
                writer.Write(obj.Name);
                writer.Write((byte)obj.FieldCount);
                foreach (KeyValuePair<string, FieldType> fieldSignature in obj)
                    WriteFieldSignature(fieldSignature);
            }
            private void WriteFieldSignature(KeyValuePair<string, FieldType> signature)
            {
                writer.Write(signature.Key);
                writer.Write((byte)signature.Value);
            }
        }
    }
}