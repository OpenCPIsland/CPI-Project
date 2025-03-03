using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain.ScheduledEvent;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	public class ScheduledEventService : BaseNetworkService, IScheduledEventService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void PostCFCDonation(int coins)
		{
			APICall<PostCFCDonationOperation> aPICall = clubPenguinClient.ScheduledEventApi.PostCFCDonation(coins);
			aPICall.OnResponse += onPostCFCDonation;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onPostCFCDonation(PostCFCDonationOperation operation, HttpResponse httpResponse)
		{
			DonationResult responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new ScheduledEventServiceEvents.CFCDonationPosted(responseBody));
		}

		public void GetCFCDonations()
		{
			APICall<GetCFCDonationsOperation> cFCDonations = clubPenguinClient.ScheduledEventApi.GetCFCDonations();
			cFCDonations.OnResponse += onGetCFCDonations;
			cFCDonations.OnError += handleCPResponseError;
			cFCDonations.Execute();
		}

		private void onGetCFCDonations(GetCFCDonationsOperation operation, HttpResponse httpResponse)
		{
			CFCDonations responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new ScheduledEventServiceEvents.CFCDonationsLoaded(responseBody));
		}
	}
}
