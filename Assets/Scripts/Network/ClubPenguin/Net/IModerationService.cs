namespace ClubPenguin.Net
{
	public interface IModerationService : INetworkService
	{
		void ReportPlayer(string displayName, string reason);
	}
}
