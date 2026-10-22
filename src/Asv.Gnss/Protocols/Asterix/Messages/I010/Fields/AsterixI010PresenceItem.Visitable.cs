using System;
using Asv.IO;

namespace Asv.Gnss;

public partial record struct AsterixI010PresenceItem : IVisitable
{
    private static readonly Field DrhoField = new Field.Builder().Name(nameof(Drho)).Title(nameof(Drho)).Description("Drho").DataType(Int8Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field DthetaField = new Field.Builder().Name(nameof(Dtheta)).Title(nameof(Dtheta)).Description("Dtheta").DataType(Int8Type.Default).Metadata("readOnly", true).Build();

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueDrho = Drho;
        Int8Type.Accept(visitor, DrhoField, ref valueDrho);
        var valueDtheta = Dtheta;
        Int8Type.Accept(visitor, DthetaField, ref valueDtheta);
    }
}
