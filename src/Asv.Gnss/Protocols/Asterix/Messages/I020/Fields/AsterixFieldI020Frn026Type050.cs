using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn026Type050 : AsterixField
{
    public const byte StaticFrn = 26;
    private const string StaticName = "Mode-2 Code in Octal Representation";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    private ushort _rawValue;

    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        ValidatedField,
        GarbledField,
        LocalTrackerField,
        Mode2CodeField
    ]);

    private static readonly Field ValidatedField = new Field.Builder()
        .Name(nameof(Validated))
        .Title("V")
        .Description("Code validation flag")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Code validation flag (bit 16)
    /// false = Code validated, true = Code not validated
    /// </summary>
    public bool Validated
    {
        get => (_rawValue & 0x8000) == 0; // Inverted logic: 0 = validated, 1 = not validated
        set => _rawValue = value ? (ushort)(_rawValue & 0x7FFF) : (ushort)(_rawValue | 0x8000);
    }

    private static readonly Field GarbledField = new Field.Builder()
        .Name(nameof(Garbled))
        .Title("G")
        .Description("Garbled code flag")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Garbled code flag (bit 15)
    /// false = Default, true = Garbled code
    /// </summary>
    public bool Garbled
    {
        get => (_rawValue & 0x4000) != 0;
        set
        {
            if (value)
                _rawValue |= 0x4000;
            else
                _rawValue &= 0xBFFF;
        }
    }

    private static readonly Field LocalTrackerField = new Field.Builder()
        .Name(nameof(LocalTracker))
        .Title("L")
        .Description("Local tracker flag")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Local tracker flag (bit 14)
    /// false = Mode-2 code derived from the reply of the transponder
    /// true = Smoothed Mode-2 code as provided by a local tracker
    /// </summary>
    public bool LocalTracker
    {
        get => (_rawValue & 0x2000) != 0;
        set
        {
            if (value)
                _rawValue |= 0x2000;
            else
                _rawValue &= 0xDFFF;
        }
    }

    private static readonly Field Mode2CodeField = new Field.Builder()
        .Name(nameof(Mode2Code))
        .Title("Mode-2 Code")
        .Description("Mode-2 code in octal representation (bits 12-1)")
        .DataType(UInt16Type.Default)
        .Build();

    /// <summary>
    /// Mode-2 code in binary representation (bits 12-1)
    /// </summary>
    public ushort Mode2Code
    {
        get => (ushort)(_rawValue & 0x0FFF);
        set => _rawValue = (ushort)((_rawValue & 0xF000) | (value & 0x0FFF));
    }

    /// <summary>
    /// Mode-2 code in octal representation (4 digits: A, B, C, D)
    /// </summary>
    public string Mode2CodeOctal
    {
        get
        {
            var code = Mode2Code;
            
            // Extract individual bits for each octal digit
            var a4 = (code >> 11) & 1;
            var a2 = (code >> 10) & 1;
            var a1 = (code >> 9) & 1;
            
            var b4 = (code >> 8) & 1;
            var b2 = (code >> 7) & 1;
            var b1 = (code >> 6) & 1;
            
            var c4 = (code >> 5) & 1;
            var c2 = (code >> 4) & 1;
            var c1 = (code >> 3) & 1;
            
            var d4 = (code >> 2) & 1;
            var d2 = (code >> 1) & 1;
            var d1 = code & 1;

            // Convert to octal digits
            var aDigit = (a4 << 2) | (a2 << 1) | a1;
            var bDigit = (b4 << 2) | (b2 << 1) | b1;
            var cDigit = (c4 << 2) | (c2 << 1) | c1;
            var dDigit = (d4 << 2) | (d2 << 1) | d1;

            return $"{aDigit}{bDigit}{cDigit}{dDigit}";
        }
    }

    /// <summary>
    /// Sets the Mode-2 code from an octal string representation
    /// </summary>
    /// <param name="octalCode">Four-digit octal code (e.g., "1234", "0567")</param>
    public void SetMode2CodeFromOctal(string octalCode)
    {
        if (string.IsNullOrEmpty(octalCode) || octalCode.Length != 4)
            throw new ArgumentException("Octal code must be exactly 4 digits", nameof(octalCode));

        var digits = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            if (!byte.TryParse(octalCode.Substring(i, 1), out digits[i]) || digits[i] > 7)
                throw new ArgumentException($"Octal digit at position {i + 1} must be 0-7", nameof(octalCode));
        }

        // Convert octal digits to binary
        var a4 = (digits[0] >> 2) & 1;
        var a2 = (digits[0] >> 1) & 1;
        var a1 = digits[0] & 1;

        var b4 = (digits[1] >> 2) & 1;
        var b2 = (digits[1] >> 1) & 1;
        var b1 = digits[1] & 1;

        var c4 = (digits[2] >> 2) & 1;
        var c2 = (digits[2] >> 1) & 1;
        var c1 = digits[2] & 1;

        var d4 = (digits[3] >> 2) & 1;
        var d2 = (digits[3] >> 1) & 1;
        var d1 = digits[3] & 1;

        Mode2Code = (ushort)((a4 << 11) | (a2 << 10) | (a1 << 9) |
                            (b4 << 8) | (b2 << 7) | (b1 << 6) |
                            (c4 << 5) | (c2 << 4) | (c1 << 3) |
                            (d4 << 2) | (d2 << 1) | d1);
    }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _rawValue = (ushort)((buffer[0] << 8) | buffer[1]);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)(_rawValue >> 8);
        buffer[1] = (byte)(_rawValue & 0xFF);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;

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

        var mode2CodeValue = Mode2Code;
        UInt16Type.Accept(visitor, Mode2CodeField, Mode2CodeField.DataType, ref mode2CodeValue);
        Mode2Code = mode2CodeValue;
    }
}