using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/RescalePanels/RescalePanel")]
	public class RescalePanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IEventSystemHandler
	{
		public Vector2 minSize;

		public Vector2 maxSize;

		private RectTransform rectTransform;

		private Transform goTransform;

		private Vector2 currentPointerPosition;

		private Vector2 previousPointerPosition;

		private RectTransform thisRectTransform;

		private Vector2 sizeDelta;

		private void Awake()
		{
			rectTransform = base.transform.parent.GetComponent<RectTransform>();
			goTransform = base.transform.parent;
			thisRectTransform = GetComponent<RectTransform>();
			sizeDelta = thisRectTransform.sizeDelta;
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
				Vector3 localScale = goTransform.localScale;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out currentPointerPosition);
				Vector2 vector = currentPointerPosition - previousPointerPosition;
				localScale += new Vector3((0f - vector.y) * 0.001f, (0f - vector.y) * 0.001f, 0f);
				localScale = new Vector3(Mathf.Clamp(localScale.x, minSize.x, maxSize.x), Mathf.Clamp(localScale.y, minSize.y, maxSize.y), 1f);
				goTransform.localScale = localScale;
				previousPointerPosition = currentPointerPosition;
				float x = sizeDelta.x;
				Vector3 localScale2 = goTransform.localScale;
				float num = x / localScale2.x;
				Vector2 vector2 = new Vector2(num, num);
				thisRectTransform.sizeDelta = vector2;
			}
		}
	}
}
