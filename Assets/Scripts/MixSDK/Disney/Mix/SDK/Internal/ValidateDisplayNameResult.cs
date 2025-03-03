using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class ValidateDisplayNameResult : IValidateDisplayNameResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public IEnumerable<string> SuggestedDisplayNames
		{
			get;
			private set;
		}

		public ValidateDisplayNameResult(bool success)
		{
			Success = success;
			SuggestedDisplayNames = new List<string>();
		}

		public ValidateDisplayNameResult(bool success, IEnumerable<string> suggestions)
		{
			Success = success;
			SuggestedDisplayNames = suggestions;
		}
	}
}
