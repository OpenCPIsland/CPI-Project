using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(RectTransform))]
	public class PhysicalSizeRectScaler : PhysicalSizeScaler
	{
		public enum AnchorPosition
		{
			TOP_LEFT,
			TOP_CENTER,
			TOP_RIGHT,
			MID_LEFT,
			MID_CENTER,
			MID_RIGHT,
			LOW_LEFT,
			LOW_CENTER,
			LOW_RIGHT
		}

		public AnchorPosition AnchoredPosition = AnchorPosition.TOP_RIGHT;

		public float TargetHeightInches_Iphone5 = 0f;

		public float TargetHeightInches_Ipad = 0f;

		public float TargetWidthInches_Iphone5 = 0f;

		public float TargetWidthInches_Ipad = 0f;

		private RectTransform rectTransform;

		private Canvas parentCanvas;

		private void Start()
		{
			rectTransform = GetComponent<RectTransform>();
			parentCanvas = GetComponentInParent<Canvas>();
			ValidateAnchorPositions();
			ValidateScaleData();
			ApplyScale();
			SetPosition();
		}

		private void ValidateScaleData()
		{
			if (TargetHeightInches_Iphone5 == 0f == (TargetHeightInches_Ipad == 0f) && TargetWidthInches_Iphone5 == 0f == (TargetWidthInches_Ipad == 0f))
			{
			}
		}

		private void ValidateAnchorPositions()
		{
			if (TargetHeightInches_Iphone5 != 0f && rectTransform.anchorMax.y != rectTransform.anchorMin.y)
			{
			}
			if (TargetWidthInches_Iphone5 == 0f || rectTransform.anchorMax.x == rectTransform.anchorMin.x)
			{
			}
		}

		private void ApplyScale()
		{
			float screenDPI = GetScreenDPI();
			float y = rectTransform.sizeDelta.y;
			float x = rectTransform.sizeDelta.x;
			Vector2 targetDimensions = GetTargetDimensions();
			if (targetDimensions.y != 0f)
			{
				y = targetDimensions.y * screenDPI * (1f / parentCanvas.scaleFactor);
			}
			if (targetDimensions.x != 0f)
			{
				x = targetDimensions.x * screenDPI * (1f / parentCanvas.scaleFactor);
			}
			rectTransform.sizeDelta = new Vector2(x, y);
		}

		private Vector2 GetTargetDimensions()
		{
			Vector2 result = default(Vector2);
			float deviceSize = GetDeviceSize();
			float num = NormalizeScaleProperty(TargetHeightInches_Ipad, TargetHeightInches_Iphone5);
			float num2 = NormalizeScaleProperty(TargetWidthInches_Ipad, TargetWidthInches_Iphone5);
			result.x = TargetWidthInches_Iphone5 + (deviceSize - 4f) * num2;
			result.y = TargetHeightInches_Iphone5 + (deviceSize - 4f) * num;
			return result;
		}

		private void SetPosition()
		{
			Vector2 anchoredPosition = new Vector2(0f, 0f);
			switch (AnchoredPosition)
			{
			case AnchorPosition.TOP_LEFT:
				anchoredPosition = new Vector2(rectTransform.rect.width / 2f, rectTransform.rect.height / -2f);
				break;
			case AnchorPosition.TOP_CENTER:
				anchoredPosition = new Vector2(0f, rectTransform.rect.height / -2f);
				break;
			case AnchorPosition.TOP_RIGHT:
				anchoredPosition = new Vector2(rectTransform.rect.width / -2f, rectTransform.rect.height / -2f);
				break;
			case AnchorPosition.MID_LEFT:
				anchoredPosition = new Vector2(rectTransform.rect.width / 2f, 0f);
				break;
			case AnchorPosition.MID_CENTER:
				anchoredPosition = new Vector2(0f, 0f);
				break;
			case AnchorPosition.MID_RIGHT:
				anchoredPosition = new Vector2(rectTransform.rect.width / -2f, 0f);
				break;
			case AnchorPosition.LOW_LEFT:
				anchoredPosition = new Vector2(rectTransform.rect.width / 2f, rectTransform.rect.height / 2f);
				break;
			case AnchorPosition.LOW_CENTER:
				anchoredPosition = new Vector2(0f, rectTransform.rect.height / 2f);
				break;
			case AnchorPosition.LOW_RIGHT:
				anchoredPosition = new Vector2(rectTransform.rect.width / -2f, rectTransform.rect.height / 2f);
				break;
			}
			rectTransform.anchoredPosition = anchoredPosition;
		}
	}
}
