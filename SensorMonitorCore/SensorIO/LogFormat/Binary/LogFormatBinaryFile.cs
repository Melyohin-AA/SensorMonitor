using System;
using System.IO;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.LogFormat.Binary
{
    public class LogFormatBinaryFile : SensorIOFile
    {
        private byte actualLogFormatBinaryFormatVersion;
        private Func<byte, ILogFormatReader> logFormatReaderCreationFunc;
        private Func<byte, ILogFormatWriter> logFormatWriterCreationFunc;

        public LogFormatBinaryFile(string path, byte actualLogFormatBinaryFormatVersion,
            Func<byte, ILogFormatReader> logFormatReaderCreationFunc,
            Func<byte, ILogFormatWriter> logFormatWriterCreationFunc) : base(path)
        {
            if ((logFormatReaderCreationFunc == null) || (logFormatWriterCreationFunc == null))
                throw new ArgumentNullException();
            this.actualLogFormatBinaryFormatVersion = actualLogFormatBinaryFormatVersion;
            this.logFormatReaderCreationFunc = logFormatReaderCreationFunc;
            this.logFormatWriterCreationFunc = logFormatWriterCreationFunc;
        }

        public SensorLogFormatTable ReadAll()
        {
            CheckIfIsOpened();
            BinaryReader reader = GetReader();
            File.Position = 0;
            byte logBinaryFormatVersion = reader.ReadByte();
            ILogFormatReader logFormatReader = logFormatReaderCreationFunc(logBinaryFormatVersion);
            byte logFormatCount = reader.ReadByte();
            SensorLogFormat[] logFormats = new SensorLogFormat[logFormatCount];
            for (short i = 0; i < logFormatCount; i++)
                logFormats[i] = logFormatReader.Read(File);
            return new SensorLogFormatTable(logFormats);
        }

        public void WriteAll(SensorLogFormatTable table)
        {
            CheckIfIsOpened();
            ILogFormatWriter logFormatWriter = logFormatWriterCreationFunc(actualLogFormatBinaryFormatVersion);
            BinaryWriter writer = GetWriter();
            File.Position = 0;
            writer.Write(actualLogFormatBinaryFormatVersion);
            writer.Write((byte)table.LogFormatCount);
            foreach (SensorLogFormat format in table)
                logFormatWriter.Write(File, format);
            File.SetLength(File.Position);
        }

        public void Append(SensorLogFormat newFormat)
        {
            if (newFormat == null) throw new ArgumentNullException();
            Append(new[] { newFormat });
        }
        public void Append(SensorLogFormat[] newFormats)
        {
            if (newFormats == null) throw new ArgumentNullException();
            CheckIfIsOpened();
            BinaryReader reader = GetReader();
            File.Position = 0;
            byte logBinaryFormatVersion = reader.ReadByte();
            ILogFormatWriter logFormatWriter = logFormatWriterCreationFunc(logBinaryFormatVersion);
            byte logFormatCount = reader.ReadByte();
            if (logFormatCount + newFormats.Length > 255) throw new OverflowException("Too much sensor formats!");
            BinaryWriter writer = GetWriter();
            File.Position--;
            logFormatCount += (byte)newFormats.Length;
            writer.Write(logFormatCount);
            File.Position = File.Length;
            foreach (SensorLogFormat format in newFormats)
                logFormatWriter.Write(File, format);
        }
    }
}