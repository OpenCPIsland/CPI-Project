using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IEpochTime
	{
		long ReferenceTime
		{
			set;
		}

		long OffsetMilliseconds
		{
			set;
		}

		TimeSpan Offset
		{
			get;
		}

		uint Seconds
		{
			get;
		}

		long Milliseconds
		{
			get;
		}

		DateTime UtcNow
		{
			get;
		}

		uint SecondsSince(DateTime time);

		uint RawSecondsSince(DateTime time);

		long MillisecondsSince(DateTime time);

		long RawMillisecondsSince(DateTime time);

		DateTime FromSeconds(uint seconds);

		DateTime FromMilliseconds(long millis);

		DateTime FromMillisecondsAndOffset(long millis);

		long MillisecondsWithOffset(long millis);
	}
}
