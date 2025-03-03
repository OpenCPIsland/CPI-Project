using hg.ApiWebKit.core.http;
using System;

namespace hg.ApiWebKit.core.attributes
{
	public abstract class HttpFaultAttribute : Attribute
	{
		public virtual void CheckFaults(HttpOperation operation, HttpResponse response)
		{
		}
	}
}
