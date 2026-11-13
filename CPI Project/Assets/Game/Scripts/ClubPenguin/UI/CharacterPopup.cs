using UnityEngine;

namespace ClubPenguin.UI
{
	public class CharacterPopup : MonoBehaviour
	{
		private RectTransform rectTransform;

		public void Start()
		{
			rectTransform = GetComponent<RectTransform>();
		}

		public void Update()
		{
			float y = Camera.main.rect.y;
			rectTransform.anchorMin = new Vector2(0f, y);
			rectTransform.anchorMax = new Vector2(1f, y);
			rectTransform.offsetMin = new Vector2(0f, 0f);
			rectTransform.offsetMax = new Vector2(0f, 0f);
		}
	}
}
