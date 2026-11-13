using System;
using System.Globalization;

namespace Disney.Kelowna.Common
{
	public static class CommonDateTime
	{
		private static TimeSpan pacificStandardTimeOffset = new TimeSpan(-8, 0, 0);

		public static DateTimeOffset CreateDate(int year, int month, int day)
		{
			return new DateTimeOffset(year, month, day, 0, 0, 0, 0, new GregorianCalendar(), pacificStandardTimeOffset);
		}

		public static DateTimeOffset CreateDate(string dateString)
		{
			string[] array = dateString.Split('-');
			int year = int.Parse(array[0]);
			int month = int.Parse(array[1]);
			int day = int.Parse(array[2]);
			return CreateDate(year, month, day);
		}

		public static string Serialize(DateTimeOffset dateTime)
		{
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
		}

		public static DateTimeOffset Deserialize(string dateTimeString)
		{
			try
			{
				return DateTimeOffset.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(dateTimeString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				return CreateDate(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day);
			}
		}
	}
}
