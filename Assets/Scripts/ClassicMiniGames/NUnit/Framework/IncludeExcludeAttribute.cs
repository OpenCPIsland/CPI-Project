namespace NUnit.Framework
{
	public abstract class IncludeExcludeAttribute : TestModificationAttribute
	{
		private string include;

		private string exclude;

		private string reason;

		public string Include
		{
			get
			{
				return include;
			}
			set
			{
				include = value;
			}
		}

		public string Exclude
		{
			get
			{
				return exclude;
			}
			set
			{
				exclude = value;
			}
		}

		public string Reason
		{
			get
			{
				return reason;
			}
			set
			{
				reason = value;
			}
		}

		public IncludeExcludeAttribute()
		{
		}

		public IncludeExcludeAttribute(string include)
		{
			this.include = include;
		}
	}
}
