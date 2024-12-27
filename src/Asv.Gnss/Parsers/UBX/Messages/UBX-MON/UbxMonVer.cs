using System;
using System.Collections.Generic;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    public class UbxMonVerPool : UbxMessageBase
    {
        public override string Name => "UBX-MON-VER-POOL";
        public override byte Class => 0x0A;
        public override byte SubClass => 0x04;

        protected override void SerializeContent(ref Span<byte> buffer) { }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer) { }

        protected override int GetContentByteSize() => 0;

        public override void Randomize(Random random) { }
    }

    [SerializationNotSupported]
    public class UbxMonVer : UbxMessageBase
    {
        public override string Name => "UBX-MON-VER";
        public override byte Class => 0x0A;
        public override byte SubClass => 0x04;

        /// <summary>
        /// Nul-terminated software version string.
        /// </summary>
        public string Software { get; set; }

        /// <summary>
        /// Nul-terminated hardware version string
        /// </summary>
        public string Hardware { get; set; }

        /// <summary>
        /// Extended software information strings. A series of nul-terminated strings.Each
        /// extension field is 30 characters long and contains varying software information.
        /// Not all extension fields may appear. Examples of reported information: the
        /// software version string of the underlying ROM (when the receiver's firmware is
        /// running from flash), the firmware version, the supported protocol version, the
        /// module identifier, the flash information structure(FIS) file information, the
        /// supported major GNSS, the supported augmentation systems. See Firmware and protocol versions for
        /// details.
        /// </summary>
        public List<string> Extensions { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            // 40 + 30*N = PayloadLength
            var extLength = (buffer.Length - 40) / 30;
            Software = buffer.Slice(0, 30).GetString(Encoding.ASCII);
            buffer = buffer.Slice(30);
            Hardware = buffer.Slice(0, 10).GetString(Encoding.ASCII);
            buffer = buffer.Slice(10);
            Extensions = new List<string>();
            for (var i = 0; i < extLength; i++)
            {
                Extensions.Add(buffer.Slice(0, 30).GetString(Encoding.ASCII));
                buffer.Slice(30);
            }
        }

        protected override int GetContentByteSize() => 40 + (Extensions?.Count ?? 0) * 30;

        public override void Randomize(Random random) { }
    }
}
