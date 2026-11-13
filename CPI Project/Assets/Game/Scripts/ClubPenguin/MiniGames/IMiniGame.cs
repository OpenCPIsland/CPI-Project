using ClubPenguin.Net.Domain;

namespace ClubPenguin.MiniGames
{
	public interface IMiniGame
	{
		SignedResponse<FishingResult> GetSignedResponse();
	}
}
