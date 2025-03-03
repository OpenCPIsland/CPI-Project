using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Wrap Content")]
public class UIWrapContent : MonoBehaviour
{
	public int itemSize = 100;

	public bool cullContent = true;

	private Transform mTrans;

	private UIPanel mPanel;

	private UIScrollView mScroll;

	private bool mHorizontal = false;

	private BetterList<Transform> mChildren = new BetterList<Transform>();

	protected virtual void Start()
	{
		SortBasedOnScrollMovement();
		WrapContent();
		if (mScroll != null)
		{
			mScroll.GetComponent<UIPanel>().onClipMove = OnMove;
			mScroll.restrictWithinPanel = false;
			if (mScroll.dragEffect == UIScrollView.DragEffect.MomentumAndSpring)
			{
				mScroll.dragEffect = UIScrollView.DragEffect.Momentum;
			}
		}
	}

	protected virtual void OnMove(UIPanel panel)
	{
		WrapContent();
	}

	[ContextMenu("Sort Based on Scroll Movement")]
	public void SortBasedOnScrollMovement()
	{
		if (CacheScrollView())
		{
			mChildren.Clear();
			for (int i = 0; i < mTrans.childCount; i++)
			{
				mChildren.Add(mTrans.GetChild(i));
			}
			if (mHorizontal)
			{
				mChildren.Sort(UIGrid.SortHorizontal);
			}
			else
			{
				mChildren.Sort(UIGrid.SortVertical);
			}
			ResetChildPositions();
		}
	}

	[ContextMenu("Sort Alphabetically")]
	public void SortAlphabetically()
	{
		if (CacheScrollView())
		{
			mChildren.Clear();
			for (int i = 0; i < mTrans.childCount; i++)
			{
				mChildren.Add(mTrans.GetChild(i));
			}
			mChildren.Sort(UIGrid.SortByName);
			ResetChildPositions();
		}
	}

	protected bool CacheScrollView()
	{
		mTrans = base.transform;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		mScroll = mPanel.GetComponent<UIScrollView>();
		if (mScroll == null)
		{
			return false;
		}
		if (mScroll.movement == UIScrollView.Movement.Horizontal)
		{
			mHorizontal = true;
		}
		else
		{
			if (mScroll.movement != UIScrollView.Movement.Vertical)
			{
				return false;
			}
			mHorizontal = false;
		}
		return true;
	}

	private void ResetChildPositions()
	{
		for (int i = 0; i < mChildren.size; i++)
		{
			Transform transform = mChildren[i];
			transform.localPosition = (mHorizontal ? new Vector3(i * itemSize, 0f, 0f) : new Vector3(0f, -i * itemSize, 0f));
		}
	}

	public void WrapContent()
	{
		float num = (float)(itemSize * mChildren.size) * 0.5f;
		Vector3[] worldCorners = mPanel.worldCorners;
		for (int i = 0; i < 4; i++)
		{
			Vector3 position = worldCorners[i];
			position = mTrans.InverseTransformPoint(position);
			worldCorners[i] = position;
		}
		Vector3 vector = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
		float num2;
		float num3;
		if (mHorizontal)
		{
			num2 = worldCorners[0].x - (float)itemSize;
			num3 = worldCorners[2].x + (float)itemSize;
			for (int i = 0; i < mChildren.size; i++)
			{
				Transform transform = mChildren[i];
				float num4 = transform.localPosition.x - vector.x;
				if (num4 < 0f - num)
				{
					transform.localPosition += new Vector3(num * 2f, 0f, 0f);
					num4 = transform.localPosition.x - vector.x;
					UpdateItem(transform, i);
				}
				else if (num4 > num)
				{
					transform.localPosition -= new Vector3(num * 2f, 0f, 0f);
					num4 = transform.localPosition.x - vector.x;
					UpdateItem(transform, i);
				}
				if (cullContent)
				{
					num4 += mPanel.clipOffset.x - mTrans.localPosition.x;
					if (!UICamera.IsPressed(transform.gameObject))
					{
						NGUITools.SetActive(transform.gameObject, num4 > num2 && num4 < num3, false);
					}
				}
			}
			return;
		}
		num2 = worldCorners[0].y - (float)itemSize;
		num3 = worldCorners[2].y + (float)itemSize;
		for (int i = 0; i < mChildren.size; i++)
		{
			Transform transform = mChildren[i];
			float num4 = transform.localPosition.y - vector.y;
			if (num4 < 0f - num)
			{
				transform.localPosition += new Vector3(0f, num * 2f, 0f);
				num4 = transform.localPosition.y - vector.y;
				UpdateItem(transform, i);
			}
			else if (num4 > num)
			{
				transform.localPosition -= new Vector3(0f, num * 2f, 0f);
				num4 = transform.localPosition.y - vector.y;
				UpdateItem(transform, i);
			}
			if (cullContent)
			{
				num4 += mPanel.clipOffset.y - mTrans.localPosition.y;
				if (!UICamera.IsPressed(transform.gameObject))
				{
					NGUITools.SetActive(transform.gameObject, num4 > num2 && num4 < num3, false);
				}
			}
		}
	}

	protected virtual void UpdateItem(Transform item, int index)
	{
	}
}
