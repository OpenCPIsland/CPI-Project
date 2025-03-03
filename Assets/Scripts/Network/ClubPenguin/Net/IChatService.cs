namespace ClubPenguin.Net
{
	public interface IChatService : INetworkService
	{
		void SendActivity();

		void SendActivityCancel();

		void SendMessage(string message, int sizzleClipID, string questId = null, string objective = null);
	}
}
