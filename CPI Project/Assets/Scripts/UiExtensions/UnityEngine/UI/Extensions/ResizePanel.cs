using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/RescalePanels/ResizePanel")]
	public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IEventSystemHandler
	{
		public Vector2 minSize;

		public Vector2 maxSize;

		private RectTransform rectTransform;

		private Vector2 currentPointerPosition;

		private Vector2 previousPointerPosition;

		private float ratio;

		private void Awake()
		{
			rectTransform = base.transform.parent.GetComponent<RectTransform>();
			float width = rectTransform.rect.width;
			float height = rectTransform.rect.height;
			ratio = height / width;
			minSize = new Vector2(0.1f * width, 0.1f * height);
			maxSize = new Vector2(10f * width, 10f * height);
		}

		public void OnPointerDown(PointerEventData data)
		{
			rectTransform.SetAsLastSibling();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out previousPointerPosition);
		}

		public void OnDrag(PointerEventData data)
		{
			if (!(rectTransform == null))
			{
				Vector2 sizeDelta = rectTransform.sizeDelta;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out currentPointerPosition);
				Vector2 vector = currentPointerPosition - previousPointerPosition;
				sizeDelta += new Vector2(vector.x, ratio * vector.x);
				sizeDelta = new Vector2(Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x), Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y));
				rectTransform.sizeDelta = sizeDelta;
				previousPointerPosition = currentPointerPosition;
			}
		}
	}
}
