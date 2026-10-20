using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class UbxNavSbas
{
    private static readonly Field iTOWField = UbxVisitor.Field(nameof(iTOW), UInt32Type.Default, "GPS time of week of the navigation epoch.", "ms");
    private static readonly Field GeoField = UbxVisitor.Field(nameof(Geo), UInt8Type.Default, "PRN Number of the GEO where correction and integrity data is used from");
    private static readonly Field ModeField = UbxVisitor.Enum<int>(nameof(Mode), Int32Type.Default, "SBAS Mode 0 Disabled 1 Enabled integrity 3 Enabled test mode",
        new EnumValue<int>((int)UbxSbasMode.Disabled, nameof(UbxSbasMode.Disabled)),
        new EnumValue<int>((int)UbxSbasMode.EnabledIntegrity, nameof(UbxSbasMode.EnabledIntegrity)),
        new EnumValue<int>((int)UbxSbasMode.EnabledTestMode, nameof(UbxSbasMode.EnabledTestMode)));
    private static readonly Field SysField = UbxVisitor.Enum<int>(nameof(Sys), Int32Type.Default, "SBAS System (WAAS/EGNOS/...) -1 Unknown 0 WAAS 1 EGNOS 2 MSAS 3 GAGAN 16 GPS",
        new EnumValue<int>((int)UbxSbasSystem.Unknown, nameof(UbxSbasSystem.Unknown)),
        new EnumValue<int>((int)UbxSbasSystem.WAAS, nameof(UbxSbasSystem.WAAS)),
        new EnumValue<int>((int)UbxSbasSystem.EGNOS, nameof(UbxSbasSystem.EGNOS)),
        new EnumValue<int>((int)UbxSbasSystem.MSAS, nameof(UbxSbasSystem.MSAS)),
        new EnumValue<int>((int)UbxSbasSystem.GAGAN, nameof(UbxSbasSystem.GAGAN)),
        new EnumValue<int>((int)UbxSbasSystem.GPS, nameof(UbxSbasSystem.GPS)));
    private static readonly Field ServiceField = UbxVisitor.Field(nameof(Service), UInt8Type.Default, "SBAS Services available (see graphic below)");
    private static readonly Field IsRangingField = UbxVisitor.Field(nameof(IsRanging), BoolType.Default, "GEO may be used as ranging source");
    private static readonly Field IsCorrectionsField = UbxVisitor.Field(nameof(IsCorrections), BoolType.Default, "GEO is providing correction data");
    private static readonly Field IsIntegrityField = UbxVisitor.Field(nameof(IsIntegrity), BoolType.Default, "GEO is providing integrity");
    private static readonly Field IsTestmodeField = UbxVisitor.Field(nameof(IsTestmode), BoolType.Default, "GEO is in test mode");
    private static readonly Field IsBadField = UbxVisitor.Field(nameof(IsBad), BoolType.Default, "Problem with signal or broadcast data indicated");
    private static readonly Field CntField = UbxVisitor.Field(nameof(Cnt), UInt8Type.Default, "Number of SV data following");
    private static readonly Field StatusFlagsField = UbxVisitor.Field(nameof(StatusFlags), UInt8Type.Default, "SBAS status flags 0 = Unknown 1 = Integrity information is not available or SBAS integrity is not enabled 2 = Receiver uses only GPS satellites for which integrity information is available");
    private static readonly Field IntegrityUsedField = UbxVisitor.Enum<int>(nameof(IntegrityUsed), Int32Type.Default, "SBAS integrity-use state: 0 unknown, 1 unavailable or disabled, 2 only GPS satellites with integrity information are used.",
        new EnumValue<int>((int)UbxStatusFlags.Unknown, nameof(UbxStatusFlags.Unknown)),
        new EnumValue<int>((int)UbxStatusFlags.IntegrityUsed, nameof(UbxStatusFlags.IntegrityUsed)),
        new EnumValue<int>((int)UbxStatusFlags.GpsOnly, nameof(UbxStatusFlags.GpsOnly)));
    private static readonly Field Reserved1Field = UbxVisitor.Field(nameof(Reserved1), UInt16Type.Default, "Reserved 1");
    private static readonly Field SvDatasField = UbxVisitor.Field(nameof(SvDatas), new ListType(SvData.StructType, 0, 255), "Per-satellite SBAS correction and integrity records; the wire count determines the number of entries.");

    /// <summary>Describes the public navigation payload fields exposed to visitors.</summary>
    public static StructType StructType { get; } = new([
        iTOWField,
        GeoField,
        ModeField,
        SysField,
        ServiceField,
        IsRangingField,
        IsCorrectionsField,
        IsIntegrityField,
        IsTestmodeField,
        IsBadField,
        CntField,
        StatusFlagsField,
        IntegrityUsedField,
        Reserved1Field,
        SvDatasField
    ]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt32Type.Accept(visitor, iTOWField, iTOWField.DataType, ref _iTOW);
        UInt8Type.Accept(visitor, GeoField, GeoField.DataType, ref _geo);
        var modeValue = (int)Mode;
        Int32Type.Accept(visitor, ModeField, ModeField.DataType, ref modeValue);
        Mode = (UbxSbasMode)modeValue;
        var sysValue = (int)Sys;
        Int32Type.Accept(visitor, SysField, SysField.DataType, ref sysValue);
        Sys = (UbxSbasSystem)sysValue;
        UInt8Type.Accept(visitor, ServiceField, ServiceField.DataType, ref _service);
        BoolType.Accept(visitor, IsRangingField, IsRangingField.DataType, ref _isRanging);
        BoolType.Accept(visitor, IsCorrectionsField, IsCorrectionsField.DataType, ref _isCorrections);
        BoolType.Accept(visitor, IsIntegrityField, IsIntegrityField.DataType, ref _isIntegrity);
        BoolType.Accept(visitor, IsTestmodeField, IsTestmodeField.DataType, ref _isTestmode);
        BoolType.Accept(visitor, IsBadField, IsBadField.DataType, ref _isBad);
        UInt8Type.Accept(visitor, CntField, CntField.DataType, ref _cnt);
        UInt8Type.Accept(visitor, StatusFlagsField, StatusFlagsField.DataType, ref _statusFlags);
        var integrityUsedValue = (int)IntegrityUsed;
        Int32Type.Accept(visitor, IntegrityUsedField, IntegrityUsedField.DataType, ref integrityUsedValue);
        IntegrityUsed = (UbxStatusFlags)integrityUsedValue;
        UInt16Type.Accept(visitor, Reserved1Field, Reserved1Field.DataType, ref _reserved1);
        UbxVisitor.VisitArray(visitor, SvDatasField, ref _svDatas, static () => new SvData(),
            static (IVisitor elementVisitor, Field elementField, IFieldType elementType, ref SvData item) =>
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

public partial class SvData : IVisitable
{
    private static readonly Field SvIdField = UbxVisitor.Field(nameof(SvId), UInt8Type.Default, "SV ID");
    private static readonly Field FlagsField = UbxVisitor.Field(nameof(Flags), UInt8Type.Default, "Flags for this SV");
    private static readonly Field UdreField = UbxVisitor.Field(nameof(Udre), UInt8Type.Default, "Monitoring status");
    private static readonly Field SvSysField = UbxVisitor.Field(nameof(SvSys), UInt8Type.Default, "System (WAAS/EGNOS/...) same as SYS");
    private static readonly Field SvServiceField = UbxVisitor.Field(nameof(SvService), UInt8Type.Default, "Services available same as SERVICE");
    private static readonly Field IsRangingField = UbxVisitor.Field(nameof(IsRanging), BoolType.Default, "GEO may be used as ranging source");
    private static readonly Field IsCorrectionsField = UbxVisitor.Field(nameof(IsCorrections), BoolType.Default, "GEO is providing correction data");
    private static readonly Field IsIntegrityField = UbxVisitor.Field(nameof(IsIntegrity), BoolType.Default, "GEO is providing integrity");
    private static readonly Field IsTestmodeField = UbxVisitor.Field(nameof(IsTestmode), BoolType.Default, "GEO is in test mode");
    private static readonly Field IsBadField = UbxVisitor.Field(nameof(IsBad), BoolType.Default, "Problem with signal or broadcast data indicated");
    private static readonly Field Reserved2Field = UbxVisitor.Field(nameof(Reserved2), UInt8Type.Default, "Reserved 2");
    private static readonly Field PrcField = UbxVisitor.Field(nameof(Prc), Int16Type.Default, "Pseudo Range correction in [cm]", "cm");
    private static readonly Field Reserved3Field = UbxVisitor.Field(nameof(Reserved3), UInt16Type.Default, "Reserved 3");
    private static readonly Field IcField = UbxVisitor.Field(nameof(Ic), Int16Type.Default, "Ionosphere correction in [cm]", "cm");

    /// <summary>Describes the public navigation payload fields exposed to visitors.</summary>
    public static StructType StructType { get; } = new([
        SvIdField,
        FlagsField,
        UdreField,
        SvSysField,
        SvServiceField,
        IsRangingField,
        IsCorrectionsField,
        IsIntegrityField,
        IsTestmodeField,
        IsBadField,
        Reserved2Field,
        PrcField,
        Reserved3Field,
        IcField
    ]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        UInt8Type.Accept(visitor, SvIdField, SvIdField.DataType, ref _svId);
        UInt8Type.Accept(visitor, FlagsField, FlagsField.DataType, ref _flags);
        UInt8Type.Accept(visitor, UdreField, UdreField.DataType, ref _udre);
        UInt8Type.Accept(visitor, SvSysField, SvSysField.DataType, ref _svSys);
        UInt8Type.Accept(visitor, SvServiceField, SvServiceField.DataType, ref _svService);
        BoolType.Accept(visitor, IsRangingField, IsRangingField.DataType, ref _isRanging);
        BoolType.Accept(visitor, IsCorrectionsField, IsCorrectionsField.DataType, ref _isCorrections);
        BoolType.Accept(visitor, IsIntegrityField, IsIntegrityField.DataType, ref _isIntegrity);
        BoolType.Accept(visitor, IsTestmodeField, IsTestmodeField.DataType, ref _isTestmode);
        BoolType.Accept(visitor, IsBadField, IsBadField.DataType, ref _isBad);
        UInt8Type.Accept(visitor, Reserved2Field, Reserved2Field.DataType, ref _reserved2);
        Int16Type.Accept(visitor, PrcField, PrcField.DataType, ref _prc);
        UInt16Type.Accept(visitor, Reserved3Field, Reserved3Field.DataType, ref _reserved3);
        Int16Type.Accept(visitor, IcField, IcField.DataType, ref _ic);
    }
}
