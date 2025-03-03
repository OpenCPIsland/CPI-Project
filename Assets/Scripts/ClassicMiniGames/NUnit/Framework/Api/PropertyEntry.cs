namespace NUnit.Framework.Api
{
	public class PropertyEntry
	{
		private readonly string name;

		private readonly object value;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public object Value
		{
			get
			{
				return value;
			}
		}

		public PropertyEntry(string name, object value)
		{
			this.name = name;
			this.value = value;
		}

		public override string ToString()
		{
			return string.Format("{0}={1}", name, value);
		}
	}
}
