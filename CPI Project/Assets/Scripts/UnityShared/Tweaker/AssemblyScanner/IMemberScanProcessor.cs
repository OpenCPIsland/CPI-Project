using System;

namespace Tweaker.AssemblyScanner
{
	public interface IMemberScanProcessor<TInput, TResult> : IScanProcessor<TInput, TResult>, IScanResultProvider<TResult> where TInput : class
	{
		void ProcessMember(TInput input, Type type, IBoundInstance instance = null);
	}
}
