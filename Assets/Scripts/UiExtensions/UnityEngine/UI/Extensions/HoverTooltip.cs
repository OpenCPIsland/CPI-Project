namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/HoverTooltip")]
	public class HoverTooltip : MonoBehaviour
	{
		public int horizontalPadding;

		public int verticalPadding;

		public Text thisText;

		public HorizontalLayoutGroup hlG;

		public RectTransform bgImage;

		private Image bgImageSource;

		private bool firstUpdate;

		private bool inside;

		private RenderMode GUIMode;

		private Camera GUICamera;

		private Vector3 lowerLeft;

		private Vector3 upperRight;

		private float currentYScaleFactor;

		private float currentXScaleFactor;

		private float defaultYOffset;

		private float defaultXOffset;

		private float tooltipRealHeight;

		private float tooltipRealWidth;

		private void Start()
		{
			GUICamera = GameObject.Find("GUICamera").GetComponent<Camera>();
			GUIMode = base.transform.parent.parent.GetComponent<Canvas>().renderMode;
			bgImageSource = bgImage.GetComponent<Image>();
			inside = false;
			HideTooltipVisibility();
			base.transform.parent.gameObject.SetActive(false);
		}

		public void SetTooltip(string text)
		{
			NewTooltip();
			thisText.text = text;
			OnScreenSpaceCamera();
		}

		public void SetTooltip(string[] texts)
		{
			NewTooltip();
			string text = "";
			int num = 0;
			foreach (string text2 in texts)
			{
				text = ((num != 0) ? (text + "\n" + text2) : (text + text2));
				num++;
			}
			thisText.text = text;
			OnScreenSpaceCamera();
		}

		public void SetTooltip(string text, bool test)
		{
			NewTooltip();
			thisText.text = text;
			OnScreenSpaceCamera();
		}

		public void OnScreenSpaceCamera()
		{
			Vector3 position = GUICamera.ScreenToViewportPoint(Input.mousePosition);
			float num = 0f;
			float num2 = 0f;
			Vector3 vector = GUICamera.ViewportToScreenPoint(position);
			float x = vector.x;
			float num3 = tooltipRealWidth;
			Vector2 pivot = bgImage.pivot;
			float num4 = x + num3 * pivot.x;
			if (num4 > upperRight.x)
			{
				float num5 = upperRight.x - num4;
				num2 = ((!((double)num5 > (double)defaultXOffset * 0.75)) ? (defaultXOffset - tooltipRealWidth * 2f) : num5);
				Vector3 vector2 = GUICamera.ViewportToScreenPoint(position);
				Vector3 position2 = new Vector3(vector2.x + num2, 0f, 0f);
				Vector3 vector3 = GUICamera.ScreenToViewportPoint(position2);
				position.x = vector3.x;
			}
			Vector3 vector4 = GUICamera.ViewportToScreenPoint(position);
			float x2 = vector4.x;
			float num6 = tooltipRealWidth;
			Vector2 pivot2 = bgImage.pivot;
			num4 = x2 - num6 * pivot2.x;
			if (num4 < lowerLeft.x)
			{
				float num7 = lowerLeft.x - num4;
				num2 = ((!((double)num7 < (double)defaultXOffset * 0.75 - (double)tooltipRealWidth)) ? (tooltipRealWidth * 2f) : (0f - num7));
				Vector3 vector5 = GUICamera.ViewportToScreenPoint(position);
				Vector3 position3 = new Vector3(vector5.x - num2, 0f, 0f);
				Vector3 vector6 = GUICamera.ScreenToViewportPoint(position3);
				position.x = vector6.x;
			}
			Vector3 vector7 = GUICamera.ViewportToScreenPoint(position);
			float y = vector7.y;
			Vector2 sizeDelta = bgImage.sizeDelta;
			float num8 = sizeDelta.y * currentYScaleFactor;
			Vector2 pivot3 = bgImage.pivot;
			num4 = y - (num8 * pivot3.y - tooltipRealHeight);
			if (num4 > upperRight.y)
			{
				float num9 = upperRight.y - num4;
				Vector2 sizeDelta2 = bgImage.sizeDelta;
				float num10 = sizeDelta2.y * currentYScaleFactor;
				Vector2 pivot4 = bgImage.pivot;
				num = num10 * pivot4.y;
				num = ((!((double)num9 > (double)defaultYOffset * 0.75)) ? (defaultYOffset - tooltipRealHeight * 2f) : num9);
				float x3 = position.x;
				Vector3 vector8 = GUICamera.ViewportToScreenPoint(position);
				Vector3 position4 = new Vector3(x3, vector8.y + num, 0f);
				Vector3 vector9 = GUICamera.ScreenToViewportPoint(position4);
				position.y = vector9.y;
			}
			Vector3 vector10 = GUICamera.ViewportToScreenPoint(position);
			float y2 = vector10.y;
			Vector2 sizeDelta3 = bgImage.sizeDelta;
			float num11 = sizeDelta3.y * currentYScaleFactor;
			Vector2 pivot5 = bgImage.pivot;
			num4 = y2 - num11 * pivot5.y;
			if (num4 < lowerLeft.y)
			{
				float num12 = lowerLeft.y - num4;
				Vector2 sizeDelta4 = bgImage.sizeDelta;
				float num13 = sizeDelta4.y * currentYScaleFactor;
				Vector2 pivot6 = bgImage.pivot;
				num = num13 * pivot6.y;
				num = ((!((double)num12 < (double)defaultYOffset * 0.75 - (double)tooltipRealHeight)) ? (tooltipRealHeight * 2f) : num12);
				float x4 = position.x;
				Vector3 vector11 = GUICamera.ViewportToScreenPoint(position);
				Vector3 position5 = new Vector3(x4, vector11.y + num, 0f);
				Vector3 vector12 = GUICamera.ScreenToViewportPoint(position5);
				position.y = vector12.y;
			}
			Transform transform = base.transform.parent.transform;
			Vector3 vector13 = GUICamera.ViewportToWorldPoint(position);
			float x5 = vector13.x;
			Vector3 vector14 = GUICamera.ViewportToWorldPoint(position);
			transform.position = new Vector3(x5, vector14.y, 0f);
			base.transform.parent.gameObject.SetActive(true);
			inside = true;
		}

		public void HideTooltip()
		{
			if (GUIMode == RenderMode.ScreenSpaceCamera && this != null)
			{
				base.transform.parent.gameObject.SetActive(false);
				inside = false;
				HideTooltipVisibility();
			}
		}

		private void Update()
		{
			LayoutInit();
			if (inside && GUIMode == RenderMode.ScreenSpaceCamera)
			{
				OnScreenSpaceCamera();
			}
		}

		private void LayoutInit()
		{
			if (firstUpdate)
			{
				firstUpdate = false;
				bgImage.sizeDelta = new Vector2(hlG.preferredWidth + (float)horizontalPadding, hlG.preferredHeight + (float)verticalPadding);
				Vector2 sizeDelta = bgImage.sizeDelta;
				float num = sizeDelta.y * currentYScaleFactor;
				Vector2 pivot = bgImage.pivot;
				defaultYOffset = num * pivot.y;
				Vector2 sizeDelta2 = bgImage.sizeDelta;
				float num2 = sizeDelta2.x * currentXScaleFactor;
				Vector2 pivot2 = bgImage.pivot;
				defaultXOffset = num2 * pivot2.x;
				Vector2 sizeDelta3 = bgImage.sizeDelta;
				tooltipRealHeight = sizeDelta3.y * currentYScaleFactor;
				Vector2 sizeDelta4 = bgImage.sizeDelta;
				tooltipRealWidth = sizeDelta4.x * currentXScaleFactor;
				ActivateTooltipVisibility();
			}
		}

		private void NewTooltip()
		{
			firstUpdate = true;
			lowerLeft = GUICamera.ViewportToScreenPoint(new Vector3(0f, 0f, 0f));
			upperRight = GUICamera.ViewportToScreenPoint(new Vector3(1f, 1f, 0f));
			float num = Screen.height;
			Vector2 referenceResolution = base.transform.root.GetComponent<CanvasScaler>().referenceResolution;
			currentYScaleFactor = num / referenceResolution.y;
			float num2 = Screen.width;
			Vector2 referenceResolution2 = base.transform.root.GetComponent<CanvasScaler>().referenceResolution;
			currentXScaleFactor = num2 / referenceResolution2.x;
		}

		public void ActivateTooltipVisibility()
		{
			Color color = thisText.color;
			thisText.color = new Color(color.r, color.g, color.b, 1f);
			Image image = bgImageSource;
			Color color2 = bgImageSource.color;
			float r = color2.r;
			Color color3 = bgImageSource.color;
			float g = color3.g;
			Color color4 = bgImageSource.color;
			image.color = new Color(r, g, color4.b, 0.8f);
		}

		public void HideTooltipVisibility()
		{
			Color color = thisText.color;
			thisText.color = new Color(color.r, color.g, color.b, 0f);
			Image image = bgImageSource;
			Color color2 = bgImageSource.color;
			float r = color2.r;
			Color color3 = bgImageSource.color;
			float g = color3.g;
			Color color4 = bgImageSource.color;
			image.color = new Color(r, g, color4.b, 0f);
		}
	}
}
