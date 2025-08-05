using System;
using System.Collections;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Data Item I020/020 – Target Report Descriptor.
///
/// FRN: 02  
/// Definition: This data item contains information to classify the target report and to indicate the type of sensor(s) which generated the report.  
/// Format: One-octet to multi-octet fixed length data item depending on FX-bit (extension).  
/// 
/// Structure (Octet 1):
/// - Bits 8–7: Type of Report (e.g., SSR, PSR, COMB, etc.)  
/// - Bits 6–5: Transversal Info (e.g., simulation, test target, etc.)  
/// - Bits 4–3: Spare  
/// - Bit 2: Antenna / Message source  
/// - Bit 1: FX (extension indicator — if 1, next octet follows)
///
/// Encoding Rule: FX bit determines whether additional octets are appended.  
/// This field provides classification and source information for the target report.
/// </summary>
public class AsterixFieldI020Frn002Type020 : AsterixField
{
    public const byte StaticFrn = 2;
    
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        OtField,
        DmeField,
        UatField,
        Vdl4Field,
        HfField,
        MsField,
        SsrField,
        RabField,
        SpiField,
        ChnField,
        GbsField,
        CrtField,
        SimField,
        TstField
    ]);
    
    public const string StaticName = "Target Report Descriptor";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    
    public override void Deserialize(ref ReadOnlySpan<byte> buffer) => RawValue.Deserialize(ref buffer);

    public override void Serialize(ref Span<byte> buffer) => RawValue.Serialize(ref buffer);

    public override int GetByteSize() => RawValue.GetByteSize();

    public override void Accept(IVisitor visitor)
    {
        
        var temp = Ot;
        BoolType.Accept(visitor, OtField, ref temp);
        Ot = temp;
        
        temp = Dme;
        BoolType.Accept(visitor, DmeField, ref temp);
        Dme = temp;
        
        temp = Uat;
        BoolType.Accept(visitor, UatField, ref temp);
        Uat = temp;
        
        temp = Vdl4;
        BoolType.Accept(visitor, Vdl4Field, ref temp);
        Vdl4 = temp;
        
        temp = Hf;
        BoolType.Accept(visitor, HfField, ref temp);
        Hf = temp;
        
        temp = Ms;
        BoolType.Accept(visitor, MsField, ref temp);
        Ms = temp;
        
        temp = Ssr;
        BoolType.Accept(visitor, SsrField, ref temp);
        Ssr = temp;

        var nullableTemp = Tst;
        BoolOptionalType.Accept(visitor, TstField,ref nullableTemp);
        Tst = nullableTemp;
        
        nullableTemp = Sim;
        BoolOptionalType.Accept(visitor, SimField, ref nullableTemp);
        Sim = nullableTemp;
        
        nullableTemp = Crt;
        BoolOptionalType.Accept(visitor, CrtField, ref nullableTemp);
        Crt = nullableTemp;
        
        nullableTemp = Gbs;
        BoolOptionalType.Accept(visitor, GbsField, ref nullableTemp);
        Gbs = nullableTemp;
        
        nullableTemp = Chn;
        BoolOptionalType.Accept(visitor, ChnField, ref nullableTemp);
        Chn = nullableTemp;
        
        nullableTemp = Spi;
        BoolOptionalType.Accept(visitor, SpiField, ref nullableTemp);
        Spi = nullableTemp;
        
        nullableTemp = Rab;
        BoolOptionalType.Accept(visitor, RabField, ref nullableTemp);
        Rab = nullableTemp;
        
    }

    public VariableLengthValue RawValue { get; } = new();
    
    private static readonly Field OtField = new Field.Builder()
        .Name(nameof(Ot))
        .Title("OT")
        .Description("Other Technology multilateration")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// Other Technology multilateration
    /// </summary>
    public bool Ot
    {
        get => RawValue[6] ?? throw new InvalidOperationException("OT field is not set");
        set => RawValue[6] = value;
    }
    
    private static readonly Field DmeField = new Field.Builder()
        .Name(nameof(Dme))
        .Title("DME")
        .Description("DME/TACAN multilateration")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// DME/TACAN multilateration
    /// </summary>
    public bool Dme
    {
        get => RawValue[5] ?? throw new InvalidOperationException("DME field is not set");
        set => RawValue[5] = value;
    }
    
    private static readonly Field UatField = new Field.Builder()
        .Name(nameof(Uat))
        .Title("UAT")
        .Description("UAT multilateration")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// UAT multilateration
    /// </summary>
    public bool Uat
    {
        get => RawValue[4] ?? throw new InvalidOperationException("UAT field is not set");
        set => RawValue[4] = value;
    }
    
    
    private static readonly Field Vdl4Field = new Field.Builder()
        .Name(nameof(Vdl4))
        .Title("VDL4")
        .Description("VDL Mode 4 multilateration")
        .DataType(BoolType.Default)
        .Build();
    /// <summary>
    /// VDL Mode 4 multilateration
    /// </summary>
    public bool Vdl4
    {
        get => RawValue[3] ?? throw new InvalidOperationException("VDL4 field is not set");
        set => RawValue[3] = value;
    }
    
    private static readonly Field HfField = new Field.Builder()
        .Name(nameof(Hf))
        .Title("HF")
        .Description("HF multilateration")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// HF multilateration
    /// </summary>
    public bool Hf
    {
        get => RawValue[2] ?? throw new InvalidOperationException("HF field is not set");
        set => RawValue[2] = value;
    }
    
    private static readonly Field MsField = new Field.Builder()
        .Name(nameof(Ms))
        .DataType(BoolType.Default)
        .Title("MS")
        .Description("Mode-S 1090 MHz multilateration")
        .Build();
    /// <summary>
    /// Non-Mode S 1090MHz multilateration
    /// </summary>
    public bool Ms
    {
        get => RawValue[1] ?? throw new InvalidOperationException("MS field is not set");
        set => RawValue[1] = value;
    }
    
    private static readonly Field SsrField = new Field.Builder()
        .Name(nameof(Ssr))
        .DataType(BoolType.Default)
        .Title("SSR")
        .Description("Non-Mode S 1090MHz multilateration")
        .Build();
    /// <summary>
    /// Non-Mode S 1090MHz multilateration
    /// </summary>
    public bool Ssr
    {
        get => RawValue[0] ?? throw new InvalidOperationException("SSR field is not set");
        set => RawValue[0] = value;
    }
    
    private static readonly Field TstField = new Field.Builder()
        .Name(nameof(Tst))
        .Title("TST")
        .Description("Test")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// 0 Default
    /// 1 Test Target
    /// </summary>
    public bool? Tst
    {
        get => RawValue[13];
        set => RawValue[13] = value;
    }
    
    
    private static readonly Field SimField = new Field.Builder()
        .Name(nameof(Sim))
        .Title("SIM")
        .Description("Actual target report")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// 0 Actual target report
    /// 1 Simulated target report
    /// </summary>
    public bool? Sim
    {
        get => RawValue[12];
        set => RawValue[12] = value;
    }
    
    private static readonly Field CrtField = new Field.Builder()
        .Name(nameof(Crt))
        .Title("CRT")
        .Description("No Corrupted reply in multilateration")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// 0 No Corrupted reply in multilateration
    /// 1 Corrupted replies in multilateration
    /// </summary>
    public bool? Crt
    {
        get => RawValue[11];
        set => RawValue[11] = value;
    }
    
    private static readonly Field GbsField = new Field.Builder()
        .Name(nameof(Gbs))
        .Title("GBS")
        .Description("Transponder Ground bit not set")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// 0 Transponder Ground bit not set
    /// 1 Transponder Ground bit set
    /// </summary>
    public bool? Gbs
    {
        get => RawValue[10];
        set => RawValue[10] = value;
    }
    
    
    private static readonly Field ChnField = new Field.Builder()
        .Name(nameof(Chn))
        .Title("CHN")
        .Description("Chain or Chain2")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// 0 Chain
    /// 1 Chain 2
    /// </summary>
    public bool? Chn
    {
        get => RawValue[9];
        set => RawValue[9] = value;
    }
    
    private static readonly Field SpiField = new Field.Builder()
        .Name(nameof(Spi))
        .Title("SPI")
        .Description("Absence of SPI or Special Position Identification")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// 0 Absence of SPI
    /// 1 Special Position Identification
    /// </summary>
    public bool? Spi
    {
        get => RawValue[8];
        set => RawValue[8] = value;
    }
    
    private static readonly Field RabField = new Field.Builder()
        .Name(nameof(Rab))
        .Title("RAB")
        .Description("Report from target transponder vs Report from field monitor")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// 0 Report from target transponder
    /// 1 Report from field monitor (fixed transponder)
    /// </summary>
    public bool? Rab
    {
        get => RawValue[7];
        set => RawValue[7] = value;
    }
}