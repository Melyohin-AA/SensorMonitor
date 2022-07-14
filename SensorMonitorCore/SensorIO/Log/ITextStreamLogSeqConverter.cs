using System;
using System.IO;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log
{
    public interface ITextStreamLogSeqConverter
    {
        IEnumerable<SensorLog> Convert(TextReader textLogSeqReader);
    }

    public static class ITextStreamLogSeqConverterExtension
    {
        public static IEnumerable<SensorLog> Convert(this ITextStreamLogSeqConverter converter, string textLogSeq)
        {
            if (textLogSeq == null) throw new ArgumentNullException();
            return converter.Convert(new StringReader(textLogSeq));
        }
    }
}