using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

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

			var gsa = new List<Nmea0183MessageGSA>();
			if (gsa == null) throw new ArgumentNullException(nameof(gsa));
			var activeSatPkt = new ComNavAsciiLogCommand
				{ Type = ComNavMessageEnum.GPGSA, Format = ComNavFormat.Ascii };

			try
			{
				var timeOutToken = new CancellationToken();
				using var linkedCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel, timeOutToken);
				linkedCancel.CancelAfter(TimeSpan.FromMilliseconds(3000));
				var tcs = new TaskCompletionSource<Unit>();
#if NETFRAMEWORK
				using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#else
				await using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#endif
				using var subscribeGsa = device.Connection.Filter<Nmea0183MessageGSA>().Subscribe(_ => gsa.Add(_));

				await device.Push(activeSatPkt, linkedCancel.Token).ConfigureAwait(false);
				await tcs.Task.ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				if (cancel.IsCancellationRequested)
				{
					throw;
				}
			}

			var isGps = gsa.Any(_ =>
				_.SatelliteId.All(__ => GetSatelliteSystemFromNmeaGsa(__) == ComNavSatelliteSystemEnum.GPS));
			var isGlo = gsa.Any(_ =>
				_.SatelliteId.All(__ => GetSatelliteSystemFromNmeaGsa(__) == ComNavSatelliteSystemEnum.GLONASS));
			var isGal = gsa.Any(_ =>
				_.SatelliteId.All(__ => GetSatelliteSystemFromNmeaGsa(__) == ComNavSatelliteSystemEnum.GALILEO));
			var isBds = gsa.Any(_ =>
				_.SatelliteId.All(__ =>
				{
					var satSys = GetSatelliteSystemFromNmeaGsa(__);
					return satSys is ComNavSatelliteSystemEnum.BD2 or ComNavSatelliteSystemEnum.BD3;
				}));

			if (isGps && isGlo && !isGal && !isBds) return;

			throw new Exception("Erorr to set only GPS and Glonass system!");
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
			await device.SetLogMessage(ComNavMessageEnum.RAWGPSSUBFRAME, ComNavTriggerEnum.ONCHANGED, cancel: cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.GLORAWEPHEM, ComNavTriggerEnum.ONTIME, 5, cancel: cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM1004, ComNavTriggerEnum.ONTIME, 1, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM1012, ComNavTriggerEnum.ONTIME, 1, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM1019, ComNavTriggerEnum.ONTIME, 5, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.RTCM1020, ComNavTriggerEnum.ONTIME, 5, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.PSRDOP, ComNavTriggerEnum.ONTIME, 5, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.PSRPOS, ComNavTriggerEnum.ONTIME, 5, cancel).ConfigureAwait(false);
			await device.SetLogMessage(ComNavMessageEnum.GPGSV, ComNavTriggerEnum.ONTIME, 5, cancel).ConfigureAwait(false);
			
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