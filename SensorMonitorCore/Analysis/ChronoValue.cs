using System;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Analysis
{
    public struct ChronoValue
    {
        public readonly double value;
        public readonly long ticks;

        public ChronoValue(double value, long ticks)
        {
            if (ticks < 0) throw new ArgumentOutOfRangeException();
            this.value = value;
            this.ticks = ticks;
        }
        public ChronoValue(object value, DateTime dt)
        {
            this.value = Convert.ToDouble(value);
            ticks = dt.Ticks;
        }
        public ChronoValue(SensorLog log, string fieldName)
        {
            value = Convert.ToDouble(log[fieldName]);
            ticks = log.Info.dt.Ticks;
        }
    }
}