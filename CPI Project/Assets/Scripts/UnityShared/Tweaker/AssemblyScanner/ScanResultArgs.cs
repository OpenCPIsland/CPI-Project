using System;

namespace Tweaker.AssemblyScanner
{
	public class ScanResultArgs<TResult> : EventArgs
	{
		public TResult result;

		public ScanResultArgs(TResult result)
		{
			this.result = result;
		}
	}
}
