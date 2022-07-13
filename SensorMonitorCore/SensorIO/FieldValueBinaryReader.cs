using System;
using System.IO;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO
{
    public interface IFieldValueBinaryReader
    {
        object Read(BinaryReader source, FieldType type);
    }

    public class FieldValueBinaryReader : IFieldValueBinaryReader
    {
        private static Dictionary<FieldType, Func<BinaryReader, object>> fieldReaders;

        static FieldValueBinaryReader()
        {
            fieldReaders = new Dictionary<FieldType, Func<BinaryReader, object>>()
            {
                { FieldType.UInt64, (source) => source.ReadUInt64() },
                { FieldType.Int64, (source) => source.ReadInt64() },
                { FieldType.UInt32, (source) => source.ReadUInt32() },
                { FieldType.Int32, (source) => source.ReadInt32() },
                { FieldType.UInt16, (source) => source.ReadUInt16() },
                { FieldType.Int16, (source) => source.ReadInt16() },
                { FieldType.UInt8, (source) => source.ReadByte() },
                { FieldType.Int8, (source) => source.ReadSByte() },

                { FieldType.Float, (source) => source.ReadSingle() },
                { FieldType.Double, (source) => source.ReadDouble() },

                { FieldType.String, (source) => source.ReadString() },
                { FieldType.Boolean, (source) => source.ReadBoolean() },

                { FieldType.DateTime, ReadDateTime },
                { FieldType.Date, ReadDate },
                { FieldType.Time, ReadTime },
            };
        }

        private static object ReadDateTime(BinaryReader source)
        {
            long dateData = source.ReadInt64();
            return DateTime.FromBinary(dateData);
        }
        private static object ReadDate(BinaryReader source)
        {
            short year = source.ReadInt16();
            byte month = source.ReadByte();
            byte day = source.ReadByte();
            return new Date(year, month, day);
        }
        private static object ReadTime(BinaryReader source)
        {
            byte hour = source.ReadByte();
            byte minute = source.ReadByte();
            byte second = source.ReadByte();
            return new Time(hour, minute, second);
        }

        public object Read(BinaryReader source, FieldType type)
        {
            return fieldReaders[type](source);
        }
    }
}