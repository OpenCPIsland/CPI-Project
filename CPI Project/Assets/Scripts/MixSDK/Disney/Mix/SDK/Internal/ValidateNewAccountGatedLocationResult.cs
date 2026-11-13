using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class ValidateNewAccountGatedLocationResult : IValidateNewAccountGatedLocationResult, IValidateNewAccountResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}

		public IEnumerable<IValidateNewAccountError> Errors
		{
			get;
			private set;
		}

		public ValidateNewAccountGatedLocationResult()
		{
			Errors = Enumerable.Empty<IValidateNewAccountError>();
		}
	}
}
