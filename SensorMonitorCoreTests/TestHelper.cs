using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Tests
{
    static class TestHelper
    {
        public static SensorLogFormatTable CreateTable()
        {
            return new SensorLogFormatTable(new[]
            {
                new SensorLogFormat("Hydra-L", new Dictionary<string, FieldType>()
                {
                    { "BME280_temp", FieldType.Float },
                    { "BME280_humidity", FieldType.Float },
                    { "BME280_pressure", FieldType.Float },
                }),
            });
        }
    }
}