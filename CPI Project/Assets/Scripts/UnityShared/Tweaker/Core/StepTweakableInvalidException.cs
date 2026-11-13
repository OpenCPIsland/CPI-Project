using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class StepTweakableInvalidException : Exception, ISerializable
	{
		public StepTweakableInvalidException(string name, string message)
			: base("The step tweakable named '" + name + "' is invalid: " + message)
		{
		}

		public StepTweakableInvalidException(string name, string message, Exception inner)
			: base("The step tweakable named '" + name + "' is invalid: " + message, inner)
		{
		}
	}
}
