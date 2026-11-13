using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct PurchaseRequest
	{
		public string receiptID;

		public string productID;

		public string signature;

		public string originalJson;

		public string store;

		public BiUserId biUserId;

		public PurchaseRequest(string receipt, string product, string signature_a, string original_json, string store_type, BiUserId bi_id)
		{
			receiptID = receipt;
			productID = product;
			signature = signature_a;
			originalJson = original_json;
			store = store_type;
			biUserId = bi_id;
		}
	}
}
