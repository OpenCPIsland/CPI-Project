using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class MouseEventsComponent : MonoBehaviour
	{
		public event Action OnMouseEnterEvent;

		public event Action OnMouseExitEvent;

		public event Action OnMouseDownEvent;

		private void OnMouseEnter()
		{
			this.OnMouseEnterEvent.InvokeSafe();
		}

		private void OnMouseExit()
		{
			this.OnMouseExitEvent.InvokeSafe();
		}

		private void OnMouseDown()
		{
			this.OnMouseDownEvent.InvokeSafe();
		}

		public void ClearListeners()
		{
			this.OnMouseEnterEvent = null;
			this.OnMouseExitEvent = null;
			this.OnMouseDownEvent = null;
		}
	}
}
