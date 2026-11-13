using ClubPenguin.Net;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;

namespace ClubPenguin.Collectibles
{
	public static class GetTime
	{
		public static long SecondsToMS(long seconds)
		{
			return seconds * 1000;
		}

		public static DateTime UtcNow()
		{
			INetworkServicesManager networkServicesManager = Service.Get<INetworkServicesManager>();
			if (networkServicesManager != null && networkServicesManager.GameTimeMilliseconds > 0L)
			{
				return networkServicesManager.GameTimeMilliseconds.MsToDateTime();
			}
			return DateTime.UtcNow;
		}

		public static long CurrentUtcMidnightInMilliseconds()
		{
			return UtcNow().Date.GetTimeInMilliseconds();
		}

		public static long NextUtcMidnightInMilliseconds()
		{
			return UtcNow().Date.AddDays(1.0).GetTimeInMilliseconds();
		}
	}
}
