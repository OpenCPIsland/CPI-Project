using System;

namespace Disney.Kelowna.Common
{
	public class RollingStatistics
	{
		public readonly int N;

		private float oldSample;

		public int SampleCount
		{
			get;
			private set;
		}

		public float Maximum
		{
			get;
			private set;
		}

		public float Minimum
		{
			get;
			private set;
		}

		public float Average
		{
			get;
			private set;
		}

		public float Variance
		{
			get;
			private set;
		}

		public float StandardDeviation
		{
			get
			{
				return (float)Math.Sqrt(Variance);
			}
		}

		public RollingStatistics(int windowSize)
		{
			if (windowSize < 2)
			{
				throw new ArgumentOutOfRangeException("windowSize");
			}
			N = windowSize;
			Maximum = float.MinValue;
			Minimum = float.MaxValue;
		}

		public void AddSample(float sample)
		{
			SampleCount++;
			Maximum = Math.Max(Maximum, sample);
			Minimum = Math.Min(Minimum, sample);
			if (SampleCount > 1)
			{
				float num = sample - oldSample;
				float average = Average;
				Average += num / (float)N;
				Variance += num * (sample - Average + oldSample - average) / (float)(N - 1);
			}
			else
			{
				Average = sample;
				Variance = 0f;
			}
			oldSample = sample;
		}

		public override string ToString()
		{
			return string.Format("Max: {0}, Min: {1}, Ave: {2}, StdDev: {3}, Cnt: {4}/{5}", Maximum, Minimum, Average, StandardDeviation, SampleCount, N);
		}
	}
}
