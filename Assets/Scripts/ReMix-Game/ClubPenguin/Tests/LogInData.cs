using System.Collections.Generic;

namespace ClubPenguin.Tests
{
	public class LogInData
	{
		public string etag
		{
			get;
			set;
		}

		public Profile profile
		{
			get;
			set;
		}

		public DisplayName displayName
		{
			get;
			set;
		}

		public Token token
		{
			get;
			set;
		}

		public List<MarketingItem> marketing
		{
			get;
			set;
		}
	}
}
