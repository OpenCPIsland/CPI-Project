using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.UI
{
	public class FullScreenPopupManager : MonoBehaviour
	{
		public delegate void PrefabInstantiatedCallback(PrefabContentKey PrefabContentKey, GameObject instance);

		private static List<RaycastResult> s_tempResults;

		private Dictionary<GameObject, ResourceCleaner> cleaners;

		public void CreatePopup(PrefabContentKey prefabContentKey, string accessibilityTitleToken, bool useCameraSpace = false, PrefabInstantiatedCallback instantiatedCallback = null)
		{
			Content.LoadAsync(delegate(string path, GameObject popupPrefab)
			{
				onPrefabLoaded(popupPrefab, prefabContentKey, accessibilityTitleToken, useCameraSpace, instantiatedCallback);
			}, prefabContentKey);
		}

		public void Awake()
		{
			s_tempResults = new List<RaycastResult>();
			cleaners = new Dictionary<GameObject, ResourceCleaner>();
			SceneRefs.FullScreenPopupManager = this;
		}

		public void OnDestroy()
		{
			foreach (KeyValuePair<GameObject, ResourceCleaner> cleaner in cleaners)
			{
				ResourceCleaner value;
				if (cleaners.TryGetValue(cleaner.Key, out value))
				{
					value.OnDestroyCallback = null;
				}
			}
			cleaners.Clear();
			SceneRefs.FullScreenPopupManager = null;
		}

		private void onPrefabLoaded(GameObject popupPrefab, PrefabContentKey prefabContentKey, string accessibilityTitleToken, bool useCameraSpace, PrefabInstantiatedCallback instantiatedCallback)
		{
			GameObject gameObject = Object.Instantiate(popupPrefab);
			if (gameObject.GetComponentInChildren<UIDisabler>() == null)
			{
				disableWorldRendering();
			}
			ResourceCleaner resourceCleaner = gameObject.GetComponent<ResourceCleaner>();
			if (resourceCleaner == null)
			{
				resourceCleaner = gameObject.AddComponent<ResourceCleaner>();
			}
			if (!cleaners.ContainsKey(popupPrefab))
			{
				cleaners.Add(popupPrefab, resourceCleaner);
			}
			resourceCleaner.OnDestroyCallback = onFullScreenObjectDestroyed;
			if (!useCameraSpace)
			{
				gameObject.transform.SetParent(base.transform, false);
				playAccessibilityTitle(accessibilityTitleToken);
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowCameraSpacePopup(gameObject, false, true, accessibilityTitleToken));
			}
			if (instantiatedCallback != null)
			{
				instantiatedCallback(prefabContentKey, gameObject);
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(PopupEvents.ShowingPopup));
		}

		private static void playAccessibilityTitle(string accessibilityTitleToken)
		{
			if (!string.IsNullOrEmpty(accessibilityTitleToken))
			{
				NativeAccessibilityManager instance = MonoSingleton<NativeAccessibilityManager>.Instance;
				if (instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE)
				{
					string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(accessibilityTitleToken);
					instance.Native.Speak(tokenTranslation);
				}
			}
		}

		private void onFullScreenObjectDestroyed(GameObject go)
		{
			cleaners.Remove(go);
			if (EventSystem.current != null)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				EventSystem.current.RaycastAll(eventData, s_tempResults);
				s_tempResults.Clear();
			}
			if (go.GetComponentInChildren<UIDisabler>() == null)
			{
				enableWorldRendering();
			}
			if (base.isActiveAndEnabled)
			{
				StartCoroutine(waitForUnloadResourceAssets());
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(PopupEvents.HidingPopup));
		}

		private static IEnumerator waitForUnloadResourceAssets()
		{
			yield return new WaitForEndOfFrame();
			yield return Resources.UnloadUnusedAssets();
		}

		private static void enableWorldRendering()
		{
			ClubPenguin.Core.SceneRefs.Get<CameraRenderingControl>().EnableRendering();
		}

		private static void disableWorldRendering()
		{
			ClubPenguin.Core.SceneRefs.Get<CameraRenderingControl>().DisableRendering(true, false);
		}
	}
}
