using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class AgeBand : IAgeBand
	{
		public IRegistrationPermissions Permissions
		{
			get;
			private set;
		}

		public IEnumerable<ILegalDocument> LegalDocuments
		{
			get;
			private set;
		}

		public IEnumerable<string> LegalDocumentsTypeOrder
		{
			get;
			private set;
		}

		public IEnumerable<IMarketingItem> Marketing
		{
			get;
			private set;
		}

		public AgeBandType AgeBandType
		{
			get;
			private set;
		}

		public string CountryCode
		{
			get;
			private set;
		}

		public AgeBand(IRegistrationPermissions permissions, IEnumerable<ILegalDocument> legalDocuments, IEnumerable<string> legalDocumentsTypeOrder, IEnumerable<IMarketingItem> marketing, AgeBandType type, string countryCode)
		{
			Permissions = permissions;
			LegalDocuments = legalDocuments;
			LegalDocumentsTypeOrder = legalDocumentsTypeOrder;
			Marketing = marketing;
			AgeBandType = type;
			CountryCode = countryCode;
		}
	}
}
