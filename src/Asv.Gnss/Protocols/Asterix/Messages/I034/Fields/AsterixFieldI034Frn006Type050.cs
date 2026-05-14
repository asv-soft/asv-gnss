using System;

namespace Asv.Gnss;

/// <summary>
/// I034/050 System Configuration and Status.
/// </summary>
public sealed class AsterixFieldI034Frn006Type050 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 6;

    /// <inheritdoc />
    public override string Name => "System Configuration and Status";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Subfield selector.
    /// </summary>
    public AsterixI034StatusSubfieldSelector Selector { get; } = new();

    /// <summary>
    /// Common system configuration and status subfield.
    /// </summary>
    public AsterixI034SystemConfigurationCommon? Common { get; set; }

    /// <summary>
    /// PSR system configuration and status subfield.
    /// </summary>
    public AsterixI034SystemConfigurationPsr? Psr { get; set; }

    /// <summary>
    /// Mode-S system configuration and status subfield.
    /// </summary>
    public AsterixI034SystemConfigurationModeS? ModeS { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Selector.Raw = buffer[0];
        buffer = buffer[1..];
        if (Selector.HasCommonPart)
        {
            Common = new AsterixI034SystemConfigurationCommon();
            Common.Deserialize(buffer[0]);
            buffer = buffer[1..];
        }

        if (Selector.HasPsrPart)
        {
            Psr = new AsterixI034SystemConfigurationPsr();
            Psr.Deserialize(buffer[0]);
            buffer = buffer[1..];
        }

        if (Selector.HasModeSPart)
        {
            ModeS = new AsterixI034SystemConfigurationModeS();
            ModeS.Deserialize(buffer[..2]);
            buffer = buffer[2..];
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
            ModeS.Serialize(buffer[..2]);
            buffer = buffer[2..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + (Common != null ? 1 : 0) + (Psr != null ? 1 : 0) + (ModeS != null ? 2 : 0);
}