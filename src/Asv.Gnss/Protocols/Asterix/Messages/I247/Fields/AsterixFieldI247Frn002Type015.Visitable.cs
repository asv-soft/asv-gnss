using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI247Frn002Type015
{
    private static readonly Field VisitorServiceIdentificationField = new Field.Builder().Name(nameof(ServiceIdentification)).Title(nameof(ServiceIdentification)).Description("ServiceIdentification").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorServiceIdentificationField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueServiceIdentification = ServiceIdentification;
        UInt8Type.Accept(visitor, VisitorServiceIdentificationField, ref valueServiceIdentification);
        if (!EqualityComparer<byte>.Default.Equals(valueServiceIdentification, ServiceIdentification)) ServiceIdentification = valueServiceIdentification;
    }
}
