using System;

namespace SensorMonitor.Core.SensorLogModel
{
    public struct SensorIdentifier
    {
        public readonly SensorLogFormat logFormat;
        public readonly string serial;

        public SensorIdentifier(SensorLogFormat logFormat, string serial)
        {
            this.logFormat = logFormat ?? throw new ArgumentNullException();
            this.serial = serial ?? throw new ArgumentNullException();
        }
        public SensorIdentifier(string nameAndSerial, SensorLogFormatTable table)
        {
            if ((nameAndSerial == null) || (table == null)) throw new ArgumentNullException();
            int serialOrigin = nameAndSerial.LastIndexOf('(') + 1;
            int serialEnd = nameAndSerial.LastIndexOf(')');
            serial = nameAndSerial.Substring(serialOrigin, serialEnd - serialOrigin);
            int sensorNameLength = serialOrigin - 2;
            string sensorName = nameAndSerial.Remove(sensorNameLength);
            logFormat = table[sensorName];
        }

        public override string ToString()
        {
            return $"{logFormat.Name} ({serial})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) throw new ArgumentNullException();
            if (!(obj is SensorIdentifier)) throw new ArgumentException();
            SensorIdentifier opp = (SensorIdentifier)obj;
            return this == opp;
        }
        public override int GetHashCode()
        {
            return logFormat.GetHashCode() ^ serial.GetHashCode();
        }

        public static bool operator ==(SensorIdentifier a, SensorIdentifier b)
        {
            return (a.logFormat == b.logFormat) && (a.serial == b.serial);
        }
        public static bool operator !=(SensorIdentifier a, SensorIdentifier b)
        {
            return !(a == b);
        }
    }
}