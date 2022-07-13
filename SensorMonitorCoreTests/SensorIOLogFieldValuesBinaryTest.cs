using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Tests
{
    [TestClass]
    public class SensorIOLogFieldValuesBinaryTest
    {
        private static void TestWR(Tuple<object, FieldType>[] sourceValues)
        {
            var writer = new SensorIO.FieldValueBinaryWriter();
            var stream = new System.IO.MemoryStream();
            var binaryWriter = new System.IO.BinaryWriter(stream);
            foreach (var pair in sourceValues)
                writer.Write(binaryWriter, pair.Item2, pair.Item1);
            stream.Position = 0;
            var reader = new SensorIO.FieldValueBinaryReader();
            var binaryReader = new System.IO.BinaryReader(stream);
            foreach (var pair in sourceValues)
                Assert.AreEqual(pair.Item1, reader.Read(binaryReader, pair.Item2));
        }

        [TestMethod]
        public void TestInteger()
        {
            const byte uint8 = 227;
            const sbyte int8 = -123;
            const ushort uint16 = 65535;
            const short int16 = -30123;
            const uint uint32 = 4012300321;
            const int int32 = -2012300321;
            const ulong uint64 = 0xFFEEDDCCBBAA9988;
            const long int64 = -0x7FEEDDCCBBAA9988;
            TestWR(new[]
            {
                new Tuple<object, FieldType>(uint8, FieldType.UInt8),
                new Tuple<object, FieldType>(int8, FieldType.Int8),
                new Tuple<object, FieldType>(uint16, FieldType.UInt16),
                new Tuple<object, FieldType>(int16, FieldType.Int16),
                new Tuple<object, FieldType>(uint32, FieldType.UInt32),
                new Tuple<object, FieldType>(int32, FieldType.Int32),
                new Tuple<object, FieldType>(uint64, FieldType.UInt64),
                new Tuple<object, FieldType>(int64, FieldType.Int64),
            });
        }

        [TestMethod]
        public void TestReal()
        {
            const float f = 123.321f;
            const double d = 765321.123567;
            TestWR(new[]
            {
                new Tuple<object, FieldType>(f, FieldType.Float),
                new Tuple<object, FieldType>(d, FieldType.Double),
            });
        }

        [TestMethod]
        public void TestString()
        {
            const string str = "АбВё~QwEr";
            TestWR(new[] { new Tuple<object, FieldType>(str, FieldType.String) });
        }

        [TestMethod]
        public void TestBoolean()
        {
            TestWR(new[]
            {
                new Tuple<object, FieldType>(false, FieldType.Boolean),
                new Tuple<object, FieldType>(true, FieldType.Boolean),
            });
        }

        [TestMethod]
        public void TestDateAndTime()
        {
            DateTime dt = new DateTime(2021, 7, 25, 17, 23, 27);
            Date date = new Date(dt);
            Time time = new Time(dt);
            TestWR(new[]
            {
                new Tuple<object, FieldType>(dt, FieldType.DateTime),
                new Tuple<object, FieldType>(date, FieldType.Date),
                new Tuple<object, FieldType>(time, FieldType.Time),
            });
        }
    }
}