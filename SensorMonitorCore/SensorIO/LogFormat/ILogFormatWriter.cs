using System;
using System.IO;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.LogFormat
{
    public interface ILogFormatWriter
    {
        void Write(Stream dest, SensorLogFormat logFormat);
    }
}