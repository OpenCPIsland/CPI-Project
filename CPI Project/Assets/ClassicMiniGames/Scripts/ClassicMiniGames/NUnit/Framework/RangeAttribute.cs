namespace NUnit.Framework
{
	public class RangeAttribute : ValuesAttribute
	{
		public RangeAttribute(int from, int to)
			: this(from, to, 1)
		{
		}

		public RangeAttribute(int from, int to, int step)
		{
			int num = (to - from) / step + 1;
			data = new object[num];
			int num2 = 0;
			int num3 = from;
			while (num2 < num)
			{
				data[num2++] = num3;
				num3 += step;
			}
		}

		public RangeAttribute(long from, long to, long step)
		{
			long num = (to - from) / step + 1;
			data = new object[num];
			int num2 = 0;
			long num3 = from;
			while (num2 < num)
			{
				data[num2++] = num3;
				num3 += step;
			}
		}

		public RangeAttribute(double from, double to, double step)
		{
			double num = step / 1000.0;
			int num2 = (int)((to - from) / step + num + 1.0);
			data = new object[num2];
			int num3 = 0;
			double num4 = from;
			while (num3 < num2)
			{
				data[num3++] = num4;
				num4 += step;
			}
		}

		public RangeAttribute(float from, float to, float step)
		{
			float num = step / 1000f;
			int num2 = (int)((to - from) / step + num + 1f);
			data = new object[num2];
			int num3 = 0;
			float num4 = from;
			while (num3 < num2)
			{
				data[num3++] = num4;
				num4 += step;
			}
		}
	}
}
