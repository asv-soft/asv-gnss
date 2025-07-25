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
    public override string Name => "Target Report Descriptor";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    
    public override void Deserialize(ref ReadOnlySpan<byte> buffer) => RawValue.Deserialize(ref buffer);

    public override void Serialize(ref Span<byte> buffer) => RawValue.Serialize(ref buffer);

    public override int GetByteSize() => RawValue.GetByteSize();

    public override void Accept(IVisitor visitor)
    {
        var temp = Ssr;
        BoolType.Accept(visitor, SsrField, SsrField.DataType, ref temp);
        Ssr = temp;
        
        temp = Ms;
        BoolType.Accept(visitor, MsField, MsField.DataType, ref temp);
        Ms = temp;
        
        temp = Hf;
        BoolType.Accept(visitor, HfField, HfField.DataType, ref temp);
        Hf = temp;
        
        temp = Vdl4;
        BoolType.Accept(visitor, Vdl4Field, Vdl4Field.DataType, ref temp);
        Vdl4 = temp;
        
        temp = Uat;
        BoolType.Accept(visitor, UatField, UatField.DataType, ref temp);
        Uat = temp;
        
        temp = Dme;
        BoolType.Accept(visitor, DmeField, DmeField.DataType, ref temp);
        Dme = temp;
        
        temp = Ot;
        BoolType.Accept(visitor, OtField, OtField.DataType, ref temp);
        Ot = temp;

        if (Rab.HasValue)
        {
            temp = Rab.Value;
            BoolType.Accept(visitor, RabField, RabField.DataType, ref temp);
            Rab = temp;
        }
        
        if (Spi.HasValue)
        {
            temp = Spi.Value;
            BoolType.Accept(visitor, SpiField, SpiField.DataType, ref temp);
            Spi = temp;
        }
        
        if (Chn.HasValue)
        {
            temp = Chn.Value;
            BoolType.Accept(visitor, ChnField, ChnField.DataType, ref temp);
            Chn = temp;
        }
        
        if (Gbs.HasValue)
        {
            temp = Gbs.Value;
            BoolType.Accept(visitor, GbsField, GbsField.DataType, ref temp);
            Gbs = temp;
        }
        
        if (Crt.HasValue)
        {
            temp = Crt.Value;
            BoolType.Accept(visitor, CrtField, CrtField.DataType, ref temp);
            Crt = temp;
        }
        
        if (Sim.HasValue)
        {
            temp = Sim.Value;
            BoolType.Accept(visitor, SimField, SimField.DataType, ref temp);
            Sim = temp;
        }
        
        if (Tst.HasValue)
        {
            temp = Tst.Value;
            BoolType.Accept(visitor, TstField, TstField.DataType, ref temp);
            Tst = temp;
        }
    }

    public VariableLengthValue RawValue { get; } = new();
    
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
        get => RawValue[0,8] ?? throw new InvalidOperationException("SSR field is not set");
        set => RawValue[0,8] = value;
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
        get => RawValue[0,7] ?? throw new InvalidOperationException("MS field is not set");
        set => RawValue[0,7] = value;
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
    private bool Hf
    {
        get => RawValue[0,6] ?? throw new InvalidOperationException("HF field is not set");
        set => RawValue[0,6] = value;
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
    private bool Vdl4
    {
        get => RawValue[0,5] ?? throw new InvalidOperationException("VDL4 field is not set");
        set => RawValue[0,5] = value;
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
    private bool Uat
    {
        get => RawValue[0,4] ?? throw new InvalidOperationException("UAT field is not set");
        set => RawValue[0,4] = value;
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
    private bool Dme
    {
        get => RawValue[0,3] ?? throw new InvalidOperationException("DME field is not set");
        set => RawValue[0,3] = value;
    }

    private static readonly Field OtField = new Field.Builder()
        .Name(nameof(Ot))
        .Title("OT")
        .Description("Other Technology multilateration")
        .DataType(BoolType.Default)
        .Build();
    
    /// <summary>
    /// Other Technology multilateration
    /// </summary>
    private bool Ot
    {
        get => RawValue[0,2] ?? throw new InvalidOperationException("OT field is not set");
        set => RawValue[0,2] = value;
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
    private bool? Rab
    {
        get => RawValue[1,8];
        set => RawValue[1,8] = value;
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
    private bool? Spi
    {
        get => RawValue[1,7];
        set => RawValue[1,7] = value;
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
    private bool? Chn
    {
        get => RawValue[1,6];
        set => RawValue[1,6] = value;
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
    private bool? Gbs
    {
        get => RawValue[1,5];
        set => RawValue[1,5] = value;
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
    private bool? Crt
    {
        get => RawValue[1,4];
        set => RawValue[1,4] = value;
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
    private bool? Sim
    {
        get => RawValue[1,3];
        set => RawValue[1,3] = value;
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
    private bool? Tst
    {
        get => RawValue[1,2];
        set => RawValue[1,2] = value;
    }
    
}