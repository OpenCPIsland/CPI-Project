using ClubPenguin.Net.Client;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	public class NewsfeedService : BaseNetworkService, INewsfeedService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void GetLatestPostTime(string language)
		{
			APICall<GetLatestPostTimeOperation> latestPostTime = clubPenguinClient.NewsfeedApi.GetLatestPostTime(language);
			latestPostTime.OnResponse += onLatestPostTimeReceived;
			latestPostTime.Execute();
		}

		private void onLatestPostTimeReceived(GetLatestPostTimeOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new NewsfeedServiceEvents.LatestPostTime(operation.LatestPostTimeResponse.timestamp));
		}
	}
}
