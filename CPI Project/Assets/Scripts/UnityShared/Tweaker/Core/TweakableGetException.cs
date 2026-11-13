using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class TweakableGetException : Exception, ISerializable
	{
		public TweakableGetException(string name, Exception inner)
			: base("Failed to get tweakable '" + name + "'. See inner exception.", inner)
		{
		}

		public TweakableGetException(string name, string message)
			: base("Failed to get tweakable '" + name + "'. " + message)
		{
		}
	}
}
