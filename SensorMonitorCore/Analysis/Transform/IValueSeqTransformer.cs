using System;
using System.Collections.Generic;

namespace SensorMonitor.Core.Analysis.Transform
{
    public interface IValueSeqTransformer
    {
        IEnumerable<ChronoValue> Transform(IEnumerable<ChronoValue> source);
    }
}