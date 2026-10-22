using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Proprietary Message Type.
    /// Trimble Navigation Ltd.
    /// </summary>
    public partial class RtcmV3Msg4094 : RtcmV3LegacyMessageBase
    {
        /// <summary>
        /// Rtcm Message Id
        /// </summary>
        public const int RtcmMessageId = 4094;
        /// <inheritdoc/>
        public override ushort MessageId => RtcmMessageId;
        
        /// <inheritdoc/>
        public override string Name => "Trimble Navigation Ltd.";

        public byte[] Data { get; set; } = Array.Empty<byte>();

        protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
        {
            for (var i = 0; i < Data.Length; i++)
                SpanBitHelper.SetBitU(buffer, ref bitIndex, i == 0 ? 4 : 8, Data[i]);
        }

        protected override int InternalGetBitSize() => Data.Length == 0 ? 0 : Data.Length * 8 - 4;

        /// <inheritdoc/>
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            Data = new byte[messageLength - 1];
            for (var i = 0; i < Data.Length; i++)
                Data[i] = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, i == 0 ? 4 : 8);
        }
    }
}
