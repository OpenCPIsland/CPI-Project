using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IValidateDisplayNamesResult
	{
		bool Success
		{
			get;
		}

		IEnumerable<string> DisplayNames
		{
			get;
		}
	}
}
