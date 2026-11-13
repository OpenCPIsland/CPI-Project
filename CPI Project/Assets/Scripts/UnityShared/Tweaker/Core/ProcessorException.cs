using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class ProcessorException : Exception, ISerializable
	{
		public ProcessorException(string msg)
			: base(msg)
		{
		}
	}
}
