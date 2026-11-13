using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class RectTransformBoundsVisibility : MonoBehaviour
	{
		public RectTransform Viewport;

		public bool CheckX;

		public bool CheckY;

		public bool CheckMinAndMax;

		private RectTransform rectTransform;

		private Canvas canvas;

		private Rect viewportRect;

		private Vector2[] rectCorners;

		private bool isVisible;

		public bool IsVisible
		{
			get
			{
				return isVisible;
			}
			private set
			{
				if (this.OnVisibilityChanged != null)
				{
					this.OnVisibilityChanged(value);
				}
				isVisible = value;
			}
		}

		public event Action<bool> OnVisibilityChanged;

		private void Start()
		{
			rectTransform = (base.transform as RectTransform);
			canvas = GetComponentInParent<Canvas>();
			IsVisible = false;
		}

		private void Update()
		{
			if (!(Viewport != null))
			{
				return;
			}
			viewportRect = RectTransformUtil.GetCanvasRect(Viewport, canvas);
			rectCorners = RectTransformUtil.GetCanvasCorners(rectTransform, canvas);
			bool flag = CheckMinAndMax;
			for (int i = 0; i < rectCorners.Length; i++)
			{
				if (CheckMinAndMax)
				{
					if (!isPointContained(viewportRect, rectCorners[i]))
					{
						flag = false;
						break;
					}
				}
				else if (isPointContained(viewportRect, rectCorners[i]))
				{
					flag = true;
					break;
				}
			}
			if (flag != IsVisible)
			{
				IsVisible = flag;
			}
		}

		private bool isPointContained(Rect rect, Vector2 point)
		{
			if (CheckX && CheckY)
			{
				return rect.Contains(point);
			}
			if (CheckX)
			{
				return RectUtil.ContainsHorizontal(rect, point);
			}
			if (CheckY)
			{
				return RectUtil.ContainsVertical(rect, point);
			}
			return false;
		}

		private void OnDestroy()
		{
			this.OnVisibilityChanged = null;
		}
	}
}
