using System;
using System.Collections.Generic;

namespace SensorMonitor.Core.Analysis
{
    public interface IValueChecker
    {
        ValueIssue CheckSingleValue(double value);
        ValueIssue CheckValueChange(ValueChangeRange.ValueChange valueChange);

        IEnumerable<ValueIssue> CheckValueSeq(IEnumerable<ChronoValue> values);
    }

    public class ValueChecker : IValueChecker
    {
        private ValueRange valueNormalRange, valuePossibleRange;
        private ValueChangeRange valueChangeNormalRange, valueChangePossibleRange;

        public ValueChecker(ValueRange valueNormalRange, ValueRange valuePossibleRange,
            ValueChangeRange valueChangeNormalRange, ValueChangeRange valueChangePossibleRange)
        {
            if (!valueNormalRange.IsSubrangeOf(valueNormalRange) ||
                !valueChangeNormalRange.IsSubrangeOf(valueChangePossibleRange))
                throw new ArgumentOutOfRangeException();
            this.valueNormalRange = valueNormalRange;
            this.valuePossibleRange = valuePossibleRange;
            this.valueChangeNormalRange = valueChangeNormalRange;
            this.valueChangePossibleRange = valueChangePossibleRange;
        }

        public ValueIssue CheckSingleValue(double value)
        {
            ValueIssue issue = 0;
            if (!valueNormalRange.ContainsValue(value))
            {
                issue |= ValueIssue.ValueOutOfNormalRange;
                if (!valuePossibleRange.ContainsValue(value))
                    issue |= ValueIssue.ValueOutOfPossibleRange;
            }
            return issue;
        }
        public ValueIssue CheckValueChange(ValueChangeRange.ValueChange valueChange)
        {
            ValueIssue issue = 0;
            if (!valueChangeNormalRange.ContainsValueChange(valueChange))
            {
                issue |= ValueIssue.ValueChangeOutOfNormalRange;
                if (!valueChangePossibleRange.ContainsValueChange(valueChange))
                    issue |= ValueIssue.ValueChangeOutOfPossibleRange;
            }
            return issue;
        }

        public IEnumerable<ValueIssue> CheckValueSeq(IEnumerable<ChronoValue> values)
        {
            double prevValue = double.NaN;
            long prevTicks = 0L;
            foreach (ChronoValue chronoValue in values)
            {
                ValueIssue issue = CheckSingleValue(chronoValue.value);
                if (!double.IsNaN(prevValue))
                {
                    double changeValue = chronoValue.value - prevValue;
                    long tickDelta = chronoValue.ticks - prevTicks;
                    var change = new ValueChangeRange.ValueChange(changeValue, tickDelta);
                    issue |= CheckValueChange(change);
                }
                yield return issue;
                prevValue = chronoValue.value;
                prevTicks = chronoValue.ticks;
            }
        }
    }
}