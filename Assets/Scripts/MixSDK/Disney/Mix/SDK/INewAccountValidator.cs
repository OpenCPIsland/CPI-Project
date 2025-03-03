using System;

namespace Disney.Mix.SDK
{
	public interface INewAccountValidator
	{
		void ValidateAdultAccount(string email, string password, Action<IValidateNewAccountResult> callback);

		void ValidateChildAccount(string username, string password, Action<IValidateNewAccountResult> callback);
	}
}
