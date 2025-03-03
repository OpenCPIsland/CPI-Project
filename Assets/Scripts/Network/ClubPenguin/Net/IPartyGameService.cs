namespace ClubPenguin.Net
{
	public interface IPartyGameService : INetworkService
	{
		void SendSessionMessage(int sessionId, int type, object data);
	}
}
