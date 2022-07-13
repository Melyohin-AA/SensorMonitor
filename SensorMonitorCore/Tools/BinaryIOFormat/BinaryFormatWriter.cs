using System;
using System.IO;
using System.Collections.Generic;

namespace SensorMonitor.Core.Tools.BinaryIOFormat
{
    public abstract class BinaryFormatWriter<T>
    {
        private static Dictionary<byte, Func<Stream, Inner>> innerCreationFuncs =
            new Dictionary<byte, Func<Stream, Inner>>();

        protected static void AddInnerCreationFunc(byte binaryFormatVersion, Func<Stream, Inner> innerCreationFunc)
        {
            innerCreationFuncs.Add(binaryFormatVersion, innerCreationFunc);
        }

        public byte BinaryFormatVersion { get; }
        protected readonly Func<Stream, Inner> appropriateInnerCreationFunc;

        protected BinaryFormatWriter(byte binaryFormatVersion)
        {
            BinaryFormatVersion = binaryFormatVersion;
            appropriateInnerCreationFunc = innerCreationFuncs[binaryFormatVersion];
        }

        protected abstract class Inner
        {
            protected BinaryWriter writer;

            public Inner(Stream dest)
            {
                if (dest == null) throw new ArgumentNullException();
                writer = new BinaryWriter(dest, System.Text.Encoding.UTF8);
            }

            public abstract void Write(T obj);
        }
    }
}