using System;
using System.Reflection;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public static class InvokableFactory
	{
		public static IInvokable MakeInvokable(InvokableAttribute attribute, MethodInfo methodInfo, IBoundInstance boundInstance)
		{
			string finalName = GetFinalName(attribute.Name, boundInstance);
			uint instanceId = (boundInstance != null) ? boundInstance.UniqueId : 0u;
			string[] argDescriptions = GetArgDescriptions(methodInfo.GetParameters());
			string returnDescription = GetReturnDescription(methodInfo);
			return MakeInvokable(new InvokableInfo(finalName, instanceId, CustomTweakerAttributes.Get(methodInfo), attribute.Description, argDescriptions, returnDescription), methodInfo, (boundInstance != null) ? boundInstance.Instance : null);
		}

		public static IInvokable MakeInvokable(InvokableAttribute attribute, EventInfo eventInfo, IBoundInstance boundInstance)
		{
			string finalName = GetFinalName(attribute.Name, boundInstance);
			uint instanceId = (boundInstance != null) ? boundInstance.UniqueId : 0u;
			object instance = (boundInstance != null) ? boundInstance.Instance : null;
			MethodInfo method = eventInfo.EventHandlerType.GetMethod("Invoke");
			string[] argDescriptions = GetArgDescriptions(method.GetParameters());
			string returnDescription = GetReturnDescription(method);
			return MakeInvokable(new InvokableInfo(finalName, instanceId, CustomTweakerAttributes.Get(eventInfo), attribute.Description, argDescriptions, returnDescription), eventInfo, instance);
		}

		public static IInvokable MakeInvokable(InvokableInfo info, Delegate del)
		{
			return new InvokableMethod(info, del);
		}

		public static IInvokable MakeInvokable(InvokableInfo info, MethodInfo methodInfo, object instance)
		{
			return new InvokableMethod(info, methodInfo, (instance == null) ? null : new WeakReference(instance));
		}

		public static IInvokable MakeInvokable(InvokableInfo info, EventInfo eventInfo, object instance)
		{
			FieldInfo backingEventField = GetBackingEventField(eventInfo, instance);
			return MakeInvokableFromBackingEventField(info, eventInfo, backingEventField, instance);
		}

		public static IInvokable MakeInvokableFromBackingEventField(InvokableInfo info, EventInfo eventInfo, FieldInfo fieldInfo, object instance)
		{
			return new InvokableEvent(info, eventInfo, fieldInfo, (instance == null) ? null : new WeakReference(instance));
		}

		public static string[] GetArgDescriptions(ParameterInfo[] parameters)
		{
			string[] array = new string[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				object[] customAttributes = parameters[i].GetCustomAttributes(typeof(ArgDescriptionAttribute), true);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					array[i] = (customAttributes[0] as ArgDescriptionAttribute).Description;
				}
				else
				{
					array[i] = "";
				}
			}
			return array;
		}

		public static string GetReturnDescription(MethodInfo methodInfo)
		{
			object[] customAttributes = methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(ReturnDescriptionAttribute), true);
			if (customAttributes.Length > 0)
			{
				return (customAttributes[0] as ReturnDescriptionAttribute).Description;
			}
			return "";
		}

		public static FieldInfo GetBackingEventField(EventInfo eventInfo, object instance = null)
		{
			Type reflectedType = eventInfo.ReflectedType;
			FieldInfo field = reflectedType.GetField(eventInfo.Name, ReflectionUtil.GetBindingFlags(instance) | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new ProcessorException("Could not find backing field for event.");
			}
			return field;
		}

		private static string GetFinalName(string name, IBoundInstance instance)
		{
			if (instance == null)
			{
				return name;
			}
			return string.Format("{0}#{1}", name, instance.UniqueId);
		}
	}
}
