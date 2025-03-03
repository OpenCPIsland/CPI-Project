using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tweaker.Core
{
	public static class MethodInfoExtensions
	{
		public static string GetSignature(this MethodInfo method, bool callable = false)
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			if (!callable)
			{
				if (method.IsPublic)
				{
					stringBuilder.Append("public ");
				}
				else if (method.IsPrivate)
				{
					stringBuilder.Append("private ");
				}
				else if (method.IsAssembly)
				{
					stringBuilder.Append("internal ");
				}
				if (method.IsFamily)
				{
					stringBuilder.Append("protected ");
				}
				if (method.IsStatic)
				{
					stringBuilder.Append("static ");
				}
				stringBuilder.Append(TypeName(method.ReturnType));
				stringBuilder.Append(' ');
			}
			stringBuilder.Append(method.Name);
			if (method.IsGenericMethod)
			{
				stringBuilder.Append("<");
				Type[] genericArguments = method.GetGenericArguments();
				foreach (Type type in genericArguments)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(TypeName(type));
				}
				stringBuilder.Append(">");
			}
			stringBuilder.Append("(");
			flag = true;
			bool flag2 = false;
			ParameterInfo[] parameters = method.GetParameters();
			foreach (ParameterInfo parameterInfo in parameters)
			{
				if (flag)
				{
					flag = false;
					if (method.IsDefined(typeof(ExtensionAttribute), false))
					{
						if (callable)
						{
							flag2 = true;
							continue;
						}
						stringBuilder.Append("this ");
					}
				}
				else if (flag2)
				{
					flag2 = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				if (parameterInfo.ParameterType.IsByRef)
				{
					stringBuilder.Append("ref ");
				}
				else if (parameterInfo.IsOut)
				{
					stringBuilder.Append("out ");
				}
				if (!callable)
				{
					stringBuilder.Append(TypeName(parameterInfo.ParameterType));
					stringBuilder.Append(' ');
				}
				stringBuilder.Append(parameterInfo.Name);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static string TypeName(Type type)
		{
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				return underlyingType.Name + "?";
			}
			if (!type.IsGenericType)
			{
				switch (type.Name)
				{
				case "String":
					return "string";
				case "Int32":
					return "int";
				case "Decimal":
					return "decimal";
				case "Object":
					return "object";
				case "Void":
					return "void";
				default:
					return string.IsNullOrEmpty(type.FullName) ? type.Name : type.FullName;
				}
			}
			StringBuilder stringBuilder = new StringBuilder(type.Name.Substring(0, type.Name.IndexOf('`')));
			stringBuilder.Append('<');
			bool flag = true;
			Type[] genericArguments = type.GetGenericArguments();
			foreach (Type type2 in genericArguments)
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(TypeName(type2));
				flag = false;
			}
			stringBuilder.Append('>');
			return stringBuilder.ToString();
		}
	}
}
