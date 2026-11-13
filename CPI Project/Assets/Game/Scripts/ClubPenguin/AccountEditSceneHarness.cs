using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class AccountEditSceneHarness : MonoBehaviour
	{
		public string StartEvent;

		private PrefabContentKey rootNodeKey = new PrefabContentKey("Prefabs/LoginCreate/TreeNodes/AccountRootNodePrefab");

		private IEnumerator Start()
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(rootNodeKey);
			while (!assetRequest.Finished)
			{
				yield return null;
			}
			GameObject popupRoot = Object.Instantiate(assetRequest.Asset);
			popupRoot.GetComponent<SEDFSMStartEventSource>().StartEvent = StartEvent;
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popupRoot));
		}
	}
}
