using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class TrayInputButtonAnimationEvent : MonoBehaviour
	{
		public event Action OnTransitionReady;

		public void FadeTransitionReady()
		{
			if (this.OnTransitionReady != null)
			{
				this.OnTransitionReady();
			}
		}

		public void PopTransitionReady()
		{
			if (this.OnTransitionReady != null)
			{
				this.OnTransitionReady();
			}
		}

		public void EmptyTransitionReady()
		{
			if (this.OnTransitionReady != null)
			{
				this.OnTransitionReady();
			}
		}
	}
}
