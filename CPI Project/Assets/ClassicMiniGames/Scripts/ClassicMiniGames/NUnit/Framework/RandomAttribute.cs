using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework
{
	public class RandomAttribute : ValuesAttribute, IParameterDataSource
	{
		private enum SampleType
		{
			Raw,
			IntRange,
			DoubleRange
		}

		private SampleType sampleType;

		private int count;

		private int min;

		private int max;

		private double dmin;

		private double dmax;

		public RandomAttribute(int count)
		{
			this.count = count;
			sampleType = SampleType.Raw;
		}

		public RandomAttribute(double min, double max, int count)
		{
			this.count = count;
			dmin = min;
			dmax = max;
			sampleType = SampleType.DoubleRange;
		}

		public RandomAttribute(int min, int max, int count)
		{
			this.count = count;
			this.min = min;
			this.max = max;
			sampleType = SampleType.IntRange;
		}

		public new IEnumerable GetData(ParameterInfo parameter)
		{
			Randomizer randomizer = Randomizer.GetRandomizer(parameter);
			IList list;
			switch (sampleType)
			{
			default:
				list = randomizer.GetDoubles(count);
				break;
			case SampleType.IntRange:
				list = randomizer.GetInts(min, max, count);
				break;
			case SampleType.DoubleRange:
				list = randomizer.GetDoubles(dmin, dmax, count);
				break;
			}
			data = new object[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				data[i] = list[i];
			}
			return base.GetData(parameter);
		}
	}
}
