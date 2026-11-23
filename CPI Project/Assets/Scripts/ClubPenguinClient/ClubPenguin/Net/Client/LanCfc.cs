using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	public static class LanCfc
	{
		private const string DeviceTotalKey = "ol.CFCDonationTotal.device";

		private const long MaxTotal = 999999999L;

		private static readonly object sync = new object();

		private static bool ready;

		private static long total;

		public static Action<string, Dictionary<string, object>> Send;

		public static bool Active
		{
			get
			{
				lock (sync)
				{
					return ready && Send != null && LanDirectory.Active;
				}
			}
		}

		public static long Total
		{
			get
			{
				lock (sync)
				{
					return total;
				}
			}
		}

		public static void Reset()
		{
			lock (sync)
			{
				ready = false;
				total = 0L;
			}
		}

		public static void SetTotal(long value, bool persistToDevice)
		{
			value = Math.Max(0L, Math.Min(value, MaxTotal));
			lock (sync)
			{
				total = value;
				ready = true;
			}
			if (persistToDevice)
			{
				SetDeviceTotal(value);
			}
		}

		public static long Donate(int coins)
		{
			long result;
			lock (sync)
			{
				total = Math.Min(total + Math.Max(0, coins), MaxTotal);
				result = total;
			}
			Dictionary<string, object> args = new Dictionary<string, object>();
			args["coins"] = coins;
			Send("cfc_donate", args);
			return result;
		}

		public static long DeviceTotal()
		{
			long value;
			if (long.TryParse(PlayerPrefs.GetString(GetPlatformKey(DeviceTotalKey), "0"), out value))
			{
				return value;
			}
			return 0L;
		}

		private static void SetDeviceTotal(long value)
		{
			PlayerPrefs.SetString(GetPlatformKey(DeviceTotalKey), value.ToString());
			PlayerPrefs.Save();
		}

		private static string GetPlatformKey(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}
	}
}
