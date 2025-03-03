using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.UI
{
	public class ClickListener : MonoBehaviour
	{
		public event Action OnClicked;

		private void Update()
		{
			if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
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
