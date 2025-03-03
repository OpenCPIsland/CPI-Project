namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class EnumMemberAttribute : Attribute
	{
		private string value;

		public string Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}
	}
}
