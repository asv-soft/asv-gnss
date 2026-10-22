using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>Satellite status text shared by the eight native GREIS constellations.</summary>
public abstract partial class JavadSatelliteStatusMessage(string title, bool hasFrequencyChannel)
    : JavadTextMessage(title, "0")
{
    public bool HasFrequencyChannel => hasFrequencyChannel;
    public int SatelliteCount => Count(0);
    public ImmutableArray<JavadSatelliteTextStatus> Satellites
    {
        get =>
            Repeated(0, 1, hasFrequencyChannel ? 6 : 5)
                .Select(x => new JavadSatelliteTextStatus(x, hasFrequencyChannel))
                .ToImmutableArray();
        set => SetRepeated(0, 1, value.Select(x => x.Value).ToImmutableArray());
    }

    protected override void ValidateFields()
    {
        Validate("i");
        foreach (var value in Repeated(0, 1, hasFrequencyChannel ? 6 : 5))
        {
            JavadTextRecordValidation.Validate(value, hasFrequencyChannel ? "iiiili" : "iiili");
            _ = new JavadSatelliteTextStatus(value, hasFrequencyChannel).SignalToNoiseDbHz;
        }
    }
}

/// <summary>[GS] Gps satellite status. The documented IRNSS title is also CSSVST.</summary>
public sealed partial class JavadGpsSatelliteStatusMessage()
    : JavadSatelliteStatusMessage("GPSVST", false)
{
    public override string Id => "GS";
    public override string Name => "Gps SVs Status";
}

/// <summary>[ES] Galileo satellite status. The documented IRNSS title is also CSSVST.</summary>
public sealed partial class JavadGalileoSatelliteStatusMessage()
    : JavadSatelliteStatusMessage("ESSVST", false)
{
    public override string Id => "ES";
    public override string Name => "Galileo SVs Status";
}

/// <summary>[WS] Sbas satellite status. The documented IRNSS title is also CSSVST.</summary>
public sealed partial class JavadSbasSatelliteStatusMessage()
    : JavadSatelliteStatusMessage("WSSVST", false)
{
    public override string Id => "WS";
    public override string Name => "Sbas SVs Status";
}

/// <summary>[NS] Glonass satellite status. The documented IRNSS title is also CSSVST.</summary>
public sealed partial class JavadGlonassSatelliteStatusMessage()
    : JavadSatelliteStatusMessage("GLSVST", true)
{
    public override string Id => "NS";
    public override string Name => "Glonass SVs Status";
}

/// <summary>[QS] Qzss satellite status. The documented IRNSS title is also CSSVST.</summary>
public sealed partial class JavadQzssSatelliteStatusMessage()
    : JavadSatelliteStatusMessage("QSSVST", false)
{
    public override string Id => "QS";
    public override string Name => "Qzss SVs Status";
}

/// <summary>[CS] BeiDou satellite status. The documented IRNSS title is also CSSVST.</summary>
public sealed partial class JavadBeiDouSatelliteStatusMessage()
    : JavadSatelliteStatusMessage("CSSVST", false)
{
    public override string Id => "CS";
    public override string Name => "BeiDou SVs Status";
}

/// <summary>[Is] Irnss satellite status. The documented IRNSS title is also CSSVST.</summary>
public sealed partial class JavadIrnssSatelliteStatusMessage()
    : JavadSatelliteStatusMessage("CSSVST", false)
{
    public override string Id => "Is";
    public override string Name => "Irnss SVs Status";
}

/// <summary>[LS] LBand satellite status. The documented IRNSS title is also CSSVST.</summary>
public sealed partial class JavadLBandSatelliteStatusMessage()
    : JavadSatelliteStatusMessage("LSSVST", true)
{
    public override string Id => "LS";
    public override string Name => "LBand SVs Status";
}
