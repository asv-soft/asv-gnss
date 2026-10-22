using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV2Message15 : RtcmV2MessageBase
{
    public const int RtcmMessageId = 15;

    public override ushort MessageId => RtcmMessageId;
    public override string Name => "Ionospheric Delay Message (Fixed)";

    public IonosphericDelayItem[] Delays { get; set; } = [];

    /// <inheritdoc />
    protected override int GetContentBitSize() => checked(Delays.Length * 36);

    /// <inheritdoc />
    protected override void SerializeContent(Span<byte> buffer, ref int bitIndex)
    {
        foreach (var item in Delays)
        {
            item.Serialize(buffer, ref bitIndex);
        }
    }

    protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
    {
        var itemCount = payloadLength * 8 / 36;
        Delays = new IonosphericDelayItem[itemCount];

        for (var i = 0; i < itemCount; i++)
        {
            Delays[i] = new IonosphericDelayItem();
            Delays[i].Deserialize(buffer, ref bitIndex);
        }
    }
}

public partial class IonosphericDelayItem
{
    private byte _reserved;
    private bool _hasOriginalInvalidDelay;
    private uint _originalDelay;

    public NavigationSystemEnum NavigationSystem { get; set; }
    public byte Prn { get; set; }
    /// <summary>Legacy model value obtained by multiplying the unsigned 14-bit wire value by 0.001; retained for API compatibility.</summary>
    public double IonosphericDelay { get; set; }
    /// <summary>Legacy model value obtained by multiplying the signed 14-bit wire value by 0.05; NaN indicates an unavailable correction.</summary>
    public double IonoRateOfChange { get; set; }

    public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
    {
        _reserved = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
        var sys = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        NavigationSystem = sys == 0 ? NavigationSystemEnum.SYS_GPS : NavigationSystemEnum.SYS_GLO;
        Prn = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
        if (Prn == 0)
        {
            Prn = 32;
        }

        _originalDelay = SpanBitHelper.GetBitU(buffer, ref bitIndex, 14);
        IonosphericDelay = _originalDelay * 0.001;
        var rateOfChange = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14);
        _hasOriginalInvalidDelay = rateOfChange == -8192;
        if (rateOfChange == -8192)
        {
            IonosphericDelay = double.NaN;
            IonoRateOfChange = double.NaN;
        }
        else
        {
            IonoRateOfChange = rateOfChange * 0.05;
        }
    }

    /// <summary>Writes one 36-bit ionospheric record using the existing model conversion formulas.</summary>
    public void Serialize(Span<byte> buffer, ref int bitIndex)
    {
        if (NavigationSystem is not (NavigationSystemEnum.SYS_GPS or NavigationSystemEnum.SYS_GLO) || Prn is < 1 or > 32)
        {
            throw new ArgumentOutOfRangeException(nameof(Prn), "An ionospheric record requires GPS or GLONASS and PRN 1 through 32.");
        }
        if (double.IsInfinity(IonosphericDelay) || double.IsInfinity(IonoRateOfChange))
        {
            throw new ArgumentOutOfRangeException(nameof(IonosphericDelay), "Corrections must be finite or NaN for an unavailable correction.");
        }

        var invalid = double.IsNaN(IonosphericDelay) || double.IsNaN(IonoRateOfChange);
        var rate = invalid ? -8192 : Math.Round(IonoRateOfChange / 0.05, MidpointRounding.AwayFromZero);
        if (!invalid && rate == -8192)
        {
            throw new ArgumentOutOfRangeException(nameof(IonoRateOfChange), "The minimum signed value is reserved for an unavailable correction.");
        }
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 2, _reserved);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 1, NavigationSystem == NavigationSystemEnum.SYS_GLO ? 1 : 0);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 5, Prn == 32 ? 0 : Prn);
        if (invalid)
        {
            RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 14, _hasOriginalInvalidDelay ? _originalDelay : 0);
        }
        else
        {
            RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 14, IonosphericDelay, 0.001);
        }
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 14, rate);
    }
}
