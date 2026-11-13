using System;
using System.Text;

namespace SwrveUnity.IAP
{
	public class RawReceipt : IapReceipt
	{
		private string base64encodedreceipt;

		private RawReceipt(string r)
		{
			base64encodedreceipt = Convert.ToBase64String(Encoding.UTF8.GetBytes(r));
		}

		public static IapReceipt FromString(string r)
		{
			return new RawReceipt(r);
		}

		public string GetBase64EncodedReceipt()
		{
			return base64encodedreceipt;
		}
	}
}
