using System;
using System.Reflection;

namespace Tweaker.AssemblyScanner
{
	public class ScanOptions
	{
		public ScanOption<Assembly> Assemblies = new ScanOption<Assembly>();

		public ScanOption<Type> Types = new ScanOption<Type>();

		public ScanOption<Type> Attributes = new ScanOption<Type>();

		public ScanOption<MemberInfo> Members = new ScanOption<MemberInfo>();

		public bool ScanStaticAndInstanceMembers = false;

		public bool CheckMatch(Assembly assembly)
		{
			if (!Assemblies.CheckRefMatch(assembly))
			{
				return false;
			}
			if (!Assemblies.CheckNameMatch(assembly.GetName().Name))
			{
				return false;
			}
			return true;
		}

		public bool CheckMatch(Type type)
		{
			if (!Types.CheckRefMatch(type))
			{
				return false;
			}
			if (!Types.CheckNameMatch(type.FullName))
			{
				return false;
			}
			bool flag = type.IsNested ? type.IsNestedPublic : type.IsPublic;
			if (Types.ScanPublic && flag)
			{
				return true;
			}
			if (Types.ScanNonPublic && !flag)
			{
				return true;
			}
			return false;
		}

		public bool CheckMatch(Attribute attribute)
		{
			Type type = attribute.GetType();
			if (!Attributes.CheckRefMatch(type))
			{
				return false;
			}
			if (!Attributes.CheckNameMatch(type.FullName))
			{
				return false;
			}
			if (Attributes.ScanPublic && type.IsPublic)
			{
				return true;
			}
			if (Attributes.ScanNonPublic && type.IsNotPublic)
			{
				return true;
			}
			return false;
		}

		public bool CheckMatch(MemberInfo member)
		{
			if (!Members.CheckRefMatch(member))
			{
				return false;
			}
			if (!Members.CheckNameMatch(member.Name))
			{
				return false;
			}
			bool flag = true;
			switch (member.MemberType)
			{
			case MemberTypes.Constructor:
				flag = ((ConstructorInfo)member).IsPublic;
				break;
			case MemberTypes.Event:
			{
				Type reflectedType = member.ReflectedType;
				FieldInfo field = reflectedType.GetField(member.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				flag = (field == null || field.IsPublic);
				break;
			}
			case MemberTypes.Field:
				flag = ((FieldInfo)member).IsPublic;
				break;
			case MemberTypes.Method:
				flag = ((MethodInfo)member).IsPublic;
				break;
			case MemberTypes.Property:
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				flag = (propertyInfo.CanRead || propertyInfo.CanWrite);
				break;
			}
			}
			if (Members.ScanPublic && flag)
			{
				return true;
			}
			if (Members.ScanNonPublic && !flag)
			{
				return true;
			}
			return false;
		}
	}
}
