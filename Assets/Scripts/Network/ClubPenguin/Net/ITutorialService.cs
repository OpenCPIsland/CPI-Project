using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public interface ITutorialService : INetworkService
	{
		void SetTutorial(Tutorial tutorial);
	}
}
