using System;

namespace Asv.Gnss;

internal static class GbasTestHelper
{
    public static GbasMessageBlockBase CreateBlock(byte type)
    {
        GbasMessageBlockBase block = type switch
        {
            1 => CreateType1(),
            2 => CreateType2(),
            3 => CreateType3(),
            4 => CreateType4(),
            5 => CreateType5(),
            11 => CreateType11(),
            101 => CreateType101(),
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
        block.GbasId = "UUEE";
        block.MessageBlockId = MessageBlockIdEnum.Normal;
        return block;
    }

    public static byte[] Serialize(GbasMessageBlockBase block)
    {
        var bytes = new byte[block.GetByteSize()];
        var position = 0;
        block.Serialize(bytes, ref position);
        return bytes;
    }

    public static byte[] Burst(params GbasMessageBlockBase[] blocks)
    {
        var burst = new GbasMessage { SsId = SlotIdEnum.C };
        burst.ApplicationData.AddRange(blocks);
        var bytes = new byte[burst.GetByteSize()];
        var position = 0;
        burst.Serialize(bytes, ref position);
        return bytes;
    }

    private static MessageBlockType1 CreateType1()
    {
        var block = new MessageBlockType1
        {
            ModifiedZCount = 123.4, AdditionalMessageFlag = AdditionalMessageEnum.FirstOfTwo,
            P = 0.0001, EphemerisCrc = 0x1234, SourceAvailabilityDuration = null,
        };
        block.Measurements.Add(new MeasurementBlockMsg1(-6.35, double.NaN, 0, 6.35)
        {
            RangingSourceId = 7, Iod = 23, Prc = -123.45, Rrc = 3.456, SigmaPrGnd = 1.24,
        });
        return block;
    }

    private static MessageBlockType2 CreateType2()
    {
        var block = new MessageBlockType2
        {
            GbasReferenceReceivers = 2, GroundAccuracyDesignatorLetter = GADLEnum.C,
            GbasDesignator = GCIDEnum.GCID4, LocalMagneticVariation = -12.25,
            Sigma_vert_iono_gradient = 0.0000005, RefractivityIndex = 397,
            ScaleHeight = 8500, RefractivityUncertainty = 7,
            Latitude = 55.75, Longitude = -37.5, EllipsoidHeight = -123.45,
        };
        block.AdditionalData.Add(new AdditionalDataBlock1
        {
            RSDS = 17, DMax = 200, Kmd_e_POS_GPS = 1.25, Kmd_e_GPS = 2.5,
            Kmd_e_POS_GLONASS = 3.75, Kmd_e_GLONASS = 4.5,
        });
        block.AdditionalData.Add(new AdditionalDataBlock2
        {
            ChannelNumber = 23456, DeltaLatitude = -1.4, DeltaLongitude = 2.6,
        });
        block.AdditionalData.Add(new AdditionalDataBlock3
        {
            Kmd_e_D_GPS = 1.2, Kmd_e_D_Glonass = 2.3,
            Sigma_vert_iono_gradient_D = 0.0000008, Y_EIG = 1.7, M_EIG = 0.4,
        });
        var authentication = new AdditionalDataBlock4();
        authentication.SlotGroupDefinition.AddRange([SlotIdEnum.A, SlotIdEnum.D, SlotIdEnum.H]);
        block.AdditionalData.Add(authentication);
        return block;
    }

    private static MessageBlockType3 CreateType3()
    {
        var block = new MessageBlockType3();
        block.Filter.AddRange([0xAA, 0x55, 0x01]);
        return block;
    }

    private static MessageBlockType4 CreateType4()
    {
        var block = new MessageBlockType4();
        block.FasDataSet.Add(new FasDataSet
        {
            OperationType = 0, SbasServiceProvider = SBASServiceProviderEnum.OnlyGBAS,
            AirportId = "UUEE", RunwayNumber = 24, RunwayLetter = RunwayLetterEnum.L,
            ApproachPerformanceDesignator = ApproachPerformanceDesignatorEnum.GastCD,
            RouteIndicator = RouteIndicatorEnum.A, RPDS = 42, ReferencePathIdentifier = "G24A",
            LtpFtpLatitude = 55.75, LtpFtpLongitude = -37.5, LtpFtpHeight = 212.3,
            DeltaFPapLatitude = -0.001, DeltaFPapLongitude = 0.002,
            TCH = 15.24, TCHUnits = TCHUnitsEnum.M, GlidePathAngle = 3.0,
            CourseWidth = 106.25, DeltaLengthOffset = 80, FASVAL = 10, FASLAL = 40,
        });
        return block;
    }

    private static MessageBlockType5 CreateType5()
    {
        var block = new MessageBlockType5 { ModifiedZCount = 987.6 };
        block.ImpactedSources.Add(new ImpactedSource
        {
            RangingSourceId = 8, SourceAvailabilitySense = SourceAvailabilitySenseEnum.WillStart,
            SourceAvailabilityDuration = 1270,
        });
        var approach = new ObstructedApproaches { RPDS = 42 };
        approach.ImpactedSources.Add(new ImpactedSource
        {
            RangingSourceId = 9, SourceAvailabilitySense = SourceAvailabilitySenseEnum.WillCease,
            SourceAvailabilityDuration = 30,
        });
        block.ObstructedApproaches.Add(approach);
        return block;
    }

    private static MessageBlockType11 CreateType11()
    {
        var block = new MessageBlockType11
        {
            ModifiedZCount = 543.2, AdditionalMessageFlag = AdditionalMessageEnum.SecondOfTwo, PD = 0.0002,
        };
        block.Measurements.Add(new MeasurementBlockD
        {
            RangingSourceId = 12, Prc30 = -327.67, Rrc30 = 32.767,
            SigmaPrGndD = 1.2, SigmaPrGndD30 = 2.4,
        });
        return block;
    }

    private static MessageBlockType101 CreateType101()
    {
        var block = new MessageBlockType101
        {
            ModifiedZCount = 1199.9, P = 0.0003, EphemerisCrc = 0xBEEF,
            SourceAvailabilityDuration = 2540,
        };
        block.Measurements.Add(new MeasurementBlockMsg101(double.NaN)
        {
            RangingSourceId = 37, Iod = 255, Prc = 327.67, Rrc = -32.767, SigmaPrGnd = 50.8,
        });
        return block;
    }
}
