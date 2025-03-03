using System;

namespace Disney.Kelowna.Common.Utils
{
	internal class CronFieldListItem : IComparable
	{
		public enum CronFieldType
		{
			Single,
			Range,
			Periodic,
			Wildcard
		}

		public readonly int FieldPos;

		public readonly CronFieldType Type;

		public readonly string part;

		public readonly int[] values;

		private int currentIndex;

		public int Value
		{
			get
			{
				int num = values[0];
				if (Type == CronFieldType.Periodic)
				{
					num = values[0] * currentIndex;
				}
				else if (Type == CronFieldType.Range)
				{
					num = ((currentIndex > values[1] - values[0]) ? (-1) : Math.Min(values[0] + currentIndex, values[1]));
				}
				else if (Type == CronFieldType.Wildcard)
				{
					num = currentIndex;
				}
				else if (currentIndex > 0)
				{
					num = -1;
				}
				if ((FieldPos == 3 || FieldPos == 4) && num == 0)
				{
					num = 1;
				}
				return num;
			}
		}

		public CronFieldListItem(int fieldPos, string part)
		{
			FieldPos = fieldPos;
			this.part = part;
			values = new int[2]
			{
				-1,
				-1
			};
			if (part.IndexOf("0/") == 0)
			{
				Type = CronFieldType.Periodic;
				string s = part.Replace("0/", "");
				if (!int.TryParse(s, out values[0]))
				{
					throw new ArgumentException("Periodic value assigned is not numeric");
				}
			}
			else if (part.IndexOf('-') > 0)
			{
				Type = CronFieldType.Range;
				string[] array = part.Split('-');
				values[0] = CronUtils.ConvertCronFieldToNumber(fieldPos, array[0]);
				values[1] = CronUtils.ConvertCronFieldToNumber(fieldPos, array[1]);
			}
			else if (part == "*")
			{
				Type = CronFieldType.Wildcard;
				values[0] = 0;
			}
			else
			{
				Type = CronFieldType.Single;
				values[0] = CronUtils.ConvertCronFieldToNumber(fieldPos, part);
			}
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			CronFieldListItem cronFieldListItem = obj as CronFieldListItem;
			if (cronFieldListItem != null)
			{
				if (Type == CronFieldType.Wildcard || cronFieldListItem.Type == CronFieldType.Wildcard)
				{
					return 0;
				}
				if (Value < cronFieldListItem.Value)
				{
					return -1;
				}
				if (Value > cronFieldListItem.Value)
				{
					return 1;
				}
				return 0;
			}
			throw new ArgumentException("Trying to compare an object that is not a CronField");
		}

		public int Next()
		{
			int value = Value;
			if (Type == CronFieldType.Periodic)
			{
				currentIndex++;
			}
			else if (Type == CronFieldType.Range)
			{
				if (currentIndex <= values[1] - values[0] + 1)
				{
					currentIndex++;
				}
			}
			else if (Type == CronFieldType.Wildcard)
			{
				currentIndex++;
			}
			else if (currentIndex == 0)
			{
				currentIndex++;
			}
			return value;
		}

		public void Reset()
		{
			currentIndex = 0;
		}
	}
}
