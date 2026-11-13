using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner
{
	public class ItemPopup : MonoBehaviour
	{
		[SerializeField]
		private Image itemImage;

		[SerializeField]
		private Text itemDescriptionText;

		[SerializeField]
		private Text itemNameText;

		public void Init(Texture2D itemTexture, string itemDescription, string itemName)
		{
			itemImage.sprite = Sprite.Create(itemTexture, new Rect(0f, 0f, itemTexture.width, itemTexture.height), default(Vector2));
			itemDescriptionText.text = itemDescription;
			itemNameText.text = itemName;
		}

		public void OnCloseButton()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
