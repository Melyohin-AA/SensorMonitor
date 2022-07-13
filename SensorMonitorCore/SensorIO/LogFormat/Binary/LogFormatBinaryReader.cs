using System;
using System.IO;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.LogFormat.Binary
{
    public class LogFormatBinaryReader : Tools.BinaryIOFormat.BinaryFormatReader<SensorLogFormat>, ILogFormatReader
    {
        public const byte ActualBinaryFormatVersion = 0;

        static LogFormatBinaryReader()
        {
            AddInnerCreationFunc(0, (source) => new InnerV0(source));
        }

        public LogFormatBinaryReader(byte binaryFormatVersion = ActualBinaryFormatVersion) :
            base(binaryFormatVersion) { }

        public SensorLogFormat Read(Stream source)
        {
            Inner inner = appropriateInnerCreationFunc(source);
            return inner.Read();
        }

        private class InnerV0 : Inner
        {
            private Dictionary<string, FieldType> fieldTypes;

            public InnerV0(Stream source) : base(source) { }

            public override SensorLogFormat Read()
            {
                string name = reader.ReadString();
                byte fieldCount = reader.ReadByte();
                fieldTypes = new Dictionary<string, FieldType>(fieldCount);
                for (short i = 0; i < fieldCount; i++)
                    ReadFieldSignature();
                return new SensorLogFormat(name, fieldTypes);
            }
            private void ReadFieldSignature()
            {
                string name = reader.ReadString();
                FieldType type = (FieldType)reader.ReadByte();
                fieldTypes.Add(name, type);
            }
        }
    }
}