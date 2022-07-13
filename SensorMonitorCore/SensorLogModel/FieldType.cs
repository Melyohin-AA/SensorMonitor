using System;

namespace SensorMonitor.Core.SensorLogModel
{
    public enum FieldType
    {
        UInt64 = 0,
        Int64 = 1,
        UInt32 = 2,
        Int32 = 3,
        UInt16 = 4,
        Int16 = 5,
        UInt8 = 6,
        Int8 = 7,

        Float = 8,
        Double = 9,

        String = 10,
        Boolean = 11,

        DateTime = 12,
        Date = 13,
        Time = 14,
    }

    public static class FieldTypeExtension
    {
        public static Type GetAppropriateDataType(this FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.UInt64: return typeof(ulong);
                case FieldType.Int64: return typeof(long);
                case FieldType.UInt32: return typeof(uint);
                case FieldType.Int32: return typeof(int);
                case FieldType.UInt16: return typeof(ushort);
                case FieldType.Int16: return typeof(short);
                case FieldType.UInt8: return typeof(byte);
                case FieldType.Int8: return typeof(sbyte);

                case FieldType.Float: return typeof(float);
                case FieldType.Double: return typeof(double);

                case FieldType.String: return typeof(string);
                case FieldType.Boolean: return typeof(bool);

                case FieldType.DateTime: return typeof(DateTime);
                case FieldType.Date: return typeof(Date);
                case FieldType.Time: return typeof(Time);
            }
            throw new Exception("Undefined case for specified FieldType!");
        }

        public static bool IsComparable(this FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.UInt64:
                case FieldType.Int64:
                case FieldType.UInt32:
                case FieldType.Int32:
                case FieldType.UInt16:
                case FieldType.Int16:
                case FieldType.UInt8:
                case FieldType.Int8:
                case FieldType.Float:
                case FieldType.Double:
                case FieldType.Boolean:
                    return true;
            }
            return false;
        }
        public static sbyte CompareFieldValues(this FieldType fieldType, object a, object b)
        {
            if ((a == null) || (b == null)) throw new ArgumentNullException();
            if (!fieldType.IsComparable()) throw new ArgumentException();
            Type type = fieldType.GetAppropriateDataType();
            if ((a.GetType() != type) || (b.GetType() != type)) throw new ArgumentException();
            double da = Convert.ToDouble(a), db = Convert.ToDouble(b);
            return (sbyte)Math.Sign(da - db);
        }

        public static string ToString(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.UInt64: return "UInt64";
                case FieldType.Int64: return "Int64";
                case FieldType.UInt32: return "UInt32";
                case FieldType.Int32: return "Int32";
                case FieldType.UInt16: return "UInt16";
                case FieldType.Int16: return "Int16";
                case FieldType.UInt8: return "UInt8";
                case FieldType.Int8: return "Int8";

                case FieldType.Float: return "Float";
                case FieldType.Double: return "Double";

                case FieldType.String: return "String";
                case FieldType.Boolean: return "Boolean";

                case FieldType.DateTime: return "DateTime";
                case FieldType.Date: return "Date";
                case FieldType.Time: return "Time";
            }
            throw new Exception();
        }
        public static FieldType FromString(string strFieldType)
        {
            switch (strFieldType)
            {
                case "UInt64": return FieldType.UInt64;
                case "Int64": return FieldType.Int64;
                case "UInt32": return FieldType.UInt32;
                case "Int32": return FieldType.Int32;
                case "UInt16": return FieldType.UInt16;
                case "Int16": return FieldType.Int16;
                case "UInt8": return FieldType.UInt8;
                case "Int8": return FieldType.Int8;

                case "Float": return FieldType.Float;
                case "Double": return FieldType.Double;

                case "String": return FieldType.String;
                case "Boolean": return FieldType.Boolean;

                case "DateTime": return FieldType.DateTime;
                case "Date": return FieldType.Date;
                case "Time": return FieldType.Time;
            }
            throw new Exception();
        }
    }
}