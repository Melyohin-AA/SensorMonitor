using System;

namespace SensorMonitor.Core.SensorLogModel
{
    public struct Date
    {
        public readonly short year;
        public readonly byte month;
        public readonly byte day;

        public DateTime AsDateTime { get { return new DateTime(year, month, day); } }

        public Date(short year, byte month, byte day)
        {
            new DateTime(year, month, day);
            this.year = year;
            this.month = month;
            this.day = day;
        }
        public Date(DateTime dt)
        {
            year = (short)dt.Year;
            month = (byte)dt.Month;
            day = (byte)dt.Day;
        }

        public static DateTime operator +(Date date, Time time)
        {
            return new DateTime(date.year, date.month, date.day, time.hour, time.minute, time.second);
        }
    }

    public struct Time
    {
        public readonly byte hour;
        public readonly byte minute;
        public readonly byte second;

        public DateTime AsDateTime { get { return new DateTime(0, 0, 0, hour, minute, second); } }

        public Time(byte hour, byte minute, byte second)
        {
            if ((hour > 23) || (minute > 59) || (second > 59)) throw new ArgumentOutOfRangeException();
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }
        public Time(DateTime dt)
        {
            hour = (byte)dt.Hour;
            minute = (byte)dt.Minute;
            second = (byte)dt.Second;
        }

        public static DateTime operator +(Time time, Date date)
        {
            return date + time;
        }
    }
}
