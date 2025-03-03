using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	public abstract class BaseNetworkService : INetworkService
	{
		protected ClubPenguinClient clubPenguinClient;

		public virtual void Initialize(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
			setupListeners();
		}

		protected abstract void setupListeners();

		protected void handleCPResponse(CPResponse response)
		{
			if (response.wsEvents != null)
			{
				for (int i = 0; i < response.wsEvents.Count; i++)
				{
					clubPenguinClient.GameServer.SendWebServiceEvent(response.wsEvents[i]);
				}
			}
		}

		protected void handleCPResponseError(HttpResponse httpResponse)
		{
			NetworkErrorService.dispatchErrorEvent(httpResponse);
		}

		public virtual void handleCPResponseError(CPAPIHttpOperation operation, HttpResponse httpResponse)
		{
			handleCPResponseError(httpResponse);
		}
	}
}
