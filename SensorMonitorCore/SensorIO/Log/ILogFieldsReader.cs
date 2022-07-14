using System;
using System.IO;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log
{
    public interface ILogFieldsReader
    {
        SensorLog.IFieldInitializer Read(Stream source, SensorLogFormat format);
    }
}