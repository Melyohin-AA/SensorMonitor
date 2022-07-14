using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log.Json
{
    public class JsonLogSeqConverter : ITextStreamLogSeqConverter
    {
        private IJsonLogConverter logConverter;

        public JsonLogSeqConverter(IJsonLogConverter logConverter)
        {
            this.logConverter = logConverter ?? throw new ArgumentNullException();
        }

        public IEnumerable<SensorLog> Convert(TextReader textLogSeqReader)
        {
            Inner inner = new Inner(this, textLogSeqReader);
            inner.Convert();
            return inner.Result;
        }

        private class Inner
        {
            private JsonLogSeqConverter host;
            private TextReader reader;
            private List<SensorLog> logs = new List<SensorLog>();
            private int level = 1;
            private StringBuilder scope = new StringBuilder();

            public ReadOnlyCollection<SensorLog> Result { get; private set; }

            public Inner(JsonLogSeqConverter host, TextReader reader)
            {
                if ((host == null) || (reader == null)) throw new ArgumentNullException();
                this.host = host;
                this.reader = reader;
            }

            public void Convert()
            {
                SkipEntryBracket();
                while (level > 0)
                {
                    char symbol = (char)reader.Read();
                    HandleReadSymbol(symbol);
                }
                logs.TrimExcess();
                Result = logs.AsReadOnly();
            }
            private void SkipEntryBracket()
            {
                while ((char)reader.Read() != '{') ;
            }
            private void HandleReadSymbol(char symbol)
            {
                if (symbol == '{') level++;
                if (level > 1) scope.Append(symbol);
                if (symbol == '}')
                {
                    level--;
                    if (level == 1)
                    {
                        SensorLog log = TryConvertLog(scope.ToString());
                        if (log != null) logs.Add(log);
                        scope.Clear();
                    }
                }
            }
            private SensorLog TryConvertLog(string jsonLog)
            {
                try
                {
                    return host.logConverter.Convert(scope.ToString());
                }
                catch (KeyNotFoundException e)
                {
                    if (e.Message != "The given key was not present in the dictionary.") throw;
                    return null;
                }
            }
        }
    }
}