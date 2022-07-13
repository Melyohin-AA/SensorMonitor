using System;

namespace SensorMonitor.Core.Analysis
{
    public enum ValueIssue
    {
        ValueOutOfNormalRange = 1 << 0,
        ValueOutOfPossibleRange = 1 << 1,
        ValueChangeOutOfNormalRange = 1 << 2,
        ValueChangeOutOfPossibleRange = 1 << 3,
    }
}