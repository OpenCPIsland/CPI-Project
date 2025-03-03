using System;

namespace Disney.Kelowna.Common.Utils
{
	public class CronExpressionEvaluator
	{
		public static bool EvaluatesTrue(DateTime dateToCompare, string schedule, out int remainingTime)
		{
			bool flag = false;
			remainingTime = 0;
			DateTime nextDate;
			flag = cronExpressionEvaluatesTrue(schedule, dateToCompare, out nextDate);
			if (EvaluatesTrue(nextDate, schedule))
			{
				remainingTime = (int)nextDate.Subtract(dateToCompare).TotalSeconds;
			}
			else
			{
				remainingTime = -1;
			}
			return flag;
		}

		public static bool EvaluatesTrue(DateTime dateToCompare, string schedule, out DateTime nextDate)
		{
			bool flag = false;
			return cronExpressionEvaluatesTrue(schedule, dateToCompare, out nextDate);
		}

		public static bool EvaluatesTrue(DateTime dateToCompare, string schedule)
		{
			bool flag = false;
			return cronExpressionEvaluatesTrue(schedule, dateToCompare);
		}

		public static bool ValidateCronTime(string schedule, DateTime cronTime)
		{
			int num = 5;
			string[] array = cronTime.ToString("s m H d M ddd yyyy").Split(' ');
			string[] array2 = prepareSchedulePartsToCompareToDateParts(schedule, array);
			return isCronTimeForField(num, array2[num], array[num]);
		}

		private static string[] prepareSchedulePartsToCompareToDateParts(string schedule, string[] dateCompareParts)
		{
			string[] array = new string[7];
			string[] array2 = schedule.Split(' ');
			for (int i = 0; i < array2.Length; i++)
			{
				array[i] = array2[i];
			}
			string str = "";
			for (int i = 0; i < dateCompareParts.Length; i++)
			{
				if (string.IsNullOrEmpty(array[i]) || array[i] == "*")
				{
					array[i] = dateCompareParts[i];
				}
				str = str + array[i] + " ";
			}
			return array;
		}

		private static bool cronExpressionEvaluatesTrue(string schedule, DateTime dateToCompare, out DateTime nextDate)
		{
			string text = dateToCompare.ToString("s m H d M ddd yyyy");
			string[] array = text.Split(' ');
			string[] array2 = prepareSchedulePartsToCompareToDateParts(schedule, array);
			nextDate = dateToCompare;
			bool result = true;
			for (int num = array2.Length - 1; num >= 0; num--)
			{
				if (!isCronTimeForField(num, array2[num], array[num]))
				{
					result = false;
					CronTime cronTime = new CronTime(schedule, dateToCompare, num);
					nextDate = cronTime.nextDate;
					break;
				}
			}
			return result;
		}

		private static bool cronExpressionEvaluatesTrue(string schedule, DateTime dateToCompare)
		{
			string text = dateToCompare.ToString("s m H d M ddd yyyy");
			string[] array = text.Split(' ');
			string[] array2 = prepareSchedulePartsToCompareToDateParts(schedule, array);
			bool result = true;
			for (int num = array2.Length - 1; num >= 0; num--)
			{
				if (!isCronTimeForField(num, array2[num], array[num]))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		private static bool isCronTimeForField(int fieldPos, string fieldValue, string timeValue)
		{
			bool result = false;
			int num = CronUtils.ConvertCronFieldToNumber(fieldPos, timeValue);
			string[] array = fieldValue.Split(',');
			foreach (string text in array)
			{
				if (text.IndexOf("0/") == 0)
				{
					string str = text.Replace("0/", "");
					int num2 = CronUtils.ConvertCronFieldToNumber(fieldPos, str);
					if (num2 != 0 && num % num2 == 0)
					{
						result = true;
						break;
					}
				}
				else if (text.IndexOf('-') >= 0)
				{
					string[] dashSplit = text.Split('-');
					int minRangeNumValue;
					int maxRangeNumValue;
					getMinMaxValuesForRange(fieldPos, dashSplit, out minRangeNumValue, out maxRangeNumValue);
					if (minRangeNumValue <= num && num <= maxRangeNumValue)
					{
						result = true;
						break;
					}
				}
				else
				{
					int num3 = CronUtils.ConvertCronFieldToNumber(fieldPos, text);
					if (num3 == num)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private static void getMinMaxValuesForRange(int fieldPos, string[] dashSplit, out int minRangeNumValue, out int maxRangeNumValue)
		{
			minRangeNumValue = CronUtils.ConvertCronFieldToNumber(fieldPos, dashSplit[0]);
			maxRangeNumValue = CronUtils.ConvertCronFieldToNumber(fieldPos, dashSplit[1]);
		}
	}
}
