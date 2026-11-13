using Disney.Mix.SDK;
using System.Collections.Generic;

namespace ClubPenguin
{
	public struct DUpdateProfilePayload
	{
		public string FirstName
		{
			get;
			set;
		}

		public string ParentEmail
		{
			get;
			set;
		}

		public IEnumerable<ILegalDocument> AcceptedLegalDocs
		{
			get;
			set;
		}
	}
}
