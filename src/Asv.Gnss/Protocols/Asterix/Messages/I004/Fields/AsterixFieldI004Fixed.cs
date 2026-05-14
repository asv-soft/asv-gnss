namespace Asv.Gnss;

public abstract class AsterixFieldI004Fixed : AsterixField
{
    public override int Category => AsterixMessageI004.Category;

    public override void Accept(Asv.IO.IVisitor visitor)
    {
    }
}