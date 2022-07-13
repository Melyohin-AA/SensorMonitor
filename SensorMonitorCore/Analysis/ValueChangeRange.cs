using System;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Analysis
{
    public class ValueChangeRange
    {
        public ValueChange LowerBorder { get; }
        public ValueChange UpperBorder { get; }

        public ValueChangeRange(ValueChange lowerBorder, ValueChange upperBorder)
        {
            if (lowerBorder.Tangent > upperBorder.Tangent) throw new ArgumentOutOfRangeException();
            LowerBorder = lowerBorder;
            UpperBorder = upperBorder;
        }

        public bool ContainsValueChange(ValueChange valueChange)
        {
            double tan = valueChange.Tangent;
            return (tan >= LowerBorder.Tangent) && (tan <= UpperBorder.Tangent);
        }
        public bool IsSubrangeOf(ValueChangeRange range)
        {
            return range.ContainsValueChange(LowerBorder) && range.ContainsValueChange(UpperBorder);
        }

        public struct ValueChange
        {
            public readonly double changeValue;
            public readonly long tickDelta;

            public double Tangent { get { return changeValue / tickDelta; } }

            public ValueChange(double changeValue, long tickDelta)
            {
                if (tickDelta <= 0) throw new ArgumentOutOfRangeException();
                this.changeValue = changeValue;
                this.tickDelta = tickDelta;
            }
        }
    }
}