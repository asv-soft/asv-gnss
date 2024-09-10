using System;
using System.Diagnostics;
using Asv.IO;

namespace Asv.Gnss
{
    public class ComNavBinaryRawGloEphemPacket : ComNavBinaryMessageBase
	{
		public const ushort ComNavMessageId = 792;
		public override ushort MessageId => ComNavMessageId;
		public override string Name => "GLORAWEPHEM";
		protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
		{
			SvId = BinSerialize.ReadUShort(ref buffer);
			Frequency = 1.60200E9 + (BinSerialize.ReadUShort(ref buffer) - 7f) * 9E6 / 16;
			SatPrn = ComNavBinaryHelper.GetPnrAndRinexCode(ComNavSatelliteSystemEnum.GLONASS, SvId, out var rCore);
			SignalType = GnssSignalTypeEnum.L1CA;
			RindexSignalCode = "1C";
			RinexSatCode = rCore;
			var signalChNum = BinSerialize.ReadUInt(ref buffer);
			var week = BinSerialize.ReadUInt(ref buffer);
			var seconds = BinSerialize.ReadUInt(ref buffer) / 1000.0;
			GpsEphTime = RtcmV3Helper.GetFromGps((int)week, seconds);
			var recNum = BinSerialize.ReadUInt(ref buffer);
			RawData = new uint[recNum][];
			GlonassWords = new GlonassWordBase[recNum];

			for (var k = 0; k < recNum; k++)
			{
				RawData[k] = new uint[3];
				// Считываем 11 byte в RawBuffer
				for (var i = 0; i < 3; i++)
				{
					for (var j = 0; j < 4 && i * j != 6; j++)
					{
						RawData[k][i] |= (uint)(BinSerialize.ReadByte(ref buffer) << (24 - j * 8));
					}
				}
				
				var reserved = BinSerialize.ReadByte(ref buffer);

				// Смещаем 85-ый bit на 88-ю (11 byte * 8) позицию
				RawData[k][0] <<= 3;
				for (var i = 1; i < 3; i++)
				{
					RawData[k][i - 1] |= (RawData[k][i] & 0xE000_0000) >> 29;
					RawData[k][i] <<= 3;
				}

				GlonassWords[k] = GlonassWordFactory.Create(RawData[k]);
				if (GlonassWords == null)
				{
					Debug.Fail("Null reference");
				}
			}

		}

		protected override void InternalContentSerialize(ref Span<byte> buffer)
		{
			throw new NotImplementedException();
		}

		protected override int InternalGetContentByteSize()
		{
			return 20 + 12 * RawData.Length;
		}

		public ushort SvId { get; set; }
		public int SatPrn { get; set; }
		public string RinexSatCode { get; set; }
		public GnssSignalTypeEnum SignalType { get; set; }

		public string RindexSignalCode { get; set; }
		public double Frequency { get; set; }
		public DateTime GpsEphTime { get; set; }
		public uint[][] RawData { get; set; }
		public GlonassWordBase[] GlonassWords { get; set; }

		public GloRawCa[] GetGnssRawNavMsg()
		{
			var result = new GloRawCa[RawData.Length];

			for (var i = 0; i < RawData.Length; i++)
			{
				var msg = new GloRawCa
				{
					NavSystem = NavSysEnum.GLONASS,
					CarrierFreq = Frequency,
					SignalType = SignalType,
					UtcTime = UtcTime,
					RawData = new uint[RawData.Length],
					SatId = SvId,
					SatPrn = SatPrn,
					RinexSatCode = RinexSatCode,
					RindexSignalCode = RindexSignalCode,
					GlonassWord = GlonassWords[i]
				};

				Array.Copy(RawData[i], msg.RawData, RawData[i].Length);
				result[i] = msg;
			}

			return result;
		}
	}
}