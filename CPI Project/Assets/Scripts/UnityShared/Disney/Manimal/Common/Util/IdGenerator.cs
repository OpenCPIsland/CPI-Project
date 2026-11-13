using System.Threading;

namespace Disney.Manimal.Common.Util
{
	public static class IdGenerator<T>
	{
		private const int MinValue = 0;

		private static int _id = 0;

		public static uint Get()
		{
			return (uint)Interlocked.Increment(ref _id);
		}

		public static void Reset()
		{
			Interlocked.Exchange(ref _id, 0);
		}
	}
}
