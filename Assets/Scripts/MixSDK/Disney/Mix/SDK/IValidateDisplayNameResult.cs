using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IValidateDisplayNameResult
	{
		bool Success
		{
			get;
		}

		IEnumerable<string> SuggestedDisplayNames
		{
			get;
		}
	}
}
