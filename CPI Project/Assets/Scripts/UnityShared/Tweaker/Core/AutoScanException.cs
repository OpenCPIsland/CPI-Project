using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class AutoScanException : Exception, ISerializable
	{
		public AutoScanException(string message)
			: base(message)
		{
		}
	}
}
