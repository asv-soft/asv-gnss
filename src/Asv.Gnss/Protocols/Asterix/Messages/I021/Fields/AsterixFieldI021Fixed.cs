using Asv.IO;

namespace Asv.Gnss;

public abstract class AsterixFieldI021Fixed : AsterixField
{
    public override int Category => AsterixMessageI021.Category;
    public override void Accept(IVisitor visitor)
    {
    }
}