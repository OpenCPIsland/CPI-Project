using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	internal class IAPService : BaseNetworkService, IIAPService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void CheckRestore(PurchaseRequest purchaseRequest)
		{
			APICall<CheckRestoreOperation> aPICall = clubPenguinClient.IAPApi.CheckRestore(purchaseRequest);
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += checkRestoreError;
			aPICall.OnResponse += checkRestoreReturned;
			aPICall.Execute();
		}

		public void CheckRestore(List<PurchaseRequest> purchaseRequestList)
		{
			APICall<CheckRestoreListOperation> aPICall = clubPenguinClient.IAPApi.CheckRestore(purchaseRequestList);
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += checkRestoreError;
			aPICall.OnResponse += checkRestoreReturned;
			aPICall.Execute();
		}

		public void Purchase(PurchaseRequest purchaseRequest)
		{
			APICall<PurchaseOperation> aPICall = clubPenguinClient.IAPApi.Purchase(purchaseRequest);
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += purchaseError;
			aPICall.OnResponse += purchaseReturned;
			aPICall.Execute();
		}

		public void StartPCSession(PCSessionStartRequest pcSessionStartRequest)
		{
			APICall<PCSessionStartOperation> aPICall = clubPenguinClient.IAPApi.PCStartSession(pcSessionStartRequest);
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += pcSessionStartedError;
			aPICall.OnResponse += pcSessionStarted;
			aPICall.Execute();
		}

		public void GetPCProductDetails(PCGetProductDetailsRequest pcGetProductDetailsRequest)
		{
			APICall<PCGetProductDetailsOperation> aPICall = clubPenguinClient.IAPApi.PCGetProductsDetails(pcGetProductDetailsRequest);
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += productDetailsLoadedError;
			aPICall.OnResponse += productsDetailsLoaded;
			aPICall.Execute();
		}

		private void pcSessionStarted(PCSessionStartOperation operation, HttpResponse httpResponse)
		{
			PCSessionStartResponse pCSessionStartResponse = operation.PCSessionStartResponse;
			Service.Get<EventDispatcher>().DispatchEvent(new IAPServiceEvents.SessionStarted(pCSessionStartResponse.SessionId, pCSessionStartResponse.SessionSummary));
		}

		private void pcSessionStartedError(PCSessionStartOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IAPServiceErrors.SessionStartError));
		}

		private void productsDetailsLoaded(PCGetProductDetailsOperation operation, HttpResponse httpResponse)
		{
			PCGetProductDetailsResponse pCGetProductDetailsResponse = operation.PCGetProductDetailsResponse;
			Service.Get<EventDispatcher>().DispatchEvent(new IAPServiceEvents.PCProductDetailsReturned(pCGetProductDetailsResponse));
		}

		private void productDetailsLoadedError(PCGetProductDetailsOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IAPServiceErrors.ProductsDetailsLoadedError));
		}

		private void checkRestoreReturned(CheckRestoreOperation operation, HttpResponse httpResponse)
		{
			PurchaseResponse purchaseResponse = operation.PurchaseResponse;
			Service.Get<EventDispatcher>().DispatchEvent(new IAPServiceEvents.CheckRestoreReturned(purchaseResponse));
		}

		private void checkRestoreError(CheckRestoreOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IAPServiceErrors.CheckRestoreError));
		}

		private void checkRestoreReturned(CheckRestoreListOperation operation, HttpResponse httpResponse)
		{
			PurchaseResponse purchaseResponse = operation.PurchaseResponse;
			Service.Get<EventDispatcher>().DispatchEvent(new IAPServiceEvents.CheckRestoreReturned(purchaseResponse));
			refreshSFSRights(purchaseResponse.rights);
		}

		private void checkRestoreError(CheckRestoreListOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IAPServiceErrors.CheckRestoreError));
		}

		private void purchaseReturned(PurchaseOperation operation, HttpResponse httpResponse)
		{
			PurchaseResponse purchaseResponse = operation.PurchaseResponse;
			Service.Get<EventDispatcher>().DispatchEvent(new IAPServiceEvents.PurchaseReturned(purchaseResponse));
			refreshSFSRights(purchaseResponse.rights);
		}

		private void purchaseError(PurchaseOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IAPServiceErrors.PurchaseError));
		}

		private void membershipGrantReturned(QAMembershipGrantOperation operation, HttpResponse httpResponse)
		{
			PurchaseResponse purchaseResponse = operation.PurchaseResponse;
			Service.Get<EventDispatcher>().DispatchEvent(new IAPServiceEvents.PurchaseReturned(purchaseResponse));
			refreshSFSRights(purchaseResponse.rights);
		}

		private void membershipGrantError(QAMembershipGrantOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IAPServiceErrors.MembershipGrantError));
		}

		private void refreshSFSRights(SignedResponse<MembershipRightsRefresh> signedRights)
		{
			if (signedRights != null)
			{
				clubPenguinClient.GameServer.RefreshMembership(signedRights);
			}
		}
	}
}
