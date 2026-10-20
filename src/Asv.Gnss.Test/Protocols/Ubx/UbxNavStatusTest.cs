using System;
using System.Buffers.Binary;
using Xunit;

namespace Asv.Gnss;

public class UbxNavStatusTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Deserialize_TwoBitStatusFields_ShouldPreserveEachEncodedValue(byte value)
    {
        var message = UbxTestHelper.Deserialize<UbxNavStatus>(CreateFrame(value));

        Assert.Equal((MapMatchingStatus)value, message.MapMatching);
        Assert.Equal((SpoofingDetectionState)value, message.SpoofDetState);
        Assert.Equal((PowerSaveModeState)value, message.PsmState);
        Assert.Equal((CPRSolutionStatus)value, message.CarrSoln);
        Assert.True(message.IsCarrSolnValid);
        Assert.True(message.IsDiffCorr);
        Assert.True(message.IsGpsFixOk);
        Assert.Equal(4200U, message.TTFF);
        Assert.Equal(1234567U, message.MSSS);
    }

    [Fact]
    public void Deserialize_ReusedMessage_ShouldResetStatusesWhenBitsAreCleared()
    {
        var message = new UbxNavStatus();
        UbxTestHelper.Deserialize(message, CreateFrame(3));

        UbxTestHelper.Deserialize(message, CreateFrame(0));

        Assert.Equal(MapMatchingStatus.None, message.MapMatching);
        Assert.Equal(SpoofingDetectionState.UnknownOrDeactivated, message.SpoofDetState);
        Assert.Equal(PowerSaveModeState.Acquisition, message.PsmState);
        Assert.Equal(CPRSolutionStatus.NoCPRSolution, message.CarrSoln);
    }

    [Fact]
    public void Deserialize_DifferentStatusValues_ShouldDecodeEachBitFieldIndependently()
    {
        var message = UbxTestHelper.Deserialize<UbxNavStatus>(CreateFrame(1, 2, 3, 1));

        Assert.Equal(MapMatchingStatus.ValidButNotUsed, message.MapMatching);
        Assert.Equal(SpoofingDetectionState.SpoofingIndicated, message.SpoofDetState);
        Assert.Equal(PowerSaveModeState.Inactive, message.PsmState);
        Assert.Equal(CPRSolutionStatus.CPRWithFloatingAmbiguities, message.CarrSoln);
        Assert.True(message.IsCarrSolnValid);
        Assert.True(message.IsDiffCorr);
        Assert.Equal(4200U, message.TTFF);
        Assert.Equal(1234567U, message.MSSS);
    }

    private static byte[] CreateFrame(byte value) => CreateFrame(value, value, value, value);

    private static byte[] CreateFrame(byte mapMatching, byte spoofing, byte powerSaveMode, byte carrierSolution)
    {
        var payload = new byte[16];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, 123456);
        payload[4] = 3;
        payload[5] = 0x0F;
        payload[6] = (byte)((mapMatching << 6) | 3);
        payload[7] = (byte)((carrierSolution << 6) | (spoofing << 3) | powerSaveMode | 0x24);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), 4200);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(12), 1234567);
        return UbxTestHelper.CreateFrame(0x01, 0x03, payload);
    }
}
