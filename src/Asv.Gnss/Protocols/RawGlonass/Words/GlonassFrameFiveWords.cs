using System;
using Asv.IO;

namespace Asv.Gnss;

public sealed partial class GlonassWord14
{
    private static readonly Field FrameNumberField = GlonassWordEncoding.Field(
        nameof(FrameNumber),
        UInt8Type.Default
    );
    private static readonly Field B1Field = GlonassWordEncoding.Field(
        nameof(B1),
        DoubleType.Default,
        "s"
    );
    private static readonly Field B2Field = GlonassWordEncoding.Field(
        nameof(B2),
        DoubleType.Default,
        "s/day"
    );
    private static readonly Field KPField = GlonassWordEncoding.Field(
        nameof(KP),
        UInt8Type.Default
    );
    private byte _frameNumber;
    private double _b1;
    private double _b2;
    private byte _kp;

    /// <summary>
    /// External frame context: zero is unknown and retains the legacy almanac layout; five selects UT1 data.
    /// Changing layouts preserves edits to an already decoded payload and reinterprets its bits.
    /// </summary>
    public byte FrameNumber
    {
        get => _frameNumber;
        set => ChangeFrameNumber(ref _frameNumber, value);
    }

    /// <summary>Frame-five UT1 correction coefficient, in seconds (11-bit sign-magnitude, 2^-10).</summary>
    public double B1
    {
        get => _b1;
        set => _b1 = value;
    }

    /// <summary>Frame-five daily change of the UT1 correction, in seconds per mean solar day (2^-16).</summary>
    public double B2
    {
        get => _b2;
        set => _b2 = value;
    }

    /// <summary>Frame-five two-bit notification of an upcoming UTC leap-second correction.</summary>
    public byte KP
    {
        get => _kp;
        set => _kp = value;
    }

    public override void Deserialize(byte[] data)
    {
        base.Deserialize(data);
        _b1 = GlonassRawHelper.GetBitG(data, 8, 11) / 1024.0;
        _b2 = GlonassRawHelper.GetBitG(data, 19, 10) / 65536.0;
        _kp = (byte)GlonassRawHelper.GetBitU(data, 29, 2);
    }

    protected override void SerializeContent(Span<byte> buffer)
    {
        if (FrameNumber != 5)
        {
            base.SerializeContent(buffer);
            return;
        }
        GlonassWordEncoding.Signed(buffer, 8, 11, B1, 1.0 / 1024, nameof(B1));
        GlonassWordEncoding.Signed(buffer, 19, 10, B2, 1.0 / 65536, nameof(B2));
        GlonassWordEncoding.Unsigned(buffer, 29, 2, KP, nameof(KP));
    }

    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var context = FrameNumber;
        UInt8Type.Accept(visitor, FrameNumberField, ref context);
        FrameNumber = context;
        if (FrameNumber != 5)
        {
            base.Accept(visitor);
            return;
        }
        DoubleType.Accept(visitor, B1Field, ref _b1);
        DoubleType.Accept(visitor, B2Field, ref _b2);
        UInt8Type.Accept(visitor, KPField, ref _kp);
    }
}

public sealed partial class GlonassWord15
{
    private static readonly Field FrameNumberField = GlonassWordEncoding.Field(
        nameof(FrameNumber),
        UInt8Type.Default
    );
    private static readonly Field ReservedDataField = GlonassWordEncoding.Field(
        nameof(ReservedData),
        new ArrayType(UInt8Type.Default, 9)
    );
    private byte _frameNumber;

    /// <summary>
    /// External frame context: zero is unknown and retains the legacy almanac layout; five selects reserved data and health.
    /// Changing layouts preserves edits to an already decoded payload and reinterprets its bits.
    /// </summary>
    public byte FrameNumber
    {
        get => _frameNumber;
        set => ChangeFrameNumber(ref _frameNumber, value);
    }

    /// <summary>
    /// Frame-five ICD bits 80 through 10, MSB-first in nine bytes with one leading zero padding bit.
    /// The getter returns a copy; assign an edited array to update the payload. No separate payload is retained.
    /// </summary>
    public byte[] ReservedData
    {
        get
        {
            var data = new byte[9];
            for (var index = 0; index < 71; index++)
            {
                var value = GlonassWordEncoding.Read(RawData, 8 + index, 1);
                GlonassWordEncoding.Unsigned(data, 1 + index, 1, value, nameof(ReservedData));
            }
            return data;
        }
        set
        {
            if (value is null || value.Length != 9 || (value[0] & 0x80) != 0)
                throw new ArgumentException(
                    "Reserved data requires nine bytes with one leading zero padding bit.",
                    nameof(value)
                );
            if (FrameNumber != 5)
                throw new InvalidOperationException(
                    "Reserved data is editable only in the fifth frame."
                );
            for (var index = 0; index < 71; index++)
            {
                var bit = GlonassWordEncoding.Read(value, 1 + index, 1);
                GlonassWordEncoding.Unsigned(RawData, 8 + index, 1, bit, nameof(ReservedData));
            }
        }
    }

    protected override void SerializeContent(Span<byte> buffer)
    {
        if (FrameNumber != 5)
        {
            base.SerializeContent(buffer);
            return;
        }
        GlonassWordEncoding.Unsigned(buffer, 79, 1, ln, nameof(ln));
    }

    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var context = FrameNumber;
        UInt8Type.Accept(visitor, FrameNumberField, ref context);
        FrameNumber = context;
        if (FrameNumber != 5)
        {
            base.Accept(visitor);
            return;
        }
        var type = (ArrayType)ReservedDataField.DataType;
        if (visitor is ArrayType.IVisitor arrayVisitor)
        {
            var data = ReservedData;
            arrayVisitor.BeginArray(ReservedDataField, type);
            for (var index = 0; index < data.Length; index++)
                UInt8Type.Accept(visitor, ReservedDataField, type.ElementType, ref data[index]);
            arrayVisitor.EndArray();
            ReservedData = data;
        }
        else
        {
            visitor.VisitUnknown(ReservedDataField, type);
        }
        AcceptHealth(visitor);
    }
}
