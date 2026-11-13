namespace Tweaker.AssemblyScanner
{
	public interface IScanProcessor<TInput, TResult> : IScanResultProvider<TResult> where TInput : class
	{
	}
}
