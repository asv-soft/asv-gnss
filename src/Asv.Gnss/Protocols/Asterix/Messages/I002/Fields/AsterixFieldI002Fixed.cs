namespace Asv.Gnss;

public abstract class AsterixFieldI002Fixed : AsterixField
{
    public override int Category => AsterixMessageI002.Category;

    public override void Accept(Asv.IO.IVisitor visitor)
    {
    }
}