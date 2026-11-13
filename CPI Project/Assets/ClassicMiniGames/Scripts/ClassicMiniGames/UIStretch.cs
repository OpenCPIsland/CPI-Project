using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Stretch")]
public class UIStretch : MonoBehaviour
{
	public enum Style
	{
		None,
		Horizontal,
		Vertical,
		Both,
		BasedOnHeight,
		FillKeepingRatio,
		FitInternalKeepingRatio
	}

	public Camera uiCamera = null;

	public GameObject container = null;

	public Style style = Style.None;

	public bool runOnlyOnce = true;

	public Vector2 relativeSize = Vector2.one;

	public Vector2 initialSize = Vector2.one;

	public Vector2 borderPadding = Vector2.zero;

	[HideInInspector]
	[SerializeField]
	private UIWidget widgetContainer;

	private Transform mTrans;

	private UIWidget mWidget;

	private UISprite mSprite;

	private UIPanel mPanel;

	private UIRoot mRoot;

	private Animation mAnim;

	private Rect mRect;

	private bool mStarted = false;

	private void Awake()
	{
		mAnim = GetComponent<Animation>();
		mRect = default(Rect);
		mTrans = base.transform;
		mWidget = GetComponent<UIWidget>();
		mSprite = GetComponent<UISprite>();
		mPanel = GetComponent<UIPanel>();
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Combine(UICamera.onScreenResize, new UICamera.OnScreenResize(ScreenSizeChanged));
	}

	private void OnDestroy()
	{
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Remove(UICamera.onScreenResize, new UICamera.OnScreenResize(ScreenSizeChanged));
	}

	private void ScreenSizeChanged()
	{
		if (mStarted && runOnlyOnce)
		{
			Update();
		}
	}

	private void Start()
	{
		if (container == null && widgetContainer != null)
		{
			container = widgetContainer.gameObject;
			widgetContainer = null;
		}
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		Update();
		mStarted = true;
	}

	private void Update()
	{
		if ((mAnim != null && mAnim.isPlaying) || style == Style.None)
		{
			return;
		}
		UIWidget uIWidget = (container == null) ? null : container.GetComponent<UIWidget>();
		UIPanel uIPanel = (container == null && uIWidget == null) ? null : container.GetComponent<UIPanel>();
		float num = 1f;
		Vector4 baseClipRegion;
		if (uIWidget != null)
		{
			Bounds bounds = uIWidget.CalculateBounds(base.transform.parent);
			mRect.x = bounds.min.x;
			mRect.y = bounds.min.y;
			mRect.width = bounds.size.x;
			mRect.height = bounds.size.y;
		}
		else if (uIPanel != null)
		{
			if (uIPanel.clipping == UIDrawCall.Clipping.None)
			{
				float num2 = (mRoot != null) ? ((float)mRoot.activeHeight / (float)Screen.height * 0.5f) : 0.5f;
				mRect.xMin = (float)(-Screen.width) * num2;
				mRect.yMin = (float)(-Screen.height) * num2;
				mRect.xMax = 0f - mRect.xMin;
				mRect.yMax = 0f - mRect.yMin;
			}
			else
			{
				baseClipRegion = uIPanel.finalClipRegion;
				mRect.x = baseClipRegion.x - baseClipRegion.z * 0.5f;
				mRect.y = baseClipRegion.y - baseClipRegion.w * 0.5f;
				mRect.width = baseClipRegion.z;
				mRect.height = baseClipRegion.w;
			}
		}
		else if (container != null)
		{
			Transform parent = base.transform.parent;
			Bounds bounds = (parent != null) ? NGUIMath.CalculateRelativeWidgetBounds(parent, container.transform) : NGUIMath.CalculateRelativeWidgetBounds(container.transform);
			mRect.x = bounds.min.x;
			mRect.y = bounds.min.y;
			mRect.width = bounds.size.x;
			mRect.height = bounds.size.y;
		}
		else
		{
			if (!(uiCamera != null))
			{
				return;
			}
			mRect = uiCamera.pixelRect;
			if (mRoot != null)
			{
				num = mRoot.pixelSizeAdjustment;
			}
		}
		float num3 = mRect.width;
		float num4 = mRect.height;
		if (num != 1f && num4 > 1f)
		{
			float num5 = (float)mRoot.activeHeight / num4;
			num3 *= num5;
			num4 *= num5;
		}
		Vector3 vector = (mWidget != null) ? new Vector3(mWidget.width, mWidget.height) : mTrans.localScale;
		if (style == Style.BasedOnHeight)
		{
			vector.x = relativeSize.x * num4;
			vector.y = relativeSize.y * num4;
		}
		else if (style == Style.FillKeepingRatio)
		{
			float num6 = num3 / num4;
			float num7 = initialSize.x / initialSize.y;
			if (num7 < num6)
			{
				float num5 = num3 / initialSize.x;
				vector.x = num3;
				vector.y = initialSize.y * num5;
			}
			else
			{
				float num5 = num4 / initialSize.y;
				vector.x = initialSize.x * num5;
				vector.y = num4;
			}
		}
		else if (style == Style.FitInternalKeepingRatio)
		{
			float num6 = num3 / num4;
			float num7 = initialSize.x / initialSize.y;
			if (num7 > num6)
			{
				float num5 = num3 / initialSize.x;
				vector.x = num3;
				vector.y = initialSize.y * num5;
			}
			else
			{
				float num5 = num4 / initialSize.y;
				vector.x = initialSize.x * num5;
				vector.y = num4;
			}
		}
		else
		{
			if (style != Style.Vertical)
			{
				vector.x = relativeSize.x * num3;
			}
			if (style != Style.Horizontal)
			{
				vector.y = relativeSize.y * num4;
			}
		}
		if (mSprite != null)
		{
			float num8 = (mSprite.atlas != null) ? mSprite.atlas.pixelSize : 1f;
			vector.x -= borderPadding.x * num8;
			vector.y -= borderPadding.y * num8;
			if (style != Style.Vertical)
			{
				mSprite.width = Mathf.RoundToInt(vector.x);
			}
			if (style != Style.Horizontal)
			{
				mSprite.height = Mathf.RoundToInt(vector.y);
			}
			vector = Vector3.one;
		}
		else if (mWidget != null)
		{
			if (style != Style.Vertical)
			{
				mWidget.width = Mathf.RoundToInt(vector.x - borderPadding.x);
			}
			if (style != Style.Horizontal)
			{
				mWidget.height = Mathf.RoundToInt(vector.y - borderPadding.y);
			}
			vector = Vector3.one;
		}
		else if (mPanel != null)
		{
			baseClipRegion = mPanel.baseClipRegion;
			if (style != Style.Vertical)
			{
				baseClipRegion.z = vector.x - borderPadding.x;
			}
			if (style != Style.Horizontal)
			{
				baseClipRegion.w = vector.y - borderPadding.y;
			}
			mPanel.baseClipRegion = baseClipRegion;
			vector = Vector3.one;
		}
		else
		{
			if (style != Style.Vertical)
			{
				vector.x -= borderPadding.x;
			}
			if (style != Style.Horizontal)
			{
				vector.y -= borderPadding.y;
			}
		}
		if (mTrans.localScale != vector)
		{
			mTrans.localScale = vector;
		}
		if (runOnlyOnce && Application.isPlaying)
		{
			base.enabled = false;
		}
	}
}
