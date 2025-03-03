namespace Disney.Kelowna.Common
{
	public sealed class HeapPriorityQueue<T> where T : PriorityQueueNode
	{
		private int _numNodes;

		private readonly T[] _nodes;

		private int _numNodesEverEnqueued;

		public int Count
		{
			get
			{
				return _numNodes;
			}
		}

		public int MaxSize
		{
			get
			{
				return _nodes.Length - 1;
			}
		}

		public T First
		{
			get
			{
				return _nodes[1];
			}
		}

		public HeapPriorityQueue(int maxNodes)
		{
			_numNodes = 0;
			_nodes = new T[maxNodes + 1];
			_numNodesEverEnqueued = 0;
		}

		public void Clear()
		{
			for (int i = 1; i < _nodes.Length; i++)
			{
				_nodes[i] = null;
			}
			_numNodes = 0;
		}

		public bool Contains(T node)
		{
			return _nodes[node.QueueIndex] == node;
		}

		public void Enqueue(T node, int priority)
		{
			node.Priority = priority;
			_numNodes++;
			_nodes[_numNodes] = node;
			node.QueueIndex = _numNodes;
			node.InsertionIndex = _numNodesEverEnqueued++;
			CascadeUp(_nodes[_numNodes]);
		}

		private void Swap(T node1, T node2)
		{
			_nodes[node1.QueueIndex] = node2;
			_nodes[node2.QueueIndex] = node1;
			int queueIndex = node1.QueueIndex;
			node1.QueueIndex = node2.QueueIndex;
			node2.QueueIndex = queueIndex;
		}

		private void CascadeUp(T node)
		{
			for (int num = node.QueueIndex / 2; num >= 1; num = node.QueueIndex / 2)
			{
				T val = _nodes[num];
				if (val.Priority < node.Priority || (val.Priority == node.Priority && val.InsertionIndex < node.InsertionIndex))
				{
					break;
				}
				Swap(node, val);
			}
		}

		private void CascadeDown(T node)
		{
			int num = node.QueueIndex;
			while (true)
			{
				bool flag = true;
				T val = node;
				int num2 = 2 * num;
				if (num2 > _numNodes)
				{
					node.QueueIndex = num;
					_nodes[num] = node;
					return;
				}
				T val2 = _nodes[num2];
				if (val2.Priority < val.Priority || (val2.Priority == val.Priority && val2.InsertionIndex < val.InsertionIndex))
				{
					val = val2;
				}
				int num3 = num2 + 1;
				if (num3 <= _numNodes)
				{
					T val3 = _nodes[num3];
					if (val3.Priority < val.Priority || (val3.Priority == val.Priority && val3.InsertionIndex < val.InsertionIndex))
					{
						val = val3;
					}
				}
				if (val == node)
				{
					break;
				}
				_nodes[num] = val;
				int queueIndex = val.QueueIndex;
				val.QueueIndex = num;
				num = queueIndex;
			}
			node.QueueIndex = num;
			_nodes[num] = node;
		}

		private bool HasHigherPriority(T higher, T lower)
		{
			return higher.Priority < lower.Priority || (higher.Priority == lower.Priority && higher.InsertionIndex < lower.InsertionIndex);
		}

		public T Dequeue()
		{
			T val = _nodes[1];
			Remove(val);
			return val;
		}

		public void UpdatePriority(T node, int priority)
		{
			node.Priority = priority;
			OnNodeUpdated(node);
		}

		private void OnNodeUpdated(T node)
		{
			int num = node.QueueIndex / 2;
			T lower = _nodes[num];
			if (num > 0 && HasHigherPriority(node, lower))
			{
				CascadeUp(node);
			}
			else
			{
				CascadeDown(node);
			}
		}

		public void Remove(T node)
		{
			if (_numNodes <= 1)
			{
				_nodes[1] = null;
				_numNodes = 0;
				return;
			}
			bool flag = false;
			T val = _nodes[_numNodes];
			if (node.QueueIndex != _numNodes)
			{
				Swap(node, val);
				flag = true;
			}
			_numNodes--;
			_nodes[node.QueueIndex] = null;
			if (flag)
			{
				OnNodeUpdated(val);
			}
		}

		public T Get(int i)
		{
			return _nodes[i];
		}

		public bool IsValidQueue()
		{
			for (int i = 1; i < _nodes.Length; i++)
			{
				if (_nodes[i] != null)
				{
					int num = 2 * i;
					if (num < _nodes.Length && _nodes[num] != null && HasHigherPriority(_nodes[num], _nodes[i]))
					{
						return false;
					}
					int num2 = num + 1;
					if (num2 < _nodes.Length && _nodes[num2] != null && HasHigherPriority(_nodes[num2], _nodes[i]))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
