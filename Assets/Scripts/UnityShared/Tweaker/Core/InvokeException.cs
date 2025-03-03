using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class InvokeException : Exception, ISerializable
	{
		public InvokeException(string name, object[] args, Exception inner)
			: base(string.Concat("Invocation of '", name, "(", args, ")' failed. Inner Exception: ", inner.Message), inner)
		{
		}
	}
}
