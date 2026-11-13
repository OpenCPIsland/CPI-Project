using System;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpProviderAttribute : Attribute
	{
		public Type ProviderType;

		public HttpProviderAttribute(Type providerType)
		{
			ProviderType = providerType;
		}
	}
}
