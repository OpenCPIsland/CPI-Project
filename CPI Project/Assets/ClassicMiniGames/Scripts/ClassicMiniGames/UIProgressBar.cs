using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/NGUI Progress Bar")]
public class UIProgressBar : UIWidgetContainer
{
	public enum FillDirection
	{
		LeftToRight,
		RightToLeft,
		BottomToTop,
		TopToBottom
	}

	public delegate void OnDragFinished();

	public static UIProgressBar current;

	public OnDragFinished onDragFinished;

	public Transform thumb;

	[SerializeField]
	[HideInInspector]
	protected UIWidget mBG;

	[SerializeField]
	[HideInInspector]
	protected UIWidget mFG;

	[SerializeField]
	[HideInInspector]
	protected float mValue = 1f;

	[HideInInspector]
	[SerializeField]
	protected FillDirection mFill = FillDirection.LeftToRight;

	protected Transform mTrans;

	protected bool mIsDirty = false;

	protected Camera mCam;

	protected float mOffset = 0f;

	public int numberOfSteps = 0;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (mCam == null)
			{
				mCam = NGUITools.FindCameraForLayer(base.gameObject.layer);
			}
			return mCam;
		}
	}

	public UIWidget foregroundWidget
	{
		get
		{
			return mFG;
		}
		set
		{
			if (mFG != value)
			{
				mFG = value;
				mIsDirty = true;
			}
		}
	}

	public UIWidget backgroundWidget
	{
		get
		{
			return mBG;
		}
		set
		{
			if (mBG != value)
			{
				mBG = value;
				mIsDirty = true;
			}
		}
	}

	public FillDirection fillDirection
	{
		get
		{
			return mFill;
		}
		set
		{
			if (mFill != value)
			{
				mFill = value;
				ForceUpdate();
			}
		}
	}

	public float value
	{
		get
		{
			if (numberOfSteps > 1)
			{
				return Mathf.Round(mValue * (float)(numberOfSteps - 1)) / (float)(numberOfSteps - 1);
			}
			return mValue;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mValue == num)
			{
				return;
			}
			float value2 = this.value;
			mValue = num;
			if (value2 != this.value)
			{
				ForceUpdate();
				if (current == null && NGUITools.GetActive(this) && EventDelegate.IsValid(onChange))
				{
					current = this;
					EventDelegate.Execute(onChange);
					current = null;
				}
			}
		}
	}

	public float alpha
	{
		get
		{
			if (mFG != null)
			{
				return mFG.alpha;
			}
			if (mBG != null)
			{
				return mBG.alpha;
			}
			return 1f;
		}
		set
		{
			if (mFG != null)
			{
				mFG.alpha = value;
				if (mFG.GetComponent<Collider>() != null)
				{
					mFG.GetComponent<Collider>().enabled = (mFG.alpha > 0.001f);
				}
			}
			if (mBG != null)
			{
				mBG.alpha = value;
				if (mBG.GetComponent<Collider>() != null)
				{
					mBG.GetComponent<Collider>().enabled = (mBG.alpha > 0.001f);
				}
			}
			if (!(thumb != null))
			{
				return;
			}
			UIWidget component = thumb.GetComponent<UIWidget>();
			if (component != null)
			{
				component.alpha = value;
				if (component.GetComponent<Collider>() != null)
				{
					component.GetComponent<Collider>().enabled = (component.alpha > 0.001f);
				}
			}
		}
	}

	protected bool isHorizontal
	{
		get
		{
			return mFill == FillDirection.LeftToRight || mFill == FillDirection.RightToLeft;
		}
	}

	protected bool isInverted
	{
		get
		{
			return mFill == FillDirection.RightToLeft || mFill == FillDirection.TopToBottom;
		}
	}

	protected void Start()
	{
		Upgrade();
		if (Application.isPlaying)
		{
			if (mBG != null)
			{
				mBG.autoResizeBoxCollider = true;
			}
			OnStart();
			if (current == null && onChange != null)
			{
				current = this;
				EventDelegate.Execute(onChange);
				current = null;
			}
		}
		ForceUpdate();
	}

	protected virtual void Upgrade()
	{
	}

	protected virtual void OnStart()
	{
	}

	protected void Update()
	{
		if (mIsDirty)
		{
			ForceUpdate();
		}
	}

	protected void OnValidate()
	{
		if (NGUITools.GetActive(this))
		{
			Upgrade();
			mIsDirty = true;
			float num = Mathf.Clamp01(mValue);
			if (mValue != num)
			{
				mValue = num;
			}
			if (numberOfSteps < 0)
			{
				numberOfSteps = 0;
			}
			else if (numberOfSteps > 20)
			{
				numberOfSteps = 20;
			}
			ForceUpdate();
		}
		else
		{
			float num = Mathf.Clamp01(mValue);
			if (mValue != num)
			{
				mValue = num;
			}
			if (numberOfSteps < 0)
			{
				numberOfSteps = 0;
			}
			else if (numberOfSteps > 20)
			{
				numberOfSteps = 20;
			}
		}
	}

	protected float ScreenToValue(Vector2 screenPos)
	{
		Transform cachedTransform = this.cachedTransform;
		Plane plane = new Plane(cachedTransform.rotation * Vector3.back, cachedTransform.position);
		Ray ray = cachedCamera.ScreenPointToRay(screenPos);
		float enter;
		if (!plane.Raycast(ray, out enter))
		{
			return value;
		}
		return LocalToValue(cachedTransform.InverseTransformPoint(ray.GetPoint(enter)));
	}

	protected virtual float LocalToValue(Vector2 localPos)
	{
		if (mFG != null)
		{
			Vector3[] localCorners = mFG.localCorners;
			Vector3 vector = localCorners[2] - localCorners[0];
			float num;
			if (isHorizontal)
			{
				num = (localPos.x - localCorners[0].x) / vector.x;
				return isInverted ? (1f - num) : num;
			}
			num = (localPos.y - localCorners[0].y) / vector.y;
			return isInverted ? (1f - num) : num;
		}
		return value;
	}

	public virtual void ForceUpdate()
	{
		mIsDirty = false;
		if (mFG != null)
		{
			UIBasicSprite uIBasicSprite = mFG as UIBasicSprite;
			if (isHorizontal)
			{
				if (uIBasicSprite != null && uIBasicSprite.type == UIBasicSprite.Type.Filled)
				{
					uIBasicSprite.fillDirection = UIBasicSprite.FillDirection.Horizontal;
					uIBasicSprite.invert = isInverted;
					uIBasicSprite.fillAmount = value;
				}
				else
				{
					mFG.drawRegion = (isInverted ? new Vector4(1f - value, 0f, 1f, 1f) : new Vector4(0f, 0f, value, 1f));
				}
			}
			else if (uIBasicSprite != null && uIBasicSprite.type == UIBasicSprite.Type.Filled)
			{
				uIBasicSprite.fillDirection = UIBasicSprite.FillDirection.Vertical;
				uIBasicSprite.invert = isInverted;
				uIBasicSprite.fillAmount = value;
			}
			else
			{
				mFG.drawRegion = (isInverted ? new Vector4(0f, 1f - value, 1f, 1f) : new Vector4(0f, 0f, 1f, value));
			}
		}
		if (thumb != null && (mFG != null || mBG != null))
		{
			Vector3[] array = (mFG != null) ? mFG.localCorners : mBG.localCorners;
			Vector4 vector = (mFG != null) ? mFG.border : mBG.border;
			array[0].x += vector.x;
			array[1].x += vector.x;
			array[2].x -= vector.z;
			array[3].x -= vector.z;
			array[0].y += vector.y;
			array[1].y -= vector.w;
			array[2].y -= vector.w;
			array[3].y += vector.y;
			Transform transform = (mFG != null) ? mFG.cachedTransform : mBG.cachedTransform;
			for (int i = 0; i < 4; i++)
			{
				array[i] = transform.TransformPoint(array[i]);
			}
			if (isHorizontal)
			{
				Vector3 a = Vector3.Lerp(array[0], array[1], 0.5f);
				Vector3 b = Vector3.Lerp(array[2], array[3], 0.5f);
				SetThumbPosition(Vector3.Lerp(a, b, isInverted ? (1f - value) : value));
			}
			else
			{
				Vector3 a = Vector3.Lerp(array[0], array[3], 0.5f);
				Vector3 b = Vector3.Lerp(array[1], array[2], 0.5f);
				SetThumbPosition(Vector3.Lerp(a, b, isInverted ? (1f - value) : value));
			}
		}
	}

	protected void SetThumbPosition(Vector3 worldPos)
	{
		Transform parent = thumb.parent;
		if (parent != null)
		{
			worldPos = parent.InverseTransformPoint(worldPos);
			worldPos.x = Mathf.Round(worldPos.x);
			worldPos.y = Mathf.Round(worldPos.y);
			worldPos.z = 0f;
			if (Vector3.Distance(thumb.localPosition, worldPos) > 0.001f)
			{
				thumb.localPosition = worldPos;
			}
		}
		else if (Vector3.Distance(thumb.position, worldPos) > 1E-05f)
		{
			thumb.position = worldPos;
		}
	}
}
