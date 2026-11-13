using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClubPenguin.UI
{
	public class ClickListener : MonoBehaviour
	{
		public event Action OnClicked;

		private void Update()
		{
			if (Mouse.current != null &&
				Mouse.current.leftButton.wasPressedThisFrame &&
				!EventSystem.current.IsPointerOverGameObject())
			{
				handleClick();
			}
		}

		protected virtual void handleClick()
		{
			if (this.OnClicked != null)
			{
				this.OnClicked();
			}
		}
	}
}