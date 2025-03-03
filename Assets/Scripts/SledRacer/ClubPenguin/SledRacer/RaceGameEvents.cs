using System.Runtime.InteropServices;

namespace ClubPenguin.SledRacer
{
	public static class RaceGameEvents
	{
		public enum EventTypes
		{
			Launch
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct Start
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct Launch
		{
		}

		public struct RaceFinished
		{
			public RaceResults RaceResults;

			public long[] RankTimes;

			public RaceFinished(RaceResults raceResults, long[] rankTimes)
			{
				RaceResults = raceResults;
				RankTimes = rankTimes;
			}
		}

		public struct RaceFinishPopupOpened
		{
			public readonly string TrackId;

			public RaceFinishPopupOpened(string trackId)
			{
				TrackId = trackId;
			}
		}

		public struct RaceFinishPopupClosed
		{
			public readonly string TrackId;

			public RaceFinishPopupClosed(string trackId)
			{
				TrackId = trackId;
			}
		}
	}
}
