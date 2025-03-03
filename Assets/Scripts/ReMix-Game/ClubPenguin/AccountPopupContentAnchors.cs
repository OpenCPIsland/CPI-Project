using UnityEngine;

namespace ClubPenguin
{
	[ExecuteInEditMode]
	public class AccountPopupContentAnchors : MonoBehaviour
	{
		public enum VerticalAlignment
		{
			Top,
			Middle,
			Bottom
		}

		public enum HorizontalAlignment
		{
			Left,
			Center,
			Right
		}

		public VerticalAlignment VerticalAlign = VerticalAlignment.Middle;

		private VerticalAlignment lastVAlign;

		public HorizontalAlignment HorizontalAlign = HorizontalAlignment.Center;

		private HorizontalAlignment lastHAlign;

		private RectTransform rectTransform;

		private Vector2 startAnchorMin;

		private Vector2 startAnchorMax;

		private void Start()
		{
			rectTransform = GetComponent<RectTransform>();
			startAnchorMin = rectTransform.anchorMin;
			startAnchorMax = rectTransform.anchorMax;
			lastVAlign = VerticalAlign;
			lastHAlign = HorizontalAlign;
		}

		private void OnGUI()
		{
			if (VerticalAlign != lastVAlign)
			{
				switch (VerticalAlign)
				{
				case VerticalAlignment.Top:
					rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 1f);
					rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1f);
					break;
				case VerticalAlignment.Middle:
					rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0.5f);
					rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0.5f);
					break;
				case VerticalAlignment.Bottom:
					rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0f);
					rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0f);
					break;
				}
				lastVAlign = VerticalAlign;
			}
			if (HorizontalAlign != lastHAlign)
			{
				switch (HorizontalAlign)
				{
				case HorizontalAlignment.Left:
					rectTransform.anchorMin = new Vector2(0f, rectTransform.anchorMin.y);
					rectTransform.anchorMax = new Vector2(0f, rectTransform.anchorMax.y);
					break;
				case HorizontalAlignment.Center:
					rectTransform.anchorMin = new Vector2(0.5f, rectTransform.anchorMin.y);
					rectTransform.anchorMax = new Vector2(0.5f, rectTransform.anchorMax.y);
					break;
				case HorizontalAlignment.Right:
					rectTransform.anchorMin = new Vector2(1f, rectTransform.anchorMin.y);
					rectTransform.anchorMax = new Vector2(1f, rectTransform.anchorMax.y);
					break;
				}
				lastHAlign = HorizontalAlign;
			}
		}

		public void Reset()
		{
			rectTransform.anchorMin = startAnchorMin;
			rectTransform.anchorMax = startAnchorMax;
		}
	}
}
