using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class MascotProgressDisplay : MonoBehaviour
	{
		public Transform MascotContentParent;

		private static PrefabContentKey mascotItemContentKey = new PrefabContentKey("Prefabs/Mascots/MascotProgressItem_*");

		public void Start()
		{
			loadMascots();
		}

		private void loadMascots()
		{
			foreach (Mascot mascot in Service.Get<MascotService>().Mascots)
			{
				CoroutineRunner.Start(loadMascotProgressItem(mascot), this, "MascotProgress.loadMascotProgressItem");
			}
		}

		private IEnumerator loadMascotProgressItem(Mascot mascot)
		{
			UILoadingController.RegisterLoad(base.gameObject);
			PrefabContentKey assetKey = new PrefabContentKey(mascotItemContentKey, mascot.Name);
			if (Content.ContainsKey(assetKey.Key))
			{
				AssetRequest<GameObject> assetRequest = Content.LoadAsync(assetKey);
				yield return assetRequest;
				GameObject mascotItemGO = Object.Instantiate(assetRequest.Asset);
				mascotItemGO.transform.SetParent(MascotContentParent, false);
				MascotProgressDisplayItem mascotProgressItem = mascotItemGO.GetComponent<MascotProgressDisplayItem>();
				mascotProgressItem.LoadMascot(mascot);
			}
			UILoadingController.RegisterLoadComplete(base.gameObject);
		}
	}
}
