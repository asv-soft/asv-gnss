using System;

namespace Asv.Gnss;

/// <summary>One explicitly modeled payload variant in the u-blox X20 HPG 2.11 interface.</summary>
/// <param name="Id">Combined UBX class and message identifier.</param>
/// <param name="Name">Manufacturer message label.</param>
/// <param name="Variant">Manufacturer payload variant name; this is not necessarily the wire version byte.</param>
/// <param name="Direction">Manufacturer message direction or delivery mode.</param>
/// <param name="MessageType">Concrete typed payload model.</param>
/// <param name="Create">Creates an initialized instance of this payload variant.</param>
public sealed record UbxX20MessageDescriptor(
    ushort Id,
    string Name,
    string Variant,
    string Direction,
    Type MessageType,
    Func<UbxX20MessageBase> Create
);
