using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	public class UIScrollToSelectionXY : MonoBehaviour
	{
		public float scrollSpeed = 10f;

		[SerializeField]
		private RectTransform layoutListGroup;

		private RectTransform targetScrollObject;

		private bool scrollToSelection = true;

		private RectTransform scrollWindow;

		private RectTransform currentCanvas;

		private ScrollRect targetScrollRect;

		private void Start()
		{
			targetScrollRect = GetComponent<ScrollRect>();
			scrollWindow = targetScrollRect.GetComponent<RectTransform>();
		}

		private void Update()
		{
			ScrollRectToLevelSelection();
		}

		private void ScrollRectToLevelSelection()
		{
			EventSystem current = EventSystem.current;
			if (targetScrollRect == null || layoutListGroup == null || scrollWindow == null)
			{
				return;
			}
			RectTransform rectTransform = (!(current.currentSelectedGameObject != null)) ? null : current.currentSelectedGameObject.GetComponent<RectTransform>();
			if (rectTransform != targetScrollObject)
			{
				scrollToSelection = true;
			}
			if (!(rectTransform == null) && scrollToSelection && !(rectTransform.transform.parent != layoutListGroup.transform))
			{
				bool flag = false;
				bool flag2 = false;
				if (targetScrollRect.vertical)
				{
					Vector2 anchoredPosition = rectTransform.anchoredPosition;
					float num = 0f - anchoredPosition.y;
					Vector2 anchoredPosition2 = layoutListGroup.anchoredPosition;
					float y = anchoredPosition2.y;
					float num2 = 0f;
					num2 = y - num;
					ScrollRect scrollRect = targetScrollRect;
					float verticalNormalizedPosition = scrollRect.verticalNormalizedPosition;
					float num3 = num2;
					Vector2 sizeDelta = layoutListGroup.sizeDelta;
					scrollRect.verticalNormalizedPosition = verticalNormalizedPosition + num3 / sizeDelta.y * Time.deltaTime * scrollSpeed;
					flag2 = (Mathf.Abs(num2) < 2f);
				}
				if (targetScrollRect.horizontal)
				{
					Vector2 anchoredPosition3 = rectTransform.anchoredPosition;
					float num4 = 0f - anchoredPosition3.x;
					Vector2 anchoredPosition4 = layoutListGroup.anchoredPosition;
					float x = anchoredPosition4.x;
					float num5 = 0f;
					num5 = x - num4;
					ScrollRect scrollRect2 = targetScrollRect;
					float horizontalNormalizedPosition = scrollRect2.horizontalNormalizedPosition;
					float num6 = num5;
					Vector2 sizeDelta2 = layoutListGroup.sizeDelta;
					scrollRect2.horizontalNormalizedPosition = horizontalNormalizedPosition + num6 / sizeDelta2.x * Time.deltaTime * scrollSpeed;
					flag = (Mathf.Abs(num5) < 2f);
				}
				if (flag && flag2)
				{
					scrollToSelection = false;
				}
				targetScrollObject = rectTransform;
			}
		}
	}
}
