using System;
using System.Reflection;

namespace Tweaker.AssemblyScanner
{
	public interface IAttributeScanProcessor<TInput, TResult> : IScanProcessor<TInput, TResult>, IScanResultProvider<TResult> where TInput : class
	{
		void ProcessAttribute(TInput input, Type type, IBoundInstance instance = null);

		void ProcessAttribute(TInput input, MemberInfo memberInfo, IBoundInstance instance = null);
	}
}
