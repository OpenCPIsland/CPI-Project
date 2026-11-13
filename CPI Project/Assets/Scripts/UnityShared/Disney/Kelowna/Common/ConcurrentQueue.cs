using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public class ConcurrentQueue<T>
	{
		private object sync = new object();

		private Queue<T> queue = new Queue<T>();

		public int Count
		{
			get
			{
				lock (sync)
				{
					return queue.Count;
				}
			}
		}

		public void Enqueue(T item)
		{
			lock (sync)
			{
				queue.Enqueue(item);
			}
		}

		public bool TryDequeue(out T item)
		{
			bool result = false;
			item = default(T);
			lock (sync)
			{
				if (queue.Count > 0)
				{
					item = queue.Dequeue();
					result = true;
				}
			}
			return result;
		}
	}
}
