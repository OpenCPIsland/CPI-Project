using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ShowIglooListButton : MonoBehaviour
	{
		private readonly PrefabContentKey containerPrefabContentKey = new PrefabContentKey("UI/IglooList/IglooListFullScreenContainer");

		private readonly PrefabContentKey contentPrefabContentKey = new PrefabContentKey("UI/IglooList/IglooListScreen");

		private bool isLoadingPrefab = false;

		public void OnButton()
		{
			Service.Get<ActionConfirmationService>().ConfirmAction(typeof(MyIglooTransitionButton), null, openIglooList);
		}

		public void OnActionGraphButton(GameObject sender)
		{
			if (sender.CompareTag("Player"))
			{
				openIglooList();
			}
		}

		private void openIglooList()
		{
			if (!isLoadingPrefab)
			{
				isLoadingPrefab = true;
				if (PlatformUtils.GetAspectRatioType() == AspectRatioType.Portrait)
				{
					SceneRefs.FullScreenPopupManager.CreatePopup(contentPrefabContentKey, "Accessibility.Popup.Title.IglooList", false, onPrefabCreated);
				}
				else
				{
					Content.LoadAsync(onContainerPrefabLoaded, containerPrefabContentKey);
				}
			}
		}

		private void onPrefabCreated(PrefabContentKey key, GameObject marketplaceObject)
		{
			isLoadingPrefab = false;
		}

		private void onContainerPrefabLoaded(string path, GameObject prefab)
		{
			isLoadingPrefab = false;
			GameObject container = Object.Instantiate(prefab);
			CoroutineRunner.Start(ShowPopup(container), this, "ShowIglooList");
		}

		private static IEnumerator ShowPopup(GameObject container)
		{
			yield return new WaitForEndOfFrame();
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(container));
		}

		public void OnDestroy()
		{
		}
	}
}
