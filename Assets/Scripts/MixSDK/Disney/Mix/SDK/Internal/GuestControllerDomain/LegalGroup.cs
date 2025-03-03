using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class LegalGroup
	{
		public List<string> documentTypeOrder
		{
			get;
			set;
		}

		public Dictionary<string, DocumentType> documents
		{
			get;
			set;
		}

		public List<LegalProxy> CREATE
		{
			get;
			set;
		}
	}
}
