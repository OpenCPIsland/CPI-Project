using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public static class TweakableFactory
	{
		public static ITweakable MakeTweakable(TweakableAttribute attribute, PropertyInfo propertyInfo, IBoundInstance instance, MemberInfo containerMemberInfo = null)
		{
			return MakeTweakable(attribute, propertyInfo.PropertyType, propertyInfo, instance, containerMemberInfo);
		}

		public static ITweakable MakeTweakable(TweakableAttribute attribute, FieldInfo fieldInfo, IBoundInstance instance, MemberInfo containerMemberInfo = null)
		{
			return MakeTweakable(attribute, fieldInfo.FieldType, fieldInfo, instance, containerMemberInfo);
		}

		public static ITweakable MakeTweakable(TweakableAttribute attribute, Type type, MemberInfo memberInfo, IBoundInstance instance, MemberInfo containerMemberInfo = null)
		{
			Type type2 = typeof(TweakableInfo<>).MakeGenericType(type);
			uint num = (instance != null) ? instance.UniqueId : 0u;
			MemberInfo memberInfo2 = (containerMemberInfo != null) ? containerMemberInfo : memberInfo;
			TweakerRangeAttribute attribute2 = memberInfo2.GetCustomAttributes(typeof(TweakerRangeAttribute), false).FirstOrDefault() as TweakerRangeAttribute;
			StepSizeAttribute attribute3 = memberInfo2.GetCustomAttributes(typeof(StepSizeAttribute), false).FirstOrDefault() as StepSizeAttribute;
			NamedToggleValueAttribute[] source = memberInfo2.GetCustomAttributes(typeof(NamedToggleValueAttribute), false) as NamedToggleValueAttribute[];
			ICustomTweakerAttribute[] array = memberInfo2.GetCustomAttributes(typeof(ICustomTweakerAttribute), true) as ICustomTweakerAttribute[];
			source = source.OrderBy((NamedToggleValueAttribute toggle) => toggle.Order).ToArray();
			object obj = MakeTweakableRange(type, attribute2);
			object obj2 = MakeTweakableStepSize(type, attribute3, attribute.Name);
			Array array2 = MakeTweakableToggleValues(type, source);
			WeakReference weakReference = null;
			if (instance != null)
			{
				weakReference = new WeakReference(instance.Instance);
			}
			string finalName = GetFinalName(attribute.Name, instance);
			object obj3 = Activator.CreateInstance(type2, finalName, obj, obj2, array2, num, array, attribute.Description);
			Type type3 = typeof(BaseTweakable<>).MakeGenericType(type);
			return Activator.CreateInstance(type3, obj3, memberInfo, weakReference) as ITweakable;
		}

		public static ITweakable MakeTweakableFromInfo<T>(TweakableInfo<T> info, PropertyInfo propertyInfo, object instance)
		{
			if (typeof(T) != propertyInfo.PropertyType)
			{
				return null;
			}
			WeakReference instance2 = (instance != null) ? new WeakReference(instance) : null;
			return new BaseTweakable<T>(info, propertyInfo, instance2);
		}

		public static ITweakable MakeTweakableFromInfo<T>(TweakableInfo<T> info, FieldInfo fieldInfo, object instance)
		{
			if (typeof(T) != fieldInfo.FieldType)
			{
				return null;
			}
			WeakReference instance2 = (instance != null) ? new WeakReference(instance) : null;
			return new BaseTweakable<T>(info, fieldInfo, instance2);
		}

		public static ITweakable MakeTweakableFromInfo<T>(TweakableInfo<T> info, out object virtualFieldRef)
		{
			VirtualField<T> virtualField = new VirtualField<T>();
			ITweakable result = new BaseTweakable<T>(info, virtualField);
			virtualFieldRef = virtualField;
			return result;
		}

		public static ITweakable MakeTweakable(Type tweakableType, string name, string description, Attribute[] attributes, out object virtualFieldRef)
		{
			TweakerRangeAttribute attribute = null;
			StepSizeAttribute attribute2 = null;
			List<NamedToggleValueAttribute> list = new List<NamedToggleValueAttribute>();
			List<ICustomTweakerAttribute> list2 = new List<ICustomTweakerAttribute>();
			if (attributes != null)
			{
				foreach (Attribute attribute3 in attributes)
				{
					if (attribute3 is TweakerRangeAttribute)
					{
						attribute = (TweakerRangeAttribute)attribute3;
					}
					else if (attribute3 is StepSizeAttribute)
					{
						attribute2 = (StepSizeAttribute)attribute3;
					}
					else if (attribute3 is NamedToggleValueAttribute)
					{
						list.Add((NamedToggleValueAttribute)attribute3);
					}
					else if (attribute3 is ICustomTweakerAttribute)
					{
						list2.Add((ICustomTweakerAttribute)attribute3);
					}
				}
			}
			object obj = MakeTweakableRange(tweakableType, attribute);
			object obj2 = MakeTweakableStepSize(tweakableType, attribute2, name);
			Array array = MakeTweakableToggleValues(tweakableType, list.ToArray());
			Type type = typeof(TweakableInfo<>).MakeGenericType(tweakableType);
			Type type2 = typeof(VirtualField<>).MakeGenericType(tweakableType);
			Type type3 = typeof(BaseTweakable<>).MakeGenericType(tweakableType);
			object value = Activator.CreateInstance(type, name, obj, obj2, array, 0u, list2.ToArray(), description);
			object obj3 = Activator.CreateInstance(type2);
			ITweakable result = Activator.CreateInstance(type3, Convert.ChangeType(value, type), Convert.ChangeType(obj3, type2)) as ITweakable;
			virtualFieldRef = obj3;
			return result;
		}

		private static string GetFinalName(string name, IBoundInstance instance)
		{
			if (instance == null)
			{
				return name;
			}
			return string.Format("{0}#{1}", name, instance.UniqueId);
		}

		private static object MakeTweakableRange(Type type, TweakerRangeAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			Type typeFromHandle = typeof(TweakableRange<>);
			typeFromHandle = typeFromHandle.MakeGenericType(type);
			return Activator.CreateInstance(typeFromHandle, attribute.MinValue, attribute.MaxValue);
		}

		private static object MakeTweakableStepSize(Type type, StepSizeAttribute attribute, string tweakableName)
		{
			if (attribute == null)
			{
				return null;
			}
			Type typeFromHandle = typeof(TweakableStepSize<>);
			typeFromHandle = typeFromHandle.MakeGenericType(type);
			object result = null;
			if (type.IsPrimitive)
			{
				result = Activator.CreateInstance(typeFromHandle, attribute.Size);
			}
			else
			{
				object obj = null;
				try
				{
					obj = Activator.CreateInstance(type, attribute.Size);
				}
				catch (MissingMethodException inner)
				{
					throw new StepTweakableInvalidException(tweakableName, "The type '" + type.FullName + "' must have a constructor that takes a single argument of type '" + attribute.Size.GetType().FullName + "'", inner);
				}
				catch (MethodAccessException inner2)
				{
					throw new StepTweakableInvalidException(tweakableName, "The type '" + type.FullName + "' has the a constructor that takes a single argument of type '" + attribute.Size.GetType().FullName + " but it is not public.", inner2);
				}
				if (obj != null)
				{
					result = Activator.CreateInstance(typeFromHandle, obj);
				}
			}
			return result;
		}

		private static Array MakeTweakableToggleValues(Type type, NamedToggleValueAttribute[] attributes)
		{
			if (attributes == null || attributes.Length == 0)
			{
				return null;
			}
			Type typeFromHandle = typeof(TweakableNamedToggleValue<>);
			typeFromHandle = typeFromHandle.MakeGenericType(type);
			Type type2 = typeof(List<>).MakeGenericType(typeFromHandle);
			IList list = (IList)Activator.CreateInstance(type2);
			for (int i = 0; i < attributes.Length; i++)
			{
				if (attributes[i].ValueGeneratorType != null)
				{
					NamedToggleValueAttribute.NamedToggleValueGenerator namedToggleValueGenerator = (NamedToggleValueAttribute.NamedToggleValueGenerator)Activator.CreateInstance(attributes[i].ValueGeneratorType);
					foreach (NamedToggleValueAttribute.NamedToggleValue nameToggleValue in namedToggleValueGenerator.GetNameToggleValues())
					{
						list.Add(Activator.CreateInstance(typeFromHandle, nameToggleValue.Name, nameToggleValue.Value));
					}
				}
				else
				{
					list.Add(Activator.CreateInstance(typeFromHandle, attributes[i].Name, attributes[i].Value));
				}
			}
			Array array = Array.CreateInstance(typeFromHandle, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				array.SetValue(list[i], i);
			}
			return array;
		}
	}
}
