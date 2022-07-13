using System;
using System.IO;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO
{
    public interface IFieldValueBinaryWriter
    {
        void Write(BinaryWriter dest, FieldType type, object value);
    }

    public class FieldValueBinaryWriter : IFieldValueBinaryWriter
    {
        private static Dictionary<FieldType, Action<BinaryWriter, object>> fieldWriters;

        static FieldValueBinaryWriter()
        {
            fieldWriters = new Dictionary<FieldType, Action<BinaryWriter, object>>()
            {
                { FieldType.UInt64, (dest, value) => dest.Write((ulong)value) },
                { FieldType.Int64, (dest, value) => dest.Write((long)value) },
                { FieldType.UInt32, (dest, value) => dest.Write((uint)value) },
                { FieldType.Int32, (dest, value) => dest.Write((int)value) },
                { FieldType.UInt16, (dest, value) => dest.Write((ushort)value) },
                { FieldType.Int16, (dest, value) => dest.Write((short)value) },
                { FieldType.UInt8, (dest, value) => dest.Write((byte)value) },
                { FieldType.Int8, (dest, value) => dest.Write((sbyte)value) },

                { FieldType.Float, (dest, value) => dest.Write((float)value) },
                { FieldType.Double, (dest, value) => dest.Write((double)value) },

                { FieldType.String, (dest, value) => dest.Write((string)value) },
                { FieldType.Boolean, (dest, value) => dest.Write((bool)value) },

                { FieldType.DateTime, WriteDateTime },
                { FieldType.Date, WriteDate },
                { FieldType.Time, WriteTime },
            };
        }

        private static void WriteDateTime(BinaryWriter dest, object value)
        {
            long dateData = ((DateTime)value).ToBinary();
            dest.Write(dateData);
        }
        private static void WriteDate(BinaryWriter dest, object value)
        {
            Date date = (Date)value;
            dest.Write(date.year);
            dest.Write(date.month);
            dest.Write(date.day);
        }
        private static void WriteTime(BinaryWriter dest, object value)
        {
            Time time = (Time)value;
            dest.Write(time.hour);
            dest.Write(time.minute);
            dest.Write(time.second);
        }

        public void Write(BinaryWriter dest, FieldType type, object value)
        {
            fieldWriters[type](dest, value);
        }
    }
}