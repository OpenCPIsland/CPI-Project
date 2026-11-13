using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class ValidateDisplayNamesResult : IValidateDisplayNamesResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public IEnumerable<string> DisplayNames
		{
			get;
			private set;
		}

		public ValidateDisplayNamesResult(bool success, IEnumerable<string> nameResults)
		{
			Success = success;
			DisplayNames = nameResults.ToList();
		}
	}
}
