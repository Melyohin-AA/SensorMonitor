using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Tests
{
    [TestClass]
    public class SensorIOLogFieldsBinaryTest
    {
        [TestMethod]
        public void TestBinaryLogFields()
        {
            SensorLogFormatTable table = TestHelper.CreateTable();
            var stream = new System.IO.MemoryStream();
            var writer = new SensorIO.Log.Binary.LogFieldsBinaryWriter(new SensorIO.FieldValueBinaryWriter());
            var reader = new SensorIO.Log.Binary.LogFieldsBinaryReader(new SensorIO.FieldValueBinaryReader());
            SensorLogInfo info = new SensorLogInfo(new DateTime(), "01");
            SensorLogFormat format = table["Hydra-L"];
            SensorLog sourceLog = new SensorLog(info, format);
            sourceLog["BME280_temp"] = 1f;
            sourceLog["BME280_humidity"] = 2f;
            sourceLog["BME280_pressure"] = 3f;
            writer.WriteFields(stream, sourceLog);
            stream.Position = 0;
            SensorLog.IFieldInitializer initializer = reader.Read(stream, format);
            SensorLog resultLog = new SensorLog(info, format, initializer);
            Assert.AreEqual(sourceLog.FieldCount, resultLog.FieldCount);
            foreach (KeyValuePair<string, FieldType> pair in format)
            {
                object sourceFieldValue = sourceLog[pair.Key];
                object resultFieldValue = resultLog[pair.Key];
                Assert.AreEqual(sourceFieldValue, resultFieldValue);
            }
        }
    }
}