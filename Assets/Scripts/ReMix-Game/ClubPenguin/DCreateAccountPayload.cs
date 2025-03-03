using Disney.Mix.SDK;
using System.Collections.Generic;

namespace ClubPenguin
{
	public struct DCreateAccountPayload
	{
		public string Username
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

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

		public string LangPref
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
