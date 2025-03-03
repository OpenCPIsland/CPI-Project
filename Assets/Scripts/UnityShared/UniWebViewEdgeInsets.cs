using System;

[Serializable]
public class UniWebViewEdgeInsets
{
	public int top;

	public int left;

	public int bottom;

	public int right;

	public UniWebViewEdgeInsets(int aTop, int aLeft, int aBottom, int aRight)
	{
		top = aTop;
		left = aLeft;
		bottom = aBottom;
		right = aRight;
	}

	public static bool operator ==(UniWebViewEdgeInsets inset1, UniWebViewEdgeInsets inset2)
	{
		return inset1.Equals(inset2);
	}

	public static bool operator !=(UniWebViewEdgeInsets inset1, UniWebViewEdgeInsets inset2)
	{
		return !inset1.Equals(inset2);
	}

	public override int GetHashCode()
	{
		return (top + left + bottom + right).GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		UniWebViewEdgeInsets uniWebViewEdgeInsets = (UniWebViewEdgeInsets)obj;
		return top == uniWebViewEdgeInsets.top && left == uniWebViewEdgeInsets.left && bottom == uniWebViewEdgeInsets.bottom && right == uniWebViewEdgeInsets.right;
	}
}
