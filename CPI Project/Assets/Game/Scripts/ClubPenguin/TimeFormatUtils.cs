using System;
using System.Text;

namespace ClubPenguin
{
	public class TimeFormatUtils
	{
		public static string FormatTimeString(TimeSpan time)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(formatTimePart(time.Hours, true, true));
			stringBuilder.Append(formatTimePart(time.Minutes, true, true));
			stringBuilder.Append(formatTimePart(time.Seconds));
			return stringBuilder.ToString();
		}

		private static string formatTimePart(int timePart, bool addZero = true, bool addSeparator = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (timePart < 10 && addZero)
			{
				stringBuilder.Append("0");
			}
			stringBuilder.Append(timePart.ToString());
			if (addSeparator)
			{
				stringBuilder.Append(":");
			}
			return stringBuilder.ToString();
		}
	}
}
