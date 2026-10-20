using Asv.IO;

namespace Asv.Gnss;

public partial class UbxCfgRate
{
    private static readonly Field RateHzField = UbxVisitor.Field(nameof(RateHz), DoubleType.Default, "GNSS measurement frequency in hertz, represented on the wire by its period in milliseconds. This implementation rounds the period to multiples of 25 ms; supported rates depend on receiver firmware.", "Hz");

    private static readonly Field NavRateField = UbxVisitor.Field(nameof(NavRate), UInt16Type.Default, "Number of measurement cycles per navigation solution, up to 127. Protocol versions below 18 use a fixed ratio of 1.", "measurements");

    private static readonly Field TimeSystemField = UbxVisitor.Enum(nameof(TimeSystem), Int32Type.Default, "Time reference used to align measurement epochs; GLONASS, BeiDou and Galileo references require protocol version 18 or later.",
        new EnumValue<int>((int)TimeSystemEnum.Utc, nameof(TimeSystemEnum.Utc)),
        new EnumValue<int>((int)TimeSystemEnum.Gps, nameof(TimeSystemEnum.Gps)),
        new EnumValue<int>((int)TimeSystemEnum.Glonass, nameof(TimeSystemEnum.Glonass)),
        new EnumValue<int>((int)TimeSystemEnum.BeiDou, nameof(TimeSystemEnum.BeiDou)),
        new EnumValue<int>((int)TimeSystemEnum.Galileo, nameof(TimeSystemEnum.Galileo)));

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        DoubleType.Accept(visitor, RateHzField, ref _rateHz);
        UInt16Type.Accept(visitor, NavRateField, ref _navRate);
        var timeSystem = (int)_timeSystem;
        Int32Type.Accept(visitor, TimeSystemField, ref timeSystem);
        _timeSystem = (TimeSystemEnum)timeSystem;
    }
}
