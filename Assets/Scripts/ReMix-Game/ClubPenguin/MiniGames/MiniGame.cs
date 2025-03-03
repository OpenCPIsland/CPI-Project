using ClubPenguin.Net.Domain;

namespace ClubPenguin.MiniGames
{
	public class MiniGame : IMiniGame
	{
		private SignedResponse<FishingResult> response;

		public MiniGame(SignedResponse<FishingResult> signedResponse)
		{
			response = signedResponse;
		}

		public SignedResponse<FishingResult> GetSignedResponse()
		{
			return response;
		}
	}
}
