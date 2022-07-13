using System;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Analysis
{
    public class ValueRange
    {
        public double LowerBorder { get; }
        public double UpperBorder { get; }

        public ValueRange(double lowerBorder, double upperBorder)
        {
            if (lowerBorder > upperBorder) throw new ArgumentOutOfRangeException();
            LowerBorder = lowerBorder;
            UpperBorder = upperBorder;
        }

        public bool ContainsValue(double value)
        {
            return (value >= LowerBorder) && (value <= UpperBorder);
        }
        public bool IsSubrangeOf(ValueRange range)
        {
            return range.ContainsValue(LowerBorder) && range.ContainsValue(UpperBorder);
        }
    }
}