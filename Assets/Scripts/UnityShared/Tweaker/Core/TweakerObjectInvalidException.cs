using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class TweakerObjectInvalidException : Exception, ISerializable
	{
		public TweakerObjectInvalidException(ITweakerObject obj)
			: base("The invokable named '" + obj.Name + "' is no longer valid and should be unregistered or uncached.")
		{
		}
	}
}
