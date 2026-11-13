using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Tests;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Tests
{
	public class AccountFormTests : BaseLoginCreateIntegrationTest
	{
		public GameObject PopupCanvas;

		private PrefabContentKey rootNodeKey = new PrefabContentKey("Prefabs/LoginCreate/AccountRootNodePrefab");

		private GameObject popupRoot;

		protected override IEnumerator setup()
		{
			yield return CoroutineRunner.Start(base.setup(), this, "AccountFormTests base.setup call");
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(rootNodeKey);
			while (!assetRequest.Finished)
			{
				yield return null;
			}
			PopupCanvas.AddComponent<PopupManager>();
			popupRoot = Object.Instantiate(assetRequest.Asset);
		}

		protected override IEnumerator runTest()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popupRoot));
			yield return new WaitForSeconds(10f);
			IntegrationTestEx.FailIf(PopupCanvas.transform.childCount == 0);
		}
	}
}
