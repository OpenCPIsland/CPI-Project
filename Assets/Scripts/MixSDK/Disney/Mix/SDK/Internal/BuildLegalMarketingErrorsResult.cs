using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class BuildLegalMarketingErrorsResult
	{
		public IEnumerable<ILegalDocument> LegalDocuments
		{
			get;
			private set;
		}

		public IEnumerable<IMarketingItem> Marketing
		{
			get;
			private set;
		}

		public BuildLegalMarketingErrorsResult(IEnumerable<ILegalDocument> legalDocuments, IEnumerable<IMarketingItem> marketing)
		{
			LegalDocuments = legalDocuments;
			Marketing = marketing;
		}
	}
}
