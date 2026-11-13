using UnityEngine;
using UnityEngine.Events;

namespace Disney.Native
{
	public class AccessibilitySettings : MonoBehaviour
	{
		public bool VisibleOnlyForSwitchControl = false;

		public bool Priority = false;

		public bool IgnoreText = false;

		public bool VoiceOnly = false;

		public string CustomToken = "";

		public GameObject ReferenceToken = null;

		public RectTransform ReferenceRect = null;

		public bool DontRender = false;

		public GameObject[] AdditionalReferenceTokens;

		public UnityEvent CustomOnClickEvent;

		[HideInInspector]
		public string DynamicText = null;

		private void Start()
		{
			Setup();
		}

		public virtual void Setup()
		{
			if (VisibleOnlyForSwitchControl)
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.OnToggleAccessibilities += ToggleAccessibilities;
			}
			if (!MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled && VisibleOnlyForSwitchControl)
			{
				base.gameObject.SetActive(false);
			}
		}

		public void OnEnable()
		{
			if (Priority)
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.AddPriorityGameObject(base.gameObject);
			}
			if (this is ScrollContentAccessibilitySettings)
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.ScrollContentComponents.Add(base.gameObject);
			}
		}

		public void OnDisable()
		{
			if (MonoSingleton<NativeAccessibilityManager>.Instance != null && !Equals(null))
			{
				if (Priority)
				{
					MonoSingleton<NativeAccessibilityManager>.Instance.RemovePriorityGameObject(base.gameObject);
				}
				if (this is ScrollContentAccessibilitySettings)
				{
					MonoSingleton<NativeAccessibilityManager>.Instance.ScrollContentComponents.Remove(base.gameObject);
				}
			}
		}

		public void Destroy()
		{
			if (MonoSingleton<NativeAccessibilityManager>.Instance != null)
			{
				if (Priority)
				{
					MonoSingleton<NativeAccessibilityManager>.Instance.RemovePriorityGameObject(base.gameObject);
				}
				if (VisibleOnlyForSwitchControl)
				{
					MonoSingleton<NativeAccessibilityManager>.Instance.OnToggleAccessibilities -= ToggleAccessibilities;
				}
				if (this is ScrollContentAccessibilitySettings)
				{
					MonoSingleton<NativeAccessibilityManager>.Instance.ScrollContentComponents.Remove(base.gameObject);
				}
			}
		}

		public void ToggleAccessibilities(object sender, ToggleAccessibilitiesEventArgs args)
		{
			if (!Equals(null) && base.gameObject != null)
			{
				base.gameObject.SetActive(args.IsOn);
			}
		}
	}
}
