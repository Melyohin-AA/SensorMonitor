using System;
using System.IO;
using System.Collections.Generic;
using SensorMonitor.Core.SensorLogModel;

namespace SensorMonitor.Core.SensorIO.Log.Binary
{
    public class LogChronoFile : SensorIOFile
    {
        private SensorLogFormatTable table;
        private Func<byte, ILogFieldsReader> logFieldsReaderCreationFunc;
        private Func<byte, ILogFieldsWriter> logFieldsWriterCreationFunc;

        private byte logFieldsBFV;
        private List<ChronoAddress> chronoAddresses;
        private bool chronoAddressesUpdated;

        public SensorIdentifier SensorIdentifier { get; private set; }

        public LogChronoFile(string path, SensorLogFormatTable table,
            Func<byte, ILogFieldsReader> logFieldsReaderCreationFunc,
            Func<byte, ILogFieldsWriter> logFieldsWriterCreationFunc) : base(path)
        {
            if ((table == null) || (logFieldsReaderCreationFunc == null) || (logFieldsWriterCreationFunc == null))
                throw new ArgumentNullException();
            this.table = table;
            this.logFieldsReaderCreationFunc = logFieldsReaderCreationFunc;
            this.logFieldsWriterCreationFunc = logFieldsWriterCreationFunc;
        }

        public void OpenAsNew(SensorIdentifier id, byte actualLogFieldsBFV)
        {
            SensorIdentifier = id;
            logFieldsBFV = actualLogFieldsBFV;
            base.Open();
            BinaryWriter writer = GetWriter();
            writer.Write(actualLogFieldsBFV);
            writer.Write(id.logFormat.Name);
            writer.Write(id.serial);
            writer.Write((long)-1);
            chronoAddresses = new List<ChronoAddress>();
        }

        public override void Open()
        {
            base.Open();
            ReadHeader();
            ReadChronoAddresses();
        }
        private void ReadHeader()
        {
            BinaryReader reader = GetReader();
            logFieldsBFV = reader.ReadByte();
            string logFormatName = reader.ReadString();
            SensorLogFormat logFormat = table[logFormatName];
            string sensorSerial = reader.ReadString();
            SensorIdentifier = new SensorIdentifier(logFormat, sensorSerial);
        }
        private void ReadChronoAddresses()
        {
            BinaryReader reader = GetReader();
            chronoAddresses = new List<ChronoAddress>();
            while (true)
            {
                long nextSegAddress = reader.ReadInt64();
                if (nextSegAddress == -1) break;
                File.Position = nextSegAddress;
                int segLength = reader.ReadInt32();
                for (uint i = 0; i < segLength; i++)
                    chronoAddresses.Add(ReadChronoAddress(reader));
            }
            chronoAddressesUpdated = false;
        }
        private ChronoAddress ReadChronoAddress(BinaryReader reader)
        {
            long address = reader.ReadInt64();
            long dateData = reader.ReadInt64();
            DateTime dt = DateTime.FromBinary(dateData);
            return new ChronoAddress(address, dt);
        }

        public override void Close()
        {
            if (chronoAddressesUpdated) ChronoAddressesWriter.Write(this);
            chronoAddressesUpdated = false;
            chronoAddresses = null;
            base.Close();
        }

        public void AddLogs(IEnumerable<SensorLog> logs)
        {
            CheckIfIsOpened();
            chronoAddressesUpdated = true;
            ILogFieldsWriter logFieldsWriter = logFieldsWriterCreationFunc(logFieldsBFV);
            foreach (SensorLog log in logs)
            {
                if (log.Format != SensorIdentifier.logFormat)
                    throw new ArgumentException("Invalid SensorLogFormat!");
                ChronoAddress chronoAddress = new ChronoAddress(File.Position, log.Info.dt);
                int thisDTExistingChronoAdress = chronoAddresses.BinarySearch(chronoAddress);
                if (thisDTExistingChronoAdress < 0)
                {
                    chronoAddresses.Add(chronoAddress);
                    File.Position = File.Length;
                    logFieldsWriter.WriteFields(File, log);
                }
                else
                {
                    File.Position = chronoAddresses[thisDTExistingChronoAdress].address;
                    logFieldsWriter.WriteFields(File, log);
                }
            }
            chronoAddresses.Sort();
        }
        public void RemoveLog(Iterator iterator)
        {
            CheckIfIsOpened();
            if (iterator.Host != this) throw new ArgumentException();
            if (!iterator.IsOutOfBounds) throw new IndexOutOfRangeException();
            chronoAddressesUpdated = true;
            chronoAddresses.RemoveAt(iterator.chronoIndex);
        }
        public void RemoveLogRange(Iterator from, Iterator until)
        {
            CheckIfIsOpened();
            if ((from.Host != this) || (until.Host != this)) throw new ArgumentException();
            if (from.IsOutOfRightBound) return;
            if (from <= until) return;
            chronoAddressesUpdated = true;
            int origin = Math.Max(from.chronoIndex, 0);
            int end = Math.Min(chronoAddresses.Count, until.chronoIndex);
            chronoAddresses.RemoveRange(origin, end - origin);
        }

        public void Defragment(byte actualLogFieldsBinaryFormatVersion)
        {
            CheckIfIsOpened();
            string tempPath = Path + "-temp";
            if (tempPath == null) throw new ArgumentNullException();
            LogChronoFile dest =
                new LogChronoFile(tempPath, table, logFieldsReaderCreationFunc, logFieldsWriterCreationFunc);
            dest.OpenAsNew(SensorIdentifier, actualLogFieldsBinaryFormatVersion);
            Defragmenter defragmenter = new Defragmenter(this, dest);
            defragmenter.Defragment();
            dest.Close();
            Close();
            GC.Collect();
            new FileInfo(Path).Delete();
            System.IO.File.Move(tempPath, Path);
            Open();
        }

        public Iterator GetIteratorAtOrigin()
        {
            CheckIfIsOpened();
            return new Iterator(this, 0);
        }
        public Iterator GetIteratorAtEnd()
        {
            CheckIfIsOpened();
            return new Iterator(this, chronoAddresses.Count - 1);
        }
        public Iterator GetIteratorOnOrJustAfter(DateTime dt)
        {
            CheckIfIsOpened();
            int index = chronoAddresses.BinarySearch(new ChronoAddress(-1, dt));
            if (index < 0) index = -(index + 1);
            return new Iterator(this, index);
        }

        private struct ChronoAddress : IComparable
        {
            public readonly long address;
            public readonly DateTime dt;

            public ChronoAddress(long address, DateTime dt)
            {
                if (address < -1) throw new ArgumentOutOfRangeException();
                this.address = address;
                this.dt = dt;
            }

            public int CompareTo(object obj)
            {
                ChronoAddress opp = (ChronoAddress)obj;
                return dt.CompareTo(opp.dt);
            }

            public override bool Equals(object obj)
            {
                if (obj == null) throw new ArgumentNullException();
                if (!(obj is ChronoAddress)) return false;
                ChronoAddress opp = (ChronoAddress)obj;
                return this == opp;
            }
            public override int GetHashCode()
            {
                return dt.GetHashCode();
            }

            public static bool operator ==(ChronoAddress a, ChronoAddress b)
            {
                return a.dt == b.dt;
            }
            public static bool operator !=(ChronoAddress a, ChronoAddress b)
            {
                return !(a == b);
            }
            public static bool operator >(ChronoAddress a, ChronoAddress b)
            {
                return a.dt > b.dt;
            }
            public static bool operator <=(ChronoAddress a, ChronoAddress b)
            {
                return !(a > b);
            }
            public static bool operator <(ChronoAddress a, ChronoAddress b)
            {
                return a.dt < b.dt;
            }
            public static bool operator >=(ChronoAddress a, ChronoAddress b)
            {
                return !(a < b);
            }
        }

        private class ChronoAddressesWriter
        {
            private LogChronoFile host;
            private BinaryReader reader;
            private BinaryWriter writer;
            private int chronoAddressIndex = 0;
            private long segAddress = -1;
            private uint chronoAddressSegSlotsLeft = 0, segSlotsUsed = 0;

            public static void Write(LogChronoFile host)
            {
                var writer = new ChronoAddressesWriter(host);
                writer.Write();
            }

            private ChronoAddressesWriter(LogChronoFile host)
            {
                this.host = host ?? throw new ArgumentNullException();
                reader = host.GetReader();
                writer = host.GetWriter();
            }

            private void Write()
            {
                host.File.Position = 0;
                SkipHeader();
                if (TryEndIfZeroLength()) return;
                while (true)
                {
                    if (TryEndIfChronoAddressesEnded()) return;
                    if (chronoAddressSegSlotsLeft == 0) GoToNextAddressSeg();
                    else WriteChronoAddress();
                }
            }

            private void SkipHeader()
            {
                reader.ReadByte();
                reader.ReadString();
                reader.ReadString();
            }

            private bool TryEndIfZeroLength()
            {
                if (host.chronoAddresses.Count > 0) return false;
                writer.Write((long)-1);
                return true;
            }
            private bool TryEndIfChronoAddressesEnded()
            {
                if (chronoAddressIndex < host.chronoAddresses.Count) return false;
                writer.Write((long)-1);
                host.File.Position = segAddress;
                writer.Write(segSlotsUsed);
                return true;
            }

            private void GoToNextAddressSeg()
            {
                segAddress = reader.ReadInt64();
                if (segAddress == -1) GoToNewAddressSeg();
                else
                {
                    host.File.Position = segAddress;
                    chronoAddressSegSlotsLeft = reader.ReadUInt32();
                }
                segSlotsUsed = 0;
            }
            private void GoToNewAddressSeg()
            {
                segAddress = host.File.Length;
                host.File.Position -= 8;
                writer.Write(segAddress);
                host.File.Position = segAddress;
                chronoAddressSegSlotsLeft = (uint)(host.chronoAddresses.Count - chronoAddressIndex);
                writer.Write(chronoAddressSegSlotsLeft);
            }

            private void WriteChronoAddress()
            {
                ChronoAddress chronoAddress = host.chronoAddresses[chronoAddressIndex];
                writer.Write(chronoAddress.address);
                long dateData = chronoAddress.dt.ToBinary();
                writer.Write(dateData);
                chronoAddressIndex++;
                chronoAddressSegSlotsLeft--;
                segSlotsUsed++;
            }
        }

        private class Defragmenter
        {
            private LogChronoFile source, dest;
            private Iterator sourceIterator;

            public Defragmenter(LogChronoFile source, LogChronoFile dest)
            {
                this.source = source;
                this.dest = dest;
            }

            public void Defragment()
            {
                sourceIterator = source.GetIteratorAtOrigin();
                dest.AddLogs(LogPeeker());
            }
            private IEnumerable<SensorLog> LogPeeker()
            {
                if (sourceIterator.IsOutOfBounds) yield break;
                while (true)
                {
                    yield return sourceIterator.ReadCurrentLog();
                    if (sourceIterator.IsAtEnd) yield break;
                    sourceIterator.MoveForward();
                }
            }
        }

        public struct Iterator
        {
            public LogChronoFile Host { get; }
            internal int chronoIndex;

            public bool IsAtOrigin { get { return chronoIndex == 0; } }
            public bool IsAtEnd { get { return chronoIndex == Host.chronoAddresses.Count - 1; } }

            public bool IsOutOfBounds { get { return IsOutOfRightBound || IsOutOfLeftBound; } }
            public bool IsOutOfLeftBound { get { return chronoIndex < 0; } }
            public bool IsOutOfRightBound { get { return chronoIndex >= Host.chronoAddresses.Count; } }

            internal Iterator(LogChronoFile host, int chronoIndex)
            {
                this.Host = host ?? throw new ArgumentNullException();
                this.chronoIndex = chronoIndex;
            }

            public void GoToOrigin()
            {
                chronoIndex = 0;
            }
            public void GoToEnd()
            {
                chronoIndex = Host.chronoAddresses.Count - 1;
            }

            public void MoveBackward()
            {
                if (IsAtOrigin) throw new InvalidOperationException();
                if (IsOutOfLeftBound) throw new IndexOutOfRangeException();
                chronoIndex--;
            }
            public void MoveForward()
            {
                if (IsAtEnd) throw new InvalidOperationException();
                if (IsOutOfRightBound) throw new IndexOutOfRangeException();
                chronoIndex++;
            }

            public DateTime GetCurrentLogDT()
            {
                return Host.chronoAddresses[chronoIndex].dt;
            }
            public SensorLog ReadCurrentLog()
            {
                ILogFieldsReader logFieldsReader = Host.logFieldsReaderCreationFunc(Host.logFieldsBFV);
                ChronoAddress chronoAddress = Host.chronoAddresses[chronoIndex];
                Host.File.Position = chronoAddress.address;
                SensorLogFormat logFormat = Host.SensorIdentifier.logFormat;
                SensorLog.IFieldInitializer initializer = logFieldsReader.Read(Host.File, logFormat);
                SensorLogInfo info = new SensorLogInfo(chronoAddress.dt, Host.SensorIdentifier.serial);
                return new SensorLog(info, logFormat, initializer);
            }

            public override bool Equals(object obj)
            {
                if (obj == null) throw new ArgumentNullException();
                if (!(obj is Iterator)) return false;
                Iterator opp = (Iterator)obj;
                return this == opp;
            }
            public override int GetHashCode()
            {
                return Host.GetHashCode() ^ chronoIndex;
            }

            public static bool operator ==(Iterator a, Iterator b)
            {
                if (a.Host != b.Host) return false;
                return a.chronoIndex == b.chronoIndex;
            }
            public static bool operator !=(Iterator a, Iterator b)
            {
                return !(a == b);
            }
            public static bool operator >(Iterator a, Iterator b)
            {
                if (a.Host != b.Host) throw new ArgumentException();
                return a.chronoIndex > b.chronoIndex;
            }
            public static bool operator <=(Iterator a, Iterator b)
            {
                return !(a > b);
            }
            public static bool operator <(Iterator a, Iterator b)
            {
                if (a.Host != b.Host) throw new ArgumentException();
                return a.chronoIndex < b.chronoIndex;
            }
            public static bool operator >=(Iterator a, Iterator b)
            {
                return !(a < b);
            }

            public static int operator -(Iterator a, Iterator b)
            {
                if (a.Host != b.Host) throw new ArgumentException();
                return a.chronoIndex - b.chronoIndex;
            }
        }
    }
}