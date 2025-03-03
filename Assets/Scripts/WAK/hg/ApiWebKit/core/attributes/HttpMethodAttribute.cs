using System;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public abstract class HttpMethodAttribute : Attribute
	{
		public string Verb
		{
			get;
			private set;
		}

		protected HttpMethodAttribute(string verb)
		{
			Verb = verb;
		}
	}
}
