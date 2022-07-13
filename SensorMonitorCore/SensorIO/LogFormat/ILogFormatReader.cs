using System;
using System.IO;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.LogFormat
{
    public interface ILogFormatReader
    {
        SensorLogFormat Read(Stream source);
    }
}