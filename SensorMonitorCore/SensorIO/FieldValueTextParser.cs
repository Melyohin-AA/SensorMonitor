using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO
{
    public interface IFieldValueTextParser
    {
        object Parse(FieldType type, string strValue);
    }

    public class FieldValueTextParser : IFieldValueTextParser
    {
        private static Dictionary<FieldType, Func<string, object>> fieldParsers;

        static FieldValueTextParser()
        {
            fieldParsers = new Dictionary<FieldType, Func<string, object>>
            {
                { FieldType.UInt64, (strValue) => ulong.Parse(strValue) },
                { FieldType.Int64, (strValue) => long.Parse(strValue) },
                { FieldType.UInt32, (strValue) => uint.Parse(strValue) },
                { FieldType.Int32, (strValue) => int.Parse(strValue) },
                { FieldType.UInt16, (strValue) => ushort.Parse(strValue) },
                { FieldType.Int16, (strValue) => short.Parse(strValue) },
                { FieldType.UInt8, (strValue) => byte.Parse(strValue) },
                { FieldType.Int8, (strValue) => sbyte.Parse(strValue) },

                { FieldType.Float, ToFloat },
                { FieldType.Double, ToDouble },

                { FieldType.String, (strValue) => strValue },
                { FieldType.Boolean, (strValue) => strValue != "0" },

                { FieldType.DateTime, ToDateTime },
                { FieldType.Date, ToDate },
                { FieldType.Time, ToTime },
            };
        }

        private static object ToFloat(string strValue)
        {
            if (strValue == "none") return 0f;
            strValue = strValue.Replace("ni.", "");
            var provider = new System.Globalization.CultureInfo("en-UK");
            return float.Parse(strValue, System.Globalization.NumberStyles.Float, provider);
        }
        private static object ToDouble(string strValue)
        {
            if (strValue == "none") return 0.0;
            var provider = new System.Globalization.CultureInfo("en-UK");
            return double.Parse(strValue, System.Globalization.NumberStyles.Float, provider);
        }

        private static object ToDateTime(string strValue)
        {
            string[] dtPair = strValue.Split(' ');
            Date date = (Date)ToDate(dtPair[0]);
            Time time = (Time)ToTime(dtPair[1]);
            return date + time;
        }
        private static object ToDate(string strValue)
        {
            string[] ymd = strValue.Split('-');
            short year = short.Parse(ymd[0]);
            byte month = byte.Parse(ymd[1]);
            byte day = byte.Parse(ymd[2]);
            return new Date(year, month, day);
        }
        private static object ToTime(string strValue)
        {
            string[] hms = strValue.Split(':');
            byte hour = byte.Parse(hms[0]);
            byte minute = byte.Parse(hms[1]);
            byte second = byte.Parse(hms[2]);
            return new Time(hour, minute, second);
        }

        public object Parse(FieldType type, string strValue)
        {
            return fieldParsers[type](strValue);
        }
    }
}