using ClubPenguin.Net.Client;

namespace ClubPenguin.Net
{
	public class ModerationService : BaseNetworkService, IModerationService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void ReportPlayer(string displayName, string reason)
		{
			APICall<ReportPlayerOperation> aPICall = clubPenguinClient.ModerationApi.ReportPlayer(displayName, reason);
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}
	}
}
