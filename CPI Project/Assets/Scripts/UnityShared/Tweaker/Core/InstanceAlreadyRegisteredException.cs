using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class InstanceAlreadyRegisteredException : Exception, ISerializable
	{
		public InstanceAlreadyRegisteredException(ITweakerObject obj)
			: base("The instance of type '" + obj.GetType().Name + "' with name '" + obj.Name + "' is already registered.")
		{
		}
	}
}
