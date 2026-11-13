using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(RectTransform))]
	public abstract class AbstractMinSizeUpdater : MonoBehaviour
	{
		public bool UpdateX;

		public bool UpdateY;

		private ILayoutElement targetLayoutElement;

		private LayoutElement layoutElement;

		protected abstract ILayoutElement getTargetLayoutElement();

		private void Awake()
		{
			layoutElement = base.gameObject.AddComponent<LayoutElement>();
			targetLayoutElement = getTargetLayoutElement();
			updateMinSize();
		}

		private void OnRectTransformDimensionsChange()
		{
			if (layoutElement != null && targetLayoutElement != null)
			{
				updateMinSize();
			}
		}

		private void updateMinSize()
		{
			bool flag = false;
			if (UpdateX && Math.Abs(layoutElement.minWidth - targetLayoutElement.preferredWidth) > float.Epsilon)
			{
				layoutElement.minWidth = targetLayoutElement.preferredWidth;
				flag = true;
			}
			if (UpdateY && Math.Abs(layoutElement.minHeight - targetLayoutElement.preferredHeight) > float.Epsilon)
			{
				layoutElement.minHeight = targetLayoutElement.preferredHeight;
				flag = true;
			}
			if (flag)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform.parent as RectTransform);
			}
		}
	}
}
