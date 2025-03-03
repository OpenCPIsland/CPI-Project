using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/RescalePanels/RescaleDragPanel")]
	public class RescaleDragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IEventSystemHandler
	{
		private Vector2 pointerOffset;

		private RectTransform canvasRectTransform;

		private RectTransform panelRectTransform;

		private Transform goTransform;

		private void Awake()
		{
			Canvas componentInParent = GetComponentInParent<Canvas>();
			if (componentInParent != null)
			{
				canvasRectTransform = (componentInParent.transform as RectTransform);
				panelRectTransform = (base.transform.parent as RectTransform);
				goTransform = base.transform.parent;
			}
		}

		public void OnPointerDown(PointerEventData data)
		{
			panelRectTransform.SetAsLastSibling();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out pointerOffset);
		}

		public void OnDrag(PointerEventData data)
		{
			if (!(panelRectTransform == null))
			{
				Vector2 screenPoint = ClampToWindow(data);
				Vector2 localPoint;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPoint, data.pressEventCamera, out localPoint))
				{
					RectTransform rectTransform = panelRectTransform;
					Vector2 a = localPoint;
					float x = pointerOffset.x;
					Vector3 localScale = goTransform.localScale;
					float x2 = x * localScale.x;
					float y = pointerOffset.y;
					Vector3 localScale2 = goTransform.localScale;
					rectTransform.localPosition = a - new Vector2(x2, y * localScale2.y);
				}
			}
		}

		private Vector2 ClampToWindow(PointerEventData data)
		{
			Vector2 position = data.position;
			Vector3[] array = new Vector3[4];
			canvasRectTransform.GetWorldCorners(array);
			float x = Mathf.Clamp(position.x, array[0].x, array[2].x);
			float y = Mathf.Clamp(position.y, array[0].y, array[2].y);
			return new Vector2(x, y);
		}
	}
}
