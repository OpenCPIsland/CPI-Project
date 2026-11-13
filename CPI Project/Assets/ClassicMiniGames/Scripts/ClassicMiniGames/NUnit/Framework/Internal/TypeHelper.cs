#define DEBUG
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Internal
{
	public class TypeHelper
	{
		public static string GetDisplayName(Type type)
		{
			if (type.IsGenericParameter)
			{
				return type.Name;
			}
			if (type.IsGenericType)
			{
				string text = type.FullName;
				int num = text.IndexOf('[');
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
				num = text.LastIndexOf('.');
				if (num >= 0)
				{
					text = text.Substring(num + 1);
				}
				num = text.IndexOf('`');
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
				StringBuilder stringBuilder = new StringBuilder(text);
				stringBuilder.Append("<");
				int num2 = 0;
				Type[] genericArguments = type.GetGenericArguments();
				foreach (Type type2 in genericArguments)
				{
					if (num2++ > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(GetDisplayName(type2));
				}
				stringBuilder.Append(">");
				return stringBuilder.ToString();
			}
			int num3 = type.FullName.LastIndexOf('.');
			return (num3 >= 0) ? type.FullName.Substring(num3 + 1) : type.FullName;
		}

		public static string GetDisplayName(Type type, object[] arglist)
		{
			string displayName = GetDisplayName(type);
			if (arglist == null || arglist.Length == 0)
			{
				return displayName;
			}
			StringBuilder stringBuilder = new StringBuilder(displayName);
			stringBuilder.Append("(");
			for (int i = 0; i < arglist.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(",");
				}
				object obj = arglist[i];
				string text = (obj == null) ? "null" : obj.ToString();
				if (obj is double || obj is float)
				{
					if (text.IndexOf('.') == -1)
					{
						text += ".0";
					}
					text += ((obj is double) ? "d" : "f");
				}
				else if (obj is decimal)
				{
					text += "m";
				}
				else if (obj is long)
				{
					text += "L";
				}
				else if (obj is ulong)
				{
					text += "UL";
				}
				else if (obj is string)
				{
					text = "\"" + text + "\"";
				}
				stringBuilder.Append(text);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static Type BestCommonType(Type type1, Type type2)
		{
			if (type1 == type2)
			{
				return type1;
			}
			if (type1 == null)
			{
				return type2;
			}
			if (type2 == null)
			{
				return type1;
			}
			if (IsNumeric(type1) && IsNumeric(type2))
			{
				if (type1 == typeof(double))
				{
					return type1;
				}
				if (type2 == typeof(double))
				{
					return type2;
				}
				if (type1 == typeof(float))
				{
					return type1;
				}
				if (type2 == typeof(float))
				{
					return type2;
				}
				if (type1 == typeof(decimal))
				{
					return type1;
				}
				if (type2 == typeof(decimal))
				{
					return type2;
				}
				if (type1 == typeof(ulong))
				{
					return type1;
				}
				if (type2 == typeof(ulong))
				{
					return type2;
				}
				if (type1 == typeof(long))
				{
					return type1;
				}
				if (type2 == typeof(long))
				{
					return type2;
				}
				if (type1 == typeof(uint))
				{
					return type1;
				}
				if (type2 == typeof(uint))
				{
					return type2;
				}
				if (type1 == typeof(int))
				{
					return type1;
				}
				if (type2 == typeof(int))
				{
					return type2;
				}
				if (type1 == typeof(ushort))
				{
					return type1;
				}
				if (type2 == typeof(ushort))
				{
					return type2;
				}
				if (type1 == typeof(short))
				{
					return type1;
				}
				if (type2 == typeof(short))
				{
					return type2;
				}
				if (type1 == typeof(byte))
				{
					return type1;
				}
				if (type2 == typeof(byte))
				{
					return type2;
				}
				if (type1 == typeof(sbyte))
				{
					return type1;
				}
				if (type2 == typeof(sbyte))
				{
					return type2;
				}
			}
			return type1;
		}

		public static bool IsNumeric(Type type)
		{
			return type == typeof(double) || type == typeof(float) || type == typeof(decimal) || type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(ulong) || type == typeof(uint) || type == typeof(ushort) || type == typeof(byte) || type == typeof(sbyte);
		}

		public static void ConvertArgumentList(object[] arglist, ParameterInfo[] parameters)
		{
			Debug.Assert(arglist.Length == parameters.Length);
			for (int i = 0; i < parameters.Length; i++)
			{
				object obj = arglist[i];
				if (obj == null || !(obj is IConvertible))
				{
					continue;
				}
				Type type = obj.GetType();
				Type parameterType = parameters[i].ParameterType;
				bool flag = false;
				if (type != parameterType && !type.IsAssignableFrom(parameterType) && IsNumeric(type) && IsNumeric(parameterType))
				{
					if (parameterType == typeof(double) || parameterType == typeof(float))
					{
						flag = (obj is int || obj is long || obj is short || obj is byte || obj is sbyte);
					}
					else if (parameterType == typeof(long))
					{
						flag = (obj is int || obj is short || obj is byte || obj is sbyte);
					}
					else if (parameterType == typeof(short))
					{
						flag = (obj is byte || obj is sbyte);
					}
				}
				if (flag)
				{
					arglist[i] = Convert.ChangeType(obj, parameterType, CultureInfo.InvariantCulture);
				}
			}
		}

		public static Type MakeGenericType(Type type, Type[] typeArgs)
		{
			return type.MakeGenericType(typeArgs);
		}

		public static bool CanDeduceTypeArgsFromArgs(Type type, object[] arglist, ref Type[] typeArgsOut)
		{
			Type[] genericArguments = type.GetGenericArguments();
			ConstructorInfo[] constructors = type.GetConstructors();
			foreach (ConstructorInfo constructorInfo in constructors)
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (parameters.Length != arglist.Length)
				{
					continue;
				}
				Type[] array = new Type[genericArguments.Length];
				for (int j = 0; j < array.Length; j++)
				{
					for (int k = 0; k < arglist.Length; k++)
					{
						if (parameters[k].ParameterType.Equals(genericArguments[j]))
						{
							array[j] = BestCommonType(array[j], arglist[k].GetType());
						}
					}
					if (array[j] == null)
					{
						array = null;
						break;
					}
				}
				if (array != null)
				{
					typeArgsOut = array;
					return true;
				}
			}
			return false;
		}
	}
}
