using System.Collections.Generic;
using System.Linq;

namespace Disney.Kelowna.Common.Utils
{
	internal class CronFieldList : List<CronFieldListItem>
	{
		public enum ValuesSource
		{
			CronExpression,
			CurrentDate
		}

		public readonly ValuesSource ListSource;

		private int currentIndex;

		public CronFieldList(ValuesSource listSource = ValuesSource.CronExpression)
		{
			ListSource = listSource;
		}

		public CronFieldListItem Next()
		{
			CronFieldListItem cronFieldListItem = null;
			if (currentIndex < base.Count)
			{
				cronFieldListItem = this.ElementAt(currentIndex);
				while (currentIndex + 1 < base.Count && cronFieldListItem.Value == -1)
				{
					currentIndex++;
					cronFieldListItem = this.ElementAt(currentIndex);
				}
			}
			return cronFieldListItem;
		}

		public void Reset()
		{
			for (int i = 0; i < base.Count; i++)
			{
				this.ElementAt(i).Reset();
			}
			currentIndex = 0;
		}
	}
}
