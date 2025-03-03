using System;
using System.Reflection;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public class InvokableManager : IInvokableManager
	{
		private BaseTweakerManager<IInvokable> baseManager;

		public InvokableManager(IScanner scanner, TweakerOptions options)
		{
			baseManager = new BaseTweakerManager<IInvokable>(scanner, options);
			if (scanner != null)
			{
				scanner.AddProcessor(new InvokableProcessor(options));
			}
		}

		public IInvokable RegisterInvokable(InvokableInfo info, Delegate del)
		{
			IInvokable invokable = InvokableFactory.MakeInvokable(info, del);
			RegisterInvokable(invokable);
			return invokable;
		}

		public IInvokable RegisterInvokable(InvokableInfo info, MethodInfo methodInfo, object instance = null)
		{
			IInvokable invokable = InvokableFactory.MakeInvokable(info, methodInfo, instance);
			RegisterInvokable(invokable);
			return invokable;
		}

		public IInvokable RegisterInvokable(InvokableInfo info, EventInfo eventInfo, object instance = null)
		{
			IInvokable invokable = InvokableFactory.MakeInvokable(info, eventInfo, instance);
			RegisterInvokable(invokable);
			return invokable;
		}

		public void RegisterInvokable(IInvokable invokable)
		{
			if (invokable.Manager != null && invokable.Manager != this)
			{
				invokable.Manager.UnregisterInvokable(invokable);
			}
			invokable.Manager = this;
			baseManager.RegisterObject(invokable);
		}

		public void UnregisterInvokable(IInvokable invokable)
		{
			baseManager.UnregisterObject(invokable);
		}

		public void UnregisterInvokable(string name)
		{
			baseManager.UnregisterObject(name);
		}

		public TweakerDictionary<IInvokable> GetInvokables(SearchOptions options = null)
		{
			return baseManager.GetObjects(options);
		}

		public IInvokable GetInvokable(SearchOptions options = null)
		{
			return baseManager.GetObject(options);
		}

		public IInvokable GetInvokable(string name)
		{
			return baseManager.GetObject(name);
		}

		public object Invoke(IInvokable invokable, params object[] args)
		{
			return invokable.Invoke(args);
		}

		public object Invoke(string name, params object[] args)
		{
			IInvokable @object = baseManager.GetObject(name);
			if (@object == null)
			{
				throw new NotFoundException(name);
			}
			return Invoke(@object, args);
		}
	}
}
