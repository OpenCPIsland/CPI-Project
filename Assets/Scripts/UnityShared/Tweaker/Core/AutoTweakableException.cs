using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class AutoTweakableException : Exception, ISerializable
	{
		public AutoTweakableException(string message)
			: base(message)
		{
		}
	}
}
