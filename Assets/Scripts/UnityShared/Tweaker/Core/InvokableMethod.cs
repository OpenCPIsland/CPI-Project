using System;
using System.Reflection;

namespace Tweaker.Core
{
	public class InvokableMethod : BaseInvokable
	{
		private readonly MethodInfo methodInfo;

		private string methodSignature;

		public MethodInfo MethodInfo
		{
			get
			{
				return methodInfo;
			}
		}

		public override string MethodSignature
		{
			get
			{
				return methodSignature;
			}
		}

		public InvokableMethod(InvokableInfo info, MethodInfo methodInfo, WeakReference instance)
			: base(info, methodInfo.DeclaringType, methodInfo.ReflectedType.Assembly, instance, methodInfo.IsPublic, methodInfo.GetParameters())
		{
			this.methodInfo = methodInfo;
			methodSignature = methodInfo.GetSignature();
		}

		public InvokableMethod(InvokableInfo info, Delegate methodDelegate)
			: base(info, methodDelegate.Method.DeclaringType, methodDelegate.Method.ReflectedType.Assembly, (methodDelegate.Target == null) ? null : new WeakReference(methodDelegate.Target), methodDelegate.Method.IsPublic, methodDelegate.Method.GetParameters())
		{
			methodInfo = methodDelegate.Method;
			methodSignature = methodInfo.GetSignature();
		}

		protected override object DoInvoke(object[] args)
		{
			return MethodInfo.Invoke(StrongInstance, args);
		}
	}
}
