using ClubPenguin.Classic;

namespace ClubPenguin.Actions
{
	public class LaunchClassicMiniGamesAction : Action
	{
		protected override void Update()
		{
			ClassicMiniGames.LaunchClassicMiniGames();
			Completed();
		}
	}
}
