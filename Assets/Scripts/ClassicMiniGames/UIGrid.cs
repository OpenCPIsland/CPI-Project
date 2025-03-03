using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : UIWidgetContainer
{
	public delegate void OnReposition();

	public enum Arrangement
	{
		Horizontal,
		Vertical
	}

	public enum Sorting
	{
		None,
		Alphabetic,
		Horizontal,
		Vertical,
		Custom
	}

	public Arrangement arrangement = Arrangement.Horizontal;

	public Sorting sorting = Sorting.None;

	public UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft;

	public int maxPerLine = 0;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public bool animateSmoothly = false;

	public bool hideInactive = true;

	public bool keepWithinPanel = false;

	public OnReposition onReposition;

	public BetterList<Transform>.CompareFunc onCustomSort;

	[HideInInspector]
	[SerializeField]
	private bool sorted = false;

	protected bool mReposition = false;

	protected UIPanel mPanel;

	protected bool mInitDone = false;

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

	public BetterList<Transform> GetChildList()
	{
		Transform transform = base.transform;
		BetterList<Transform> betterList = new BetterList<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!hideInactive || ((bool)child && NGUITools.GetActive(child.gameObject)))
			{
				betterList.Add(child);
			}
		}
		if (sorting != 0)
		{
			if (sorting == Sorting.Alphabetic)
			{
				betterList.Sort(SortByName);
			}
			else if (sorting == Sorting.Horizontal)
			{
				betterList.Sort(SortHorizontal);
			}
			else if (sorting == Sorting.Vertical)
			{
				betterList.Sort(SortVertical);
			}
			else if (onCustomSort != null)
			{
				betterList.Sort(onCustomSort);
			}
			else
			{
				Sort(betterList);
			}
		}
		return betterList;
	}

	public Transform GetChild(int index)
	{
		BetterList<Transform> childList = GetChildList();
		return (index < childList.size) ? childList[index] : null;
	}

	public int GetIndex(Transform trans)
	{
		return GetChildList().IndexOf(trans);
	}

	public void AddChild(Transform trans)
	{
		AddChild(trans, true);
	}

	public void AddChild(Transform trans, bool sort)
	{
		if (trans != null)
		{
			BetterList<Transform> childList = GetChildList();
			childList.Add(trans);
			ResetPosition(childList);
		}
	}

	public void AddChild(Transform trans, int index)
	{
		if (trans != null)
		{
			if (sorting != 0)
			{
				Debug.LogWarning("The Grid has sorting enabled, so AddChild at index may not work as expected.", this);
			}
			BetterList<Transform> childList = GetChildList();
			childList.Insert(index, trans);
			ResetPosition(childList);
		}
	}

	public Transform RemoveChild(int index)
	{
		BetterList<Transform> childList = GetChildList();
		if (index < childList.size)
		{
			Transform result = childList[index];
			childList.RemoveAt(index);
			ResetPosition(childList);
			return result;
		}
		return null;
	}

	public bool RemoveChild(Transform t)
	{
		BetterList<Transform> childList = GetChildList();
		if (childList.Remove(t))
		{
			ResetPosition(childList);
			return true;
		}
		return false;
	}

	protected virtual void Init()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	protected virtual void Start()
	{
		if (!mInitDone)
		{
			Init();
		}
		bool flag = animateSmoothly;
		animateSmoothly = false;
		Reposition();
		animateSmoothly = flag;
		base.enabled = false;
	}

	protected virtual void Update()
	{
		if (mReposition)
		{
			Reposition();
		}
		base.enabled = false;
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	public static int SortHorizontal(Transform a, Transform b)
	{
		return a.localPosition.x.CompareTo(b.localPosition.x);
	}

	public static int SortVertical(Transform a, Transform b)
	{
		return b.localPosition.y.CompareTo(a.localPosition.y);
	}

	protected virtual void Sort(BetterList<Transform> list)
	{
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			mReposition = true;
			return;
		}
		if (sorted)
		{
			sorted = false;
			if (sorting == Sorting.None)
			{
				sorting = Sorting.Alphabetic;
			}
			NGUITools.SetDirty(this);
		}
		if (!mInitDone)
		{
			Init();
		}
		BetterList<Transform> childList = GetChildList();
		ResetPosition(childList);
		if (keepWithinPanel)
		{
			ConstrainWithinPanel();
		}
		if (onReposition != null)
		{
			onReposition();
		}
	}

	public void ConstrainWithinPanel()
	{
		if (mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(base.transform, true);
		}
	}

	protected void ResetPosition(BetterList<Transform> list)
	{
		mReposition = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		Transform transform = base.transform;
		int i = 0;
		Vector3 vector;
		for (int size = list.size; i < size; i++)
		{
			Transform transform2 = list[i];
			float z = transform2.localPosition.z;
			vector = ((arrangement == Arrangement.Horizontal) ? new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, z) : new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, z));
			if (animateSmoothly && Application.isPlaying)
			{
				SpringPosition.Begin(transform2.gameObject, vector, 15f).updateScrollView = true;
			}
			else
			{
				transform2.localPosition = vector;
			}
			num3 = Mathf.Max(num3, num);
			num4 = Mathf.Max(num4, num2);
			if (++num >= maxPerLine && maxPerLine > 0)
			{
				num = 0;
				num2++;
			}
		}
		if (pivot == UIWidget.Pivot.TopLeft)
		{
			return;
		}
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(pivot);
		float num5;
		float num6;
		if (arrangement == Arrangement.Horizontal)
		{
			num5 = Mathf.Lerp(0f, (float)num3 * cellWidth, pivotOffset.x);
			num6 = Mathf.Lerp((float)(-num4) * cellHeight, 0f, pivotOffset.y);
		}
		else
		{
			num5 = Mathf.Lerp(0f, (float)num4 * cellWidth, pivotOffset.x);
			num6 = Mathf.Lerp((float)(-num3) * cellHeight, 0f, pivotOffset.y);
		}
		for (i = 0; i < transform.childCount; i++)
		{
			Transform transform2 = transform.GetChild(i);
			SpringPosition component = transform2.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.target.x -= num5;
				component.target.y -= num6;
				continue;
			}
			vector = transform2.localPosition;
			vector.x -= num5;
			vector.y -= num6;
			transform2.localPosition = vector;
		}
	}
}
