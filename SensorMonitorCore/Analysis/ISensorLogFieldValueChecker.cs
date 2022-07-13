using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Analysis
{
    public interface ISensorLogFieldValueChecker
    {
        FieldType ValuesType { get; }
        SensorLogFieldPath ValuesField { get; }

        ValueIssue CheckSingleLog(SensorLog log);
        IEnumerable<ValueIssue> CheckLogSeq(IEnumerable<SensorLog> logs);
    }
}