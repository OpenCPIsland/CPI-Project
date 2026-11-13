using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public class BreadcrumbService : BaseNetworkService, IBreadcrumbService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void AddBreadcrumbIds(List<Breadcrumb> breadcrumbIds)
		{
			APICall<AddBreadcrumbIdsOperation> aPICall = clubPenguinClient.BreadcrumbApi.AddBreadcrumbIds(breadcrumbIds);
			aPICall.OnResponse += onBreadcrumbIdsAdded;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		public void RemoveBreadcrumbIds(List<Breadcrumb> breadcrumbIds)
		{
			APICall<RemoveBreadcrumbIdsOperation> aPICall = clubPenguinClient.BreadcrumbApi.RemoveBreadcrumbIds(breadcrumbIds);
			aPICall.OnResponse += onBreadcrumbIdsRemoved;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onBreadcrumbIdsAdded(AddBreadcrumbIdsOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(BreadcrumbServiceEvents.BreadcrumbIdsAdded));
		}

		private void onBreadcrumbIdsRemoved(RemoveBreadcrumbIdsOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(BreadcrumbServiceEvents.BreadcrumbIdsRemoved));
		}
	}
}
