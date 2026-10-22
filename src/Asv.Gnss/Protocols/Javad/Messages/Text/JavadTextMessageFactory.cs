using Asv.IO;

namespace Asv.Gnss;

/// <summary>Registers all native GREIS text messages and interactive/wrapper messages.</summary>
public static class JavadTextMessageFactory
{
    public static void Register(IProtocolMessageFactoryBuilder<JavadMessageBase, string> builder)
    {
        builder.Add<JavadDataLinkStatusMessage>();
        builder.Add<JavadGpsSatelliteStatusMessage>();
        builder.Add<JavadGalileoSatelliteStatusMessage>();
        builder.Add<JavadSbasSatelliteStatusMessage>();
        builder.Add<JavadGlonassSatelliteStatusMessage>();
        builder.Add<JavadQzssSatelliteStatusMessage>();
        builder.Add<JavadBeiDouSatelliteStatusMessage>();
        builder.Add<JavadIrnssSatelliteStatusMessage>();
        builder.Add<JavadLBandSatelliteStatusMessage>();
        builder.Add<JavadReferenceStationStatusMessage>();
        builder.Add<JavadCorrectionTextMessage>();
        builder.Add<JavadRaimMessage>();
        builder.Add<JavadNavigationPositionMessage>();
        builder.Add<JavadMapProjectionMessage>();
        builder.Add<JavadLeverArmPositionTextMessage>();
        builder.Add<JavadQuasiGeographicPositionMessage>();
        builder.Add<JavadTimeResidualsMessage>();
        builder.Add<JavadClockOffsetsMessage>();
        builder.Add<JavadReferenceStationParametersMessage>();
        builder.Add<JavadRtkParametersMessage>();
        builder.Add<JavadPositionCovarianceTextMessage>();
        builder.Add<JavadBaselineTextMessage>();
        builder.Add<JavadImuAttitudeMessage>();
        builder.Add<JavadImuMeasurementsMessage>();
        builder.Add<JavadImuAntennaVelocityMessage>();
        builder.Add<JavadTextDataMessage>();
        builder.Add<JavadRtcmStatusMessage>();
        builder.Add<JavadReplyMessage>();
        builder.Add<JavadErrorMessage>();
        builder.Add<JavadParametersMessage>();
        builder.Add<JavadWrapperMessage>();
        builder.Add<JavadEventMessage>();
    }
}
