using System;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Widget")]
[ExecuteInEditMode]
public class UIWidget : UIRect
{
	public enum Pivot
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}

	public delegate void OnDimensionsChanged();

	public enum AspectRatioSource
	{
		Free,
		BasedOnWidth,
		BasedOnHeight
	}

	public delegate bool HitCheck(Vector3 worldPos);

	[SerializeField]
	[HideInInspector]
	protected Color mColor = Color.white;

	[HideInInspector]
	[SerializeField]
	protected Pivot mPivot = Pivot.Center;

	[HideInInspector]
	[SerializeField]
	protected int mWidth = 100;

	[SerializeField]
	[HideInInspector]
	protected int mHeight = 100;

	[SerializeField]
	[HideInInspector]
	protected int mDepth = 0;

	public OnDimensionsChanged onChange;

	public bool autoResizeBoxCollider = false;

	public bool hideIfOffScreen = false;

	public AspectRatioSource keepAspectRatio = AspectRatioSource.Free;

	public float aspectRatio = 1f;

	public HitCheck hitCheck;

	[NonSerialized]
	public UIPanel panel;

	[NonSerialized]
	public UIGeometry geometry = new UIGeometry();

	[NonSerialized]
	public bool fillGeometry = true;

	[NonSerialized]
	protected bool mPlayMode = true;

	[NonSerialized]
	protected Vector4 mDrawRegion = new Vector4(0f, 0f, 1f, 1f);

	[NonSerialized]
	private Matrix4x4 mLocalToPanel;

	[NonSerialized]
	private bool mIsVisibleByAlpha = true;

	[NonSerialized]
	private bool mIsVisibleByPanel = true;

	[NonSerialized]
	private bool mIsInFront = true;

	[NonSerialized]
	private float mLastAlpha = 0f;

	[NonSerialized]
	private bool mMoved = false;

	[NonSerialized]
	public UIDrawCall drawCall;

	[NonSerialized]
	protected Vector3[] mCorners = new Vector3[4];

	[NonSerialized]
	private int mAlphaFrameID = -1;

	private int mMatrixFrame = -1;

	private Vector3 mOldV0;

	private Vector3 mOldV1;

	public Vector4 drawRegion
	{
		get
		{
			return mDrawRegion;
		}
		set
		{
			if (mDrawRegion != value)
			{
				mDrawRegion = value;
				if (autoResizeBoxCollider)
				{
					ResizeCollider();
				}
				MarkAsChanged();
			}
		}
	}

	public Vector2 pivotOffset
	{
		get
		{
			return NGUIMath.GetPivotOffset(pivot);
		}
	}

	public int width
	{
		get
		{
			return mWidth;
		}
		set
		{
			int minWidth = this.minWidth;
			if (value < minWidth)
			{
				value = minWidth;
			}
			if (mWidth == value || keepAspectRatio == AspectRatioSource.BasedOnHeight)
			{
				return;
			}
			if (isAnchoredHorizontally)
			{
				if (leftAnchor.target != null && rightAnchor.target != null)
				{
					if (mPivot == Pivot.BottomLeft || mPivot == Pivot.Left || mPivot == Pivot.TopLeft)
					{
						NGUIMath.AdjustWidget(this, 0f, 0f, value - mWidth, 0f);
						return;
					}
					if (mPivot == Pivot.BottomRight || mPivot == Pivot.Right || mPivot == Pivot.TopRight)
					{
						NGUIMath.AdjustWidget(this, mWidth - value, 0f, 0f, 0f);
						return;
					}
					int num = value - mWidth;
					num -= (num & 1);
					if (num != 0)
					{
						NGUIMath.AdjustWidget(this, (float)(-num) * 0.5f, 0f, (float)num * 0.5f, 0f);
					}
				}
				else if (leftAnchor.target != null)
				{
					NGUIMath.AdjustWidget(this, 0f, 0f, value - mWidth, 0f);
				}
				else
				{
					NGUIMath.AdjustWidget(this, mWidth - value, 0f, 0f, 0f);
				}
			}
			else
			{
				SetDimensions(value, mHeight);
			}
		}
	}

	public int height
	{
		get
		{
			return mHeight;
		}
		set
		{
			int minHeight = this.minHeight;
			if (value < minHeight)
			{
				value = minHeight;
			}
			if (mHeight == value || keepAspectRatio == AspectRatioSource.BasedOnWidth)
			{
				return;
			}
			if (isAnchoredVertically)
			{
				if (bottomAnchor.target != null && topAnchor.target != null)
				{
					if (mPivot == Pivot.BottomLeft || mPivot == Pivot.Bottom || mPivot == Pivot.BottomRight)
					{
						NGUIMath.AdjustWidget(this, 0f, 0f, 0f, value - mHeight);
						return;
					}
					if (mPivot == Pivot.TopLeft || mPivot == Pivot.Top || mPivot == Pivot.TopRight)
					{
						NGUIMath.AdjustWidget(this, 0f, mHeight - value, 0f, 0f);
						return;
					}
					int num = value - mHeight;
					num -= (num & 1);
					if (num != 0)
					{
						NGUIMath.AdjustWidget(this, 0f, (float)(-num) * 0.5f, 0f, (float)num * 0.5f);
					}
				}
				else if (bottomAnchor.target != null)
				{
					NGUIMath.AdjustWidget(this, 0f, 0f, 0f, value - mHeight);
				}
				else
				{
					NGUIMath.AdjustWidget(this, 0f, mHeight - value, 0f, 0f);
				}
			}
			else
			{
				SetDimensions(mWidth, value);
			}
		}
	}

	public Color color
	{
		get
		{
			return mColor;
		}
		set
		{
			if (mColor != value)
			{
				bool includeChildren = mColor.a != value.a;
				mColor = value;
				Invalidate(includeChildren);
			}
		}
	}

	public override float alpha
	{
		get
		{
			return mColor.a;
		}
		set
		{
			if (mColor.a != value)
			{
				mColor.a = value;
				Invalidate(true);
			}
		}
	}

	public bool isVisible
	{
		get
		{
			return mIsVisibleByPanel && mIsVisibleByAlpha && mIsInFront && finalAlpha > 0.001f && NGUITools.GetActive(this);
		}
	}

	public bool hasVertices
	{
		get
		{
			return geometry != null && geometry.hasVertices;
		}
	}

	public Pivot rawPivot
	{
		get
		{
			return mPivot;
		}
		set
		{
			if (mPivot != value)
			{
				mPivot = value;
				if (autoResizeBoxCollider)
				{
					ResizeCollider();
				}
				MarkAsChanged();
			}
		}
	}

	public Pivot pivot
	{
		get
		{
			return mPivot;
		}
		set
		{
			if (mPivot != value)
			{
				Vector3 vector = worldCorners[0];
				mPivot = value;
				mChanged = true;
				Vector3 vector2 = worldCorners[0];
				Transform cachedTransform = base.cachedTransform;
				Vector3 vector3 = cachedTransform.position;
				float z = cachedTransform.localPosition.z;
				vector3.x += vector.x - vector2.x;
				vector3.y += vector.y - vector2.y;
				base.cachedTransform.position = vector3;
				vector3 = base.cachedTransform.localPosition;
				vector3.x = Mathf.Round(vector3.x);
				vector3.y = Mathf.Round(vector3.y);
				vector3.z = z;
				base.cachedTransform.localPosition = vector3;
			}
		}
	}

	public int depth
	{
		get
		{
			return mDepth;
		}
		set
		{
			if (mDepth == value)
			{
				return;
			}
			if (panel != null)
			{
				panel.RemoveWidget(this);
			}
			mDepth = value;
			if (panel != null)
			{
				panel.AddWidget(this);
				if (!Application.isPlaying)
				{
					panel.SortWidgets();
					panel.RebuildAllDrawCalls();
				}
			}
		}
	}

	public int raycastDepth
	{
		get
		{
			if (panel == null)
			{
				CreatePanel();
			}
			return (panel != null) ? (mDepth + panel.depth * 1000) : mDepth;
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			Vector2 pivotOffset = this.pivotOffset;
			float num = (0f - pivotOffset.x) * (float)mWidth;
			float num2 = (0f - pivotOffset.y) * (float)mHeight;
			float x = num + (float)mWidth;
			float y = num2 + (float)mHeight;
			mCorners[0] = new Vector3(num, num2);
			mCorners[1] = new Vector3(num, y);
			mCorners[2] = new Vector3(x, y);
			mCorners[3] = new Vector3(x, num2);
			return mCorners;
		}
	}

	public virtual Vector2 localSize
	{
		get
		{
			Vector3[] localCorners = this.localCorners;
			return localCorners[2] - localCorners[0];
		}
	}

	public Vector3 localCenter
	{
		get
		{
			Vector3[] localCorners = this.localCorners;
			return Vector3.Lerp(localCorners[0], localCorners[2], 0.5f);
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			Vector2 pivotOffset = this.pivotOffset;
			float num = (0f - pivotOffset.x) * (float)mWidth;
			float num2 = (0f - pivotOffset.y) * (float)mHeight;
			float x = num + (float)mWidth;
			float y = num2 + (float)mHeight;
			Transform cachedTransform = base.cachedTransform;
			mCorners[0] = cachedTransform.TransformPoint(num, num2, 0f);
			mCorners[1] = cachedTransform.TransformPoint(num, y, 0f);
			mCorners[2] = cachedTransform.TransformPoint(x, y, 0f);
			mCorners[3] = cachedTransform.TransformPoint(x, num2, 0f);
			return mCorners;
		}
	}

	public Vector3 worldCenter
	{
		get
		{
			return base.cachedTransform.TransformPoint(localCenter);
		}
	}

	public virtual Vector4 drawingDimensions
	{
		get
		{
			Vector2 pivotOffset = this.pivotOffset;
			float num = (0f - pivotOffset.x) * (float)mWidth;
			float num2 = (0f - pivotOffset.y) * (float)mHeight;
			float num3 = num + (float)mWidth;
			float num4 = num2 + (float)mHeight;
			return new Vector4((mDrawRegion.x == 0f) ? num : Mathf.Lerp(num, num3, mDrawRegion.x), (mDrawRegion.y == 0f) ? num2 : Mathf.Lerp(num2, num4, mDrawRegion.y), (mDrawRegion.z == 1f) ? num3 : Mathf.Lerp(num, num3, mDrawRegion.z), (mDrawRegion.w == 1f) ? num4 : Mathf.Lerp(num2, num4, mDrawRegion.w));
		}
	}

	public virtual Material material
	{
		get
		{
			return null;
		}
		set
		{
			throw new NotImplementedException(string.Concat(GetType(), " has no material setter"));
		}
	}

	public virtual Texture mainTexture
	{
		get
		{
			Material material = this.material;
			return (material != null) ? material.mainTexture : null;
		}
		set
		{
			throw new NotImplementedException(string.Concat(GetType(), " has no mainTexture setter"));
		}
	}

	public virtual Shader shader
	{
		get
		{
			Material material = this.material;
			return (material != null) ? material.shader : null;
		}
		set
		{
			throw new NotImplementedException(string.Concat(GetType(), " has no shader setter"));
		}
	}

	[Obsolete("There is no relative scale anymore. Widgets now have width and height instead")]
	public Vector2 relativeSize
	{
		get
		{
			return Vector2.one;
		}
	}

	public bool hasBoxCollider
	{
		get
		{
			BoxCollider x = GetComponent<Collider>() as BoxCollider;
			if (x != null)
			{
				return true;
			}
			return GetComponent<BoxCollider2D>() != null;
		}
	}

	public virtual int minWidth
	{
		get
		{
			return 2;
		}
	}

	public virtual int minHeight
	{
		get
		{
			return 2;
		}
	}

	public virtual Vector4 border
	{
		get
		{
			return Vector4.zero;
		}
		set
		{
		}
	}

	public void SetDimensions(int w, int h)
	{
		if (mWidth != w || mHeight != h)
		{
			mWidth = w;
			mHeight = h;
			if (keepAspectRatio == AspectRatioSource.BasedOnWidth)
			{
				mHeight = Mathf.RoundToInt((float)mWidth / aspectRatio);
			}
			else if (keepAspectRatio == AspectRatioSource.BasedOnHeight)
			{
				mWidth = Mathf.RoundToInt((float)mHeight * aspectRatio);
			}
			else if (keepAspectRatio == AspectRatioSource.Free)
			{
				aspectRatio = (float)mWidth / (float)mHeight;
			}
			mMoved = true;
			if (autoResizeBoxCollider)
			{
				ResizeCollider();
			}
			MarkAsChanged();
		}
	}

	public override Vector3[] GetSides(Transform relativeTo)
	{
		Vector2 pivotOffset = this.pivotOffset;
		float num = (0f - pivotOffset.x) * (float)mWidth;
		float num2 = (0f - pivotOffset.y) * (float)mHeight;
		float num3 = num + (float)mWidth;
		float num4 = num2 + (float)mHeight;
		float x = (num + num3) * 0.5f;
		float y = (num2 + num4) * 0.5f;
		Transform cachedTransform = base.cachedTransform;
		mCorners[0] = cachedTransform.TransformPoint(num, y, 0f);
		mCorners[1] = cachedTransform.TransformPoint(x, num4, 0f);
		mCorners[2] = cachedTransform.TransformPoint(num3, y, 0f);
		mCorners[3] = cachedTransform.TransformPoint(x, num2, 0f);
		if (relativeTo != null)
		{
			for (int i = 0; i < 4; i++)
			{
				mCorners[i] = relativeTo.InverseTransformPoint(mCorners[i]);
			}
		}
		return mCorners;
	}

	public override float CalculateFinalAlpha(int frameID)
	{
		if (mAlphaFrameID != frameID)
		{
			mAlphaFrameID = frameID;
			UpdateFinalAlpha(frameID);
		}
		return finalAlpha;
	}

	protected void UpdateFinalAlpha(int frameID)
	{
		if (!mIsVisibleByAlpha || !mIsInFront)
		{
			finalAlpha = 0f;
			return;
		}
		UIRect parent = base.parent;
		finalAlpha = ((base.parent != null) ? (parent.CalculateFinalAlpha(frameID) * mColor.a) : mColor.a);
	}

	public override void Invalidate(bool includeChildren)
	{
		mChanged = true;
		mAlphaFrameID = -1;
		if (panel != null)
		{
			bool visibleByPanel = (!hideIfOffScreen && !panel.hasCumulativeClipping) || panel.IsVisible(this);
			UpdateVisibility(CalculateCumulativeAlpha(Time.frameCount) > 0.001f, visibleByPanel);
			UpdateFinalAlpha(Time.frameCount);
			if (includeChildren)
			{
				base.Invalidate(true);
			}
		}
	}

	public float CalculateCumulativeAlpha(int frameID)
	{
		UIRect parent = base.parent;
		return (parent != null) ? (parent.CalculateFinalAlpha(frameID) * mColor.a) : mColor.a;
	}

	public override void SetRect(float x, float y, float width, float height)
	{
		Vector2 pivotOffset = this.pivotOffset;
		float num = Mathf.Lerp(x, x + width, pivotOffset.x);
		float num2 = Mathf.Lerp(y, y + height, pivotOffset.y);
		int num3 = Mathf.FloorToInt(width + 0.5f);
		int num4 = Mathf.FloorToInt(height + 0.5f);
		if (pivotOffset.x == 0.5f)
		{
			num3 = num3 >> 1 << 1;
		}
		if (pivotOffset.y == 0.5f)
		{
			num4 = num4 >> 1 << 1;
		}
		Transform cachedTransform = base.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x = Mathf.Floor(num + 0.5f);
		localPosition.y = Mathf.Floor(num2 + 0.5f);
		if (num3 < minWidth)
		{
			num3 = minWidth;
		}
		if (num4 < minHeight)
		{
			num4 = minHeight;
		}
		cachedTransform.localPosition = localPosition;
		this.width = num3;
		this.height = num4;
		if (base.isAnchored)
		{
			cachedTransform = cachedTransform.parent;
			if ((bool)leftAnchor.target)
			{
				leftAnchor.SetHorizontal(cachedTransform, x);
			}
			if ((bool)rightAnchor.target)
			{
				rightAnchor.SetHorizontal(cachedTransform, x + width);
			}
			if ((bool)bottomAnchor.target)
			{
				bottomAnchor.SetVertical(cachedTransform, y);
			}
			if ((bool)topAnchor.target)
			{
				topAnchor.SetVertical(cachedTransform, y + height);
			}
		}
	}

	public void ResizeCollider()
	{
		if (NGUITools.GetActive(this))
		{
			NGUITools.UpdateWidgetCollider(base.gameObject);
		}
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static int FullCompareFunc(UIWidget left, UIWidget right)
	{
		int num = UIPanel.CompareFunc(left.panel, right.panel);
		return (num == 0) ? PanelCompareFunc(left, right) : num;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static int PanelCompareFunc(UIWidget left, UIWidget right)
	{
		if (left.mDepth < right.mDepth)
		{
			return -1;
		}
		if (left.mDepth > right.mDepth)
		{
			return 1;
		}
		Material material = left.material;
		Material material2 = right.material;
		if (material == material2)
		{
			return 0;
		}
		if (material != null)
		{
			return -1;
		}
		if (material2 != null)
		{
			return 1;
		}
		return (material.GetInstanceID() >= material2.GetInstanceID()) ? 1 : (-1);
	}

	public Bounds CalculateBounds()
	{
		return CalculateBounds(null);
	}

	public Bounds CalculateBounds(Transform relativeParent)
	{
		Vector3[] localCorners;
		Bounds result;
		if (relativeParent == null)
		{
			localCorners = this.localCorners;
			result = new Bounds(localCorners[0], Vector3.zero);
			for (int i = 1; i < 4; i++)
			{
				result.Encapsulate(localCorners[i]);
			}
			return result;
		}
		Matrix4x4 worldToLocalMatrix = relativeParent.worldToLocalMatrix;
		localCorners = worldCorners;
		result = new Bounds(worldToLocalMatrix.MultiplyPoint3x4(localCorners[0]), Vector3.zero);
		for (int i = 1; i < 4; i++)
		{
			result.Encapsulate(worldToLocalMatrix.MultiplyPoint3x4(localCorners[i]));
		}
		return result;
	}

	public void SetDirty()
	{
		if (drawCall != null)
		{
			drawCall.isDirty = true;
		}
		else if (isVisible && hasVertices)
		{
			CreatePanel();
		}
	}

	protected void RemoveFromPanel()
	{
		if (panel != null)
		{
			panel.RemoveWidget(this);
			panel = null;
		}
	}

	public virtual void MarkAsChanged()
	{
		if (NGUITools.GetActive(this))
		{
			mChanged = true;
			if (panel != null && base.enabled && NGUITools.GetActive(base.gameObject) && !mPlayMode)
			{
				SetDirty();
				CheckLayer();
			}
		}
	}

	public UIPanel CreatePanel()
	{
		if (mStarted && panel == null && base.enabled && NGUITools.GetActive(base.gameObject))
		{
			panel = UIPanel.Find(base.cachedTransform, true, base.cachedGameObject.layer);
			if (panel != null)
			{
				mParentFound = false;
				panel.AddWidget(this);
				CheckLayer();
				Invalidate(true);
			}
		}
		return panel;
	}

	public void CheckLayer()
	{
		if (panel != null && panel.gameObject.layer != base.gameObject.layer)
		{
			UnityEngine.Debug.LogWarning("You can't place widgets on a layer different than the UIPanel that manages them.\nIf you want to move widgets to a different layer, parent them to a new panel instead.", this);
			base.gameObject.layer = panel.gameObject.layer;
		}
	}

	public override void ParentHasChanged()
	{
		base.ParentHasChanged();
		if (panel != null)
		{
			UIPanel y = UIPanel.Find(base.cachedTransform, true, base.cachedGameObject.layer);
			if (panel != y)
			{
				RemoveFromPanel();
				CreatePanel();
			}
		}
	}

	protected virtual void Awake()
	{
		mGo = base.gameObject;
		mPlayMode = Application.isPlaying;
	}

	protected override void OnInit()
	{
		base.OnInit();
		RemoveFromPanel();
		mMoved = true;
		if (mWidth == 100 && mHeight == 100 && base.cachedTransform.localScale.magnitude > 8f)
		{
			UpgradeFrom265();
			base.cachedTransform.localScale = Vector3.one;
		}
		Update();
	}

	protected virtual void UpgradeFrom265()
	{
		Vector3 localScale = base.cachedTransform.localScale;
		mWidth = Mathf.Abs(Mathf.RoundToInt(localScale.x));
		mHeight = Mathf.Abs(Mathf.RoundToInt(localScale.y));
		NGUITools.UpdateWidgetCollider(base.gameObject, true);
	}

	protected override void OnStart()
	{
		CreatePanel();
	}

	protected override void OnAnchor()
	{
		Transform cachedTransform = base.cachedTransform;
		Transform parent = cachedTransform.parent;
		Vector3 localPosition = cachedTransform.localPosition;
		Vector2 pivotOffset = this.pivotOffset;
		float num;
		float num2;
		float num3;
		float num4;
		if (leftAnchor.target == bottomAnchor.target && leftAnchor.target == rightAnchor.target && leftAnchor.target == topAnchor.target)
		{
			Vector3[] sides = leftAnchor.GetSides(parent);
			if (sides != null)
			{
				num = NGUIMath.Lerp(sides[0].x, sides[2].x, leftAnchor.relative) + (float)leftAnchor.absolute;
				num2 = NGUIMath.Lerp(sides[0].x, sides[2].x, rightAnchor.relative) + (float)rightAnchor.absolute;
				num3 = NGUIMath.Lerp(sides[3].y, sides[1].y, bottomAnchor.relative) + (float)bottomAnchor.absolute;
				num4 = NGUIMath.Lerp(sides[3].y, sides[1].y, topAnchor.relative) + (float)topAnchor.absolute;
				mIsInFront = true;
			}
			else
			{
				Vector3 localPos = GetLocalPos(leftAnchor, parent);
				num = localPos.x + (float)leftAnchor.absolute;
				num3 = localPos.y + (float)bottomAnchor.absolute;
				num2 = localPos.x + (float)rightAnchor.absolute;
				num4 = localPos.y + (float)topAnchor.absolute;
				mIsInFront = (!hideIfOffScreen || localPos.z >= 0f);
			}
		}
		else
		{
			mIsInFront = true;
			if ((bool)leftAnchor.target)
			{
				Vector3[] sides = leftAnchor.GetSides(parent);
				num = ((sides == null) ? (GetLocalPos(leftAnchor, parent).x + (float)leftAnchor.absolute) : (NGUIMath.Lerp(sides[0].x, sides[2].x, leftAnchor.relative) + (float)leftAnchor.absolute));
			}
			else
			{
				num = localPosition.x - pivotOffset.x * (float)mWidth;
			}
			if ((bool)rightAnchor.target)
			{
				Vector3[] sides = rightAnchor.GetSides(parent);
				num2 = ((sides == null) ? (GetLocalPos(rightAnchor, parent).x + (float)rightAnchor.absolute) : (NGUIMath.Lerp(sides[0].x, sides[2].x, rightAnchor.relative) + (float)rightAnchor.absolute));
			}
			else
			{
				num2 = localPosition.x - pivotOffset.x * (float)mWidth + (float)mWidth;
			}
			if ((bool)bottomAnchor.target)
			{
				Vector3[] sides = bottomAnchor.GetSides(parent);
				num3 = ((sides == null) ? (GetLocalPos(bottomAnchor, parent).y + (float)bottomAnchor.absolute) : (NGUIMath.Lerp(sides[3].y, sides[1].y, bottomAnchor.relative) + (float)bottomAnchor.absolute));
			}
			else
			{
				num3 = localPosition.y - pivotOffset.y * (float)mHeight;
			}
			if ((bool)topAnchor.target)
			{
				Vector3[] sides = topAnchor.GetSides(parent);
				num4 = ((sides == null) ? (GetLocalPos(topAnchor, parent).y + (float)topAnchor.absolute) : (NGUIMath.Lerp(sides[3].y, sides[1].y, topAnchor.relative) + (float)topAnchor.absolute));
			}
			else
			{
				num4 = localPosition.y - pivotOffset.y * (float)mHeight + (float)mHeight;
			}
		}
		Vector3 vector = new Vector3(Mathf.Lerp(num, num2, pivotOffset.x), Mathf.Lerp(num3, num4, pivotOffset.y), localPosition.z);
		int num5 = Mathf.FloorToInt(num2 - num + 0.5f);
		int num6 = Mathf.FloorToInt(num4 - num3 + 0.5f);
		if (keepAspectRatio != 0 && aspectRatio != 0f)
		{
			if (keepAspectRatio == AspectRatioSource.BasedOnHeight)
			{
				num5 = Mathf.RoundToInt((float)num6 * aspectRatio);
			}
			else
			{
				num6 = Mathf.RoundToInt((float)num5 / aspectRatio);
			}
		}
		if (num5 < minWidth)
		{
			num5 = minWidth;
		}
		if (num6 < minHeight)
		{
			num6 = minHeight;
		}
		if (Vector3.SqrMagnitude(localPosition - vector) > 0.001f)
		{
			base.cachedTransform.localPosition = vector;
			if (mIsInFront)
			{
				mChanged = true;
			}
		}
		if (mWidth != num5 || mHeight != num6)
		{
			mWidth = num5;
			mHeight = num6;
			if (mIsInFront)
			{
				mChanged = true;
			}
			if (autoResizeBoxCollider)
			{
				ResizeCollider();
			}
		}
	}

	protected override void OnUpdate()
	{
		if (panel == null)
		{
			CreatePanel();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			MarkAsChanged();
		}
	}

	protected override void OnDisable()
	{
		RemoveFromPanel();
		base.OnDisable();
	}

	private void OnDestroy()
	{
		RemoveFromPanel();
	}

	public bool UpdateVisibility(bool visibleByAlpha, bool visibleByPanel)
	{
		if (mIsVisibleByAlpha != visibleByAlpha || mIsVisibleByPanel != visibleByPanel)
		{
			mChanged = true;
			mIsVisibleByAlpha = visibleByAlpha;
			mIsVisibleByPanel = visibleByPanel;
			return true;
		}
		return false;
	}

	public bool UpdateTransform(int frame)
	{
		if (!mMoved && !panel.widgetsAreStatic && base.cachedTransform.hasChanged)
		{
			mTrans.hasChanged = false;
			mLocalToPanel = panel.worldToLocal * base.cachedTransform.localToWorldMatrix;
			mMatrixFrame = frame;
			Vector2 pivotOffset = this.pivotOffset;
			float num = (0f - pivotOffset.x) * (float)mWidth;
			float num2 = (0f - pivotOffset.y) * (float)mHeight;
			float x = num + (float)mWidth;
			float y = num2 + (float)mHeight;
			Transform cachedTransform = base.cachedTransform;
			Vector3 point = cachedTransform.TransformPoint(num, num2, 0f);
			Vector3 point2 = cachedTransform.TransformPoint(x, y, 0f);
			point = panel.worldToLocal.MultiplyPoint3x4(point);
			point2 = panel.worldToLocal.MultiplyPoint3x4(point2);
			if (Vector3.SqrMagnitude(mOldV0 - point) > 1E-06f || Vector3.SqrMagnitude(mOldV1 - point2) > 1E-06f)
			{
				mMoved = true;
				mOldV0 = point;
				mOldV1 = point2;
			}
		}
		if (mMoved && onChange != null)
		{
			onChange();
		}
		return mMoved || mChanged;
	}

	public bool UpdateGeometry(int frame)
	{
		float num = CalculateFinalAlpha(frame);
		if (mIsVisibleByAlpha && mLastAlpha != num)
		{
			mChanged = true;
		}
		mLastAlpha = num;
		if (mChanged)
		{
			mChanged = false;
			if (mIsVisibleByAlpha && num > 0.001f && shader != null)
			{
				bool hasVertices = geometry.hasVertices;
				if (fillGeometry)
				{
					geometry.Clear();
					OnFill(geometry.verts, geometry.uvs, geometry.cols);
				}
				if (geometry.hasVertices)
				{
					if (mMatrixFrame != frame)
					{
						mLocalToPanel = panel.worldToLocal * base.cachedTransform.localToWorldMatrix;
						mMatrixFrame = frame;
					}
					geometry.ApplyTransform(mLocalToPanel);
					mMoved = false;
					return true;
				}
				return hasVertices;
			}
			if (geometry.hasVertices)
			{
				if (fillGeometry)
				{
					geometry.Clear();
				}
				mMoved = false;
				return true;
			}
		}
		else if (mMoved && geometry.hasVertices)
		{
			if (mMatrixFrame != frame)
			{
				mLocalToPanel = panel.worldToLocal * base.cachedTransform.localToWorldMatrix;
				mMatrixFrame = frame;
			}
			geometry.ApplyTransform(mLocalToPanel);
			mMoved = false;
			return true;
		}
		mMoved = false;
		return false;
	}

	public void WriteToBuffers(BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
	{
		geometry.WriteToBuffers(v, u, c, n, t);
	}

	public virtual void MakePixelPerfect()
	{
		Vector3 localPosition = base.cachedTransform.localPosition;
		localPosition.z = Mathf.Round(localPosition.z);
		localPosition.x = Mathf.Round(localPosition.x);
		localPosition.y = Mathf.Round(localPosition.y);
		base.cachedTransform.localPosition = localPosition;
		Vector3 localScale = base.cachedTransform.localScale;
		base.cachedTransform.localScale = new Vector3(Mathf.Sign(localScale.x), Mathf.Sign(localScale.y), 1f);
	}

	public virtual void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
	}
}
