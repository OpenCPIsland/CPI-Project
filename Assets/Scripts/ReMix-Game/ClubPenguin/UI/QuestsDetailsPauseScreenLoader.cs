using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class QuestsDetailsPauseScreenLoader : MonoBehaviour
	{
		private static PrefabContentKey questPauseContentKey = new PrefabContentKey("ScreenQuestsDetailsPrefabs/PauseContent_*");

		public void Start()
		{
			Content.LoadAsync(onContentPrefabLoaded, questPauseContentKey, Service.Get<QuestService>().ActiveQuest.Mascot.AbbreviatedName);
		}

		private void onContentPrefabLoaded(string path, GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.transform.SetParent(base.transform, false);
		}
	}
}
