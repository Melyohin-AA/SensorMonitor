using System;
using System.IO;

namespace SensorMonitor.Core.SensorIO
{
    public abstract class SensorIOFile
    {
        protected FileStream File { get; private set; }

        public string Path { get; }

        public bool IsOpened { get { return File != null; } }
        public bool IsClosed { get { return File == null; } }
        public bool IsEmpty { get { return File.Length == 0; } }

        protected SensorIOFile(string path)
        {
            Path = path ?? throw new ArgumentNullException();
        }
        ~SensorIOFile()
        {
            if (IsOpened) Close();
        }

        public virtual void Open()
        {
            CheckIfIsClosed();
            File = new FileStream(Path, FileMode.OpenOrCreate);
            File.Position = 0;
        }
        public virtual void Close()
        {
            CheckIfIsOpened();
            File.Close();
            File = null;
        }

        protected BinaryReader GetReader()
        {
            return new BinaryReader(File, System.Text.Encoding.UTF8);
        }
        protected BinaryWriter GetWriter()
        {
            return new BinaryWriter(File, System.Text.Encoding.UTF8);
        }

        protected void CheckIfIsOpened()
        {
            if (IsClosed) throw new InvalidOperationException();
        }
        protected void CheckIfIsClosed()
        {
            if (IsOpened) throw new InvalidOperationException();
        }
    }
}