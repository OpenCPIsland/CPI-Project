using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class ValidateNewAccountEmbargoedCountryResult : IValidateNewAccountEmbargoedCountryResult, IValidateNewAccountResult
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

		public ValidateNewAccountEmbargoedCountryResult()
		{
			Errors = Enumerable.Empty<IValidateNewAccountError>();
		}
	}
}
