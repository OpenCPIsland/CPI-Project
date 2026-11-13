using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestItemPopupImage : MonoBehaviour
	{
		public Text ItemText;

		public Image ItemImage;

		public GameObject ItemCountImage;

		public Text ItemCountText;

		public void LoadItem(string itemName, Sprite itemSprite, int itemCount)
		{
			ItemText.text = itemName;
			ItemImage.sprite = itemSprite;
			if (itemCount <= 1)
			{
				ItemCountImage.gameObject.SetActive(false);
			}
			else
			{
				ItemCountText.text = itemCount.ToString();
			}
		}
	}
}
