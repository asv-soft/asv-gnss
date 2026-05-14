using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// I247/550 Category Version Number Report.
/// </summary>
public class AsterixFieldI247Frn004Type550 : AsterixFieldI247
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 4;

    /// <inheritdoc />
    public override string Name => "Category Version Number Report";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Reported ASTERIX category versions.
    /// </summary>
    public List<AsterixCategoryVersion> Versions { get; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Versions.Clear();
        for (var i = 0; i < count; i++)
        {
            Versions.Add(new AsterixCategoryVersion
            {
                Category = buffer[0],
                Major = buffer[1],
                Minor = buffer[2]
            });
            buffer = buffer[3..];
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Versions.Count;
        buffer = buffer[1..];
        foreach (var version in Versions)
        {
            buffer[0] = version.Category;
            buffer[1] = version.Major;
            buffer[2] = version.Minor;
            buffer = buffer[3..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Versions.Count * 3;
}