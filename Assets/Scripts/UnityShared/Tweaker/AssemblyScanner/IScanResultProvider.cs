using System;

namespace Tweaker.AssemblyScanner
{
	public interface IScanResultProvider<TResult>
	{
		event EventHandler<ScanResultArgs<TResult>> ResultProvided;
	}
}
