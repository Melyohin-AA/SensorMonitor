using System;

namespace SensorMonitor.Core.SensorLogModel
{
    public struct SensorLogFieldPath
    {
        public readonly SensorIdentifier sensorId;
        public readonly string logFieldName;

        public SensorLogFieldPath(SensorIdentifier sensorId, string logFieldName)
        {
            this.sensorId = sensorId;
            this.logFieldName = logFieldName ?? throw new ArgumentNullException();
        }

        public override string ToString()
        {
            return $"{sensorId} : {logFieldName}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) throw new ArgumentNullException();
            if (!(obj is SensorLogFieldPath)) throw new ArgumentException();
            SensorLogFieldPath opp = (SensorLogFieldPath)obj;
            return this == opp;
        }
        public override int GetHashCode()
        {
            return sensorId.GetHashCode() ^ logFieldName.GetHashCode();
        }

        public static bool operator ==(SensorLogFieldPath a, SensorLogFieldPath b)
        {
            return (a.sensorId == b.sensorId) && (a.logFieldName == b.logFieldName);
        }
        public static bool operator !=(SensorLogFieldPath a, SensorLogFieldPath b)
        {
            return !(a == b);
        }
    }
}