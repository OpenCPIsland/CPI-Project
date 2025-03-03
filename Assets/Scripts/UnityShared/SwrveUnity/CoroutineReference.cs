namespace SwrveUnity
{
	public class CoroutineReference<T>
	{
		private T val;

		public CoroutineReference()
		{
		}

		public CoroutineReference(T val)
		{
			this.val = val;
		}

		public T Value()
		{
			return val;
		}

		public void Value(T val)
		{
			this.val = val;
		}
	}
}
