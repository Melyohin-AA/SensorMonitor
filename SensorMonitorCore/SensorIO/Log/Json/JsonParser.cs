using System;
using System.Collections.Generic;

namespace SensorMonitor.Core.SensorIO.Log.Json
{
    public interface IJsonParser
    {
        Dictionary<string, string> Parse(string json);
    }

    public class JsonParser : IJsonParser
    {
        public Dictionary<string, string> Parse(string json)
        {
            Inner inner = new Inner(json);
            inner.Parse();
            return inner.Result;
        }

        private class Inner
        {
            private string json;
            private int cursor = 0;
            private int level = 0;
            private string key = null, value = null;
            private bool isKeyNotValue = true;

            public Dictionary<string, string> Result { get; private set; }

            public Inner(string json)
            {
                this.json = json ?? throw new ArgumentNullException();
            }

            public void Parse()
            {
                Result = new Dictionary<string, string>();
                while (cursor < json.Length)
                {
                    char symbol = json[cursor++];
                    switch (symbol)
                    {
                        case '{':
                            level++;
                            if (level > 1) ReadSubjsonAsValue();
                            break;
                        case '}':
                            level--;
                            break;
                        case '"':
                            ReadKeyOrValue();
                            break;
                        case ',':
                            isKeyNotValue = true;
                            break;
                        case ':':
                            isKeyNotValue = false;
                            break;
                    }
                    TryAddKeyValuePair();
                }
            }

            private void ReadKeyOrValue()
            {
                string str = ReadString();
                if (isKeyNotValue) key = str;
                else value = str;
            }
            private string ReadString()
            {
                int end = json.IndexOf('"', cursor);
                string str = json.Substring(cursor, end - cursor);
                cursor = end + 1;
                return str;
            }

            private void ReadSubjsonAsValue()
            {
                if (isKeyNotValue) throw new InvalidOperationException();
                int origin = cursor - 1;
                while (level > 1)
                {
                    char symbol = json[cursor++];
                    if (symbol == '}') level--;
                    else if (symbol == '{') level++;
                }
                int subjsonLength = cursor - origin;
                value = json.Substring(origin, subjsonLength);
            }

            private void TryAddKeyValuePair()
            {
                if (isKeyNotValue || (key == null) || (value == null)) return;
                Result.Add(key, value);
                key = value = null;
            }
        }
    }
}