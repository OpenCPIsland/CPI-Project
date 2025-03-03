using System;
using System.Collections.Generic;

namespace Disney.Manimal.Common.Memory
{
	public class BasicPool<T> : IPool<T> where T : class
	{
		private readonly Func<T> _createFunc;

		private readonly Action<T> _resetFunc;

		private readonly Stack<T> _store;

		public int PooledSize
		{
			get
			{
				return _store.Count;
			}
		}

		public int Allocations
		{
			get;
			private set;
		}

		public long Utilization
		{
			get;
			private set;
		}

		public BasicPool(Func<T> createFunc, Action<T> resetFunc)
		{
			if (createFunc == null)
			{
				throw new ArgumentNullException("createFunc");
			}
			_createFunc = createFunc;
			_resetFunc = resetFunc;
			_store = new Stack<T>();
		}

		public BasicPool(Func<T> createFunc)
			: this(createFunc, (Action<T>)null)
		{
		}

		public T Acquire()
		{
			Utilization++;
			if (PooledSize > 0)
			{
				return _store.Pop();
			}
			Allocations++;
			return _createFunc();
		}

		public void Release(T item)
		{
			if (_resetFunc != null)
			{
				_resetFunc(item);
			}
			_store.Push(item);
		}
	}
}
