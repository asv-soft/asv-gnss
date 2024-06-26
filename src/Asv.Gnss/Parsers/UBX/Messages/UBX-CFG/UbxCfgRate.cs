﻿using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class UbxCfgRatePool : UbxMessageBase
    {
        public override string Name => "UBX-CFG-RATE-POOL";
        public override byte Class => 0x06;
        public override byte SubClass => 0x08;

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            
        }

        protected override int GetContentByteSize() => 0;

        public override void Randomize(Random random)
        {
            
        }
    }

    /// <summary>
    /// Navigation/measurement rate settings
    /// Supported on:
    /// • u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
    /// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
    /// This message allows the user to alter the rate at which navigation solutions (and
    /// the measurements that they depend on) are generated by the receiver. The
    /// calculation of the navigation solution will always be aligned to the top of a
    /// second zero (first second of the week) of the configured reference time system.
    /// (Navigation period is an integer multiple of the measurement period in protocol
    /// versions greater than 17).
    /// • Each measurement triggers the measurements generation and, if available,
    /// raw data output.
    /// • The navRate value defines that every nth measurement triggers a navigation
    /// epoch.
    /// • The update rate has a direct influence on the power consumption. The more
    /// fixes that are required, the more CPU power and communication resources are
    /// required.
    /// • For most applications a 1 Hz update rate would be sufficient.
    /// • When using power save mode, measurement and navigation rate can differ
    /// from the values configured here.
    /// • See Measurement and navigation rate with power save mode for details
    /// </summary>
    public class UbxCfgRate:UbxMessageBase
    {
        public override string Name => "UBX-CFG-RATE";
        public override byte Class => 0x06;
        public override byte SubClass => 0x08;


        public double RateHz { get; set; } = 1.0;
        public ushort NavRate { get; set; } = 1;
        public TimeSystemEnum TimeSystem { get; set; } = TimeSystemEnum.Gps;

        public enum TimeSystemEnum
        {
            Utc = 0,
            Gps = 1,
            Glonass = 2,
            BeiDou = 3,
            Galileo = 4
        }

        

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteUShort(ref buffer,GetRate(RateHz));
            BinSerialize.WriteUShort(ref buffer, NavRate);
            BinSerialize.WriteUShort(ref buffer, (ushort)TimeSystem);
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            RateHz = 1000.0 / BinSerialize.ReadUShort(ref buffer);
            NavRate = BinSerialize.ReadUShort(ref buffer);
            TimeSystem = (TimeSystemEnum)BinSerialize.ReadUShort(ref buffer);
        }

        protected override int GetContentByteSize() => 6;

        private ushort GetRate(double rateHz)
        {
            var result = rateHz <= 0.0152613506295307 ? (ushort)65525 : (ushort)Math.Round(1000.0 / rateHz);

            if (result <= 25) return 25;

            var multiplicity = (ushort)(result % 25);
            if (multiplicity <= 12) return (ushort)(result - multiplicity);
            return (ushort)(result + (25 - multiplicity));
        }


        public override void Randomize(Random random)
        {
            RateHz = 1;
            NavRate = (ushort)random.Next(0,127);
            TimeSystem = (TimeSystemEnum)(random.Next() % 5);
        }
    }
}