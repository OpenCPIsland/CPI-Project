using System;

namespace Disney.Kelowna.Common.Utils
{
	public class CronTime
	{
		private DateTime start;

		private string schedule;

		private CronSchedule cronSchedule;

		private int offset;

		public DateTime nextDate
		{
			get
			{
				return cronSchedule.GetNextDate(start, offset);
			}
		}

		public int remaining
		{
			get
			{
				return (int)nextDate.Subtract(start).TotalSeconds;
			}
		}

		public CronTime(string schedule, DateTime start, int offset = 7)
		{
			this.start = start;
			this.offset = offset;
			cronSchedule = new CronSchedule();
			this.schedule = schedule;
			string[] array = start.ToString("s m H d M ddd yyyy").Split(' ');
			string[] array2 = this.schedule.Split(' ');
			for (int i = 0; i < array.Length; i++)
			{
				CronFieldList.ValuesSource listSource;
				string[] array3;
				if (array2.Length <= i)
				{
					listSource = CronFieldList.ValuesSource.CurrentDate;
					array3 = array[i].Split(',');
				}
				else
				{
					listSource = CronFieldList.ValuesSource.CronExpression;
					array3 = array2[i].Split(',');
				}
				CronFieldList cronFieldList = new CronFieldList(listSource);
				string[] array4 = array3;
				foreach (string part in array4)
				{
					cronFieldList.Add(new CronFieldListItem(i, part));
				}
				cronFieldList.Sort();
				cronSchedule.Add(i, cronFieldList);
			}
		}
	}
}
