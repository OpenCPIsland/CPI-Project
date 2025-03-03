using System;
using System.Collections.Generic;

public class FixedSizeQueue<T> : Queue<T>
{
	public int Limit
	{
		get;
		set;
	}

	public FixedSizeQueue(int limit)
	{
		if (limit <= 0)
		{
			throw new ArgumentException("Limit must be greater than zero", "limit");
		}
		Limit = limit;
	}

	public new void Enqueue(T item)
	{
		base.Enqueue(item);
		while (base.Count > Limit)
		{
			Dequeue();
		}
	}
}
