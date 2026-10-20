using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class UbxNavSat
{
    private static readonly Field iTOWField = UbxVisitor.Field(nameof(iTOW), UInt64Type.Default, "GPS time of week of the navigation epoch. See the description of iTOW for details.", "ms");
    private static readonly Field VersionField = UbxVisitor.Field(nameof(Version), UInt8Type.Default, "Message version (0x01 for this version)");
    private static readonly Field NumSvsField = UbxVisitor.Field(nameof(NumSvs), UInt8Type.Default, "Number of satellites");
    private static readonly Field ItemsField = UbxVisitor.Field(nameof(Items), new ListType(UbxNavSatelliteItem.StructType, 0, 255), "Satellite records for this navigation epoch; one entry per satellite reported in numSvs.");

    /// <summary>Describes the public navigation payload fields exposed to visitors.</summary>
    public static StructType StructType { get; } = new([
        iTOWField,
        VersionField,
        NumSvsField,
        ItemsField
    ]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt64Type.Accept(visitor, iTOWField, iTOWField.DataType, ref _iTOW);
        UInt8Type.Accept(visitor, VersionField, VersionField.DataType, ref _version);
        UInt8Type.Accept(visitor, NumSvsField, NumSvsField.DataType, ref _numSvs);
        UbxVisitor.VisitArray(visitor, ItemsField, ref _items, static () => new UbxNavSatelliteItem(),
            static (IVisitor elementVisitor, Field elementField, IFieldType elementType, ref UbxNavSatelliteItem item) =>
            {
                if (item is not null)
                {
                    Asv.IO.StructType.Accept(elementVisitor, elementField, elementType, item);
                }
                else
                {
                    elementVisitor.VisitUnknown(elementField, elementType);
                }
            });
    }
}

public partial class UbxNavSatelliteItem : IVisitable
{
    private static readonly Field GnssTypeField = UbxVisitor.Enum<byte>(nameof(GnssType), UInt8Type.Default, "GNSS constellation identifier associated with this satellite.",
        new EnumValue<byte>((byte)UbxNavSatGnssId.GPS, nameof(UbxNavSatGnssId.GPS)),
        new EnumValue<byte>((byte)UbxNavSatGnssId.SBAS, nameof(UbxNavSatGnssId.SBAS)),
        new EnumValue<byte>((byte)UbxNavSatGnssId.Galileo, nameof(UbxNavSatGnssId.Galileo)),
        new EnumValue<byte>((byte)UbxNavSatGnssId.BeiDou, nameof(UbxNavSatGnssId.BeiDou)),
        new EnumValue<byte>((byte)UbxNavSatGnssId.IMES, nameof(UbxNavSatGnssId.IMES)),
        new EnumValue<byte>((byte)UbxNavSatGnssId.QZSS, nameof(UbxNavSatGnssId.QZSS)),
        new EnumValue<byte>((byte)UbxNavSatGnssId.GLONASS, nameof(UbxNavSatGnssId.GLONASS)));
    private static readonly Field GnssIdField = UbxVisitor.Field(nameof(GnssId), UInt8Type.Default, "GNSS identifier (see Satellite Numbering) for assignment");
    private static readonly Field SvIdField = UbxVisitor.Field(nameof(SvId), UInt8Type.Default, "Satellite identifier (see Satellite Numbering) for assignment");
    private static readonly Field CnobBHzField = UbxVisitor.Field(nameof(CnobBHz), UInt8Type.Default, "Carrier to noise ratio (signal strength)", "dB-Hz");
    private static readonly Field ElevDegField = UbxVisitor.Field(nameof(ElevDeg), Int8Type.Default, "Elevation (range: +/-90), unknown if out of range", "deg");
    private static readonly Field AzimDegField = UbxVisitor.Field(nameof(AzimDeg), Int16Type.Default, "Azimuth (range 0-360), unknown if elevation is out of range", "deg");
    private static readonly Field PrResMField = UbxVisitor.Field(nameof(PrResM), DoubleType.Default, "Pseudorange residual", "m");
    private static readonly Field QualityIndField = UbxVisitor.Enum<int>(nameof(QualityInd), Int32Type.Default, "Signal acquisition and tracking quality decoded from bits 0 through 2 of the satellite flags.",
        new EnumValue<int>((int)UbxQualityIndEnum.NoSignal, nameof(UbxQualityIndEnum.NoSignal)),
        new EnumValue<int>((int)UbxQualityIndEnum.SearchingSignal, nameof(UbxQualityIndEnum.SearchingSignal)),
        new EnumValue<int>((int)UbxQualityIndEnum.SignalAcquired, nameof(UbxQualityIndEnum.SignalAcquired)),
        new EnumValue<int>((int)UbxQualityIndEnum.SignalDetectedButUnusable, nameof(UbxQualityIndEnum.SignalDetectedButUnusable)),
        new EnumValue<int>((int)UbxQualityIndEnum.CodeLockedAndTimeSynchronized, nameof(UbxQualityIndEnum.CodeLockedAndTimeSynchronized)),
        new EnumValue<int>((int)UbxQualityIndEnum.CodeAndCarrierLockedAndTimeSynchronized1, nameof(UbxQualityIndEnum.CodeAndCarrierLockedAndTimeSynchronized1)),
        new EnumValue<int>((int)UbxQualityIndEnum.CodeAndCarrierLockedAndTimeSynchronized2, nameof(UbxQualityIndEnum.CodeAndCarrierLockedAndTimeSynchronized2)),
        new EnumValue<int>((int)UbxQualityIndEnum.CodeAndCarrierLockedAndTimeSynchronized3, nameof(UbxQualityIndEnum.CodeAndCarrierLockedAndTimeSynchronized3)));
    private static readonly Field SvUsedField = UbxVisitor.Field(nameof(SvUsed), BoolType.Default, "Signal in the subset specified in Signal Identifiers is currently being used for navigation");
    private static readonly Field HealthField = UbxVisitor.Enum<byte>(nameof(Health), UInt8Type.Default, "Satellite health decoded from bits 4 and 5 of the satellite flags; unknown health is distinct from healthy.",
        new EnumValue<byte>((byte)UbxHealthInd.Unknown, nameof(UbxHealthInd.Unknown)),
        new EnumValue<byte>((byte)UbxHealthInd.Healthy, nameof(UbxHealthInd.Healthy)),
        new EnumValue<byte>((byte)UbxHealthInd.Unhealthy, nameof(UbxHealthInd.Unhealthy)));
    private static readonly Field DiffCorrField = UbxVisitor.Field(nameof(DiffCorr), BoolType.Default, "differential correction data is available for this SV");
    private static readonly Field SmoothedField = UbxVisitor.Field(nameof(Smoothed), BoolType.Default, "Carrier smoothed pseudorange used");
    private static readonly Field OrbitSourceField = UbxVisitor.Enum<int>(nameof(OrbitSource), Int32Type.Default, "Orbit source",
        new EnumValue<int>((int)UbxOrbitSource.NoOrbitInformationIsAvailableForThisSV, nameof(UbxOrbitSource.NoOrbitInformationIsAvailableForThisSV)),
        new EnumValue<int>((int)UbxOrbitSource.EphemerisIsUsed, nameof(UbxOrbitSource.EphemerisIsUsed)),
        new EnumValue<int>((int)UbxOrbitSource.AlmanacIsUsed, nameof(UbxOrbitSource.AlmanacIsUsed)),
        new EnumValue<int>((int)UbxOrbitSource.AssistNowOfflineOrbitIsUsed, nameof(UbxOrbitSource.AssistNowOfflineOrbitIsUsed)),
        new EnumValue<int>((int)UbxOrbitSource.AssistNowAutonomousOrbitIsUsed, nameof(UbxOrbitSource.AssistNowAutonomousOrbitIsUsed)),
        new EnumValue<int>((int)UbxOrbitSource.OtherOrbitInformationIsUsed1, nameof(UbxOrbitSource.OtherOrbitInformationIsUsed1)),
        new EnumValue<int>((int)UbxOrbitSource.OtherOrbitInformationIsUsed2, nameof(UbxOrbitSource.OtherOrbitInformationIsUsed2)),
        new EnumValue<int>((int)UbxOrbitSource.OtherOrbitInformationIsUsed3, nameof(UbxOrbitSource.OtherOrbitInformationIsUsed3)));
    private static readonly Field IsephAvailField = UbxVisitor.Field(nameof(IsephAvail), BoolType.Default, "ephemeris is available for this SV");
    private static readonly Field IsalmAvailField = UbxVisitor.Field(nameof(IsalmAvail), BoolType.Default, "almanac is available for this SV");
    private static readonly Field IsanoAvailField = UbxVisitor.Field(nameof(IsanoAvail), BoolType.Default, "AssistNow Offline data is available for this SV");
    private static readonly Field IsaopAvailField = UbxVisitor.Field(nameof(IsaopAvail), BoolType.Default, "AssistNow Autonomous data is available for this SV");
    private static readonly Field IssbasCorrUsedField = UbxVisitor.Field(nameof(IssbasCorrUsed), BoolType.Default, "SBAS corrections have been used for a signal in the subset specified in Signal Identifiers");
    private static readonly Field IsrtcmCorrUsedField = UbxVisitor.Field(nameof(IsrtcmCorrUsed), BoolType.Default, "RTCM corrections have been used for a signal in the subset specified in Signal Identifiers");
    private static readonly Field IsslasCorrUsedField = UbxVisitor.Field(nameof(IsslasCorrUsed), BoolType.Default, "QZSS SLAS corrections have been used for a signal in the subset specified in Signal Identifiers");
    private static readonly Field IsspartnCorrUsedField = UbxVisitor.Field(nameof(IsspartnCorrUsed), BoolType.Default, "SPARTN corrections have been used for a signal in the subset specified in Signal Identifiers");
    private static readonly Field IsprCorrUsedField = UbxVisitor.Field(nameof(IsprCorrUsed), BoolType.Default, "Pseudorange corrections have been used for a signal in the subset specified in Signal Identifiers");
    private static readonly Field IscrCorrUsedField = UbxVisitor.Field(nameof(IscrCorrUsed), BoolType.Default, "Carrier range corrections have been used for a signal in the subset specified in Signal Identifiers");
    private static readonly Field IsdoCorrUsedField = UbxVisitor.Field(nameof(IsdoCorrUsed), BoolType.Default, "Range rate (Doppler) corrections have been used for a signal in the subset specified in Signal");

    /// <summary>Describes the public navigation payload fields exposed to visitors.</summary>
    public static StructType StructType { get; } = new([
        GnssTypeField,
        GnssIdField,
        SvIdField,
        CnobBHzField,
        ElevDegField,
        AzimDegField,
        PrResMField,
        QualityIndField,
        SvUsedField,
        HealthField,
        DiffCorrField,
        SmoothedField,
        OrbitSourceField,
        IsephAvailField,
        IsalmAvailField,
        IsanoAvailField,
        IsaopAvailField,
        IssbasCorrUsedField,
        IsrtcmCorrUsedField,
        IsslasCorrUsedField,
        IsspartnCorrUsedField,
        IsprCorrUsedField,
        IscrCorrUsedField,
        IsdoCorrUsedField
    ]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var gnssTypeValue = (byte)GnssType;
        UInt8Type.Accept(visitor, GnssTypeField, GnssTypeField.DataType, ref gnssTypeValue);
        GnssType = (UbxNavSatGnssId)gnssTypeValue;
        UInt8Type.Accept(visitor, GnssIdField, GnssIdField.DataType, ref _gnssId);
        UInt8Type.Accept(visitor, SvIdField, SvIdField.DataType, ref _svId);
        UInt8Type.Accept(visitor, CnobBHzField, CnobBHzField.DataType, ref _cnobBHz);
        Int8Type.Accept(visitor, ElevDegField, ElevDegField.DataType, ref _elevDeg);
        Int16Type.Accept(visitor, AzimDegField, AzimDegField.DataType, ref _azimDeg);
        DoubleType.Accept(visitor, PrResMField, PrResMField.DataType, ref _prResM);
        var qualityIndValue = (int)QualityInd;
        Int32Type.Accept(visitor, QualityIndField, QualityIndField.DataType, ref qualityIndValue);
        QualityInd = (UbxQualityIndEnum)qualityIndValue;
        BoolType.Accept(visitor, SvUsedField, SvUsedField.DataType, ref _svUsed);
        var healthValue = (byte)Health;
        UInt8Type.Accept(visitor, HealthField, HealthField.DataType, ref healthValue);
        Health = (UbxHealthInd)healthValue;
        BoolType.Accept(visitor, DiffCorrField, DiffCorrField.DataType, ref _diffCorr);
        BoolType.Accept(visitor, SmoothedField, SmoothedField.DataType, ref _smoothed);
        var orbitSourceValue = (int)OrbitSource;
        Int32Type.Accept(visitor, OrbitSourceField, OrbitSourceField.DataType, ref orbitSourceValue);
        OrbitSource = (UbxOrbitSource)orbitSourceValue;
        BoolType.Accept(visitor, IsephAvailField, IsephAvailField.DataType, ref _isephAvail);
        BoolType.Accept(visitor, IsalmAvailField, IsalmAvailField.DataType, ref _isalmAvail);
        BoolType.Accept(visitor, IsanoAvailField, IsanoAvailField.DataType, ref _isanoAvail);
        BoolType.Accept(visitor, IsaopAvailField, IsaopAvailField.DataType, ref _isaopAvail);
        BoolType.Accept(visitor, IssbasCorrUsedField, IssbasCorrUsedField.DataType, ref _issbasCorrUsed);
        BoolType.Accept(visitor, IsrtcmCorrUsedField, IsrtcmCorrUsedField.DataType, ref _isrtcmCorrUsed);
        BoolType.Accept(visitor, IsslasCorrUsedField, IsslasCorrUsedField.DataType, ref _isslasCorrUsed);
        BoolType.Accept(visitor, IsspartnCorrUsedField, IsspartnCorrUsedField.DataType, ref _isspartnCorrUsed);
        BoolType.Accept(visitor, IsprCorrUsedField, IsprCorrUsedField.DataType, ref _isprCorrUsed);
        BoolType.Accept(visitor, IscrCorrUsedField, IscrCorrUsedField.DataType, ref _iscrCorrUsed);
        BoolType.Accept(visitor, IsdoCorrUsedField, IsdoCorrUsedField.DataType, ref _isdoCorrUsed);
    }
}
