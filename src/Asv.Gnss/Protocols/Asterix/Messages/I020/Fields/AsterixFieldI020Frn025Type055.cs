using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn025Type055 : AsterixField
{
    public const byte StaticFrn = 25;
    private const string StaticName = "Mode-1 Code in Octal Representation";
    
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    private byte _rawValue;

    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        ValidatedField,
        GarbledField,
        LocalTrackerField,
        Mode1CodeField
    ]);

    private static readonly Field ValidatedField = new Field.Builder()
        .Name(nameof(Validated))
        .Title("V")
        .Description("Code validation flag")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Code validation flag (bit 8)
    /// false = Code validated, true = Code not validated
    /// </summary>
    public bool Validated
    {
        get => (_rawValue & 0x80) == 0; // Inverted logic: 0 = validated, 1 = not validated
        set => _rawValue = value ? (byte)(_rawValue & 0x7F) : (byte)(_rawValue | 0x80);
    }

    private static readonly Field GarbledField = new Field.Builder()
        .Name(nameof(Garbled))
        .Title("G")
        .Description("Garbled code flag")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Garbled code flag (bit 7)
    /// false = Default, true = Garbled code
    /// </summary>
    public bool Garbled
    {
        get => (_rawValue & 0x40) != 0;
        set
        {
            if (value)
                _rawValue |= 0x40;
            else
                _rawValue &= 0xBF;
        }
    }

    private static readonly Field LocalTrackerField = new Field.Builder()
        .Name(nameof(LocalTracker))
        .Title("L")
        .Description("Local tracker flag")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Local tracker flag (bit 6)
    /// false = Mode-1 code derived from transponder reply
    /// true = Smoothed Mode-1 code from local tracker
    /// </summary>
    public bool LocalTracker
    {
        get => (_rawValue & 0x20) != 0;
        set
        {
            if (value)
                _rawValue |= 0x20;
            else
                _rawValue &= 0xDF;
        }
    }

    private static readonly Field Mode1CodeField = new Field.Builder()
        .Name(nameof(Mode1Code))
        .Title("Mode-1 Code")
        .Description("Mode-1 code in octal representation (bits 5-1)")
        .DataType(UInt8Type.Default)
        .Build();

    /// <summary>
    /// Mode-1 code in octal representation (bits 5-1)
    /// Valid range: 0-31 (5 bits)
    /// </summary>
    public byte Mode1Code
    {
        get => (byte)(_rawValue & 0x1F);
        set => _rawValue = (byte)((_rawValue & 0xE0) | (value & 0x1F));
    }

    /// <summary>
    /// Gets the Mode-1 code as an octal string representation
    /// </summary>
    public string Mode1CodeOctal
    {
        get
        {
            var code = Mode1Code;
            var a4 = (code >> 4) & 1;
            var a2 = (code >> 3) & 1;
            var a1 = (code >> 2) & 1;
            var b2 = (code >> 1) & 1;
            var b1 = code & 1;
            
            // Convert to octal: A4A2A1 and B2B1
            var aDigit = a4 * 4 + a2 * 2 + a1;
            var bDigit = b2 * 2 + b1;
            
            return $"{aDigit}{bDigit}";
        }
    }

    /// <summary>
    /// Sets the Mode-1 code from an octal string representation
    /// </summary>
    /// <param name="octalCode">Two-digit octal code (e.g., "17", "05")</param>
    public void SetMode1CodeFromOctal(string octalCode)
    {
        if (string.IsNullOrEmpty(octalCode) || octalCode.Length != 2)
            throw new ArgumentException("Octal code must be exactly 2 digits", nameof(octalCode));

        if (!byte.TryParse(octalCode.Substring(0, 1), out var aDigit) || aDigit > 7)
            throw new ArgumentException("First octal digit must be 0-7", nameof(octalCode));

        if (!byte.TryParse(octalCode.Substring(1, 1), out var bDigit) || bDigit > 7)
            throw new ArgumentException("Second octal digit must be 0-7", nameof(octalCode));

        // Convert octal digits to binary
        var a4 = (aDigit >> 2) & 1;
        var a2 = (aDigit >> 1) & 1;
        var a1 = aDigit & 1;
        var b2 = (bDigit >> 1) & 1;
        var b1 = bDigit & 1;

        Mode1Code = (byte)((a4 << 4) | (a2 << 3) | (a1 << 2) | (b2 << 1) | b1);
    }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _rawValue = buffer[0];
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = _rawValue;
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;

    public override void Accept(IVisitor visitor)
    {
        var validatedValue = Validated;
        BoolType.Accept(visitor, ValidatedField, ValidatedField.DataType, ref validatedValue);
        Validated = validatedValue;

        var garbledValue = Garbled;
        BoolType.Accept(visitor, GarbledField, GarbledField.DataType, ref garbledValue);
        Garbled = garbledValue;

        var localTrackerValue = LocalTracker;
        BoolType.Accept(visitor, LocalTrackerField, LocalTrackerField.DataType, ref localTrackerValue);
        LocalTracker = localTrackerValue;

        var mode1CodeValue = Mode1Code;
        UInt8Type.Accept(visitor, Mode1CodeField, Mode1CodeField.DataType, ref mode1CodeValue);
        Mode1Code = mode1CodeValue;
    }

    
}