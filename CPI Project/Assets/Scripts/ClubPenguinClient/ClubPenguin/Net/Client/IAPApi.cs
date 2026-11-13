using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	public class IAPApi
	{
		private ClubPenguinClient clubPenguinClient;

		public IAPApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<GetProductsOperation> GetProducts()
		{
			GetProductsOperation operation = new GetProductsOperation();
			return new APICall<GetProductsOperation>(clubPenguinClient, operation);
		}

		public APICall<CheckRestoreOperation> CheckRestore(PurchaseRequest purchase)
		{
			CheckRestoreOperation operation = new CheckRestoreOperation(purchase);
			return new APICall<CheckRestoreOperation>(clubPenguinClient, operation);
		}

		public APICall<CheckRestoreListOperation> CheckRestore(List<PurchaseRequest> purchaseList)
		{
			CheckRestoreListOperation operation = new CheckRestoreListOperation(purchaseList);
			return new APICall<CheckRestoreListOperation>(clubPenguinClient, operation);
		}

		public APICall<PurchaseOperation> Purchase(PurchaseRequest purchase)
		{
			PurchaseOperation operation = new PurchaseOperation(purchase);
			return new APICall<PurchaseOperation>(clubPenguinClient, operation);
		}

		public APICall<PCSessionStartOperation> PCStartSession(PCSessionStartRequest session)
		{
			PCSessionStartOperation operation = new PCSessionStartOperation(session);
			return new APICall<PCSessionStartOperation>(clubPenguinClient, operation);
		}

		public APICall<PCGetProductDetailsOperation> PCGetProductsDetails(PCGetProductDetailsRequest detailsRequest)
		{
			PCGetProductDetailsOperation operation = new PCGetProductDetailsOperation(detailsRequest);
			return new APICall<PCGetProductDetailsOperation>(clubPenguinClient, operation);
		}
	}
}
