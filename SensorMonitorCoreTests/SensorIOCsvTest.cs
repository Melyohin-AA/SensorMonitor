using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Tests
{
    [TestClass]
    public class SensorIOCsvTest
    {
        private static SensorIO.Log.Csv.CsvLogSeqConverter CreateLogSeqConverter()
        {
            SensorLogFormatTable table = TestHelper.CreateTable();
            var fieldValueParser = new SensorIO.FieldValueTextParser();
            return new SensorIO.Log.Csv.CsvLogSeqConverter(table, fieldValueParser);
        }

        [TestMethod]
        public void TestLogSeqConversion()
        {
            const string csv = "Прибор: ;Hydra-L (01); ; Интервал: ;2021-01-01 10:05:00; / ;2021-01-31 10:05:00;\n" +
                "Date;BME280_temp;BME280_pressure;BME280_humidity;\n" +
                "2021-01-01 10:05:34;24.91;747.9;25.11;\n" +
                "2021-01-01 10:06:34;24.92;747.92;25.12;\n" +
                "2021-01-01 10:07:34;24.91;747.9;25.1;\n" +
                "2021-01-01 10:08:34;24.91;747.9;25.12;\n" +
                "2021-01-01 10:09:34;24.91;747.88;25.13;\n";
            var logSeqConverter = CreateLogSeqConverter();
            var logs = SensorIO.Log.ITextStreamLogSeqConverterExtension.Convert(logSeqConverter, csv);
            byte count = 0;
            foreach (SensorLog log in logs)
            {
                count++;
                Assert.AreEqual("Hydra-L", log.SensorName);
                Assert.AreEqual("01", log.Info.serial);
                Assert.AreEqual(3, log.FieldCount);
            }
            Assert.AreEqual((byte)5, count);
        }
    }
}