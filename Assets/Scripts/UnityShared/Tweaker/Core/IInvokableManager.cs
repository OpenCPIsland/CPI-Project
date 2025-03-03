using System;
using System.Reflection;

namespace Tweaker.Core
{
	public interface IInvokableManager
	{
		IInvokable RegisterInvokable(InvokableInfo info, Delegate del);

		IInvokable RegisterInvokable(InvokableInfo info, MethodInfo methodInfo, object instance = null);

		IInvokable RegisterInvokable(InvokableInfo info, EventInfo eventInfo, object instance = null);

		void RegisterInvokable(IInvokable invokable);

		void UnregisterInvokable(IInvokable invokable);

		void UnregisterInvokable(string name);

		TweakerDictionary<IInvokable> GetInvokables(SearchOptions options = null);

		IInvokable GetInvokable(SearchOptions options = null);

		IInvokable GetInvokable(string name);

		object Invoke(IInvokable invokable, params object[] args);

		object Invoke(string name, params object[] args);
	}
}
