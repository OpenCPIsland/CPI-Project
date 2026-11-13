using ClubPenguin.Input;
using Fabric;
using UnityEngine;

namespace ClubPenguin.Audio
{
	[RequireComponent(typeof(ButtonClickListener))]
	public class ButtonToFabricEvents : MonoBehaviour
	{
		private const string DEFAULT_CLICK_AUDIO = "SFX/UI/MainTray/ButtonSmall";

		public bool Mute;

		[Tooltip("If left empty, this defaults to SFX/UI/MainTray/ButtonSmall")]
		public string AudioName = "";

		public EventAction EventType = EventAction.PlaySound;

		public GameObject OverrideSoundSource;

		private ButtonClickListener clickListener;

		private void Awake()
		{
			clickListener = GetComponent<ButtonClickListener>();
		}

		private void OnEnable()
		{
			if (clickListener != null)
			{
				clickListener.OnClick.AddListener(onClicked);
			}
		}

		private void OnDisable()
		{
			if (clickListener != null)
			{
				clickListener.OnClick.RemoveListener(onClicked);
			}
		}

		private void onClicked(ButtonClickListener.ClickType interactedType)
		{
			if (EventManager.Instance != null)
			{
				string eventName = string.IsNullOrEmpty(AudioName) ? "SFX/UI/MainTray/ButtonSmall" : AudioName;
				GameObject parentGameObject = (OverrideSoundSource != null) ? OverrideSoundSource : base.gameObject;
				EventManager.Instance.PostEvent(eventName, EventType, parentGameObject);
			}
		}
	}
}
