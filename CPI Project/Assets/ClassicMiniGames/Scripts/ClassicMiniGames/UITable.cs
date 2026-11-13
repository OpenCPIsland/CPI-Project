using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Table")]
public class UITable : UIWidgetContainer
{
	public delegate void OnReposition();

	public enum Direction
	{
		Down,
		Up
	}

	public enum Sorting
	{
		None,
		Alphabetic,
		Horizontal,
		Vertical,
		Custom
	}

	public int columns = 0;

	public Direction direction = Direction.Down;

	public Sorting sorting = Sorting.None;

	public bool hideInactive = true;

	public bool keepWithinPanel = false;

	public Vector2 padding = Vector2.zero;

	public OnReposition onReposition;

	protected UIPanel mPanel;

	protected bool mInitDone = false;

	protected bool mReposition = false;

	protected List<Transform> mChildren = new List<Transform>();

	[HideInInspector]
	[SerializeField]
	private bool sorted = false;

	public bool repositionNow
	{
		set
		{
			if (value)
			{
				mReposition = true;
				base.enabled = true;
			}
		}
	}

	public List<Transform> children
	{
		get
		{
			if (mChildren.Count == 0)
			{
				Transform transform = base.transform;
				mChildren.Clear();
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if ((bool)child && (bool)child.gameObject && (!hideInactive || NGUITools.GetActive(child.gameObject)))
					{
						mChildren.Add(child);
					}
				}
				if (sorting != 0 || sorted)
				{
					if (sorting == Sorting.Alphabetic)
					{
						mChildren.Sort(UIGrid.SortByName);
					}
					else if (sorting == Sorting.Horizontal)
					{
						mChildren.Sort(UIGrid.SortHorizontal);
					}
					else if (sorting == Sorting.Vertical)
					{
						mChildren.Sort(UIGrid.SortVertical);
					}
					else
					{
						Sort(mChildren);
					}
				}
			}
			return mChildren;
		}
	}

	protected virtual void Sort(List<Transform> list)
	{
		list.Sort(UIGrid.SortByName);
	}

	protected void RepositionVariableSize(List<Transform> children)
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = (columns <= 0) ? 1 : (children.Count / columns + 1);
		int num4 = (columns > 0) ? columns : children.Count;
		Bounds[,] array = new Bounds[num3, num4];
		Bounds[] array2 = new Bounds[num4];
		Bounds[] array3 = new Bounds[num3];
		int num5 = 0;
		int num6 = 0;
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Transform transform = children[i];
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform, !hideInactive);
			Vector3 localScale = transform.localScale;
			bounds.min = Vector3.Scale(bounds.min, localScale);
			bounds.max = Vector3.Scale(bounds.max, localScale);
			array[num6, num5] = bounds;
			array2[num5].Encapsulate(bounds);
			array3[num6].Encapsulate(bounds);
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
			}
		}
		num5 = 0;
		num6 = 0;
		i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Transform transform = children[i];
			Bounds bounds = array[num6, num5];
			Bounds bounds2 = array2[num5];
			Bounds bounds3 = array3[num6];
			Vector3 localPosition = transform.localPosition;
			localPosition.x = num + bounds.extents.x - bounds.center.x;
			localPosition.x += bounds.min.x - bounds2.min.x + padding.x;
			if (direction == Direction.Down)
			{
				localPosition.y = 0f - num2 - bounds.extents.y - bounds.center.y;
				localPosition.y += (bounds.max.y - bounds.min.y - bounds3.max.y + bounds3.min.y) * 0.5f - padding.y;
			}
			else
			{
				localPosition.y = num2 + (bounds.extents.y - bounds.center.y);
				localPosition.y -= (bounds.max.y - bounds.min.y - bounds3.max.y + bounds3.min.y) * 0.5f - padding.y;
			}
			num += bounds2.max.x - bounds2.min.x + padding.x * 2f;
			transform.localPosition = localPosition;
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
				num = 0f;
				num2 += bounds3.size.y + padding.y * 2f;
			}
		}
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			mReposition = true;
			return;
		}
		if (!mInitDone)
		{
			Init();
		}
		mReposition = false;
		Transform transform = base.transform;
		mChildren.Clear();
		List<Transform> children = this.children;
		if (children.Count > 0)
		{
			RepositionVariableSize(children);
		}
		if (keepWithinPanel && mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(transform, true);
			UIScrollView component = mPanel.GetComponent<UIScrollView>();
			if (component != null)
			{
				component.UpdateScrollbars(true);
			}
		}
		if (onReposition != null)
		{
			onReposition();
		}
	}

	protected virtual void Start()
	{
		Init();
		Reposition();
		base.enabled = false;
	}

	protected virtual void Init()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	protected virtual void LateUpdate()
	{
		if (mReposition)
		{
			Reposition();
		}
		base.enabled = false;
	}
}
