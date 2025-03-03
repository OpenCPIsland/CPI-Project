using Disney.Kelowna.Common;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class ScavengerHuntPlayerResults : MonoBehaviour
	{
		private readonly PrefabContentKey ITEM_UI_PREFAB_KEY = new PrefabContentKey("Prefabs/ScavengerHunt/ScavengerHuntUIItem");

		public Text PlayerName;

		public RectTransform ItemsFoundContainer;

		private int totalItemsHidden;

		private int totalItemsFound;

		public void Init(int totalItemsHidden, int totalItemsFound, string title)
		{
			this.totalItemsHidden = totalItemsHidden;
			this.totalItemsFound = totalItemsFound;
			PlayerName.text = title;
			Content.LoadAsync(onUIItemLoaded, ITEM_UI_PREFAB_KEY);
		}

		private void onUIItemLoaded(string path, GameObject prefab)
		{
			for (int i = 0; i < totalItemsHidden; i++)
			{
				GameObject gameObject = Object.Instantiate(prefab, ItemsFoundContainer, false);
				SpriteSelector component = gameObject.GetComponent<SpriteSelector>();
				int index = (i >= totalItemsFound) ? 1 : 0;
				component.SelectSprite(index);
			}
		}
	}
}
