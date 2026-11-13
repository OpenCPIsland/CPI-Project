using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface ILoginRequiresLegalMarketingUpdateResult : ILoginResult
	{
		IEnumerable<ILegalDocument> LegalDocuments
		{
			get;
		}

		IEnumerable<IMarketingItem> Marketing
		{
			get;
		}
	}
}
