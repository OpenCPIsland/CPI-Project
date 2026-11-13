using Disney.Mix.SDK;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public abstract class AbstractCreateController : MonoBehaviour
	{
		public abstract bool IsOnline
		{
			get;
		}

		public abstract bool IsConfigReady
		{
			get;
		}

		public bool CanShowAccountError
		{
			get;
			set;
		}

		public event Action<IRegisterResult> OnCreateError;

		public event Action<IUpdateProfileResult> OnProfileUpdateError;

		public event Action<IValidateDisplayNameResult> OnValidateDisplayNameError;

		public event Action OnValidateDisplayNameSuccess;

		public event Action<IUpdateDisplayNameResult> OnUpdateDisplayNameError;

		public abstract void Create(DCreateAccountPayload payload);

		public abstract void UpdateProfile(DUpdateProfilePayload payload);

		public abstract void ValidateDisplayName(string displayName);

		public abstract void UpdateDisplayName(string displayName, string referer = null);

		public abstract bool CheckRegConfigReady();

		protected virtual void dispatchCreateError(IRegisterResult result)
		{
			if (this.OnCreateError != null)
			{
				this.OnCreateError(result);
			}
		}

		protected virtual void dispatchProfileUpdateError(IUpdateProfileResult result)
		{
			if (this.OnProfileUpdateError != null)
			{
				this.OnProfileUpdateError(result);
			}
		}

		protected virtual void dispatchValidateDisplayNameSuccess()
		{
			if (this.OnValidateDisplayNameSuccess != null)
			{
				this.OnValidateDisplayNameSuccess();
			}
		}

		protected virtual void dispatchValidateDisplayNameError(IValidateDisplayNameResult result)
		{
			if (this.OnValidateDisplayNameError != null)
			{
				this.OnValidateDisplayNameError(result);
			}
		}

		protected virtual void dispatchUpdateDisplayNameError(IUpdateDisplayNameResult result)
		{
			if (this.OnUpdateDisplayNameError != null)
			{
				this.OnUpdateDisplayNameError(result);
			}
		}
	}
}
