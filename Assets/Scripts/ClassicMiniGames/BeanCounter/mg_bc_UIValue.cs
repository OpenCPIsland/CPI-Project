namespace BeanCounter
{
	public class mg_bc_UIValue<T>
	{
		public T Value
		{
			get;
			private set;
		}

		public mg_bc_UIValueDisplayer<T> Displayer
		{
			get;
			private set;
		}

		public void SetValue(T _newValue)
		{
			Value = _newValue;
			if (Displayer != null)
			{
				Displayer.SetValue(Value);
			}
		}

		public void SetDisplayer(mg_bc_UIValueDisplayer<T> _displayer)
		{
			Displayer = _displayer;
			Displayer.SetValue(Value);
		}
	}
}
