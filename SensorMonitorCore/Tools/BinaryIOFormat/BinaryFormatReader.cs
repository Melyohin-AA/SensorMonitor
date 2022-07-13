using System;
using System.IO;
using System.Collections.Generic;

namespace SensorMonitor.Core.Tools.BinaryIOFormat
{
    public abstract class BinaryFormatReader<T>
    {
        private static Dictionary<byte, Func<Stream, Inner>> innerCreationFuncs =
            new Dictionary<byte, Func<Stream, Inner>>();

        protected static void AddInnerCreationFunc(byte binaryFormatVersion,
            Func<Stream, Inner> innerCreationFunc)
        {
            innerCreationFuncs.Add(binaryFormatVersion, innerCreationFunc);
        }

        public byte BinaryFormatVersion { get; }
        protected readonly Func<Stream, Inner> appropriateInnerCreationFunc;

        protected BinaryFormatReader(byte binaryFormatVersion)
        {
            BinaryFormatVersion = binaryFormatVersion;
            appropriateInnerCreationFunc = innerCreationFuncs[binaryFormatVersion];
        }

        protected abstract class Inner
        {
            protected BinaryReader reader;

            public Inner(Stream source)
            {
                if (source == null) throw new ArgumentNullException();
                reader = new BinaryReader(source, System.Text.Encoding.UTF8);
            }

            public abstract T Read();
        }
    }
}