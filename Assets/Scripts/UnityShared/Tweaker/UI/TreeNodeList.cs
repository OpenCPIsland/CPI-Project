using System.Collections.Generic;

namespace Tweaker.UI
{
	public class TreeNodeList<TValue> : List<TValue> where TValue : TreeNode<TValue>
	{
		public TValue Parent
		{
			get;
			private set;
		}

		public TreeNodeList(TValue parent)
		{
			Parent = parent;
		}

		public new TValue Add(TValue node)
		{
			base.Add(node);
			node.Parent = Parent;
			return node;
		}
	}
}
