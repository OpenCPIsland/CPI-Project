using System;

namespace Tweaker.AssemblyScanner
{
	public interface ITypeScanProcessor<TInput, TResult> : IScanProcessor<TInput, TResult>, IScanResultProvider<TResult> where TInput : class
	{
		void ProcessType(Type type, IBoundInstance instance = null);
	}
}
