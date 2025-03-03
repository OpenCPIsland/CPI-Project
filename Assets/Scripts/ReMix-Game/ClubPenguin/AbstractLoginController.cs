using Disney.Mix.SDK;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public abstract class AbstractLoginController : MonoBehaviour
	{
		public event Action<ILoginResult> OnLoginError;

		public event Action OnRecoveryFailed;

		public event Action OnRecoverySuccess;

		public abstract void Login(DLoginPayload payload);

		public abstract void PasswordRecoverySend(string lookupValue);

		public abstract void UsernameRecoverySend(string lookupValue);

		protected virtual void dispatchLoginError(ILoginResult result)
		{
			if (this.OnLoginError != null)
			{
				this.OnLoginError(result);
			}
		}

		protected virtual void dispatchRecoveryFailed()
		{
			if (this.OnRecoveryFailed != null)
			{
				this.OnRecoveryFailed();
			}
		}

		protected virtual void dispatchRecoverySuccess()
		{
			if (this.OnRecoverySuccess != null)
			{
				this.OnRecoverySuccess();
			}
		}
	}
}
