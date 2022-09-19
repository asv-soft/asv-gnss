using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class UbxCfgNav5Pool:UbxMessageBase
    {
        public override string Name => "UBX-CFG-NAV5-POOL";
        public override byte Class => 0x06;
        public override byte SubClass => 0x24;

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
    /// Navigation engine settings
    /// Supported on:
    /// • u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
    /// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
    /// See the Navigation Configuration Settings Description for a detailed description
    /// of how these settings affect receiver operation.
    /// </summary>
    public class UbxCfgNav5 : UbxMessageBase
    {
        public override string Name => "UBX-CFG-NAV5";
        public override byte Class => 0x06;
        public override byte SubClass => 0x24;

        public bool ApplyDynamicModel { get; set; } = true;
        public bool ApplyMinimumElevation { get; set; } = true;
        public bool ApplyFixMode { get; set; } = true;
        public bool ApplyDrLimit { get; set; } = true;
        public bool ApplyPositionMask { get; set; } = true;
        public bool ApplyTimeMask { get; set; } = true;
        public bool ApplyStaticHold { get; set; } = true;
        public bool ApplyDGPS { get; set; } = true;
        public bool ApplyCnoThreshold { get; set; } = true;
        public bool ApplyUTC { get; set; } = true;

        public ModelEnum PlatformModel { get; set; } = ModelEnum.Stationary;
        public PositionModeEnum PositionMode { get; set; } = PositionModeEnum.Auto;

        public double FixedAltitude { get; set; }
        public double FixedAltitudeVariance { get; set; } = 1.0;
        public byte DrLimit { get; set; }
        public sbyte MinimumElevation { get; set; } = 15;
        public double PositionDOP { get; set; } = 25.0;
        public double TimeDOP { get; set; } = 25.0;
        public ushort PositionAccuracy { get; set; } = 100;
        public ushort TimeAccuracy { get; set; } = 300;
        public double StaticHoldThreshold { get; set; }
        public byte DGNSSTimeout { get; set; }
        public byte CnoThreshNumSVs { get; set; }
        public byte CnoThreshold { get; set; } = 35;
        public ushort StaticHoldMaxDistance { get; set; }
        public byte UtcStandard { get; set; }
        public enum PositionModeEnum
        {
            Only2D = 1,
            Only3D = 2,
            Auto = 3
        }
        public enum ModelEnum
        {
            Portable = 0,
            Stationary = 2,
            Pedestrian = 3,
            Automotive = 4,
            Sea = 5,
            AirborneWithLess1gAcceleration = 6,
            AirborneWithLess2gAcceleration = 7,
            AirborneWithLess4gAcceleration = 8,
            WristWornWatch = 9,
            Bike = 10
        }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            ushort appliedBitMask = 0;
            if (ApplyDynamicModel) appliedBitMask |= 0x01;
            if (ApplyMinimumElevation) appliedBitMask |= 0x02;
            if (ApplyFixMode) appliedBitMask |= 0x04;
            if (ApplyDrLimit) appliedBitMask |= 0x08;
            if (ApplyPositionMask) appliedBitMask |= 0x10;
            if (ApplyTimeMask) appliedBitMask |= 0x20;
            if (ApplyStaticHold) appliedBitMask |= 0x40;
            if (ApplyDGPS) appliedBitMask |= 0x80;
            if (ApplyCnoThreshold) appliedBitMask |= 0x100;
            if (ApplyUTC) appliedBitMask |= 0x400;
            BinSerialize.WriteUShort(ref buffer,appliedBitMask);

            BinSerialize.WriteByte(ref buffer,(byte)PlatformModel);
            BinSerialize.WriteByte(ref buffer, (byte)PositionMode);

            BinSerialize.WriteInt(ref buffer, (int)Math.Round(FixedAltitude * 100));
            BinSerialize.WriteUInt(ref buffer, (uint)Math.Round(FixedAltitudeVariance * 10000));
            BinSerialize.WriteSByte(ref buffer, (sbyte)MinimumElevation);
            BinSerialize.WriteByte(ref buffer, (byte)DrLimit);
            BinSerialize.WriteUShort(ref buffer, (ushort)Math.Round(PositionDOP * 10));
            BinSerialize.WriteUShort(ref buffer, (ushort)Math.Round(TimeDOP * 10));
            BinSerialize.WriteUShort(ref buffer, PositionAccuracy);
            BinSerialize.WriteUShort(ref buffer, TimeAccuracy);

            BinSerialize.WriteByte(ref buffer, (byte)Math.Round(StaticHoldThreshold * 100));
            BinSerialize.WriteByte(ref buffer, DGNSSTimeout);
            BinSerialize.WriteByte(ref buffer, CnoThreshNumSVs);
            BinSerialize.WriteByte(ref buffer, CnoThreshold);

            BinSerialize.WriteByte(ref buffer, 0); // reserved
            BinSerialize.WriteByte(ref buffer, 0); // reserved

            BinSerialize.WriteUShort(ref buffer, StaticHoldMaxDistance);
            BinSerialize.WriteByte(ref buffer, UtcStandard);

            BinSerialize.WriteByte(ref buffer, 0); // reserved
            BinSerialize.WriteByte(ref buffer, 0); // reserved
            BinSerialize.WriteByte(ref buffer, 0); // reserved
            BinSerialize.WriteByte(ref buffer, 0); // reserved
            BinSerialize.WriteByte(ref buffer, 0); // reserved
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            var appliedBitMask = BinSerialize.ReadUShort(ref buffer);
            ApplyDynamicModel = (appliedBitMask & 0x01) != 0;
            ApplyMinimumElevation = (appliedBitMask & 0x02) != 0;
            ApplyFixMode = (appliedBitMask & 0x04) != 0;
            ApplyDrLimit = (appliedBitMask & 0x08) != 0;
            ApplyPositionMask = (appliedBitMask & 0x10) != 0;
            ApplyTimeMask = (appliedBitMask & 0x20) != 0;
            ApplyStaticHold = (appliedBitMask & 0x40) != 0;
            ApplyDGPS = (appliedBitMask & 0x80) != 0;
            ApplyCnoThreshold = (appliedBitMask & 0x100) != 0;
            ApplyUTC = (appliedBitMask & 0x400) != 0;

            PlatformModel = (ModelEnum)BinSerialize.ReadByte(ref buffer);
            PositionMode = (PositionModeEnum)BinSerialize.ReadByte(ref buffer);
            FixedAltitude = BinSerialize.ReadInt(ref buffer) * 0.01;
            FixedAltitudeVariance = BinSerialize.ReadUInt(ref buffer) * 0.0001;
            MinimumElevation = BinSerialize.ReadSByte(ref buffer);
            DrLimit = BinSerialize.ReadByte(ref buffer);
            PositionDOP = BinSerialize.ReadUShort(ref buffer) * 0.1;
            TimeDOP = BinSerialize.ReadUShort(ref buffer) * 0.1;
            PositionAccuracy = BinSerialize.ReadUShort(ref buffer);
            TimeAccuracy = BinSerialize.ReadUShort(ref buffer);
            StaticHoldThreshold = BinSerialize.ReadByte(ref buffer) * 0.01;
            DGNSSTimeout = BinSerialize.ReadByte(ref buffer);
            CnoThreshNumSVs = BinSerialize.ReadByte(ref buffer);
            CnoThreshold = BinSerialize.ReadByte(ref buffer);

            BinSerialize.ReadByte(ref buffer); // reserved
            BinSerialize.ReadByte(ref buffer); // reserved

            StaticHoldMaxDistance = BinSerialize.ReadUShort(ref buffer);
            UtcStandard = BinSerialize.ReadByte(ref buffer);
            BinSerialize.ReadByte(ref buffer); // reserved
            BinSerialize.ReadByte(ref buffer); // reserved
            BinSerialize.ReadByte(ref buffer); // reserved
            BinSerialize.ReadByte(ref buffer); // reserved
            BinSerialize.ReadByte(ref buffer); // reserved
        }

        protected override int GetContentByteSize() => 36;
        

        public override void Randomize(Random random)
        {
            ApplyDynamicModel = random.NextDouble() > 0.5;
            ApplyMinimumElevation = random.NextDouble() > 0.5; ;
            ApplyFixMode = random.NextDouble() > 0.5; ;
            ApplyDrLimit = random.NextDouble() > 0.5; ;
            ApplyPositionMask = random.NextDouble() > 0.5; ;
            ApplyTimeMask = random.NextDouble() > 0.5; ;
            ApplyStaticHold = random.NextDouble() > 0.5; ;
            ApplyDGPS = random.NextDouble() > 0.5; ;
            ApplyCnoThreshold = random.NextDouble() > 0.5; ;
            ApplyUTC = random.NextDouble() > 0.5; ;
            PlatformModel = (ModelEnum)random.Next(0, 10);
            PositionMode = (PositionModeEnum)random.Next(1, 3);
            FixedAltitude = random.NextDouble()*60;
            FixedAltitudeVariance = random.NextDouble() * 6;
            DrLimit = (byte)random.Next(0,byte.MaxValue);
            MinimumElevation = (sbyte)(random.Next(0, byte.MaxValue) - 127);
            PositionDOP = random.NextDouble() * 60;
            TimeDOP = random.NextDouble() * 60;
            PositionAccuracy = (ushort)random.Next(0, ushort.MaxValue);
            TimeAccuracy = (ushort)random.Next(0, ushort.MaxValue);
            StaticHoldThreshold = 0.1;
            DGNSSTimeout = (byte)random.Next(0, byte.MaxValue);
            CnoThreshNumSVs = (byte)random.Next(0, byte.MaxValue);
            CnoThreshold = (byte)random.Next(0, byte.MaxValue);
            StaticHoldMaxDistance = (ushort)random.Next(0, ushort.MaxValue);
            UtcStandard = (byte)random.Next(0, byte.MaxValue);
        }
    }
}