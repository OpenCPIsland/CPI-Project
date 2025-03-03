using System;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false)]
	public class ReturnDescriptionAttribute : Attribute
	{
		public string Description
		{
			get;
			private set;
		}

		public ReturnDescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}
