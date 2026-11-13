using System;

namespace Disney.Mix.SDK.Internal
{
	public class SystemEpochTime : IEpochTime
	{
		private static readonly DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public long ReferenceTime
		{
			set
			{
				DateTime d = epochStart.AddMilliseconds(value);
				Offset = d - DateTime.UtcNow;
			}
		}

		public long OffsetMilliseconds
		{
			set
			{
				ReferenceTime = Milliseconds + value;
			}
		}

		public TimeSpan Offset
		{
			get;
			private set;
		}

		public uint Seconds
		{
			get
			{
				return SecondsSince(DateTime.UtcNow);
			}
		}

		public long Milliseconds
		{
			get
			{
				return MillisecondsSince(DateTime.UtcNow);
			}
		}

		public DateTime UtcNow
		{
			get
			{
				return DateTime.UtcNow.Add(Offset);
			}
		}

		public SystemEpochTime()
		{
			Offset = TimeSpan.Zero;
		}

		public uint SecondsSince(DateTime time)
		{
			return (uint)(time - epochStart).Add(Offset).TotalSeconds;
		}

		public uint RawSecondsSince(DateTime time)
		{
			return (uint)(time - epochStart).TotalSeconds;
		}

		public long MillisecondsSince(DateTime time)
		{
			return (long)(time - epochStart).Add(Offset).TotalMilliseconds;
		}

		public long RawMillisecondsSince(DateTime time)
		{
			return (long)(time - epochStart).TotalMilliseconds;
		}

		public DateTime FromSeconds(uint seconds)
		{
			return epochStart.AddSeconds(seconds);
		}

		public DateTime FromMilliseconds(long millis)
		{
			return epochStart.AddMilliseconds(millis);
		}

		public DateTime FromMillisecondsAndOffset(long millis)
		{
			return epochStart.AddMilliseconds(millis).Add(Offset);
		}

		public long MillisecondsWithOffset(long millis)
		{
			return millis + (long)Offset.TotalMilliseconds;
		}
	}
}
