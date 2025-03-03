using System;
using System.Reflection;

namespace Tweaker.AssemblyScanner
{
	public interface IScanner
	{
		void Scan(ScanOptions options = null);

		void ScanAssembly(Assembly assembly, ScanOptions options = null);

		void ScanType(Type type, ScanOptions options = null);

		void ScanMember(MemberInfo member, ScanOptions options = null);

		IBoundInstance ScanInstance(object instance, ScanOptions options = null);

		void ScanType(Type type, IBoundInstance instance, ScanOptions options = null);

		void ScanGenericType(Type type, IBoundInstance instance, ScanOptions options = null);

		void ScanMember(MemberInfo member, IBoundInstance instance, ScanOptions options = null);

		void ScanAttribute(Attribute attribute, object reflectedObject, IBoundInstance instance, ScanOptions options = null);

		IScanResultProvider<TResult> GetResultProvider<TResult>();

		void AddProcessor<TInput, TResult>(IAttributeScanProcessor<TInput, TResult> processor) where TInput : class;

		void AddProcessor<TInput, TResult>(ITypeScanProcessor<TInput, TResult> processor) where TInput : class;

		void AddProcessor<TInput, TResult>(IMemberScanProcessor<TInput, TResult> processor) where TInput : class;

		void RemoveProcessor<TInput, TResult>(IScanProcessor<TInput, TResult> processor) where TInput : class;
	}
}
