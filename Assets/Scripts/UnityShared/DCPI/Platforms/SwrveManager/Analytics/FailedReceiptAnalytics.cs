using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class FailedReceiptAnalytics : GameAnalytics
	{
		private string _productId = null;

		private string _error = null;

		public string ProductId
		{
			get
			{
				return _productId;
			}
		}

		public string Error
		{
			get
			{
				return _error;
			}
		}

		public string Context
		{
			get
			{
				return "IAP";
			}
		}

		public FailedReceiptAnalytics(string productId, string error)
		{
			_productId = productId;
			_error = error;
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("context", "IAP");
			dictionary.Add("product_id", _productId);
			dictionary.Add("error", _error);
			return dictionary;
		}

		public override string GetSwrveEvent()
		{
			return "failed_receipt";
		}
	}
}
