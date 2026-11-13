using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IAgeBand
	{
		IRegistrationPermissions Permissions
		{
			get;
		}

		IEnumerable<ILegalDocument> LegalDocuments
		{
			get;
		}

		IEnumerable<string> LegalDocumentsTypeOrder
		{
			get;
		}

		IEnumerable<IMarketingItem> Marketing
		{
			get;
		}

		AgeBandType AgeBandType
		{
			get;
		}

		string CountryCode
		{
			get;
		}
	}
}
