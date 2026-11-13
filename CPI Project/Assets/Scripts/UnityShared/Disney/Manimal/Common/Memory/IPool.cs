namespace Disney.Manimal.Common.Memory
{
	public interface IPool<T>
	{
		int PooledSize
		{
			get;
		}

		int Allocations
		{
			get;
		}

		long Utilization
		{
			get;
		}

		T Acquire();

		void Release(T item);
	}
}
