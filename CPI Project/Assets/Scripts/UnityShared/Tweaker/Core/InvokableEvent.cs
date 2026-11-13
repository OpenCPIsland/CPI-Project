using System;
using System.Reflection;

namespace Tweaker.Core
{
	public class InvokableEvent : BaseInvokable
	{
		private readonly FieldInfo fieldInfo;

		private string methodSignature;

		public override string MethodSignature
		{
			get
			{
				return methodSignature;
			}
		}

		public FieldInfo FieldInfo
		{
			get
			{
				return fieldInfo;
			}
		}

		public InvokableEvent(InvokableInfo info, EventInfo eventInfo, FieldInfo fieldInfo, WeakReference instance)
			: base(info, fieldInfo.DeclaringType, fieldInfo.ReflectedType.Assembly, instance, fieldInfo.IsPublic)
		{
			this.fieldInfo = fieldInfo;
			methodSignature = "[Unknown]";
			MethodInfo method = eventInfo.EventHandlerType.GetMethod("Invoke");
			SetParameters(method.GetParameters());
			methodSignature = method.GetSignature();
		}

		private MulticastDelegate GetEventDelegate()
		{
			MulticastDelegate multicastDelegate = null;
			object value = fieldInfo.GetValue(StrongInstance);
			if (value != null)
			{
				multicastDelegate = (MulticastDelegate)value;
				if ((object)multicastDelegate == null)
				{
					throw new Exception("Could not retrieve the event delegate for invokable '" + base.Name + "'.");
				}
			}
			return multicastDelegate;
		}

		protected override object DoInvoke(object[] args)
		{
			object result = null;
			MulticastDelegate eventDelegate = GetEventDelegate();
			if ((object)eventDelegate != null)
			{
				Delegate[] invocationList = eventDelegate.GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					result = @delegate.Method.Invoke(@delegate.Target, args);
				}
			}
			return result;
		}
	}
}
