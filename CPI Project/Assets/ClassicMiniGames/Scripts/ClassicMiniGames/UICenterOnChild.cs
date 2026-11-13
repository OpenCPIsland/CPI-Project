using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Center Scroll View on Child")]
public class UICenterOnChild : MonoBehaviour
{
	public float springStrength = 8f;

	public float nextPageThreshold = 0f;

	public SpringPanel.OnFinished onFinished;

	private UIScrollView mScrollView;

	private GameObject mCenteredObject;

	public GameObject centeredObject
	{
		get
		{
			return mCenteredObject;
		}
	}

	private void OnEnable()
	{
		Recenter();
	}

	private void OnDragFinished()
	{
		if (base.enabled)
		{
			Recenter();
		}
	}

	private void OnValidate()
	{
		nextPageThreshold = Mathf.Abs(nextPageThreshold);
	}

	public void Recenter()
	{
		Transform transform = base.transform;
		if (transform.childCount == 0)
		{
			return;
		}
		if (mScrollView == null)
		{
			mScrollView = NGUITools.FindInParents<UIScrollView>(base.gameObject);
			if (mScrollView == null)
			{
				Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIScrollView), " on a parent object in order to work"), this);
				base.enabled = false;
				return;
			}
			mScrollView.onDragFinished = OnDragFinished;
			if (mScrollView.horizontalScrollBar != null)
			{
				mScrollView.horizontalScrollBar.onDragFinished = OnDragFinished;
			}
			if (mScrollView.verticalScrollBar != null)
			{
				mScrollView.verticalScrollBar.onDragFinished = OnDragFinished;
			}
		}
		if (mScrollView.panel == null)
		{
			return;
		}
		Vector3[] worldCorners = mScrollView.panel.worldCorners;
		Vector3 vector = (worldCorners[2] + worldCorners[0]) * 0.5f;
		Vector3 b = vector - mScrollView.currentMomentum * (mScrollView.momentumAmount * 0.1f);
		mScrollView.currentMomentum = Vector3.zero;
		float num = float.MaxValue;
		Transform target = null;
		int num2 = 0;
		int i = 0;
		for (int childCount = transform.childCount; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			float num3 = Vector3.SqrMagnitude(child.position - b);
			if (num3 < num)
			{
				num = num3;
				target = child;
				num2 = i;
			}
		}
		if (nextPageThreshold > 0f && UICamera.currentTouch != null && mCenteredObject != null && mCenteredObject.transform == transform.GetChild(num2))
		{
			Vector2 totalDelta = UICamera.currentTouch.totalDelta;
			float num4 = 0f;
			switch (mScrollView.movement)
			{
			case UIScrollView.Movement.Horizontal:
				num4 = totalDelta.x;
				break;
			case UIScrollView.Movement.Vertical:
				num4 = totalDelta.y;
				break;
			default:
				num4 = totalDelta.magnitude;
				break;
			}
			if (num4 > nextPageThreshold)
			{
				if (num2 > 0)
				{
					target = transform.GetChild(num2 - 1);
				}
			}
			else if (num4 < 0f - nextPageThreshold && num2 < transform.childCount - 1)
			{
				target = transform.GetChild(num2 + 1);
			}
		}
		CenterOn(target, vector);
	}

	private void CenterOn(Transform target, Vector3 panelCenter)
	{
		if (target != null && mScrollView != null && mScrollView.panel != null)
		{
			Transform cachedTransform = mScrollView.panel.cachedTransform;
			mCenteredObject = target.gameObject;
			Vector3 a = cachedTransform.InverseTransformPoint(target.position);
			Vector3 b = cachedTransform.InverseTransformPoint(panelCenter);
			Vector3 b2 = a - b;
			if (!mScrollView.canMoveHorizontally)
			{
				b2.x = 0f;
			}
			if (!mScrollView.canMoveVertically)
			{
				b2.y = 0f;
			}
			b2.z = 0f;
			SpringPanel.Begin(mScrollView.panel.cachedGameObject, cachedTransform.localPosition - b2, springStrength).onFinished = onFinished;
		}
		else
		{
			mCenteredObject = null;
		}
	}

	public void CenterOn(Transform target)
	{
		if (mScrollView != null && mScrollView.panel != null)
		{
			Vector3[] worldCorners = mScrollView.panel.worldCorners;
			Vector3 panelCenter = (worldCorners[2] + worldCorners[0]) * 0.5f;
			CenterOn(target, panelCenter);
		}
	}
}
