using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	internal class ValidateNewAccountResult : IValidateNewAccountResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public IEnumerable<IValidateNewAccountError> Errors
		{
			get;
			private set;
		}

		public ValidateNewAccountResult(bool success, IEnumerable<IValidateNewAccountError> errors)
		{
			Success = success;
			Errors = errors;
		}
	}
}
