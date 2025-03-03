using System;

namespace Disney.Mix.SDK.Internal
{
	public class SystemRandom : IRandom
	{
		private readonly Random random;

		public SystemRandom()
		{
			random = new Random();
		}

		public int Next(int maxValue)
		{
			return random.Next(maxValue);
		}

		public long NextLong()
		{
			long num = (long)random.Next() << 32;
			long num2 = random.Next();
			return num | num2;
		}
	}
}
