using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.Tests
{
    [TestClass]
    public class SensorIOLogFieldValuesTextTest
    {
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
            var parser = new SensorIO.FieldValueTextParser();
            Assert.AreEqual(uint8, parser.Parse(FieldType.UInt8, uint8.ToString()));
            Assert.AreEqual(int8, parser.Parse(FieldType.Int8, int8.ToString()));
            Assert.AreEqual(uint16, parser.Parse(FieldType.UInt16, uint16.ToString()));
            Assert.AreEqual(int16, parser.Parse(FieldType.Int16, int16.ToString()));
            Assert.AreEqual(uint32, parser.Parse(FieldType.UInt32, uint32.ToString()));
            Assert.AreEqual(int32, parser.Parse(FieldType.Int32, int32.ToString()));
            Assert.AreEqual(uint64, parser.Parse(FieldType.UInt64, uint64.ToString()));
            Assert.AreEqual(int64, parser.Parse(FieldType.Int64, int64.ToString()));
        }

        [TestMethod]
        public void TestReal()
        {
            const float f = 123.321f;
            const double d = 765321.123567;
            var parser = new SensorIO.FieldValueTextParser();
            var format = new System.Globalization.CultureInfo("en-UK");
            Assert.AreEqual(f, parser.Parse(FieldType.Float, f.ToString(format)));
            Assert.AreEqual(d, parser.Parse(FieldType.Double, d.ToString(format)));
        }

        [TestMethod]
        public void TestString()
        {
            const string str = "ÀáÂ¸~QwEr";
            var parser = new SensorIO.FieldValueTextParser();
            Assert.AreEqual(str, parser.Parse(FieldType.String, str));
        }

        [TestMethod]
        public void TestBoolean()
        {
            var parser = new SensorIO.FieldValueTextParser();
            Assert.AreEqual(false, parser.Parse(FieldType.Boolean, "0"));
            Assert.AreEqual(true, parser.Parse(FieldType.Boolean, "1"));
        }

        [TestMethod]
        public void TestDateAndTime()
        {
            DateTime dt = new DateTime(2021, 7, 25, 17, 23, 27);
            Date date = new Date(dt);
            Time time = new Time(dt);
            var parser = new SensorIO.FieldValueTextParser();
            string strDate = $"{date.year}-{date.month}-{date.day}";
            string strTime = $"{time.hour}:{time.minute}:{time.second}";
            string strDT = $"{strDate} {strTime}";
            Assert.AreEqual(dt, parser.Parse(FieldType.DateTime, strDT));
            Assert.AreEqual(date, parser.Parse(FieldType.Date, strDate));
            Assert.AreEqual(time, parser.Parse(FieldType.Time, strTime));
        }
    }
}