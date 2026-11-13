using System;
using System.Collections;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems.Utility.Collections
{
	public class AvlTree<TKey, TValue> : IEnumerable<TValue>, IEnumerable
	{
		private class AvlNode
		{
			public AvlNode Parent
			{
				get;
				set;
			}

			public AvlNode Left
			{
				get;
				set;
			}

			public AvlNode Right
			{
				get;
				set;
			}

			public TKey Key
			{
				get;
				set;
			}

			public TValue Value
			{
				get;
				set;
			}

			public int Balance
			{
				get;
				set;
			}
		}

		private class AvlNodeEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
		{
			private enum Action
			{
				Parent,
				Right,
				End
			}

			private readonly AvlNode _root;

			private Action _action;

			private AvlNode _current;

			private AvlNode _right;

			public TValue Current
			{
				get
				{
					return _current.Value;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public AvlNodeEnumerator(AvlNode root)
			{
				_right = (_root = root);
				_action = ((_root != null) ? Action.Right : Action.End);
			}

			public bool MoveNext()
			{
				switch (_action)
				{
				case Action.Right:
					_current = _right;
					while (_current.Left != null)
					{
						_current = _current.Left;
					}
					_right = _current.Right;
					_action = ((_right != null) ? Action.Right : Action.Parent);
					return true;
				case Action.Parent:
					while (_current.Parent != null)
					{
						AvlNode current = _current;
						_current = _current.Parent;
						if (_current.Left == current)
						{
							_right = _current.Right;
							_action = ((_right != null) ? Action.Right : Action.Parent);
							return true;
						}
					}
					_action = Action.End;
					return false;
				default:
					return false;
				}
			}

			public void Reset()
			{
				_right = _root;
				_action = ((_root != null) ? Action.Right : Action.End);
			}

			public void Dispose()
			{
			}
		}

		private readonly IComparer<TKey> _comparer;

		private AvlNode _root;

		public AvlTree(IComparer<TKey> comparer)
		{
			_comparer = comparer;
		}

		public AvlTree()
			: this((IComparer<TKey>)Comparer<TKey>.Default)
		{
		}

		public void Insert(TKey key, TValue value)
		{
			if (_root == null)
			{
				_root = new AvlNode
				{
					Key = key,
					Value = value
				};
				return;
			}
			AvlNode avlNode = _root;
			while (true)
			{
				if (avlNode == null)
				{
					return;
				}
				int num = _comparer.Compare(key, avlNode.Key);
				if (num < 0)
				{
					AvlNode left = avlNode.Left;
					if (left == null)
					{
						avlNode.Left = new AvlNode
						{
							Key = key,
							Value = value,
							Parent = avlNode
						};
						BalanceAfterInsertion(avlNode, 1);
						return;
					}
					avlNode = left;
					continue;
				}
				if (num <= 0)
				{
					break;
				}
				AvlNode right = avlNode.Right;
				if (right == null)
				{
					avlNode.Right = new AvlNode
					{
						Key = key,
						Value = value,
						Parent = avlNode
					};
					BalanceAfterInsertion(avlNode, -1);
					return;
				}
				avlNode = right;
			}
			avlNode.Value = value;
		}

		public bool Delete(TKey key)
		{
			AvlNode avlNode = _root;
			while (avlNode != null)
			{
				if (_comparer.Compare(key, avlNode.Key) < 0)
				{
					avlNode = avlNode.Left;
					continue;
				}
				if (_comparer.Compare(key, avlNode.Key) > 0)
				{
					avlNode = avlNode.Right;
					continue;
				}
				AvlNode left = avlNode.Left;
				AvlNode right = avlNode.Right;
				if (left == null)
				{
					if (right == null)
					{
						if (avlNode == _root)
						{
							_root = null;
						}
						else
						{
							AvlNode parent = avlNode.Parent;
							if (parent.Left == avlNode)
							{
								parent.Left = null;
								BalanceAfterDeletion(parent, -1);
							}
							else
							{
								parent.Right = null;
								BalanceAfterDeletion(parent, 1);
							}
						}
					}
					else
					{
						Replace(avlNode, right);
						BalanceAfterDeletion(avlNode, 0);
					}
				}
				else if (right == null)
				{
					Replace(avlNode, left);
					BalanceAfterDeletion(avlNode, 0);
				}
				else
				{
					AvlNode avlNode2 = right;
					if (avlNode2.Left == null)
					{
						AvlNode parent = avlNode2.Parent = avlNode.Parent;
						avlNode2.Left = left;
						avlNode2.Balance = avlNode.Balance;
						if (left != null)
						{
							left.Parent = avlNode2;
						}
						if (avlNode == _root)
						{
							_root = avlNode2;
						}
						else if (parent.Left == avlNode)
						{
							parent.Left = avlNode2;
						}
						else
						{
							parent.Right = avlNode2;
						}
						BalanceAfterDeletion(avlNode2, 1);
					}
					else
					{
						while (avlNode2.Left != null)
						{
							avlNode2 = avlNode2.Left;
						}
						AvlNode parent = avlNode.Parent;
						AvlNode parent3 = avlNode2.Parent;
						AvlNode right2 = avlNode2.Right;
						if (parent3.Left == avlNode2)
						{
							parent3.Left = right2;
						}
						else
						{
							parent3.Right = right2;
						}
						if (right2 != null)
						{
							right2.Parent = parent3;
						}
						avlNode2.Parent = parent;
						avlNode2.Left = left;
						avlNode2.Balance = avlNode.Balance;
						avlNode2.Right = right;
						right.Parent = avlNode2;
						if (left != null)
						{
							left.Parent = avlNode2;
						}
						if (avlNode == _root)
						{
							_root = avlNode2;
						}
						else if (parent.Left == avlNode)
						{
							parent.Left = avlNode2;
						}
						else
						{
							parent.Right = avlNode2;
						}
						BalanceAfterDeletion(parent3, -1);
					}
				}
				return true;
			}
			return false;
		}

		public bool Search(TKey key, out TValue value)
		{
			AvlNode avlNode = _root;
			while (avlNode != null)
			{
				if (_comparer.Compare(key, avlNode.Key) < 0)
				{
					avlNode = avlNode.Left;
					continue;
				}
				if (_comparer.Compare(key, avlNode.Key) > 0)
				{
					avlNode = avlNode.Right;
					continue;
				}
				value = avlNode.Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		public void Clear()
		{
			_root = null;
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return new AvlNodeEnumerator(_root);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static void Replace(AvlNode target, AvlNode source)
		{
			AvlNode left = source.Left;
			AvlNode right = source.Right;
			target.Balance = source.Balance;
			target.Key = source.Key;
			target.Value = source.Value;
			target.Left = left;
			target.Right = right;
			if (left != null)
			{
				left.Parent = target;
			}
			if (right != null)
			{
				right.Parent = target;
			}
		}

		private void BalanceAfterInsertion(AvlNode node, int balance)
		{
			while (node != null)
			{
				balance = (node.Balance += balance);
				switch (balance)
				{
				case 0:
					return;
				case 2:
					if (node.Left.Balance == 1)
					{
						RotateRight(node);
					}
					else
					{
						RotateLeftRight(node);
					}
					return;
				case -2:
					if (node.Right.Balance == -1)
					{
						RotateLeft(node);
					}
					else
					{
						RotateRightLeft(node);
					}
					return;
				}
				AvlNode parent = node.Parent;
				if (parent != null)
				{
					balance = ((parent.Left == node) ? 1 : (-1));
				}
				node = parent;
			}
		}

		private void BalanceAfterDeletion(AvlNode node, int balance)
		{
			while (node != null)
			{
				balance = (node.Balance += balance);
				switch (balance)
				{
				default:
					return;
				case 2:
					if (node.Left.Balance >= 0)
					{
						node = RotateRight(node);
						if (node.Balance == -1)
						{
							return;
						}
					}
					else
					{
						node = RotateLeftRight(node);
					}
					break;
				case -2:
					if (node.Right.Balance <= 0)
					{
						node = RotateLeft(node);
						if (node.Balance == 1)
						{
							return;
						}
					}
					else
					{
						node = RotateRightLeft(node);
					}
					break;
				case 0:
					break;
				}
				AvlNode parent = node.Parent;
				if (parent != null)
				{
					balance = ((parent.Left != node) ? 1 : (-1));
				}
				node = parent;
			}
		}

		private AvlNode RotateLeft(AvlNode node)
		{
			AvlNode right = node.Right;
			AvlNode left = right.Left;
			AvlNode avlNode = right.Parent = node.Parent;
			right.Left = node;
			node.Right = left;
			node.Parent = right;
			if (left != null)
			{
				left.Parent = node;
			}
			if (node == _root)
			{
				_root = right;
			}
			else if (avlNode.Right == node)
			{
				avlNode.Right = right;
			}
			else
			{
				avlNode.Left = right;
			}
			right.Balance++;
			node.Balance = -right.Balance;
			return right;
		}

		private AvlNode RotateRight(AvlNode node)
		{
			AvlNode left = node.Left;
			AvlNode right = left.Right;
			AvlNode avlNode = left.Parent = node.Parent;
			left.Right = node;
			node.Left = right;
			node.Parent = left;
			if (right != null)
			{
				right.Parent = node;
			}
			if (node == _root)
			{
				_root = left;
			}
			else if (avlNode.Left == node)
			{
				avlNode.Left = left;
			}
			else
			{
				avlNode.Right = left;
			}
			left.Balance--;
			node.Balance = -left.Balance;
			return left;
		}

		private AvlNode RotateLeftRight(AvlNode node)
		{
			AvlNode left = node.Left;
			AvlNode right = left.Right;
			AvlNode parent = node.Parent;
			AvlNode right2 = right.Right;
			AvlNode left2 = right.Left;
			right.Parent = parent;
			node.Left = right2;
			left.Right = left2;
			right.Left = left;
			right.Right = node;
			left.Parent = right;
			node.Parent = right;
			if (right2 != null)
			{
				right2.Parent = node;
			}
			if (left2 != null)
			{
				left2.Parent = left;
			}
			if (node == _root)
			{
				_root = right;
			}
			else if (parent.Left == node)
			{
				parent.Left = right;
			}
			else
			{
				parent.Right = right;
			}
			if (right.Balance == -1)
			{
				node.Balance = 0;
				left.Balance = 1;
			}
			else if (right.Balance == 0)
			{
				node.Balance = 0;
				left.Balance = 0;
			}
			else
			{
				node.Balance = -1;
				left.Balance = 0;
			}
			right.Balance = 0;
			return right;
		}

		private AvlNode RotateRightLeft(AvlNode node)
		{
			AvlNode right = node.Right;
			AvlNode left = right.Left;
			AvlNode parent = node.Parent;
			AvlNode left2 = left.Left;
			AvlNode right2 = left.Right;
			left.Parent = parent;
			node.Right = left2;
			right.Left = right2;
			left.Right = right;
			left.Left = node;
			right.Parent = left;
			node.Parent = left;
			if (left2 != null)
			{
				left2.Parent = node;
			}
			if (right2 != null)
			{
				right2.Parent = right;
			}
			if (node == _root)
			{
				_root = left;
			}
			else if (parent.Right == node)
			{
				parent.Right = left;
			}
			else
			{
				parent.Left = left;
			}
			if (left.Balance == 1)
			{
				node.Balance = 0;
				right.Balance = -1;
			}
			else if (left.Balance == 0)
			{
				node.Balance = 0;
				right.Balance = 0;
			}
			else
			{
				node.Balance = 1;
				right.Balance = 0;
			}
			left.Balance = 0;
			return left;
		}
	}
}
