using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class HorizontalLayoutCenterOnElement : MonoBehaviour
	{
		private HorizontalLayoutGroup horizontalLayoutGroup;

		private void Start()
		{
			horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		public void CenterOnElement<T>(int elementIndex) where T : class
		{
			CoroutineRunner.Start(forceLayoutRebuildThenCenterOnElement<T>(elementIndex), this, "CenterOnElement");
		}

		private IEnumerator forceLayoutRebuildThenCenterOnElement<T>(int elementIndex) where T : class
		{
			LayoutRebuilder.MarkLayoutForRebuild(horizontalLayoutGroup.transform as RectTransform);
			yield return new WaitForEndOfFrame();
			float leftPosition = 0f;
			int elementCount = 0;
			float elementWidth = 0f;
			for (int i = 0; i < horizontalLayoutGroup.transform.childCount; i++)
			{
				RectTransform rectTransform2 = horizontalLayoutGroup.transform.GetChild(i) as RectTransform;
				float width = rectTransform2.rect.width;
				if (rectTransform2.GetComponentInChildren<T>() != null)
				{
					if (elementCount == elementIndex)
					{
						elementWidth = width;
						break;
					}
					elementCount++;
				}
				leftPosition += width;
			}
			RectTransform rectTransform = (RectTransform)base.transform;
			RectTransform parentRectTransform = (RectTransform)base.transform.parent;
			float centerOffset = parentRectTransform.rect.width * 0.5f - elementWidth * 0.5f;
			rectTransform.anchoredPosition = new Vector2(0f - leftPosition + centerOffset, parentRectTransform.anchoredPosition.y);
		}
	}
}
