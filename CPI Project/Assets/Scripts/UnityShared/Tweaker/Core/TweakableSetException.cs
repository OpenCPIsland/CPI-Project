using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class TweakableSetException : Exception, ISerializable
	{
		public TweakableSetException(string name, object value, Exception inner)
			: base("Failed to set tweakable '" + name + "' to value '" + value.ToString() + "'. See inner exception.", inner)
		{
		}

		public TweakableSetException(string name, string message)
			: base("Failed to set tweakable '" + name + "'. " + message)
		{
		}
	}
}
