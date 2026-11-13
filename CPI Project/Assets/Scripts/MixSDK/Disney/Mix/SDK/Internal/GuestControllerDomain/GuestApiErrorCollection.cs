using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class GuestApiErrorCollection
	{
		public string keyCategory
		{
			get;
			set;
		}

		public List<GuestApiError> errors
		{
			get;
			set;
		}
	}
}
