using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IValidateNewAccountResult
	{
		bool Success
		{
			get;
		}

		IEnumerable<IValidateNewAccountError> Errors
		{
			get;
		}
	}
}
