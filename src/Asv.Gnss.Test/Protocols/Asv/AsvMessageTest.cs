using System;
using Xunit;

namespace Asv.Gnss.Test;

public class AsvMessageTest
{
    [Fact]
    public void AllLegacyV163AsvMessages_ShouldRoundTrip()
    {
        var random = new Random(163);
        AsvMessageBase[] messages =
        [
            new AsvMessageHeartBeat(),
            new AsvMessageGbasVdbSend(),
            new AsvMessageGbasVdbSendV2(),
            new AsvMessageGbasMonDevSendV2(),
            new AsvMessageGpsObservations(),
            new AsvMessageGloObservations(),
            new AsvMessageGpsRawCa(),
            new AsvMessageGloRawCa(),
            new AsvMessagePvtGeo(),
        ];

        foreach (var message in messages)
        {
            message.Randomize(random);
            var buffer = new byte[message.GetByteSize()];
            var write = buffer.AsSpan();
            message.Serialize(ref write);

            ReadOnlySpan<byte> read = buffer;
            var copy = (AsvMessageBase)Activator.CreateInstance(message.GetType())!;
            copy.Deserialize(ref read);

            Assert.Equal(0, read.Length);
            Assert.Equal(message.GetByteSize(), copy.GetByteSize());
            Assert.Equal(message.MessageId, copy.MessageId);
        }
    }

    [Fact]
    public void HeartBeat_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageHeartBeat
        {
            Sequence = 42,
            SenderId = 2,
            TargetId = 7,
            DeviceType = AsvDeviceType.GbasServer,
            DeviceState = AsvDeviceState.Active,
            Reserved1 = 1,
            Reserved2 = 2,
            Reserved3 = 3,
            Reserved4 = 4,
        };

        var copy = RoundTrip<AsvMessageHeartBeat>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.SenderId, copy.SenderId);
        Assert.Equal(origin.TargetId, copy.TargetId);
        Assert.Equal(origin.DeviceType, copy.DeviceType);
        Assert.Equal(origin.DeviceState, copy.DeviceState);
        Assert.Equal(origin.Reserved1, copy.Reserved1);
        Assert.Equal(origin.Reserved2, copy.Reserved2);
        Assert.Equal(origin.Reserved3, copy.Reserved3);
        Assert.Equal(origin.Reserved4, copy.Reserved4);
    }

    [Fact]
    public void HeartBeat_WithLegacyV1Buffer_ShouldParseAndSerializeByteForByte()
    {
        byte[] legacyBuffer =
        [
            0xAA, 0x44, 0x07, 0x00, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00,
            0x02, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0xF7, 0x1B
        ];

        ReadOnlySpan<byte> read = legacyBuffer;
        var message = new AsvMessageHeartBeat();
        message.Deserialize(ref read);

        Assert.Equal(0, read.Length);
        Assert.Equal(0, message.Sequence);
        Assert.Equal(2, message.SenderId);
        Assert.Equal(1, message.TargetId);
        Assert.Equal(AsvDeviceType.GbasModulator, message.DeviceType);
        Assert.Equal(AsvDeviceState.Active, message.DeviceState);

        var serialized = new byte[message.GetByteSize()];
        var write = serialized.AsSpan();
        message.Serialize(ref write);

        Assert.Equal(legacyBuffer, serialized);
    }

    [Fact]
    public void GbasVdbSend_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageGbasVdbSend
        {
            Sequence = 11,
            SenderId = 1,
            TargetId = 9,
            Slot = AsvGbasSlot.SlotA | AsvGbasSlot.SlotC,
            Msgs = AsvGbasMessage.Msg1 | AsvGbasMessage.Msg4,
            LastByteLength = 6,
            Data = [1, 2, 3, 4, 5],
        };

        var copy = RoundTrip<AsvMessageGbasVdbSend>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.Slot, copy.Slot);
        Assert.Equal(origin.Msgs, copy.Msgs);
        Assert.Equal(origin.LastByteLength, copy.LastByteLength);
        Assert.Equal(origin.Data, copy.Data);
    }

    [Fact]
    public void GbasVdbSendV2_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageGbasVdbSendV2
        {
            Sequence = 12,
            SenderId = 3,
            TargetId = 10,
            Slot = AsvGbasSlotMsg.SlotD,
            GbasMessageId = 17,
            ActiveSlots = AsvGbasSlot.SlotB | AsvGbasSlot.SlotD,
            LifeTime = 5,
            LastByteOffset = 3,
            ReservedFlags = 4,
            IsLastSlotInFrame = true,
            Data = [9, 8, 7],
        };

        var copy = RoundTrip<AsvMessageGbasVdbSendV2>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.Slot, copy.Slot);
        Assert.Equal(origin.GbasMessageId, copy.GbasMessageId);
        Assert.Equal(origin.ActiveSlots, copy.ActiveSlots);
        Assert.Equal(origin.LifeTime, copy.LifeTime);
        Assert.Equal(origin.LastByteOffset, copy.LastByteOffset);
        Assert.Equal(origin.ReservedFlags, copy.ReservedFlags);
        Assert.Equal(origin.IsLastSlotInFrame, copy.IsLastSlotInFrame);
        Assert.Equal(origin.Data, copy.Data);
    }

    [Fact]
    public void GbasMonDevSendV2_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageGbasMonDevSendV2
        {
            Sequence = 13,
            SenderId = 4,
            TargetId = 11,
            Slot = AsvGbasSlotMsg.SlotF,
            IsLastSlotInFrame = true,
            LifeTime = 6,
            MsgLength = 213,
            MsgCrc = 0xBEEF,
        };

        var copy = RoundTrip<AsvMessageGbasMonDevSendV2>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.SenderId, copy.SenderId);
        Assert.Equal(origin.TargetId, copy.TargetId);
        Assert.Equal(origin.Slot, copy.Slot);
        Assert.Equal(origin.IsLastSlotInFrame, copy.IsLastSlotInFrame);
        Assert.Equal(origin.LifeTime, copy.LifeTime);
        Assert.Equal(origin.MsgLength, copy.MsgLength);
        Assert.Equal(origin.MsgCrc, copy.MsgCrc);
    }

    [Fact]
    public void PvtGeo_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessagePvtGeo
        {
            Sequence = 14,
            SenderId = 5,
            TargetId = 12,
            Tow = new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc),
            PosType = AsvPosTypeEnum.DifferentialPvt,
            Error = 2,
            Latitude = 56.835,
            Longitude = 60.612,
            Height = 250.25,
            Undulation = -12.5,
            RxClkBias = 32 * GpsRawHelper.P2_30,
            RxClkDrift = -8 * GpsRawHelper.P2_30,
            TimeSystem = AsvTimeSystemEnum.Gps,
            Datum = AsvDatumEnum.WGS84,
            NrSv = 18,
            MeanCorrAge = 1.25,
            HAccuracy = 0.45,
            VAccuracy = 0.67,
        };

        var copy = RoundTrip<AsvMessagePvtGeo>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.Tow, copy.Tow);
        Assert.Equal(origin.PosType, copy.PosType);
        Assert.Equal(origin.Error, copy.Error);
        Assert.Equal(origin.Latitude, copy.Latitude, 7);
        Assert.Equal(origin.Longitude, copy.Longitude, 7);
        Assert.Equal(origin.Height, copy.Height, 2);
        Assert.Equal(origin.Undulation, copy.Undulation, 2);
        Assert.Equal(origin.RxClkBias, copy.RxClkBias, 12);
        Assert.Equal(origin.RxClkDrift, copy.RxClkDrift, 12);
        Assert.Equal(origin.TimeSystem, copy.TimeSystem);
        Assert.Equal(origin.Datum, copy.Datum);
        Assert.Equal(origin.NrSv, copy.NrSv);
        Assert.Equal(origin.MeanCorrAge, copy.MeanCorrAge, 2);
        Assert.Equal(origin.HAccuracy, copy.HAccuracy, 2);
        Assert.Equal(origin.VAccuracy, copy.VAccuracy, 2);
    }

    [Fact]
    public void GpsObservations_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageGpsObservations
        {
            Sequence = 15,
            SenderId = 6,
            TargetId = 13,
            Tow = new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc),
            TimeOffset = 5 * GpsRawHelper.P2_30,
            Observations =
            [
                new AsvGpsObservation
                {
                    Prn = 3,
                    L1Code = AsvHelper.CODE_L1C,
                    L1PseudoRange = 12 * AsvHelper.PRUNIT_GPS + 1234.56,
                    L1CarrierPhase = 20.5,
                    L1LockTime = 937,
                    ParticipationIndicator = true,
                    ReasonForException = ReasonForException.Raim,
                    Elevation = 35.4,
                    Azimuth = -124.8,
                    L1CNR = 42.25,
                }
            ],
        };

        var copy = RoundTrip<AsvMessageGpsObservations>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.Tow, copy.Tow);
        Assert.Equal(origin.TimeOffset, copy.TimeOffset, 12);
        Assert.Single(copy.Observations);
        AssertGpsObservation(origin.Observations[0], copy.Observations[0]);
    }

    [Fact]
    public void GloObservations_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageGloObservations
        {
            Sequence = 16,
            SenderId = 7,
            TargetId = 14,
            Tod = new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc),
            TimeOffset = -7 * GpsRawHelper.P2_30,
            Observations =
            [
                new AsvGloObservation
                {
                    Prn = 5,
                    L1Code = AsvHelper.CODE_L1C,
                    Frequency = 1602000000 + 2 * 562500,
                    L1PseudoRange = 3 * AsvHelper.PRUNIT_GLO + 2468.02,
                    L1CarrierPhase = 18.25,
                    L1LockTime = 937,
                    ParticipationIndicator = true,
                    ReasonForException = ReasonForException.Health,
                    Elevation = 41.2,
                    Azimuth = 87.6,
                    L1CNR = 38.5,
                }
            ],
        };

        var copy = RoundTrip<AsvMessageGloObservations>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.Tod, copy.Tod);
        Assert.Equal(origin.TimeOffset, copy.TimeOffset, 12);
        Assert.Single(copy.Observations);
        AssertGloObservation(origin.Observations[0], copy.Observations[0]);
    }

    [Fact]
    public void GpsRawCa_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageGpsRawCa();
        origin.Randomize(new Random(112));
        origin.Sequence = 17;
        origin.SenderId = 8;
        origin.TargetId = 15;
        origin.CrcPassed = true;

        var copy = RoundTrip<AsvMessageGpsRawCa>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.UtcTime, copy.UtcTime);
        Assert.Equal(origin.Prn, copy.Prn);
        Assert.Equal(origin.SatelliteId, copy.SatelliteId);
        Assert.Equal(origin.CrcPassed, copy.CrcPassed);
        Assert.Equal(origin.L1Code, copy.L1Code);
        Assert.Equal(origin.RinexSatCode, copy.RinexSatCode);
        Assert.Equal(origin.SignalType, copy.SignalType);
        Assert.Equal(origin.RindexSignalCode, copy.RindexSignalCode);
        Assert.Equal(origin.NAVBitsU32, copy.NAVBitsU32);

        var raw = copy.GetGnssRawNavMsg();
        Assert.Equal(NavSysEnum.GPS, raw.NavSystem);
        Assert.Equal(copy.Prn, raw.SatPrn);
        Assert.Equal(copy.NAVBitsU32, raw.RawData);
    }

    [Fact]
    public void GloRawCa_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageGloRawCa();
        origin.Randomize(new Random(113));
        origin.Sequence = 18;
        origin.SenderId = 9;
        origin.TargetId = 16;
        origin.CrcPassed = true;

        var copy = RoundTrip<AsvMessageGloRawCa>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.EpochTime, copy.EpochTime);
        Assert.Equal(origin.Prn, copy.Prn);
        Assert.Equal(origin.SatelliteId, copy.SatelliteId);
        Assert.Equal(origin.CrcPassed, copy.CrcPassed);
        Assert.Equal(origin.L1Code, copy.L1Code);
        Assert.Equal(origin.Frequency, copy.Frequency);
        Assert.Equal(origin.RinexSatCode, copy.RinexSatCode);
        Assert.Equal(origin.SignalType, copy.SignalType);
        Assert.Equal(origin.RindexSignalCode, copy.RindexSignalCode);
        Assert.Equal(origin.NAVBitsU32.Length, copy.NAVBitsU32.Length);
        for (var i = 0; i < origin.NAVBitsU32.Length; i++)
        {
            Assert.Equal(origin.NAVBitsU32[i], copy.NAVBitsU32[i]);
        }

        var raw = copy.GetGnssRawNavMsg();
        Assert.Equal(copy.NAVBitsU32.Length, raw.Length);
        Assert.All(raw, item =>
        {
            Assert.Equal(NavSysEnum.GLONASS, item.NavSystem);
            Assert.Equal(copy.Prn, item.SatPrn);
        });
    }

    private static T RoundTrip<T>(T origin)
        where T : AsvMessageBase, new()
    {
        var buffer = new byte[origin.GetByteSize()];
        var write = buffer.AsSpan();
        origin.Serialize(ref write);

        ReadOnlySpan<byte> read = buffer;
        var copy = new T();
        copy.Deserialize(ref read);
        Assert.Equal(0, read.Length);
        return copy;
    }

    private static void AssertGpsObservation(AsvGpsObservation origin, AsvGpsObservation copy)
    {
        Assert.Equal(origin.Prn, copy.Prn);
        Assert.Equal(AsvHelper.satno(NavigationSystemEnum.SYS_GPS, origin.Prn), copy.SatelliteId);
        Assert.Equal(origin.L1Code, copy.L1Code);
        Assert.Equal(origin.L1PseudoRange, copy.L1PseudoRange, 2);
        Assert.Equal(origin.L1CarrierPhase * 10, copy.L1CarrierPhase, 2);
        Assert.Equal(origin.L1LockTime, copy.L1LockTime);
        Assert.Equal(origin.ParticipationIndicator, copy.ParticipationIndicator);
        Assert.Equal(origin.ReasonForException, copy.ReasonForException);
        Assert.Equal(origin.Elevation, copy.Elevation, 1);
        Assert.Equal(origin.Azimuth, copy.Azimuth, 1);
        Assert.Equal(origin.L1CNR, copy.L1CNR, 2);
    }

    private static void AssertGloObservation(AsvGloObservation origin, AsvGloObservation copy)
    {
        Assert.Equal(origin.Prn, copy.Prn);
        Assert.Equal(AsvHelper.satno(NavigationSystemEnum.SYS_GLO, origin.Prn), copy.SatelliteId);
        Assert.Equal(origin.L1Code, copy.L1Code);
        Assert.Equal(origin.Frequency, copy.Frequency);
        Assert.Equal(origin.L1PseudoRange, copy.L1PseudoRange, 2);
        Assert.Equal(origin.L1CarrierPhase * 10, copy.L1CarrierPhase, 3);
        Assert.Equal(origin.L1LockTime, copy.L1LockTime);
        Assert.Equal(origin.ParticipationIndicator, copy.ParticipationIndicator);
        Assert.Equal(origin.ReasonForException, copy.ReasonForException);
        Assert.Equal(origin.Elevation, copy.Elevation, 1);
        Assert.Equal(origin.Azimuth, copy.Azimuth, 1);
        Assert.Equal(origin.L1CNR, copy.L1CNR, 2);
    }
}
