using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Tests
{
    [TestClass]
    public class SensorIOLogFormatBinaryTest
    {
        [TestMethod]
        public void TestBinaryLogFormat()
        {
            SensorLogFormatTable table = TestHelper.CreateTable();
            SensorLogFormat sourceFormat = table["Hydra-L"];
            var stream = new System.IO.MemoryStream();
            var writer = new SensorIO.LogFormat.Binary.LogFormatBinaryWriter();
            var reader = new SensorIO.LogFormat.Binary.LogFormatBinaryReader();
            writer.Write(stream, sourceFormat);
            stream.Position = 0;
            SensorLogFormat resultFormat = reader.Read(stream);
            Assert.AreEqual(sourceFormat.Name, resultFormat.Name);
            Assert.AreEqual(sourceFormat.FieldCount, resultFormat.FieldCount);
            foreach (KeyValuePair<string, FieldType> pair in sourceFormat)
                Assert.AreEqual(pair.Value, resultFormat[pair.Key]);
        }
    }
}