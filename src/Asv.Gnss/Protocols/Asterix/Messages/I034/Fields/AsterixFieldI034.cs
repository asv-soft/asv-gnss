using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Base class for ASTERIX CAT034 fields.
/// </summary>
public abstract class AsterixFieldI034 : AsterixField
{
    /// <inheritdoc />
    public override int Category => AsterixMessageI034.Category;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }
}