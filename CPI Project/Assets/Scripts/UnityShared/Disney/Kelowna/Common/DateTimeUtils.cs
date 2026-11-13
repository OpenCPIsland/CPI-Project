using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;

namespace Disney.Kelowna.Common
{
	public static class DateTimeUtils
	{
		public static bool DoesDateFallBetween(DateTime target, DateTime startDate, DateTime endDate)
		{
			return IsAfter(target, startDate) && IsBefore(target, endDate);
		}

		public static bool IsAfter(DateTime target, DateTime startDate)
		{
			return target.Date >= startDate.Date || StartsImmediately(startDate);
		}

		public static bool IsBefore(DateTime target, DateTime endDate)
		{
			return target < endDate.Date || LastsForever(endDate);
		}

		public static bool StartsImmediately(DateTime date)
		{
			return date.Date == DateTime.MinValue;
		}

		public static bool LastsForever(DateTime date)
		{
			return date.Date == DateTime.MinValue;
		}

		public static DateTime DateTimeFromUnixTime(long unixMilliseconds)
		{
			return UnixEpoch().AddMilliseconds(unixMilliseconds).ToLocalTime();
		}

		public static long UnixTimeFromDateTime(DateTime dateTime)
		{
			return (long)(dateTime - UnixEpoch()).TotalMilliseconds;
		}

		public static DateTime UnixEpoch()
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		}

		public static string ToMediumDateString(this DateTime dt)
		{
			string text = dt.ToLongDateString();
			text = text.Replace(dt.DayOfWeek.ToString(), "");
			return text.Trim(' ', ',');
		}

		public static string GetLocalizedMonth(this DateTime dt)
		{
			Localizer localizer = Service.Get<Localizer>();
			switch (dt.Month)
			{
			case 1:
				return localizer.GetTokenTranslation("GlobalUI.Months.January");
			case 2:
				return localizer.GetTokenTranslation("GlobalUI.Months.February");
			case 3:
				return localizer.GetTokenTranslation("GlobalUI.Months.March");
			case 4:
				return localizer.GetTokenTranslation("GlobalUI.Months.April");
			case 5:
				return localizer.GetTokenTranslation("GlobalUI.Months.May");
			case 6:
				return localizer.GetTokenTranslation("GlobalUI.Months.June");
			case 7:
				return localizer.GetTokenTranslation("GlobalUI.Months.July");
			case 8:
				return localizer.GetTokenTranslation("GlobalUI.Months.August");
			case 9:
				return localizer.GetTokenTranslation("GlobalUI.Months.September");
			case 10:
				return localizer.GetTokenTranslation("GlobalUI.Months.October");
			case 11:
				return localizer.GetTokenTranslation("GlobalUI.Months.November");
			case 12:
				return localizer.GetTokenTranslation("GlobalUI.Months.December");
			default:
				return null;
			}
		}

		public static string GetLocalizedDate(this DateTime dt)
		{
			Localizer localizer = Service.Get<Localizer>();
			string tokenTranslation = localizer.GetTokenTranslation("GlobalUI.Date.FullDate");
			return string.Format(tokenTranslation, dt.GetLocalizedMonth(), dt.Day, dt.Year);
		}
	}
}
