using System;
using System.Reflection;

namespace Tweaker.Core
{
	public interface IInvokable : ITweakerObject
	{
		InvokableInfo InvokableInfo
		{
			get;
		}

		string MethodSignature
		{
			get;
		}

		Type DeclaringType
		{
			get;
		}

		Type[] ParameterTypes
		{
			get;
		}

		ParameterInfo[] Parameters
		{
			get;
		}

		IInvokableManager Manager
		{
			get;
			set;
		}

		object Invoke(params object[] args);
	}
}
