using System;
using System.Reflection;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public class AutoInvokable : AutoInvokableBase, IDisposable
	{
		public IInvokable invokable;

		public AutoInvokable(string invokableName, string methodName, IBoundInstance instance, string description = "", string[] argDescriptions = null, string returnDescription = "")
		{
			if (!CheckForManager())
			{
				return;
			}
			MethodInfo[] methods = instance.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.Name == methodName)
				{
					uint uniqueId = instance.UniqueId;
					string name = string.Format("{0}#{1}", invokableName, uniqueId);
					invokable = AutoInvokableBase.Manager.RegisterInvokable(new InvokableInfo(name, uniqueId, CustomTweakerAttributes.Get(methodInfo), description, argDescriptions, returnDescription), methodInfo, instance.Instance);
				}
			}
		}

		public AutoInvokable(string name, Delegate del, IBoundInstance instance = null, string description = "", string[] argDescriptions = null, string returnDescription = "")
		{
			if (CheckForManager())
			{
				uint num = (instance != null) ? instance.UniqueId : 0u;
				string name2 = string.Format("{0}#{1}", name, num);
				invokable = AutoInvokableBase.Manager.RegisterInvokable(new InvokableInfo(name2, num, CustomTweakerAttributes.Get(del.Method), description, argDescriptions, returnDescription), del);
			}
		}

		~AutoInvokable()
		{
			Dispose();
		}

		private bool CheckForManager()
		{
			if (AutoInvokableBase.Manager == null)
			{
				throw new AutoInvokableException("No manager has been set. Set a manager through AutoInvokableBase.Manager before creating auto invokable instance.");
			}
			return true;
		}

		public void Dispose()
		{
			if (invokable != null && invokable.Manager != null)
			{
				AutoInvokableBase.Manager.UnregisterInvokable(invokable);
			}
			invokable = null;
		}
	}
}
