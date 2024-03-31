using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Gnss
{
    public static class ComNavDeviceHelper
	{
		#region UnLogAllMessages

		public static Task SetUnLogAllMessages(this IComNavDevice device, CancellationToken cancel = default)
		{
			return device.Push(new ComNavUnLogAllCommand(), cancel);
		}

		#endregion

		#region LogMessage

		public static Task SetUnLogAllMessage(this IComNavDevice device, CancellationToken cancel = default)
		{
			return device.Push(new ComNavUnLogAllCommand(), cancel);
		}

		public static async Task SetLogMessage(this IComNavDevice device, ComNavMessageEnum msg, ComNavTriggerEnum? trigger = null, uint? period = null, CancellationToken cancel = default)
		{
			var pkt = new ComNavAsciiLogCommand
			{
				Format = ComNavFormat.Binary,
				Type = msg,
				Trigger = trigger,
				Period = period
			};
			await device.Push(pkt, cancel).ConfigureAwait(false);

			var logList = await device
				.Pool<ComNavLogListAMessage, ComNavAsciiLogCommand>(new ComNavAsciiLogCommand { Type = ComNavMessageEnum.LOGLIST, Format = ComNavFormat.Ascii },
					cancel).ConfigureAwait(false);

			var log = logList.Messages.FirstOrDefault(_ => _.Message == msg);

			if (log == null) throw new ComNavDeviceResponseException(device.Connection.Stream.Name, pkt);
		}


		#endregion


		#region ActiveSatellitesConfigure
		public static Task SetLockoutSystem(this IComNavDevice device, ComNavSatelliteSystemEnum sys, CancellationToken cancel = default)
		{
			return device.Push(new ComNavSetLockoutSystemCommand { SatelliteSystem = sys }, cancel);
		}

		public static Task SetUnLockoutSystem(this IComNavDevice device, ComNavSatelliteSystemEnum sys, CancellationToken cancel = default)
		{
			return device.Push(new ComNavSetUnLockoutSystemCommand { SatelliteSystem = sys }, cancel);
		}

		public static Task SetUnLockoutAllSystem(this IComNavDevice device, CancellationToken cancel = default)
		{
			return device.Push(new ComNavUnLockoutAllSystemCommand(), cancel);
		}

		public static async Task SetOnlyGpsAndGlonassSystem(this IComNavDevice device, CancellationToken cancel = default)
		{
			await device.SetUnLockoutAllSystem(cancel).ConfigureAwait(false);
			await device.SetLockoutSystem(ComNavSatelliteSystemEnum.BD2, cancel).ConfigureAwait(false);
			await device.SetLockoutSystem(ComNavSatelliteSystemEnum.BD3, cancel).ConfigureAwait(false);
			await device.SetLockoutSystem(ComNavSatelliteSystemEnum.GALILEO, cancel).ConfigureAwait(false);
			await device.SetUnLockoutSystem(ComNavSatelliteSystemEnum.GPS, cancel).ConfigureAwait(false);
			await device.SetUnLockoutSystem(ComNavSatelliteSystemEnum.GLONASS, cancel).ConfigureAwait(false);

			var att = 1;
			while (att <= 3)
			{
				Console.WriteLine($"Att: {att}");
				var sys = (await GetActiveGnssSystem(device, cancel).ConfigureAwait(false)).ToArray();
				Console.Write("Active satellite system: ");
				sys.ForEach(_ =>
				{
					var str = _ != null ? $"{_:G}" : "null";
					Console.Write($"{str} ");
				});
				Console.WriteLine();
				return;
				if (sys.Length == 2 && sys.Contains(ComNavSatelliteSystemEnum.GPS) &&
				    sys.Contains(ComNavSatelliteSystemEnum.GLONASS)) return;
				att++;
			}
			throw new Exception("Erorr to set only GPS and Glonass system!");
		}

		private static async Task<IEnumerable<ComNavSatelliteSystemEnum?>> GetActiveGnssSystem(this IComNavDevice device, CancellationToken cancel = default)
		{
			var gsa = new List<ComNavSatelliteSystemEnum?>();
			
			var activeSatPkt = new ComNavAsciiLogCommand
				{ Type = ComNavMessageEnum.GPGSA, Format = ComNavFormat.Ascii };

			var timeOutToken = new CancellationTokenSource();
			try
			{
				using var subscribeGsa = device.Connection.Filter<Nmea0183MessageGSA>().Subscribe(_ =>
				{
					foreach (var i in _.SatelliteId)
					{
						Console.Write($"{i}, ");
					}
					Console.WriteLine();
					gsa.AddRange(_.SatelliteId.Select(GetSatelliteSystemFromNmeaGsa));
				});
				await device.Push(activeSatPkt, cancel).ConfigureAwait(false);


				var tcs = new TaskCompletionSource<Unit>();
				timeOutToken.CancelAfter(1000);
#if NETFRAMEWORK
				using var c1 = timeOutToken.Token.Register(() => tcs.TrySetCanceled());
#else
				await using var c1 = timeOutToken.Token.Register(() => tcs.TrySetCanceled());
#endif
				await tcs.Task.ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				if (!timeOutToken.Token.IsCancellationRequested)
				{
					throw;
				}
			}
			return gsa.Distinct();
		}
		
		#endregion

		#region PPSControl

		public static Task SetPpsControl(this IComNavDevice device, ComNavPpsPolarityEnum polarity = ComNavPpsPolarityEnum.Negative, double period = 1.0, int pulseWidth = 1000, CancellationToken cancel = default)
		{
			return device.Push(new ComNavSetPpsControlCommand { Polarity = polarity, Period = period, PulseWidth = pulseWidth }, cancel);
		}

		#endregion
		
		public static async Task SetBaseStationSettings(this IComNavDevice device, double lat, double lon, double alt, CancellationToken cancel = default)
		{
			await device.SetUnLogAllMessage(cancel).ConfigureAwait(false);
			await device.SetOnlyGpsAndGlonassSystem(cancel).ConfigureAwait(false);
			
			var smooth = new ComNavAsciiSetCommand { Type = ComNavSetTypeEnum.CPSMOOTHPR };
			smooth.Params.Add("ON");
			smooth.Params.Add("100");
			await device.Push(smooth, cancel).ConfigureAwait(false);

			await device.SetPpsControl(ComNavPpsPolarityEnum.Positive, 1.0, 10000, cancel).ConfigureAwait(false);

			await device.SetLogMessage(ComNavMessageEnum.RTCM1, ComNavTriggerEnum.ONTIME, 1, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM31, ComNavTriggerEnum.ONTIME, 1, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RAWGPSSUBFRAME, ComNavTriggerEnum.ONTIME, 1, cancel: cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.GLORAWEPHEM, ComNavTriggerEnum.ONTIME, 1, cancel: cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM1004, ComNavTriggerEnum.ONTIME, 1, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RANGE, ComNavTriggerEnum.ONTIME, 1, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM1012, ComNavTriggerEnum.ONTIME, 1, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM1019, ComNavTriggerEnum.ONTIME, 1, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM1020, ComNavTriggerEnum.ONTIME, 1, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM1005, ComNavTriggerEnum.ONTIME, 5, cancel).ConfigureAwait(false);
			// await device.SetLogMessage(ComNavMessageEnum.PSRDOP, ComNavTriggerEnum.ONTIME, 5, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.PSRPOS, ComNavTriggerEnum.ONTIME, 5, cancel).ConfigureAwait(false);
			// await device.SetLogMessage(ComNavMessageEnum.GPGSV, ComNavTriggerEnum.ONTIME, 5, cancel).ConfigureAwait(false);
			
			await device.Push(new ComNavFixCommand { FixType = ComNavFixType.Auto }, cancel).ConfigureAwait(false);
			
			await device.Push(new ComNavSaveConfigCommand(), cancel).ConfigureAwait(false);
		}

		public static Task SetRoverSettings(this IComNavDevice device, CancellationToken cancel = default)
		{
			return Task.CompletedTask;
		}

		private static ComNavSatelliteSystemEnum? GetSatelliteSystemFromNmeaGsa(int satId)
		{
			return satId switch
			{
				>= 1 and <= 32 => ComNavSatelliteSystemEnum.GPS,
				>= 38 and <= 61 => ComNavSatelliteSystemEnum.GLONASS,
				>= 71 and <= 106 => ComNavSatelliteSystemEnum.GALILEO,
				>= 141 and <= 177 => ComNavSatelliteSystemEnum.BD2,
				_ => null
			};
		}
		
		
		public static bool GetPrnFromNmeaGsvSatId(this IComNavDevice device, int satId, out int PRN, out NmeaNavigationSystemEnum nav)
		{
			nav = NmeaNavigationSystemEnum.SYS_NONE;
			PRN = -1;
			if (satId <= 0) return false;

			switch (satId)
			{
				case >= 1 and <= 32:
					nav = NmeaNavigationSystemEnum.SYS_GPS;
					PRN = satId;
					return true;
				case >= 38 and <= 61:
					nav = NmeaNavigationSystemEnum.SYS_GLO;
					PRN = satId - 37;
					return true;
				case >= 71 and <= 106:
					nav = NmeaNavigationSystemEnum.SYS_GAL;
					PRN = satId - 70;
					return true;
				case >= 141 and <= 177:
					nav = NmeaNavigationSystemEnum.SYS_CMP;
					PRN = satId - 140;
					return true;
			}
			return false;
		}


		
	}
}