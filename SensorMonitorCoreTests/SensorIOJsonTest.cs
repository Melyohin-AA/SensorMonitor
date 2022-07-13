using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Tests
{
    [TestClass]
    public class SensorIOJsonTest
    {
        private static SensorIO.Log.Json.JsonLogConverter CreateLogConverter()
        {
            SensorLogFormatTable table = TestHelper.CreateTable();
            var jsonParser = new SensorIO.Log.Json.JsonParser();
            var fieldValueParser = new SensorIO.FieldValueTextParser();
            return new SensorIO.Log.Json.JsonLogConverter(jsonParser, fieldValueParser, table);
        }

        [TestMethod]
        public void TestLogConversion()
        {
            const string json = "{\"Date\":\"2021-07-25 17:29:26\",\"uName\":\"Hydra-L\"," +
                "\"serial\":\"08\",\"data\":{\"system_Serial\":\"8\",\"system_Version\":\"2021-02-24\"," +
                "\"system_RSSI\":\"-74\",\"system_MAC\":\"68:C6:3A:F8:0F:02\",\"system_IP\":\"10.200.1.27\"," +
                "\"BME280_temp\":\"31.48\",\"BME280_humidity\":\"31.92\",\"BME280_pressure\":\"749.28\"}";
            var logConverter = CreateLogConverter();
            SensorLog log = logConverter.Convert(json);
            Assert.AreEqual(new DateTime(2021, 7, 25, 17, 29, 26), log.Info.dt);
            Assert.AreEqual("Hydra-L", log.SensorName);
            Assert.AreEqual("08", log.Info.serial);
            Assert.AreEqual(3, log.FieldCount);
            Assert.AreEqual(31.48f, log["BME280_temp"]);
            Assert.AreEqual(31.92f, log["BME280_humidity"]);
            Assert.AreEqual(749.28f, log["BME280_pressure"]);
        }

        [TestMethod]
        public void TestLogSeqConversion()
        {
            const string json = "{\"0\":{\"Date\":\"2021-07-25 17:29:26\",\"uName\":\"Hydra-L\",\"serial\":\"08\"," +
                "\"data\":{\"system_Serial\":\"8\",\"system_Version\":\"2021-02-24\",\"system_RSSI\":\"-74\"," +
                "\"system_MAC\":\"68:C6:3A:F8:0F:02\",\"system_IP\":\"10.200.1.27\",\"BME280_temp\":\"31.48\"," +
                "\"BME280_humidity\":\"31.92\",\"BME280_pressure\":\"749.28\"}}," +
                "\"1\":{\"Date\":\"2021-07-25 17:28:27\",\"uName\":\"Hydra-L\",\"serial\":\"08\"," +
                "\"data\":{\"system_Serial\":\"8\",\"system_Version\":\"2021-02-24\",\"system_RSSI\":\"-74\"," +
                "\"system_MAC\":\"68:C6:3A:F8:0F:02\",\"system_IP\":\"10.200.1.27\",\"BME280_temp\":\"31.49\"," +
                "\"BME280_humidity\":\"31.91\",\"BME280_pressure\":\"749.24\"}}," +
                "\"2\":{\"Date\":\"2021-07-25 17:26:26\",\"uName\":\"Hydra-L\",\"serial\":\"08\"," +
                "\"data\":{\"system_Serial\":\"8\",\"system_Version\":\"2021-02-24\",\"system_RSSI\":\"-74\"," +
                "\"system_MAC\":\"68:C6:3A:F8:0F:02\",\"system_IP\":\"10.200.1.27\",\"BME280_temp\":\"31.48\"," +
                "\"BME280_humidity\":\"31.92\",\"BME280_pressure\":\"749.28\"}}," +
                "\"3\":{\"Date\":\"2021-07-25 17:25:26\",\"uName\":\"Hydra-L\",\"serial\":\"08\"," +
                "\"data\":{\"system_Serial\":\"8\",\"system_Version\":\"2021-02-24\",\"system_RSSI\":\"-74\"," +
                "\"system_MAC\":\"68:C6:3A:F8:0F:02\",\"system_IP\":\"10.200.1.27\",\"BME280_temp\":\"31.5\"," +
                "\"BME280_humidity\":\"31.91\",\"BME280_pressure\":\"749.29\"}}," +
                "\"4\":{\"Date\":\"2021-07-25 17:27:26\",\"uName\":\"Hydra-L\",\"serial\":\"08\"," +
                "\"data\":{\"system_Serial\":\"8\",\"system_Version\":\"2021-02-24\",\"system_RSSI\":\"-74\"," +
                "\"system_MAC\":\"68:C6:3A:F8:0F:02\",\"system_IP\":\"10.200.1.27\",\"BME280_temp\":\"31.49\"," +
                "\"BME280_humidity\":\"31.91\",\"BME280_pressure\":\"749.27\"}}}";
            var logConverter = CreateLogConverter();
            var logSeqConverter = new SensorIO.Log.Json.JsonLogSeqConverter(logConverter);
            var logs = SensorIO.Log.ITextStreamLogSeqConverterExtension.Convert(logSeqConverter, json);
            byte count = 0;
            foreach (SensorLog log in logs)
            {
                count++;
                Assert.AreEqual("Hydra-L", log.SensorName);
                Assert.AreEqual("08", log.Info.serial);
                Assert.AreEqual(3, log.FieldCount);
            }
            Assert.AreEqual((byte)5, count);
        }
    }
}