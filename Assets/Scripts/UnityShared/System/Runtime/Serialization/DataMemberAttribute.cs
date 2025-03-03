namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class DataMemberAttribute : Attribute
	{
		private bool is_required;

		private bool emit_default = true;

		private string name;

		private int order = -1;

		public bool EmitDefaultValue
		{
			get
			{
				return emit_default;
			}
			set
			{
				emit_default = value;
			}
		}

		public bool IsRequired
		{
			get
			{
				return is_required;
			}
			set
			{
				is_required = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public int Order
		{
			get
			{
				return order;
			}
			set
			{
				order = value;
			}
		}
	}
}
