using System;

namespace SensorMonitor.Core.SensorLogModel
{
    public struct SensorLogInfo
    {
        public readonly DateTime dt;
        public readonly string serial;

        public SensorLogInfo(DateTime dt, string serial)
        {
            this.dt = dt;
            this.serial = serial ?? throw new ArgumentNullException();
        }
    }
}