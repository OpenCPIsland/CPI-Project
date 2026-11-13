using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class BasePopupManager : MonoBehaviour
	{
		private const string DEFAULT_ACCESSIBILITY_TITLE_TOKEN = "Accessibility.Popup.Title.Generic";

		protected EventChannel eventChannel;

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		protected void showPopup(GameObject popup, bool destroyPopupOnBackPressed = false, bool scaleToFit = true, string accessibilityTitleToken = null)
		{
			if (!scaleToFit)
			{
				CanvasScalerExt component = GetComponent<CanvasScalerExt>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			popup.transform.SetParent(base.transform, false);
			popup.SetActive(true);
			if (destroyPopupOnBackPressed)
			{
				popup.gameObject.AddComponent<DestroyPopupOnBackPressed>();
			}
			playAccessibilityTitle(accessibilityTitleToken);
		}

		protected void createPopup(PrefabContentKey prefabContentKey, bool destroyPopupOnBackPressed = false, string accessibilityTitleToken = null)
		{
			Content.LoadAsync(delegate(string path, GameObject popupPrefab)
			{
				onPopupLoaded(popupPrefab, destroyPopupOnBackPressed, accessibilityTitleToken);
			}, prefabContentKey);
		}

		private void onPopupLoaded(GameObject popupPrefab, bool destroyPopupOnBackPressed, string accessibilityTitleToken)
		{
			GameObject gameObject = Object.Instantiate(popupPrefab);
			gameObject.transform.SetParent(base.transform, false);
			if (destroyPopupOnBackPressed)
			{
				gameObject.gameObject.AddComponent<DestroyPopupOnBackPressed>();
			}
			playAccessibilityTitle(accessibilityTitleToken);
		}

		private void playAccessibilityTitle(string accessibilityTitleToken)
		{
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE && !string.IsNullOrEmpty(accessibilityTitleToken))
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(accessibilityTitleToken);
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(tokenTranslation);
			}
		}
	}
}
