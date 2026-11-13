using System.Runtime.InteropServices;

namespace ClubPenguin.Benchmarking
{
	public static class BenchmarkRuntimeProfileEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct RuntimeProfileReset
		{
		}

		public struct RuntimeProfileStart
		{
			public readonly int PollRate;

			public readonly bool PollMemory;

			public RuntimeProfileStart(bool pollMemory, int pollRate = 30)
			{
				PollMemory = pollMemory;
				PollRate = pollRate;
				if (PollRate <= 0)
				{
					PollRate = 30;
				}
			}
		}
	}
}
