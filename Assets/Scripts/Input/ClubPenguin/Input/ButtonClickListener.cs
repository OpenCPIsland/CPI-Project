using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ClubPenguin.Input
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Button))]
	public class ButtonClickListener : MonoBehaviour
	{
		public enum ClickType
		{
			UI,
			InputMap
		}

		[Serializable]
		public class ButtonClickListenerEvent : UnityEvent<ClickType>
		{
		}

		[SerializeField]
		private ButtonClickListenerEvent onClick = new ButtonClickListenerEvent();

		public ButtonClickListenerEvent OnClick
		{
			get
			{
				return onClick;
			}
		}

		public bool Interactable
		{
			get
			{
				return Button.interactable && Button.IsActive();
			}
		}

		public Button Button
		{
			get;
			private set;
		}

		private void Awake()
		{
			Button = GetComponent<Button>();
			if (Button.onClick.GetPersistentEventCount() <= 0)
			{
			}
		}

		private void OnEnable()
		{
			Button.onClick.AddListener(onButtonClicked);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(onButtonClicked);
		}

		private void onButtonClicked()
		{
			InvokeClick(ClickType.UI);
		}

		public void InvokeClick(ClickType type)
		{
			if (Interactable)
			{
				onClick.Invoke(type);
			}
		}
	}
}
