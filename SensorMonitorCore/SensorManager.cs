using System;
using System.IO;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core
{
    public class SensorManager
    {
        public SensorLogFormatTable Table { get; private set; }
        public SensorIO.LogFormat.Binary.LogFormatBinaryFile LogFormatFile { get; private set; }

        private Dictionary<SensorIdentifier, SensorIO.Log.Binary.LogChronoFile> logChronoFiles;
        public SensorIO.Log.Binary.LogChronoFile this[SensorIdentifier id]
        {
            get { return logChronoFiles[id]; }
        }
        public IEnumerable<SensorIO.Log.Binary.LogChronoFile> LogChronoFiles => logChronoFiles.Values;
        public IEnumerable<SensorIdentifier> LogChronoFilesIds => logChronoFiles.Keys;
        public IEnumerable<KeyValuePair<SensorIdentifier, SensorIO.Log.Binary.LogChronoFile>> LogChronoFilesPairs
        {
            get { foreach (var pair in logChronoFiles) yield return pair; }
        }

        public DirectoryInfo LogFilesDirectory { get; }

        public SensorManager(string logFilesDirectory)
        {
            if (logFilesDirectory == null) throw new ArgumentNullException();
            LogFilesDirectory = new DirectoryInfo(logFilesDirectory);
            if (!LogFilesDirectory.Exists) LogFilesDirectory.Create();
            ReadLogFormats();
            InitLogChronoFilesDict();
        }
        private void ReadLogFormats()
        {
            string path = GenerateLogFormatFilePath();
            const byte actualBFV = SensorIO.LogFormat.Binary.LogFormatBinaryReader.ActualBinaryFormatVersion;
            LogFormatFile = new SensorIO.LogFormat.Binary.LogFormatBinaryFile(path, actualBFV,
                (bfv) => new SensorIO.LogFormat.Binary.LogFormatBinaryReader(bfv),
                (bfv) => new SensorIO.LogFormat.Binary.LogFormatBinaryWriter(bfv));
            LogFormatFile.Open();
            if (LogFormatFile.IsEmpty)
            {
                Table = new SensorLogFormatTable();
                LogFormatFile.WriteAll(Table);
            }
            else Table = LogFormatFile.ReadAll();
            LogFormatFile.Close();
        }
        private void InitLogChronoFilesDict()
        {
            logChronoFiles = new Dictionary<SensorIdentifier, SensorIO.Log.Binary.LogChronoFile>();
            foreach (FileInfo file in LogFilesDirectory.GetFiles("SensorLogChrono-*"))
            {
                var chronoFile = CreateLogChronoFile(file.FullName);
                chronoFile.Open();
                logChronoFiles.Add(chronoFile.SensorIdentifier, chronoFile);
                chronoFile.Close();
            }
        }

        private string GenerateLogFormatFilePath()
        {
            return Path.Combine(LogFilesDirectory.FullName, "SensorLogFormats");
        }
        private string GenerateLogChronoFilePath(SensorIdentifier id)
        {
            string path = $"SensorLogChrono-{id.logFormat.Name}-{id.serial}";
            return Path.Combine(LogFilesDirectory.FullName, path);
        }

        private SensorIO.Log.Binary.LogChronoFile CreateLogChronoFile(string path)
        {
            return new SensorIO.Log.Binary.LogChronoFile(path, Table,
                (bfv) => new SensorIO.Log.Binary.LogFieldsBinaryReader(new SensorIO.FieldValueBinaryReader(), bfv),
                (bfv) => new SensorIO.Log.Binary.LogFieldsBinaryWriter(new SensorIO.FieldValueBinaryWriter(), bfv));
        }

        public void OpenLogChronoFileAsNew(SensorIdentifier id)
        {
            const byte actualBFV = SensorIO.Log.Binary.LogFieldsBinaryReader.ActualBinaryFormatVersion;
            var chronoFile = logChronoFiles[id];
            if (chronoFile.IsOpened) chronoFile.Close();
            chronoFile.OpenAsNew(id, actualBFV);
        }
        public void DefragmentLogChronoFile(SensorIdentifier id)
        {
            const byte actualBFV = SensorIO.Log.Binary.LogFieldsBinaryReader.ActualBinaryFormatVersion;
            var chronoFile = logChronoFiles[id];
            bool wasClosed = chronoFile.IsClosed;
            if (wasClosed) chronoFile.Open();
            chronoFile.Defragment(actualBFV);
            if (wasClosed) chronoFile.Close();
        }

        public void LoadCsv(TextReader source)
        {
            SensorIO.IFieldValueTextParser textParser = new SensorIO.FieldValueTextParser();
            var converter = new SensorIO.Log.Csv.CsvLogSeqConverter(Table, textParser);
            IEnumerable<SensorLog> logs = converter.Convert(source);
            IEnumerator<SensorLog> logsEnumerator = logs.GetEnumerator();
            if (!logsEnumerator.MoveNext()) return;
            AddLogsToLogChronoFile(logsEnumerator.Current.Identifier, logs);
        }
        public void LoadJson(TextReader source)
        {
            var jsonLogConverter = new SensorIO.Log.Json.JsonLogConverter(new SensorIO.Log.Json.JsonParser(),
                new SensorIO.FieldValueTextParser(), Table);
            var jsonLogSeqConverter = new SensorIO.Log.Json.JsonLogSeqConverter(jsonLogConverter);
            IEnumerable<SensorLog> logs = jsonLogSeqConverter.Convert(source);
            Dictionary<SensorIdentifier, List<SensorLog>> idAssignedLogs = AssignLogsById(logs);
            foreach (var pair in idAssignedLogs)
                AddLogsToLogChronoFile(pair.Key, pair.Value);
        }
        private Dictionary<SensorIdentifier, List<SensorLog>> AssignLogsById(IEnumerable<SensorLog> logs)
        {
            var idAssignedLogs = new Dictionary<SensorIdentifier, List<SensorLog>>();
            foreach (SensorLog log in logs)
            {
                bool exists = idAssignedLogs.TryGetValue(log.Identifier, out List<SensorLog> assignedLogs);
                if (!exists)
                {
                    assignedLogs = new List<SensorLog>();
                    idAssignedLogs.Add(log.Identifier, assignedLogs);
                }
                assignedLogs.Add(log);
            }
            return idAssignedLogs;
        }
        private void AddLogsToLogChronoFile(SensorIdentifier id, IEnumerable<SensorLog> logs)
        {
            if (!logChronoFiles.ContainsKey(id)) return;
            SensorIO.Log.Binary.LogChronoFile chronoFile = logChronoFiles[id];
            bool wasOpened = chronoFile.IsOpened;
            if (!wasOpened) chronoFile.Open();
            chronoFile.AddLogs(logs);
            if (!wasOpened) chronoFile.Close();
        }

        public void AddLogFormat(SensorLogFormat newFormat)
        {
            Table.AddLogFormat(newFormat);
            if (LogFormatFile.IsClosed) LogFormatFile.Open();
            LogFormatFile.Append(newFormat);
            LogFormatFile.Close();
        }
        public void DeleteLogFormat(SensorLogFormat format)
        {
            var sensorsToDelete = new List<SensorIdentifier>();
            foreach (SensorIdentifier id in logChronoFiles.Keys)
                if (id.logFormat == format) sensorsToDelete.Add(id);
            foreach (SensorIdentifier id in sensorsToDelete)
                DeleteSensor(id);
            Table.RemoveLogFormat(format);
            if (LogFormatFile.IsClosed) LogFormatFile.Open();
            LogFormatFile.WriteAll(Table);
            LogFormatFile.Close();
        }

        public void AddSensor(SensorIdentifier id)
        {
            if (!Table.Contains(id.logFormat)) throw new ArgumentException();
            string path = $"SensorLogChrono-{id.logFormat.Name}-{id.serial}";
            path = Path.Combine(LogFilesDirectory.FullName, path);
            var chronoFile = CreateLogChronoFile(path);
            logChronoFiles.Add(id, chronoFile);
            OpenLogChronoFileAsNew(id);
            chronoFile.Close();
        }
        public void DeleteSensor(SensorIdentifier id)
        {
            if (!Table.Contains(id.logFormat)) throw new ArgumentException();
            var chronoFile = logChronoFiles[id];
            logChronoFiles.Remove(id);
            if (chronoFile.IsOpened) chronoFile.Close();
            new FileInfo(chronoFile.Path).Delete();
        }
        public bool DoesSensorExist(SensorIdentifier id)
        {
            return logChronoFiles.ContainsKey(id);
        }
    }
}