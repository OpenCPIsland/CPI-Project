using System;
using System.Collections.Generic;

namespace Disney.Kelowna.Common.Utils
{
	internal class CronSchedule : Dictionary<int, CronFieldList>
	{
		public DateTime GetNextDate(DateTime startDate, int failedFieldPosition)
		{
			if (failedFieldPosition >= 7)
			{
				return startDate;
			}
			DateTime dateTime = startDate;
			int[] array = new int[7];
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, CronFieldList> current = enumerator.Current;
					CronFieldList value = current.Value;
					CronFieldListItem cronFieldListItem = value.Next();
					if (cronFieldListItem == null)
					{
						return dateTime;
					}
					if (current.Key != 5)
					{
						int dateValueForField = getDateValueForField(current.Key, dateTime);
						if (current.Key < failedFieldPosition)
						{
							array[current.Key] = cronFieldListItem.Value - dateValueForField;
							if (array[current.Key] < 0)
							{
								dateTime = incrementValueForField(getNextPositionToIterate(current.Key), dateTime);
							}
						}
						else if (current.Key == failedFieldPosition)
						{
							while (cronFieldListItem != null && cronFieldListItem.Value > -1 && cronFieldListItem.Value < dateValueForField)
							{
								cronFieldListItem.Next();
								value.Sort();
								cronFieldListItem = value.Next();
							}
							if (cronFieldListItem != null && cronFieldListItem.Value > -1)
							{
								array[current.Key] = cronFieldListItem.Value - dateValueForField;
							}
							else if (value.ListSource == CronFieldList.ValuesSource.CronExpression)
							{
								value.Reset();
								value.Sort();
								return GetNextDate(startDate, getNextPositionToIterate(current.Key));
							}
						}
						else
						{
							while (cronFieldListItem != null && cronFieldListItem.Value > -1 && cronFieldListItem.Value < dateValueForField)
							{
								cronFieldListItem.Next();
								value.Sort();
								cronFieldListItem = value.Next();
							}
							if (cronFieldListItem != null && cronFieldListItem.Value > -1)
							{
								array[current.Key] = cronFieldListItem.Value - dateValueForField;
							}
						}
						dateTime = incrementValueForField(cronFieldListItem.FieldPos, dateTime, array[current.Key]);
						value.Reset();
						value.Sort();
					}
				}
			}
			int num = 5;
			if (failedFieldPosition == num)
			{
				CronFieldList cronFieldList = base[num];
				CronFieldListItem cronFieldListItem2 = cronFieldList.Next();
				while (cronFieldListItem2 != null && cronFieldListItem2.Value > -1 && cronFieldListItem2.Value != (int)dateTime.DayOfWeek)
				{
					while (cronFieldListItem2 != null && cronFieldListItem2.Value > -1 && cronFieldListItem2.Value != (int)dateTime.DayOfWeek)
					{
						if (cronFieldListItem2.Value == (int)dateTime.DayOfWeek)
						{
							return dateTime;
						}
						cronFieldListItem2.Next();
						cronFieldList.Sort();
						cronFieldListItem2 = cronFieldList.Next();
					}
					cronFieldList.Reset();
					cronFieldList.Sort();
					cronFieldListItem2 = cronFieldList.Next();
					dateTime = dateTime.AddDays(1.0);
				}
			}
			return dateTime;
		}

		private int getDateValueForField(int fieldPos, DateTime dateTime)
		{
			string[] array = dateTime.ToString("s m H d M ddd yyyy").Split(' ');
			return CronUtils.ConvertCronFieldToNumber(fieldPos, array[fieldPos]);
		}

		private int getNextPositionToIterate(int key)
		{
			if (key == 4)
			{
				return key + 2;
			}
			return key + 1;
		}

		private DateTime incrementValueForField(int fieldPos, DateTime dateTimeRef, int amount = 1)
		{
			if (fieldPos == 0)
			{
				dateTimeRef = dateTimeRef.AddSeconds(amount);
			}
			else if (fieldPos == 1)
			{
				dateTimeRef = dateTimeRef.AddMinutes(amount);
			}
			else if (fieldPos == 2)
			{
				dateTimeRef = dateTimeRef.AddHours(amount);
			}
			else if (fieldPos == 4)
			{
				dateTimeRef = dateTimeRef.AddMonths(amount);
			}
			else if (fieldPos == 5 || fieldPos == 6)
			{
				dateTimeRef = dateTimeRef.AddYears(amount);
			}
			else if (fieldPos == 3)
			{
				dateTimeRef = dateTimeRef.AddDays(amount);
			}
			return dateTimeRef;
		}
	}
}
