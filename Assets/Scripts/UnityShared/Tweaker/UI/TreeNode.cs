using System.Collections.Generic;

namespace Tweaker.UI
{
	public class TreeNode<TValue> where TValue : TreeNode<TValue>
	{
		private TValue parent;

		public TValue Root
		{
			get;
			private set;
		}

		public TreeNodeList<TValue> Children
		{
			get;
			private set;
		}

		public TValue Parent
		{
			get
			{
				return (parent == null) ? null : parent.Value;
			}
			set
			{
				if (parent != value)
				{
					if (parent != null)
					{
						parent.Children.Remove(Value);
					}
					if (value != null && !value.Children.Contains(Value))
					{
						value.Children.Add(Value);
					}
					parent = value;
					UpdateRoot();
				}
			}
		}

		public TValue Value
		{
			get
			{
				return (TValue)this;
			}
		}

		public uint Depth
		{
			get;
			private set;
		}

		public TreeNode()
			: this((TValue)null)
		{
		}

		public TreeNode(TValue parent)
		{
			Root = Value;
			Parent = parent;
			Children = new TreeNodeList<TValue>(Value);
		}

		private void UpdateRoot()
		{
			uint num = 0u;
			TValue value = Value;
			while (value.Parent != null)
			{
				num++;
				value = value.Parent;
			}
			Root = value;
			Depth = num;
		}

		public IEnumerable<TreeNode<TValue>> TraverseBreadthFirst()
		{
			foreach (TValue child2 in Children)
			{
				yield return child2;
			}
			foreach (TValue child in Children)
			{
				TValue val = child;
				foreach (TreeNode<TValue> item in val.TraverseBreadthFirst())
				{
					yield return item;
				}
			}
		}

		public IEnumerable<TreeNode<TValue>> TraverseDepthFirst()
		{
			foreach (TValue child in Children)
			{
				yield return child;
				TValue val = child;
				foreach (TreeNode<TValue> item in val.TraverseDepthFirst())
				{
					yield return item;
				}
			}
		}

		public IEnumerable<TreeNode<TValue>> GetBranchNodes()
		{
			foreach (TreeNode<TValue> node in TraverseBreadthFirst())
			{
				if (node.Children.Count > 0)
				{
					yield return node;
				}
			}
		}

		public IEnumerable<TreeNode<TValue>> GetLeafNodes()
		{
			foreach (TreeNode<TValue> node in TraverseBreadthFirst())
			{
				if (node.Children.Count == 0)
				{
					yield return node;
				}
			}
		}
	}
}
