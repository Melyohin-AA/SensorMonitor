using System;
using System.Collections.Generic;

namespace SensorMonitor.Core.Analysis.Transform
{
    public class PeriodAverageTransformer : IValueSeqTransformer
    {
        private long logPeriod;

        public PeriodAverageTransformer(long logPeriod)
        {
            if (logPeriod < 0) throw new ArgumentOutOfRangeException();
            this.logPeriod = logPeriod;
        }

        public IEnumerable<ChronoValue> Transform(IEnumerable<ChronoValue> source)
        {
            Inner inner = new Inner(this, source);
            return inner.Transform();
        }

        private class Inner
        {
            private PeriodAverageTransformer host;
            private IEnumerator<ChronoValue> values;
            private double avgValue = 0.0;
            private long periodOrigin = 0;
            private uint logsInPeriod = 0;

            public Inner(PeriodAverageTransformer host, IEnumerable<ChronoValue> source)
            {
                this.host = host;
                values = source.GetEnumerator();
            }

            public IEnumerable<ChronoValue> Transform()
            {
                bool atEnd;
                reset:
                ResetVars();
                read:
                UpdateVars();
                atEnd = !values.MoveNext();
                if (atEnd || IsToCompleteResultValue())
                {
                    yield return new ChronoValue(avgValue, periodOrigin);
                    if (!atEnd) goto reset;
                }
                if (!atEnd) goto read;
            }
            private void ResetVars()
            {
                avgValue = 0.0;
                logsInPeriod = 0;
                periodOrigin = values.Current.ticks;
            }
            private void UpdateVars()
            {
                logsInPeriod++;
                double part = 1.0 / logsInPeriod;
                avgValue = (1.0 - part) * avgValue + part * values.Current.value;
            }
            private bool IsToCompleteResultValue()
            {
                return values.Current.ticks >= periodOrigin + host.logPeriod;
            }
        }
    }
}