using Fabric;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Audio
{
	[RequireComponent(typeof(Toggle))]
	internal class ToggleToFabricEvents : MonoBehaviour
	{
		private const string DEFAULT_CLICK_AUDIO = "SFX/UI/Button/Select";

		public bool Mute;

		[Tooltip("If left empty, this defaults to SFX/UI/Button/Select")]
		public string AudioName = "";

		public EventAction EventType = EventAction.PlaySound;

		public GameObject OverrideSoundSource;

		private void Start()
		{
			GetComponent<Toggle>().onValueChanged.AddListener(onToggleChanged);
		}

		private void onToggleChanged(bool isActive)
		{
			string eventName = "SFX/UI/Button/Select";
			if (!string.IsNullOrEmpty(AudioName))
			{
				eventName = AudioName;
			}
			GameObject parentGameObject = (OverrideSoundSource != null) ? OverrideSoundSource : base.gameObject;
			EventManager.Instance.PostEvent(eventName, EventType, parentGameObject);
		}
	}
}
