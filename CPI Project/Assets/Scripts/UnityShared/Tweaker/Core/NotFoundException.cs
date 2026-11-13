using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class NotFoundException : Exception, ISerializable
	{
		public NotFoundException(string name)
			: base("The name '" + name + "' is not currently in use.")
		{
		}
	}
}
