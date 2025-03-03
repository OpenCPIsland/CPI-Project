using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ExchangeScreenActivation : MonoBehaviour
	{
		private GameObject exchangeObject;

		private bool isLoadingPrefab = false;

		private readonly PrefabContentKey PREFAB_KEY = new PrefabContentKey("Prefabs/ExchangeV2");

		public void ActivateExchangeScreen(GameObject sender)
		{
			if (sender.CompareTag("Player") && !isLoadingPrefab && Object.FindObjectOfType<ExchangeScreenControllerV2>() == null)
			{
				Content.LoadAsync(onPrefabLoaded, PREFAB_KEY);
				isLoadingPrefab = true;
			}
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			exchangeObject = Object.Instantiate(prefab);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowCameraSpacePopup(exchangeObject, false, true, "Accessibility.Popup.Title.Exchange"));
			isLoadingPrefab = false;
		}
	}
}
