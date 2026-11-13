using System;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class ArgDescriptionAttribute : Attribute
	{
		public string Description
		{
			get;
			private set;
		}

		public ArgDescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}
