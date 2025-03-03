using System;
using System.Runtime.Serialization;

namespace Tweaker.AssemblyScanner
{
	public class ScannerException : Exception, ISerializable
	{
		public ScannerException(string msg)
			: base(msg)
		{
		}
	}
}
