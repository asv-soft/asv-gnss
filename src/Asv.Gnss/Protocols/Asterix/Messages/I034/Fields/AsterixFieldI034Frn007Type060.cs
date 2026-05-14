using System;

namespace Asv.Gnss;

/// <summary>
/// I034/060 System Processing Mode.
/// </summary>
public sealed class AsterixFieldI034Frn007Type060 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 7;

    /// <inheritdoc />
    public override string Name => "System Processing Mode";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Subfield selector.
    /// </summary>
    public AsterixI034StatusSubfieldSelector Selector { get; } = new();

    /// <summary>
    /// Common system processing mode subfield.
    /// </summary>
    public AsterixI034SystemProcessingCommon? Common { get; set; }

    /// <summary>
    /// PSR system processing mode subfield.
    /// </summary>
    public AsterixI034SystemProcessingPsr? Psr { get; set; }

    /// <summary>
    /// Mode-S system processing mode subfield.
    /// </summary>
    public AsterixI034SystemProcessingModeS? ModeS { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Selector.Raw = buffer[0];
        buffer = buffer[1..];
        if (Selector.HasCommonPart)
        {
            Common = new AsterixI034SystemProcessingCommon();
            Common.Deserialize(buffer[0]);
            buffer = buffer[1..];
        }

        if (Selector.HasPsrPart)
        {
            Psr = new AsterixI034SystemProcessingPsr();
            Psr.Deserialize(buffer[0]);
            buffer = buffer[1..];
        }

        if (Selector.HasModeSPart)
        {
            ModeS = new AsterixI034SystemProcessingModeS();
            ModeS.Deserialize(buffer[0]);
            buffer = buffer[1..];
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        Selector.HasCommonPart = Common != null;
        Selector.HasPsrPart = Psr != null;
        Selector.HasModeSPart = ModeS != null;
        Selector.HasExtension = false;
        buffer[0] = Selector.Raw;
        buffer = buffer[1..];
        if (Common != null)
        {
            buffer[0] = Common.Serialize();
            buffer = buffer[1..];
        }

        if (Psr != null)
        {
            buffer[0] = Psr.Serialize();
            buffer = buffer[1..];
        }

        if (ModeS != null)
        {
            buffer[0] = ModeS.Serialize();
            buffer = buffer[1..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + (Common != null ? 1 : 0) + (Psr != null ? 1 : 0) + (ModeS != null ? 1 : 0);
}