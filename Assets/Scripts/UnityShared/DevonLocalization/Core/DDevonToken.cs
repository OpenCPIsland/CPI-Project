using System;
using System.Collections.Generic;

namespace DevonLocalization.Core
{
	[Serializable]
	public class DDevonToken
	{
		public string token;

		public string devonString;

		public Dictionary<string, string> metadata;
	}
}
