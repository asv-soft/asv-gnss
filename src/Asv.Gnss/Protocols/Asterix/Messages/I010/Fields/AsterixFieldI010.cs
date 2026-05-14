using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Base class for ASTERIX CAT010 fields.
/// </summary>
public abstract class AsterixFieldI010 : AsterixField
{
    /// <inheritdoc />
    public override int Category => AsterixMessageI010.Category;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }
}