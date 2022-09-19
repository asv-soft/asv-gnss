using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class Nmea0183MessageBase : GnssMessageBase<string>
    {
        public override string ProtocolId => Nmea0183Parser.GnssProtocolId;
        public override string Name => MessageId;

        private string _sourceId;

        public string SourceTitle { get; private set; }

        public string SourceId
        {
            get => _sourceId;
            set
            {
                _sourceId = value;
                SourceTitle = Nmea0183Helper.TryFindSourceTitleById(value);
            }
        }

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 5) throw new Exception("Too small string for NMEA");
            var message = buffer.GetString(Encoding.ASCII);
            SourceId = message.Substring(0, 2);
            var items = message.Trim().Split(',');
            InternalDeserializeFromStringArray(items);
            buffer = buffer.Slice(buffer.Length);
        }

        protected abstract void InternalDeserializeFromStringArray(string[] items);

        public override void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override int GetByteSize()
        {
            throw new NotImplementedException();
        }
    }
}