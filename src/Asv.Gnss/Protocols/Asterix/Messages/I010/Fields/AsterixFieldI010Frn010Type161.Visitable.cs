using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn010Type161
{
    private static readonly Field VisitorTrackNumberField = new Field.Builder().Name(nameof(TrackNumber)).Title(nameof(TrackNumber)).Description("TrackNumber").DataType(UInt16Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTrackNumberField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueTrackNumber = TrackNumber;
        UInt16Type.Accept(visitor, VisitorTrackNumberField, ref valueTrackNumber);
        if (!EqualityComparer<ushort>.Default.Equals(valueTrackNumber, TrackNumber)) TrackNumber = valueTrackNumber;
    }
}
