using System;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxX20CompatibilityTest
{
    private const string LegacyPvt = "B56201075C00D06FB302E80701070C22380700000000000000000381000CC00B5A16E0732EEC40E20100A0860100000000000000000000000000000000000000000000000000000000000000000000000000000001000000000087D6120000000000AE8C";
    private const string X20Pvt = "B56201075C00D06FB302E80701070C22380700000000000000000381000CC00B5A16E0732EEC40E20100A0860100000000000000000000000000000000000000000000000000000000000000000000000000000019E00000000087D6120000000000A63C";

    [Theory]
    [InlineData(LegacyPvt, 0x0001, 0, false)]
    [InlineData(X20Pvt, 0xE019, 12, true)]
    public void Deserialize_LegacyAndX20Pvt_PreservesNavigationAndAdditionalFlags(
        string frame, ushort flags, byte correctionAge, bool authenticated)
    {
        var message = UbxTestHelper.Deserialize<UbxNavPvt>(Convert.FromHexString(frame));

        Assert.Equal(flags, message.Flags3);
        Assert.Equal(correctionAge, message.LastCorrectionAge);
        Assert.Equal(authenticated, message.IsTimeAuthenticated);
        Assert.Equal(authenticated, message.IsNmaFixVerified);
        Assert.False(message.IsValidLLH);
        Assert.Equal(-33.25, message.Latitude, 7);
        Assert.Equal(37.5, message.Longitude, 7);
        Assert.Equal(123.456, message.AltElipsoid, 3);
        Assert.True(message.GnssFixOK);
        Assert.Equal(UbxCarrierSolutionStatus.FixedAmbiguities, message.CarrierSolution);
        Assert.Equal(new DateTime(2024, 1, 7, 12, 34, 56, DateTimeKind.Utc), message.UtcTime);
    }

    [Fact]
    public void Deserialize_ReusedX20Pvt_ClearsAuthenticationAndCorrectionAgeForLegacyFrame()
    {
        var message = UbxTestHelper.Deserialize<UbxNavPvt>(Convert.FromHexString(X20Pvt));

        UbxTestHelper.Deserialize(message, Convert.FromHexString(LegacyPvt));

        Assert.Equal((ushort)1, message.Flags3);
        Assert.Equal((byte)0, message.LastCorrectionAge);
        Assert.False(message.IsTimeAuthenticated);
        Assert.False(message.IsNmaFixVerified);
    }

    [Fact]
    public void Accept_PvtFlags3_UsesDirectReferenceAndPreservesUninterpretedBits()
    {
        var message = new UbxNavPvt();

        message.Accept(new PvtFlagsVisitor(message));
        message.LastCorrectionAge = 15;
        message.IsTimeAuthenticated = false;
        message.IsNmaFixVerified = false;

        Assert.Equal((ushort)0x801F, message.Flags3);
        Assert.Equal((byte)15, message.LastCorrectionAge);
        Assert.Throws<ArgumentOutOfRangeException>(() => message.Accept(new InvalidBucketVisitor()));
        Assert.Equal((ushort)0x801F, message.Flags3);
    }

    [Theory]
    [InlineData("B5620135140040E201000101000007032D235A0085FF5D097F008C72", false)]
    [InlineData("B5620135140040E201000101000007032D235A0085FF5D09FF030F75", true)]
    public void Deserialize_LegacyAndX20Sat_PreservesExistingAndNewCorrectionFlags(string frame, bool extended)
    {
        var message = UbxTestHelper.Deserialize<UbxNavSat>(Convert.FromHexString(frame));
        var satellite = Assert.Single(message.Items);

        Assert.Equal(UbxNavSatGnssId.NavIC, satellite.GnssType);
        Assert.Equal((byte)7, satellite.GnssId);
        Assert.Equal((short)90, satellite.AzimDeg);
        Assert.Equal(-12.3, satellite.PrResM, 1);
        Assert.True(satellite.SvUsed);
        Assert.True(satellite.DiffCorr);
        Assert.True(satellite.IsephAvail);
        Assert.True(satellite.IssbasCorrUsed);
        Assert.True(satellite.IsdoCorrUsed);
        Assert.Equal(extended, satellite.IsclasCorrUsed);
        Assert.Equal(extended, satellite.IslppCorrUsed);
        Assert.Equal(extended, satellite.IshasCorrUsed);
    }

    [Fact]
    public void Accept_X20SatelliteCorrectionFlags_UsesDirectReferences()
    {
        var satellite = new UbxNavSatelliteItem();
        var visitor = new SatelliteFlagsVisitor(satellite);

        satellite.Accept(visitor);

        Assert.Equal(3, visitor.Visits);
        Assert.True(satellite.IsclasCorrUsed);
        Assert.True(satellite.IslppCorrUsed);
        Assert.True(satellite.IshasCorrUsed);
    }

    [Theory]
    [InlineData("B5620103100040E20100030FC3916810000087D6120084AB", false, 0)]
    [InlineData("B5620103100040E20100030FD7916810000087D612009873", true, 2)]
    public void Deserialize_StatusBitExtensions_PreservesLegacyFixAndRawStatus(
        string frame, bool indoor, byte selectiveL5)
    {
        var message = UbxTestHelper.Deserialize<UbxNavStatus>(Convert.FromHexString(frame));

        Assert.Equal(indoor, message.IsIndoorDetected);
        Assert.Equal(selectiveL5, message.SelectiveL5State);
        Assert.Equal(MapMatchingStatus.ValidAndUsedSensor, message.MapMatching);
        Assert.True(message.IsDiffCorr);
        Assert.True(message.IsCarrSolnValid);
        Assert.Equal(4200U, message.TTFF);
        Assert.Equal(1234567U, message.MSSS);
    }

    [Fact]
    public void Accept_RawFixStat_UpdatesComputedFlagsThroughDirectReference()
    {
        var message = new UbxNavStatus();

        message.Accept(new FixStatVisitor(message));
        message.IsIndoorDetected = false;
        message.SelectiveL5State = 1;

        Assert.Equal((byte)0xCB, message.FixStat);
        Assert.Throws<ArgumentOutOfRangeException>(() => message.SelectiveL5State = 4);
        Assert.Equal((byte)0xCB, message.FixStat);
    }

    [Theory]
    [InlineData("B562021310000704000A020C020078563412EFCDAB905544", UbxGnssTypeEnum.NavIC, 0)]
    [InlineData("B562021310000604070A020C020078563412EFCDAB905B96", UbxGnssTypeEnum.GLONASS, 7)]
    [InlineData("B562021310000004060A020C020078563412EFCDAB905428", UbxGnssTypeEnum.GPS, 6)]
    public void Deserialize_X20RawSignals_PreservesWordsWithoutLegacyNavigationProjection(
        string frame, UbxGnssTypeEnum constellation, byte signal)
    {
        var message = UbxTestHelper.Deserialize<UbxRxmSfrbx>(Convert.FromHexString(frame));

        Assert.Equal(constellation, message.UbxGnssId);
        Assert.Equal(signal, message.SigId);
        Assert.Equal(signal, message.Reserved1);
        Assert.Equal((sbyte)3, message.FreqId);
        Assert.Equal(new uint[] { 0x12345678, 0x90ABCDEF }, message.RawData);
        Assert.Null(message.GpsSubFrame);
        Assert.Null(message.GlonassWord);
    }

    [Fact]
    public void Accept_SfrbxSignalAlias_MutatesLegacyStorageByReference()
    {
        var message = new UbxRxmSfrbx { Reserved1 = 4 };

        message.Accept(new SignalIdVisitor(message));

        Assert.Equal((byte)6, message.SigId);
        Assert.Equal((byte)6, message.Reserved1);
        message.SigId = 3;
        Assert.Equal((byte)3, message.Reserved1);
    }

    [Fact]
    public void Deserialize_InfWarningWithoutTerminator_KeepsWarningText()
    {
        var message = UbxTestHelper.Deserialize<UbxInfWarning>(Convert.FromHexString("B562040104007761726EC190"));

        Assert.Equal("warn", message.Message);
        Assert.Equal("UBX-INF-WARNING", message.Name);
        Assert.Equal((ushort)0x0401, message.Id);
    }

    [Fact]
    public void Serialize_PwseqResetRetainingBbr_WritesDocumentedModeWithoutChangingLegacyModes()
    {
        var message = new UbxCfgRst { Bbr = BbrMask.HotStart, Mode = ResetMode.HardwareResetRetainingBbr };

        Assert.Equal(Convert.FromHexString("B5620604040000000A001878"), UbxTestHelper.Serialize(message));
        Assert.Equal((byte)0x09, (byte)ResetMode.ControlledGnssStart);
        Assert.Equal((byte)0x00, (byte)ResetMode.HardwareResetImmediately);
    }

    private sealed class PvtFlagsVisitor(UbxNavPvt message) : UInt16Type.IVisitor
    {
        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, UInt16Type type, ref ushort value)
        {
            if (field.Name != nameof(UbxNavPvt.Flags3)) return;
            value = 0xE019;
            Assert.Equal(value, message.Flags3);
            Assert.Equal((byte)12, message.LastCorrectionAge);
            Assert.True(message.IsTimeAuthenticated);
            Assert.True(message.IsNmaFixVerified);
        }
    }

    private sealed class InvalidBucketVisitor : UInt8Type.IVisitor
    {
        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name == nameof(UbxNavPvt.LastCorrectionAge)) value = 16;
        }
    }

    private sealed class SatelliteFlagsVisitor(UbxNavSatelliteItem satellite) : BoolType.IVisitor
    {
        public int Visits { get; private set; }
        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, BoolType type, ref bool value)
        {
            if (field.Name is not (nameof(UbxNavSatelliteItem.IsclasCorrUsed)
                or nameof(UbxNavSatelliteItem.IslppCorrUsed) or nameof(UbxNavSatelliteItem.IshasCorrUsed))) return;
            value = true;
            Assert.True(field.Name switch
            {
                nameof(UbxNavSatelliteItem.IsclasCorrUsed) => satellite.IsclasCorrUsed,
                nameof(UbxNavSatelliteItem.IslppCorrUsed) => satellite.IslppCorrUsed,
                _ => satellite.IshasCorrUsed
            });
            Visits++;
        }
    }

    private sealed class FixStatVisitor(UbxNavStatus message) : UInt8Type.IVisitor
    {
        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name != nameof(UbxNavStatus.FixStat)) return;
            value = 0xD7;
            Assert.Equal(value, message.FixStat);
            Assert.True(message.IsIndoorDetected);
            Assert.Equal((byte)2, message.SelectiveL5State);
        }
    }

    private sealed class SignalIdVisitor(UbxRxmSfrbx message) : UInt8Type.IVisitor
    {
        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name != nameof(UbxRxmSfrbx.SigId)) return;
            value = 6;
            Assert.Equal(value, message.Reserved1);
            Assert.Equal(value, message.SigId);
        }
    }
}
