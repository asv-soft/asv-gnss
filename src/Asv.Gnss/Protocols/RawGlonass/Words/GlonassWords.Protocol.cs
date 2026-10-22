using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class GlonassWord1
{
    private static readonly Field P1Field = GlonassWordEncoding.Field(
        nameof(P1),
        UInt8Type.Default
    );
    private static readonly Field TofLocalSecField = GlonassWordEncoding.Field(
        nameof(TofLocalSec),
        DoubleType.Default,
        "s"
    );
    private static readonly Field VelocityXField = GlonassWordEncoding.Field(
        nameof(VelocityX),
        DoubleType.Default,
        "m/s"
    );
    private static readonly Field AccelerationXField = GlonassWordEncoding.Field(
        nameof(AccelerationX),
        DoubleType.Default,
        "m/s^2"
    );
    private static readonly Field PositionXField = GlonassWordEncoding.Field(
        nameof(PositionX),
        DoubleType.Default,
        "m"
    );

    protected override void SerializeContent(Span<byte> buffer)
    {
        GlonassWordEncoding.Unsigned(buffer, 10, 2, P1, nameof(P1));
        GlonassWordEncoding.Time(buffer, TofLocalSec);
        GlonassWordEncoding.Signed(
            buffer,
            24,
            24,
            VelocityX,
            GlonassRawHelper.P2_20 * 1E3,
            nameof(VelocityX)
        );
        GlonassWordEncoding.Signed(
            buffer,
            48,
            5,
            AccelerationX,
            GlonassRawHelper.P2_30 * 1E3,
            nameof(AccelerationX)
        );
        GlonassWordEncoding.Signed(
            buffer,
            53,
            27,
            PositionX,
            GlonassRawHelper.P2_11 * 1E3,
            nameof(PositionX)
        );
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, P1Field, ref _p1);
        DoubleType.Accept(visitor, TofLocalSecField, ref _tofLocalSec);
        DoubleType.Accept(visitor, VelocityXField, ref _velocityX);
        DoubleType.Accept(visitor, AccelerationXField, ref _accelerationX);
        DoubleType.Accept(visitor, PositionXField, ref _positionX);
    }
}

public partial class GlonassWord2
{
    private static readonly Field HealthField = GlonassWordEncoding.Field(
        nameof(Health),
        UInt8Type.Default
    );
    private static readonly Field P2Field = GlonassWordEncoding.Field(
        nameof(P2),
        UInt8Type.Default
    );
    private static readonly Field TbField = GlonassWordEncoding.Field(
        nameof(Tb),
        UInt8Type.Default
    );
    private static readonly Field VelocityYField = GlonassWordEncoding.Field(
        nameof(VelocityY),
        DoubleType.Default,
        "m/s"
    );
    private static readonly Field AccelerationYField = GlonassWordEncoding.Field(
        nameof(AccelerationY),
        DoubleType.Default,
        "m/s^2"
    );
    private static readonly Field PositionYField = GlonassWordEncoding.Field(
        nameof(PositionY),
        DoubleType.Default,
        "m"
    );

    protected override void SerializeContent(Span<byte> buffer)
    {
        GlonassWordEncoding.Unsigned(buffer, 8, 3, Health, nameof(Health));
        GlonassWordEncoding.Unsigned(buffer, 11, 1, P2, nameof(P2));
        GlonassWordEncoding.Unsigned(buffer, 12, 7, Tb, nameof(Tb));
        GlonassWordEncoding.Signed(
            buffer,
            24,
            24,
            VelocityY,
            GlonassRawHelper.P2_20 * 1E3,
            nameof(VelocityY)
        );
        GlonassWordEncoding.Signed(
            buffer,
            48,
            5,
            AccelerationY,
            GlonassRawHelper.P2_30 * 1E3,
            nameof(AccelerationY)
        );
        GlonassWordEncoding.Signed(
            buffer,
            53,
            27,
            PositionY,
            GlonassRawHelper.P2_11 * 1E3,
            nameof(PositionY)
        );
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, HealthField, ref _health);
        UInt8Type.Accept(visitor, P2Field, ref _p2);
        UInt8Type.Accept(visitor, TbField, ref _tb);
        DoubleType.Accept(visitor, VelocityYField, ref _velocityY);
        DoubleType.Accept(visitor, AccelerationYField, ref _accelerationY);
        DoubleType.Accept(visitor, PositionYField, ref _positionY);
    }
}

public partial class GlonassWord3
{
    private static readonly Field P3Field = GlonassWordEncoding.Field(
        nameof(P3),
        UInt8Type.Default
    );
    private static readonly Field GamNField = GlonassWordEncoding.Field(
        nameof(GamN),
        DoubleType.Default
    );
    private static readonly Field PField = GlonassWordEncoding.Field(nameof(P), UInt8Type.Default);
    private static readonly Field lnField = GlonassWordEncoding.Field(
        nameof(ln),
        UInt8Type.Default
    );
    private static readonly Field VelocityZField = GlonassWordEncoding.Field(
        nameof(VelocityZ),
        DoubleType.Default,
        "m/s"
    );
    private static readonly Field AccelerationZField = GlonassWordEncoding.Field(
        nameof(AccelerationZ),
        DoubleType.Default,
        "m/s^2"
    );
    private static readonly Field PositionZField = GlonassWordEncoding.Field(
        nameof(PositionZ),
        DoubleType.Default,
        "m"
    );

    protected override void SerializeContent(Span<byte> buffer)
    {
        GlonassWordEncoding.Unsigned(buffer, 8, 1, P3, nameof(P3));
        GlonassWordEncoding.Signed(buffer, 9, 11, GamN, GlonassRawHelper.P2_40, nameof(GamN));
        GlonassWordEncoding.Unsigned(buffer, 21, 2, P, nameof(P));
        GlonassWordEncoding.Unsigned(buffer, 23, 1, ln, nameof(ln));
        GlonassWordEncoding.Signed(
            buffer,
            24,
            24,
            VelocityZ,
            GlonassRawHelper.P2_20 * 1E3,
            nameof(VelocityZ)
        );
        GlonassWordEncoding.Signed(
            buffer,
            48,
            5,
            AccelerationZ,
            GlonassRawHelper.P2_30 * 1E3,
            nameof(AccelerationZ)
        );
        GlonassWordEncoding.Signed(
            buffer,
            53,
            27,
            PositionZ,
            GlonassRawHelper.P2_11 * 1E3,
            nameof(PositionZ)
        );
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, P3Field, ref _p3);
        DoubleType.Accept(visitor, GamNField, ref _gamN);
        UInt8Type.Accept(visitor, PField, ref _p);
        UInt8Type.Accept(visitor, lnField, ref _ln);
        DoubleType.Accept(visitor, VelocityZField, ref _velocityZ);
        DoubleType.Accept(visitor, AccelerationZField, ref _accelerationZ);
        DoubleType.Accept(visitor, PositionZField, ref _positionZ);
    }
}

public partial class GlonassWord4
{
    private static readonly Field TauNField = GlonassWordEncoding.Field(
        nameof(TauN),
        DoubleType.Default,
        "s"
    );
    private static readonly Field DelayTauNField = GlonassWordEncoding.Field(
        nameof(DelayTauN),
        DoubleType.Default,
        "s"
    );
    private static readonly Field AgeField = GlonassWordEncoding.Field(
        nameof(Age),
        UInt8Type.Default
    );
    private static readonly Field P4Field = GlonassWordEncoding.Field(
        nameof(P4),
        UInt8Type.Default
    );
    private static readonly Field AccuracyField = GlonassWordEncoding.Field(
        nameof(Accuracy),
        DoubleType.Default,
        "m"
    );
    private static readonly Field NtField = GlonassWordEncoding.Field(
        nameof(Nt),
        UInt16Type.Default
    );
    private static readonly Field SlotField = GlonassWordEncoding.Field(
        nameof(Slot),
        UInt8Type.Default
    );
    private static readonly Field MField = GlonassWordEncoding.Field(nameof(M), UInt8Type.Default);

    protected override void SerializeContent(Span<byte> buffer)
    {
        GlonassWordEncoding.Signed(buffer, 8, 22, TauN, GlonassRawHelper.P2_30, nameof(TauN));
        GlonassWordEncoding.Signed(
            buffer,
            30,
            5,
            DelayTauN,
            GlonassRawHelper.P2_30,
            nameof(DelayTauN)
        );
        GlonassWordEncoding.Unsigned(buffer, 35, 5, Age, nameof(Age));
        GlonassWordEncoding.Unsigned(buffer, 54, 1, P4, nameof(P4));
        GlonassWordEncoding.Accuracy(buffer, Accuracy);
        GlonassWordEncoding.Unsigned(buffer, 62, 11, Nt, nameof(Nt));
        GlonassWordEncoding.Unsigned(buffer, 73, 5, Slot, nameof(Slot));
        GlonassWordEncoding.Unsigned(buffer, 78, 2, M, nameof(M));
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        DoubleType.Accept(visitor, TauNField, ref _tauN);
        DoubleType.Accept(visitor, DelayTauNField, ref _delayTauN);
        UInt8Type.Accept(visitor, AgeField, ref _age);
        UInt8Type.Accept(visitor, P4Field, ref _p4);
        DoubleType.Accept(visitor, AccuracyField, ref _accuracy);
        UInt16Type.Accept(visitor, NtField, ref _nt);
        UInt8Type.Accept(visitor, SlotField, ref _slot);
        UInt8Type.Accept(visitor, MField, ref _m);
    }
}

public partial class GlonassWord5
{
    private static readonly Field NaField = GlonassWordEncoding.Field(
        nameof(Na),
        UInt16Type.Default
    );
    private static readonly Field TauCField = GlonassWordEncoding.Field(
        nameof(TauC),
        DoubleType.Default,
        "s"
    );
    private static readonly Field N4Field = GlonassWordEncoding.Field(
        nameof(N4),
        UInt8Type.Default
    );
    private static readonly Field TauGPSField = GlonassWordEncoding.Field(
        nameof(TauGPS),
        DoubleType.Default,
        "s"
    );
    private static readonly Field lnField = GlonassWordEncoding.Field(
        nameof(ln),
        UInt8Type.Default
    );

    protected override void SerializeContent(Span<byte> buffer)
    {
        GlonassWordEncoding.Unsigned(buffer, 8, 11, Na, nameof(Na));
        GlonassWordEncoding.Signed(buffer, 19, 32, TauC, GlonassRawHelper.P2_31, nameof(TauC));
        GlonassWordEncoding.Unsigned(buffer, 52, 5, N4, nameof(N4));
        GlonassWordEncoding.Signed(buffer, 57, 22, TauGPS, GlonassRawHelper.P2_30, nameof(TauGPS));
        GlonassWordEncoding.Unsigned(buffer, 79, 1, ln, nameof(ln));
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt16Type.Accept(visitor, NaField, ref _na);
        DoubleType.Accept(visitor, TauCField, ref _tauC);
        UInt8Type.Accept(visitor, N4Field, ref _n4);
        DoubleType.Accept(visitor, TauGPSField, ref _tauGPS);
        UInt8Type.Accept(visitor, lnField, ref _ln);
    }
}

public partial class GlonassWordEven
{
    private static readonly Field СField = GlonassWordEncoding.Field(nameof(С), UInt8Type.Default);
    private static readonly Field MField = GlonassWordEncoding.Field(nameof(M), UInt8Type.Default);
    private static readonly Field PrnField = GlonassWordEncoding.Field(
        nameof(Prn),
        UInt8Type.Default
    );
    private static readonly Field DelaT2Field = GlonassWordEncoding.Field(
        nameof(DelaT2),
        DoubleType.Default,
        "s"
    );
    private static readonly Field LOmegaField = GlonassWordEncoding.Field(
        nameof(LOmega),
        DoubleType.Default,
        "rad"
    );
    private static readonly Field iField = GlonassWordEncoding.Field(
        nameof(i),
        DoubleType.Default,
        "rad"
    );
    private static readonly Field eField = GlonassWordEncoding.Field(nameof(e), DoubleType.Default);

    protected override void SerializeContent(Span<byte> buffer)
    {
        GlonassWordEncoding.Unsigned(buffer, 8, 1, С, nameof(С));
        GlonassWordEncoding.Unsigned(buffer, 9, 2, M, nameof(M));
        GlonassWordEncoding.Unsigned(buffer, 11, 5, Prn, nameof(Prn));
        GlonassWordEncoding.Signed(buffer, 16, 10, DelaT2, GlonassRawHelper.P2_18, nameof(DelaT2));
        GlonassWordEncoding.Signed(
            buffer,
            26,
            21,
            LOmega,
            GlonassRawHelper.P2_20 * Math.PI,
            nameof(LOmega)
        );
        GlonassWordEncoding.Inclination(buffer, i);
        GlonassWordEncoding.ScaledUnsigned(buffer, 65, 15, e, GlonassRawHelper.P2_20, nameof(e));
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, СField, ref _с);
        UInt8Type.Accept(visitor, MField, ref _m);
        UInt8Type.Accept(visitor, PrnField, ref _prn);
        DoubleType.Accept(visitor, DelaT2Field, ref _delaT2);
        DoubleType.Accept(visitor, LOmegaField, ref _lOmega);
        DoubleType.Accept(visitor, iField, ref _i);
        DoubleType.Accept(visitor, eField, ref _e);
    }
}

public partial class GlonassWordOdd
{
    private static readonly Field lnField = GlonassWordEncoding.Field(
        nameof(ln),
        UInt8Type.Default
    );
    private static readonly Field omegaField = GlonassWordEncoding.Field(
        nameof(omega),
        DoubleType.Default,
        "rad"
    );
    private static readonly Field ToaSecField = GlonassWordEncoding.Field(
        nameof(ToaSec),
        DoubleType.Default,
        "s"
    );
    private static readonly Field DeltaTField = GlonassWordEncoding.Field(
        nameof(DeltaT),
        DoubleType.Default,
        "s"
    );
    private static readonly Field DeltaDTField = GlonassWordEncoding.Field(
        nameof(DeltaDT),
        DoubleType.Default,
        "s"
    );
    private static readonly Field FrequencyField = GlonassWordEncoding.Field(
        nameof(Frequency),
        Int64Type.Default,
        "Hz"
    );

    protected override void SerializeContent(Span<byte> buffer)
    {
        GlonassWordEncoding.Signed(
            buffer,
            8,
            16,
            omega,
            GlonassRawHelper.P2_15 * Math.PI,
            nameof(omega)
        );
        GlonassWordEncoding.ScaledUnsigned(
            buffer,
            24,
            21,
            ToaSec,
            GlonassRawHelper.P2_5,
            nameof(ToaSec)
        );
        GlonassWordEncoding.Signed(buffer, 45, 22, DeltaT, GlonassRawHelper.P2_9, nameof(DeltaT));
        GlonassWordEncoding.Signed(buffer, 67, 7, DeltaDT, GlonassRawHelper.P2_14, nameof(DeltaDT));
        GlonassWordEncoding.Frequency(buffer, Frequency);
        GlonassWordEncoding.Unsigned(buffer, 79, 1, ln, nameof(ln));
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        DoubleType.Accept(visitor, omegaField, ref _omega);
        DoubleType.Accept(visitor, ToaSecField, ref _toaSec);
        DoubleType.Accept(visitor, DeltaTField, ref _deltaT);
        DoubleType.Accept(visitor, DeltaDTField, ref _deltaDT);
        Int64Type.Accept(visitor, FrequencyField, ref _frequency);
        AcceptHealth(visitor);
    }

    protected void AcceptHealth(IVisitor visitor) => UInt8Type.Accept(visitor, lnField, ref _ln);
}
