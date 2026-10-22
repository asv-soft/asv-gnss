using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Base class for ASTERIX CAT247 fields.
/// </summary>
public abstract class AsterixFieldI247 : AsterixField
{
    /// <inheritdoc />
    public override int Category => AsterixMessageI247.Category;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        System.ArgumentNullException.ThrowIfNull(visitor);
    }
}