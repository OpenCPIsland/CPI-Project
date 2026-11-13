using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class AutoInvokableException : Exception, ISerializable
	{
		public AutoInvokableException(string message)
			: base(message)
		{
		}
	}
}
