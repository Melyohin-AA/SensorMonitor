using System;
using System.IO;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log
{
    public interface ILogFieldsWriter
    {
        void WriteFields(Stream dest, SensorLog log);
    }
}