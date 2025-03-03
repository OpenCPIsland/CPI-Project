using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(Canvas))]
	[DisallowMultipleComponent]
	public class SafeAreaCanvasOffset : AbstractSafeAreaComponent
	{
		public bool IncludeAllChildren;

		private HashSet<RectTransform> children;

		private float normalizedOffset;

		private void Start()
		{
			children = new HashSet<RectTransform>();
			RectOffset safeAreaOffset = safeAreaService.GetSafeAreaOffset();
			if (safeAreaOffset.top <= 0)
			{
				return;
			}
			normalizedOffset = safeAreaService.GetNormalizedVerticalOffset(safeAreaOffset.top);
			for (int i = 0; i < base.transform.childCount; i++)
			{
				RectTransform rectTransform = base.transform.GetChild(i) as RectTransform;
				if (rectTransform != null && isValidTransform(rectTransform))
				{
					children.Add(rectTransform);
					setCanvasOffset(normalizedOffset, rectTransform);
				}
			}
		}

		private void Update()
		{
			removeNullChildren(children);
			for (int i = 0; i < base.transform.childCount; i++)
			{
				RectTransform rectTransform = base.transform.GetChild(i) as RectTransform;
				if (rectTransform != null && !children.Contains(rectTransform) && isValidTransform(rectTransform))
				{
					children.Add(rectTransform);
					setCanvasOffset(normalizedOffset, rectTransform);
				}
			}
		}

		private static void setCanvasOffset(float normalizedOffset, RectTransform rectTransform)
		{
			Vector2 anchorMin = rectTransform.anchorMin;
			Vector2 anchorMax = rectTransform.anchorMax;
			float num = 1f - normalizedOffset;
			if (rectTransform.anchorMax.y > num)
			{
				float num2 = rectTransform.anchorMax.y - num;
				anchorMax.y -= num2;
				if (anchorMin.y > 0f)
				{
					anchorMin.y = Math.Max(anchorMin.y - num2, 0f);
				}
			}
			rectTransform.anchorMin = anchorMin;
			rectTransform.anchorMax = anchorMax;
		}

		private bool isValidTransform(RectTransform rectTransform)
		{
			if (IncludeAllChildren)
			{
				return rectTransform.GetComponent<SafeAreaExcludeOverride>() == null;
			}
			return rectTransform.GetComponent<SafeAreaIncludeOverride>() != null;
		}

		private void removeNullChildren(HashSet<RectTransform> children)
		{
			List<RectTransform> list = null;
			foreach (RectTransform child in children)
			{
				if (child == null || child.parent != base.transform)
				{
					if (list == null)
					{
						list = new List<RectTransform>();
					}
					list.Add(child);
				}
			}
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					children.Remove(list[i]);
				}
			}
		}
	}
}
