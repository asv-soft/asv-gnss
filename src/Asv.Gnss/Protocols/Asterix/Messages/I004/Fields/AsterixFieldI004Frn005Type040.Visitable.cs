using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI004Frn005Type040
{
    private static readonly Field VisitorAlertIdentifierField = new Field.Builder().Name(nameof(AlertIdentifier)).Title(nameof(AlertIdentifier)).Description("AlertIdentifier").DataType(UInt16Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAlertIdentifierField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueAlertIdentifier = AlertIdentifier;
        UInt16Type.Accept(visitor, VisitorAlertIdentifierField, ref valueAlertIdentifier);
        if (!EqualityComparer<ushort>.Default.Equals(valueAlertIdentifier, AlertIdentifier)) AlertIdentifier = valueAlertIdentifier;
    }
}
