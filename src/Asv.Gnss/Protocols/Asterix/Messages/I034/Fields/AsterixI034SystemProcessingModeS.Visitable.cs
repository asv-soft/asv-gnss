using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI034SystemProcessingModeS : IVisitable
{
    private static readonly Field VisitorReducedRadarField = new Field.Builder().Name(nameof(ReducedRadar)).Title(nameof(ReducedRadar)).Description("ReducedRadar").DataType(BoolType.Default).Build();
    private static readonly Field VisitorClusterStateField = new Field.Builder().Name(nameof(ClusterState)).Title(nameof(ClusterState)).Description("ClusterState").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorReducedRadarField, VisitorClusterStateField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueReducedRadar = ReducedRadar;
        BoolType.Accept(visitor, VisitorReducedRadarField, ref valueReducedRadar);
        if (!EqualityComparer<bool>.Default.Equals(valueReducedRadar, ReducedRadar)) ReducedRadar = valueReducedRadar;
        var valueClusterState = ClusterState;
        BoolType.Accept(visitor, VisitorClusterStateField, ref valueClusterState);
        if (!EqualityComparer<bool>.Default.Equals(valueClusterState, ClusterState)) ClusterState = valueClusterState;
    }
}
