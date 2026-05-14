using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Base class for ASTERIX CAT048 fields.
/// </summary>
public abstract class AsterixFieldI048 : AsterixField
{
    /// <inheritdoc />
    public override int Category => AsterixMessageI048.Category;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }
}