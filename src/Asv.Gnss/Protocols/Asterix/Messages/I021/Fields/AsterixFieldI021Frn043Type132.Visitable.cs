using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn043Type132
{
    private static readonly Field VisitorMessageAmplitudeField = new Field.Builder().Name(nameof(MessageAmplitude)).Title(nameof(MessageAmplitude)).Description("MessageAmplitude").DataType(Int8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorMessageAmplitudeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueMessageAmplitude = MessageAmplitude;
        Int8Type.Accept(visitor, VisitorMessageAmplitudeField, ref valueMessageAmplitude);
        if (!EqualityComparer<sbyte>.Default.Equals(valueMessageAmplitude, MessageAmplitude)) MessageAmplitude = valueMessageAmplitude;
    }
}
