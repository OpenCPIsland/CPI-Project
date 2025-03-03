using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class QuestsScreenDetailsInventoryScreenLoader : MonoBehaviour
	{
		private static PrefabContentKey questInventoryContentKey = new PrefabContentKey("ScreenQuestsDetailsPrefabs/InventoryContent_*");

		public void Start()
		{
			Content.LoadAsync(onContentPrefabLoaded, questInventoryContentKey, Service.Get<QuestService>().ActiveQuest.Mascot.AbbreviatedName);
		}

		private void onContentPrefabLoaded(string path, GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.transform.SetParent(base.transform, false);
		}
	}
}
