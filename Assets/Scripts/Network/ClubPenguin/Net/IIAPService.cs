using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public interface IIAPService : INetworkService
	{
		void CheckRestore(PurchaseRequest purchaseRequest);

		void CheckRestore(List<PurchaseRequest> purchaseRequestList);

		void Purchase(PurchaseRequest purchaseRequest);

		void StartPCSession(PCSessionStartRequest pcSessionRequest);

		void GetPCProductDetails(PCGetProductDetailsRequest pcGetProductDetailsRequest);
	}
}
