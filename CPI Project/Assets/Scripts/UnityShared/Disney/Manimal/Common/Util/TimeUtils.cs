using System;

namespace Disney.Manimal.Common.Util
{
	public static class TimeUtils
	{
		private const int MINUTES_PER_HOUR = 60;

		public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static object ToUnixTimeMsBoxed(DateTime dateTime)
		{
			return dateTime.GetTimeInMilliseconds();
		}

		public static object ToUnixTimeSecBoxed(DateTime dateTime)
		{
			return dateTime.GetTimeInSeconds();
		}

		public static DateTime MsToDateTime(this int msSinceEpoch)
		{
			try
			{
				return Epoch.AddMilliseconds(msSinceEpoch);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new InvalidCastException(string.Format("Unable to convert {0} milliseconds to DateTime.", msSinceEpoch));
			}
		}

		public static DateTime SecToDateTime(this int secSinceEpoch)
		{
			try
			{
				return Epoch.AddSeconds(secSinceEpoch);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new InvalidCastException(string.Format("Unable to convert {0} seconds to DateTime.", secSinceEpoch));
			}
		}

		public static DateTime MsToDateTime(this long msSinceEpoch)
		{
			try
			{
				return Epoch.AddMilliseconds(msSinceEpoch);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new InvalidCastException(string.Format("Unable to convert {0} milliseconds to DateTime.", msSinceEpoch));
			}
		}

		public static DateTime SecToDateTime(this long secSinceEpoch)
		{
			try
			{
				return Epoch.AddSeconds(secSinceEpoch);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new InvalidCastException(string.Format("Unable to convert {0} seconds to DateTime.", secSinceEpoch));
			}
		}

		public static DateTime MsToDateTime(this object msSinceEpoch)
		{
			if (msSinceEpoch == null)
			{
				throw new ArgumentNullException("msSinceEpoch", "Can't convert object to DateTime if object is null.");
			}
			if (msSinceEpoch is string)
			{
				long msSinceEpoch2 = long.Parse((string)msSinceEpoch);
				return msSinceEpoch2.MsToDateTime();
			}
			if (msSinceEpoch is int || msSinceEpoch is long)
			{
				return Convert.ToInt64(msSinceEpoch).MsToDateTime();
			}
			throw new InvalidCastException("Don't know how to convert this object to a DateTime");
		}

		public static DateTime SecToDateTime(this object secSinceEpoch)
		{
			if (secSinceEpoch == null)
			{
				throw new ArgumentNullException("secSinceEpoch", "Can't convert object to DateTime if object is null.");
			}
			if (secSinceEpoch is string)
			{
				long msSinceEpoch = long.Parse((string)secSinceEpoch);
				return msSinceEpoch.MsToDateTime();
			}
			if (secSinceEpoch is int || secSinceEpoch is long)
			{
				return Convert.ToInt64(secSinceEpoch).SecToDateTime();
			}
			throw new InvalidCastException("Don't know how to convert this object to a DateTime");
		}

		public static long GetTimeInMilliseconds(this DateTime dateTime)
		{
			return (long)(dateTime - Epoch).TotalMilliseconds;
		}

		public static long GetTimeInSeconds(this DateTime dateTime)
		{
			return (long)(dateTime - Epoch).TotalSeconds;
		}

		public static bool isMultipleOfXMinutesAfterTheHour(DateTime dt, int minutes)
		{
			if (minutes > 60 || minutes < 1)
			{
				throw new ArgumentNullException("minutes", "Minutes must be between 1 and 60");
			}
			if (minutes == 60 && dt.Minute == 0)
			{
				return true;
			}
			return dt.Minute % minutes == 0;
		}

		public static DateTime calculateNextMultipleAfterTheHour(DateTime now, int everyXMinutesOnTheClock)
		{
			int num = 1 + now.Minute / everyXMinutesOnTheClock;
			int num2 = everyXMinutesOnTheClock * num;
			DateTime result;
			if (num2 < 60)
			{
				result = new DateTime(now.Year, now.Month, now.Day, now.Hour, num2, 0);
			}
			else if (num2 == 60)
			{
				result = now.AddHours(1.0);
				result = new DateTime(result.Year, result.Month, result.Day, result.Hour, 0, 0);
			}
			else if (everyXMinutesOnTheClock == 60)
			{
				result = now.AddHours(1.0);
				result = new DateTime(result.Year, result.Month, result.Day, result.Hour, 0, 0);
			}
			else
			{
				result = now.AddHours(1.0);
				result = new DateTime(result.Year, result.Month, result.Day, result.Hour, everyXMinutesOnTheClock, 0);
			}
			return result;
		}
	}
}
