namespace Fabric
{
	public struct InitialiseParameter<T>
	{
		private bool _isDirty;

		private T _value;

		public bool IsDirty
		{
			get
			{
				return _isDirty;
			}
		}

		public T Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				_isDirty = true;
			}
		}
	}
}
