using LitJson;
using Prime31;
using System;
using System.Collections.Generic;

public class PurchaseInfo
{
	public enum PurchaseState
	{
		Purchased,
		Canceled,
		Refunded,
		Deferred,
		Failed
	}

	public string packageName
	{
		get;
		private set;
	}

	public string orderId
	{
		get;
		private set;
	}

	public string sku
	{
		get;
		private set;
	}

	public string developerPayload
	{
		get;
		private set;
	}

	public string type
	{
		get;
		private set;
	}

	public long purchaseTime
	{
		get;
		private set;
	}

	public PurchaseState purchaseState
	{
		get;
		private set;
	}

	public string token
	{
		get;
		private set;
	}

	public string signature
	{
		get;
		private set;
	}

	public string payload
	{
		get;
		private set;
	}

	public string originalJson
	{
		get;
		private set;
	}

	public static List<PurchaseInfo> fromList(List<JsonData> transactions, List<PurchaseInfo> passedPurchaseInfos = null)
	{
		List<PurchaseInfo> list = new List<PurchaseInfo>();
		if (passedPurchaseInfos != null)
		{
			list = passedPurchaseInfos;
		}
		foreach (JsonData transaction in transactions)
		{
			list.Add(new PurchaseInfo(transaction));
		}
		return list;
	}

	public PurchaseInfo(JsonData transaction)
	{
		if (transaction.Contains("order") && transaction["order"].Contains("Order"))
		{
			JsonData jsonData = transaction["order"]["Order"];
			if (jsonData.Contains("OrderNumber"))
			{
				orderId = jsonData["OrderNumber"].ToString();
			}
			if (jsonData.Contains("Items") && jsonData["Items"].Count > 0 && jsonData["Items"][0].Contains("PricingPlan") && jsonData["Items"][0]["PricingPlan"].Contains("Id"))
			{
				sku = jsonData["Items"][0]["PricingPlan"]["Id"].ToString();
			}
			DateTime result;
			if (jsonData.Contains("Ordered") && DateTime.TryParse((string)jsonData["Ordered"], out result))
			{
				purchaseTime = result.toEpochTime();
			}
			purchaseState = PurchaseState.Purchased;
			originalJson = jsonData.ToJson();
			signature = null;
		}
	}

	public PurchaseInfo(string i_orderId, string i_sku, long i_purchaseTime, string i_token, string i_purchase_state, string i_signature = null, string i_payload = null)
	{
		orderId = i_orderId;
		sku = i_sku;
		purchaseTime = i_purchaseTime;
		token = i_token;
		purchaseState = getPurchaseState(i_purchase_state);
		signature = i_signature;
		payload = i_payload;
	}

	public bool IsPurchased()
	{
		return purchaseState == PurchaseState.Purchased;
	}

	public override string ToString()
	{
		return string.Format("<PurchaseInfo> orderId: {0}, productId: {1}, purchaseTime: {2}, purchaseToken: {3}, signature: {4}, payload: {5}", orderId, sku, purchaseTime, token, signature, payload);
	}

	private PurchaseState getPurchaseState(string pt)
	{
		PurchaseState result = PurchaseState.Purchased;
		switch (pt)
		{
		case "Purchased":
			result = PurchaseState.Purchased;
			break;
		case "Canceled":
			result = PurchaseState.Canceled;
			break;
		case "Refunded":
			result = PurchaseState.Refunded;
			break;
		}
		return result;
	}
}
