using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public static class MinigameServiceEvents
	{
		public struct FishingResultRecieved
		{
			public SignedResponse<FishingResult> Result;

			public FishingResultRecieved(SignedResponse<FishingResult> result)
			{
				Result = result;
			}
		}

		public struct FishCaught
		{
			public readonly string WinningRewardName;

			public FishCaught(string winningRewardName)
			{
				WinningRewardName = winningRewardName;
			}
		}
	}
}
