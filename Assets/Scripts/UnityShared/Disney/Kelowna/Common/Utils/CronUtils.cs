using System;

namespace Disney.Kelowna.Common.Utils
{
	public class CronUtils
	{
		public static T ParseEnum<T>(string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static int ConvertCronFieldToNumber(int fieldPos, string str)
		{
			int result;
			if (!int.TryParse(str, out result))
			{
				str = str.ToUpper();
				try
				{
					switch (fieldPos)
					{
					case 4:
						result = (int)ParseEnum<Month>(str);
						return result;
					case 5:
						result = (int)ParseEnum<WeekDay>(str);
						return result;
					}
				}
				catch
				{
					result = -1;
					return result;
				}
				finally
				{
				}
			}
			return result;
		}
	}
}
