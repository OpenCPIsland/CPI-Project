using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ChatPhraseImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IEventSystemHandler
	{
		[SerializeField]
		private GameObject content;

		[SerializeField]
		private float expandSize;

		private RectTransform rectTransform;

		private float originalWidth;

		private float originalHeight;

		private void Awake()
		{
			base.gameObject.SetActive(false);
		}

		public void UpdatePosition()
		{
			base.gameObject.SetActive(true);
			VerticalLayoutGroup componentInParent = GetComponentInParent<VerticalLayoutGroup>();
			this.rectTransform = (base.transform as RectTransform);
			RectTransform rectTransform = content.transform as RectTransform;
			float num = originalWidth = rectTransform.rect.width + (float)componentInParent.padding.left + (float)componentInParent.padding.right;
			originalHeight = this.rectTransform.rect.height;
			scaleNormal();
			updatePosition(originalWidth, originalHeight);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			scaleUp();
		}

		public void OnSelect(BaseEventData eventData)
		{
			scaleUp();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			scaleNormal();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			scaleNormal();
		}

		private void scaleUp()
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalWidth + expandSize);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalHeight + expandSize);
		}

		private void scaleNormal()
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalWidth);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalHeight);
		}

		private void updatePosition(float width, float height)
		{
			rectTransform.anchoredPosition = new Vector3(width * 0.5f, (0f - height) * 0.5f, rectTransform.localPosition.z);
		}
	}
}
