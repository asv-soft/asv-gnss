# GNSS endpoint devices

The user-approved identity is one GNSS receiver per endpoint. Keep `GnssDeviceId.AsString()` stable. A canonical address uses family `gnss-endpoint` and the exact endpoint identifier (ordinal, no normalization); it identifies a logical endpoint source, not a hardware serial number. Factory classification, source extraction and identity conversion share this rule. Unsupported packets and missing/blank endpoints fail without side effects. Create/update reject unsupported or mismatched inputs.

Device services cover NMEA, RTCM2, RTCM3, UBX (includingX20), SBF, ComNav binary/ASCII/simple-answer, JavadGREIS, ASV, GBAS and RINEX. Each wire family has one concrete microservice with a typed receive stream and typed Filter API. The three existing services retain their reactive properties and UBX commands. New clients expose existing decoded messages without inventing navigation computations or receiver commands. RINEX participates only when its records carry an explicit endpoint tag. ASTERIX and ADS-B are outside GNSSreceiver discovery.

The device initializes all services once and passes context/extenders through. The link uses context.TimeProvider, accepted traffic refreshes it, and disposing the device releases its link timer and services without disposing the shared connection. Address-only lookups never discover or refresh devices.

Validation: tests exercise every supported family, identical source/identityaddresses, invalidinputs, independent endpoints, mixed traffic, realDeviceExplorer lifecycle, stream isolation and sync/asyncdisposal. Tests are offline with in-memory connections and controlledtime; no hardware, sockets or sleep-based timing. User's pending dependency3.7.4 edit is preserved.

User correction: raw GPS and raw GLONASS are explicitly excluded from receiver discovery and microservices.
