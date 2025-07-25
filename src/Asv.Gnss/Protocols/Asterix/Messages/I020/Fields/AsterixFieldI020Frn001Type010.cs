using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Data Item I020/010 – Data Source Identifier.
///
/// FRN: 01
/// Definition: Identification of the system from which the data are received.
/// Format: Two-octet fixed length data item.
/// 
/// Structure:
/// Octet 1 (bits 16–9): SAC – System Area Code
/// Octet 2 (bits 8–1):  SIC – System Identification Code
/// 
/// Encoding Rule: This data item shall be present in each ASTERIX record.
/// 
/// NOTE: The up-to-date list of SAC values is published on the EUROCONTROL website.
/// </summary>
public class AsterixFieldI020Frn001Type010 : AsterixField
{
    public const byte StaticFrn = 1;
    public override string Name => "Data Source Identifier";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    private static readonly Field SacField = new Field.Builder()
        .Name(nameof(Sac))
        .DataType(Int8Type.Default)
        .Title("SAC")
        .Enum(SystemAreaCodeMixin.GetSystemAreaCodes())
        .Description("System Area Code")
        .Build();

    public SystemAreaCode Sac { get; set; }

    private static readonly Field SicField = new Field.Builder()
        .Name(nameof(Sic))
        .DataType(Int8Type.Default)
        .Title("SIC")
        .Description("System Identification Code")
        .Build();
    private byte _sic;
    public byte Sic
    {
        get => _sic;
        set => _sic = value;
    }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Sac = (SystemAreaCode)buffer[0];
        Sic = buffer[1];
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Sac;
        buffer[1] = Sic;
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;

    public override void Accept(IVisitor visitor)
    {
        var temp = (byte)Sac;
        UInt8Type.Accept(visitor, SacField, SacField.DataType, ref temp);
        Sac = (SystemAreaCode)temp;
        UInt8Type.Accept(visitor, SicField, SicField.DataType, ref _sic);
    }
}

public enum SystemAreaCode : byte
{
    LocalAirport       = 0x00, // Local use
    Eurocontrol        = 0x01, // EUROCONTROL central
    Greece             = 0x02,
    Netherlands        = 0x04,
    Belgium            = 0x06,
    NATO               = 0x07,
    FranceCivil        = 0x08,
    FranceMilitary     = 0x09,
    Ukraine            = 0x11,
    Monaco             = 0x12,
    Spain              = 0x14,
    Hungary            = 0x16,
    BosniaHerzegovina  = 0x17,
    NorthMacedonia     = 0x18,
    Croatia            = 0x19,
    Serbia             = 0x20,
    Montenegro         = 0x21,
    Italy              = 0x22,
    Albania            = 0x24,
    Romania            = 0x26,
    Switzerland        = 0x28,
    SlovakRepublic     = 0x30,
    CzechRepublic      = 0x31,
    Austria            = 0x32,
    UnitedKingdomCivil = 0x34,
    UnitedKingdom1     = 0x35,
    UnitedKingdom2     = 0x36,
    DenmarkCivil       = 0x38,
    DenmarkMilitary    = 0x39,
    SwedenCivil        = 0x40,
    SwedenMilitary     = 0x41,
    NorwayCivil        = 0x42,
    NorwayMilitary     = 0x43,
    FinlandCivil       = 0x44,
    FinlandMilitary    = 0x45,
    Lithuania          = 0x46,
    Latvia             = 0x47,
    Estonia            = 0x48,
    RussiaNorthWest    = 0x49,
    RussiaCentral      = 0x50,
    RussiaFarEast      = 0x51,
    Tajikistan         = 0x52,
    RussiaSouthCaucasus= 0x53,
    RussiaVolga        = 0x54,
    RussiaUrals        = 0x55,
    RussiaSiberian     = 0x56,
    Belarus            = 0x57,
    Poland             = 0x60,
    GermanyCivil       = 0x61,
    GermanySec         = 0x62,
    GermanyWeather     = 0x63,
    GermanyMilitary    = 0x64,
    Algeria            = 0x65,
    Morocco            = 0x66,
    Tunisia            = 0x67,
    Portugal           = 0x68,
    Luxembourg         = 0x70,
    Ireland            = 0x72,
    Iceland            = 0x74,
    Malta              = 0x78,
    Cyprus             = 0x80,
    Armenia            = 0x82,
    Bulgaria           = 0x84,
    TurkeyMilitary     = 0x85,
    TurkeyCivil        = 0x86,
    TurkeyMilitary2    = 0x87,
    Georgia            = 0x88,
    Turkmenistan       = 0x90,
    Slovenia           = 0x93,
    Moldova            = 0x94,
    Uzbekistan         = 0x95,
    Kazakhstan         = 0x97,
    KyrgyzRepublic     = 0x98,
    Azerbaijan         = 0x99,
    Israel             = 0xDF
}

public static class SystemAreaCodeMixin
{
    public static IEnumerable<EnumValue<SystemAreaCode>> GetSystemAreaCodes()
    {
        foreach (var code in Enum.GetValues<SystemAreaCode>())
        {
            yield return new EnumValue<SystemAreaCode>(code, code.ToString());
        }
    }
}