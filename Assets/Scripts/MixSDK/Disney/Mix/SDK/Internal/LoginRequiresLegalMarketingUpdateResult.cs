using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	internal class LoginRequiresLegalMarketingUpdateResult : ILoginRequiresLegalMarketingUpdateResult, ILoginResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public ISession Session
		{
			get;
			private set;
		}

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

		public LoginRequiresLegalMarketingUpdateResult()
		{
			Success = false;
			Session = null;
		}

		public LoginRequiresLegalMarketingUpdateResult(bool success, ISession session, IEnumerable<ILegalDocument> legalDocuments, IEnumerable<IMarketingItem> marketing)
		{
			Success = success;
			Session = session;
			LegalDocuments = legalDocuments;
			Marketing = marketing;
		}
	}
}
