using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>A GNSS receiver identity scoped to one exact endpoint.</summary>
public class GnssDeviceId : DeviceId, IEquatable<GnssDeviceId>, IEquatable<DeviceId>
{
    /// <summary>Stable device class used by existing receiver identities.</summary>
    public const string GnssDeviceClass = "Gnss receiver";

    /// <summary>Creates an identity without normalizing the endpoint's spelling or case.</summary>
    public GnssDeviceId(string endpointId) : base(GnssDeviceClass)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointId);
        EndpointId = endpointId;
    }

    /// <summary>The transport endpoint identifying this logical receiver.</summary>
    public string EndpointId { get; }

    /// <summary>Compares endpoint identities without folding case or normalizing transport names.</summary>
    public bool Equals(GnssDeviceId? other) =>
        other is not null && StringComparer.Ordinal.Equals(EndpointId, other.EndpointId);

    // DeviceId's default equality only considers the class. Collections typed as DeviceId
    // must use the same endpoint identity as object equality and the explorer's catalog.
    bool IEquatable<DeviceId>.Equals(DeviceId? other) => other is GnssDeviceId gnss && Equals(gnss);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is GnssDeviceId other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), StringComparer.Ordinal.GetHashCode(EndpointId));

    /// <summary>Compares endpoint identities using ordinal equality.</summary>
    public static bool operator ==(GnssDeviceId? left, GnssDeviceId? right) => Equals(left, right);

    /// <summary>Compares endpoint identities using ordinal inequality.</summary>
    public static bool operator !=(GnssDeviceId? left, GnssDeviceId? right) => !Equals(left, right);

    /// <inheritdoc />
    public override string AsString() => $"{DeviceClass}:{EndpointId}";
}
