using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class NameAlreadyRegisteredException : Exception, ISerializable
	{
		public NameAlreadyRegisteredException(string name)
			: base("The name '" + name + "' is already registered.")
		{
		}
	}
}
