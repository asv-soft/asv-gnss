using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn007Type170 : AsterixField
{
    public const byte StaticFrn = 7;
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        CdmField,
        CnfField,
        TreField,
        CstField,
        CdmField,
        MahField,
        SthField,
        GhoField
    ]);
    
    public const string StaticName = "Track Status";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer) => RawValue.Deserialize(ref buffer);

    public override void Serialize(ref Span<byte> buffer) => RawValue.Serialize(ref buffer);

    public override int GetByteSize() => RawValue.GetByteSize();

    public override void Accept(IVisitor visitor)
    {
        var temp = (byte)Cdm;
        UInt8Type.Accept(visitor, CdmField, CdmField.DataType, ref temp);
        Cdm = (CdmEnum)temp;
        
        var boolTemp = Cnf;
        BoolType.Accept(visitor, CnfField, CnfField.DataType, ref boolTemp);
        Cnf = boolTemp;
        
        boolTemp = Tre;
        BoolType.Accept(visitor, TreField, TreField.DataType, ref boolTemp);
        Tre = boolTemp;
        
        boolTemp = Cst;
        BoolType.Accept(visitor, CstField, CstField.DataType, ref boolTemp);
        Cst = boolTemp;
        
        boolTemp = Mah;
        BoolType.Accept(visitor, MahField, MahField.DataType, ref boolTemp);
        Mah = boolTemp;
        
        boolTemp = Sth;
        BoolType.Accept(visitor, SthField, SthField.DataType, ref boolTemp);
        Sth = boolTemp;
        
        if (Gho.HasValue)
        {
            boolTemp = Gho.Value;
            BoolType.Accept(visitor, GhoField, GhoField.DataType, ref boolTemp);
            Gho = boolTemp;
        }
    }
    
    public VariableLengthValue RawValue { get; } = new();

    
    private static readonly Field CnfField = new Field.Builder()
        .Name(nameof(Cnf))
        .Title("CNF")
        .Description("0 Confirmed track, 1 Track in initiation phase")
        .DataType(BoolType.Default)
        .Build();
    /// <summary>
    /// bit-8 (CNF)
    /// = 0 Confirmed track
    /// = 1 Track in initiation phase
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public bool Cnf
    {
        get => RawValue[0] ?? throw new InvalidOperationException("Cnf is not initialized");
        set => RawValue[0] = value;
    }

    
    private static readonly Field TreField = new Field.Builder()
        .Name(nameof(Tre))
        .Title("TRE")
        .Description("0 Default, 1 Last report for a track")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Indicates the Track Termination Report (TRE) status:
    /// = 0 Default (not the last report for a track)
    /// = 1 Last report for a track
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the value of Tre is not initialized.</exception
    public bool Tre
    {
        get => RawValue[1] ?? throw new InvalidOperationException("Tre is not initialized");
        set => RawValue[1] = value;
    }
    
    private static readonly Field CstField = new Field.Builder()
        .Name(nameof(Cst))
        .Title("CST")
        .Description("0 Default, 1 Extrapolated")
        .DataType(BoolType.Default)
        .Build();
    public bool Cst 
    {
        get => RawValue[2] ?? throw new InvalidOperationException("Cst is not initialized");
        set => RawValue[2] = value;
    }
    
    private static readonly Field CdmField = new Field.Builder()
        .Name(nameof(Cdm))
        .DataType(Int8Type.Default)
        .Title("CDM")
        .Enum(CdmEnumMixin.GetCdmEnum())
        .Build();
    public CdmEnum Cdm
    {
        get
        {
            if (RawValue[3] == null || RawValue[4] == null)
                throw new InvalidOperationException("Cdm is not initialized");
            return (CdmEnum)((RawValue[3] ?? false ? 0 : 1) * 2  + (RawValue[4] ?? false ? 0 : 1));
        }
        set
        {
            if (RawValue[3] == null || RawValue[4] == null)
                throw new InvalidOperationException("Cdm is not initialized");
            var val = (int)value;
            RawValue[3] = (val / 2) % 2 == 0;
            RawValue[4] = val % 2 == 0;
        }
    }
    
    
    private static readonly Field MahField = new Field.Builder()
        .Name(nameof(Mah))
        .Title("MAH")
        .Description("0 Default, 1 Horizontal manoeuvre")
        .DataType(BoolType.Default)
        .Build();
    public bool Mah 
    {
        get => RawValue[5] ?? throw new InvalidOperationException("Mah is not initialized");
        set => RawValue[5] = value;
    }
    
    private static readonly Field SthField = new Field.Builder()
        .Name(nameof(Sth))
        .Title("STH")
        .Description("0 Measured position, 1 Smoothed position")
        .DataType(BoolType.Default)
        .Build();
    public bool Sth 
    {
        get => RawValue[6] ?? throw new InvalidOperationException("Sth is not initialized");
        set => RawValue[6] = value;
    }
    
    private static readonly Field GhoField = new Field.Builder()
        .Name(nameof(Gho))
        .Title("GHO")
        .Description("0 Default, 1 Ghost track")
        .DataType(BoolType.Default)
        .Build();
    
    public bool? Gho 
    {
        get => RawValue[7];
        set => RawValue[7] = value;
    }
    
    
    
    
}

public enum CdmEnum
{
    Maintaining = 0,
    Climbing = 1,
    Descending = 2,
    Invalid = 3,
}

public static class CdmEnumMixin
{
    public static IEnumerable<EnumValue<CdmEnum>> GetCdmEnum()
    {
        foreach (var code in Enum.GetValues<CdmEnum>())
        {
            yield return new EnumValue<CdmEnum>(code, code.ToString());
        }
    }
}