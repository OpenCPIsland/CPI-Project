using System;

namespace Tweaker.Core
{
	public static class TypeExtensions
	{
		public static bool IsNumericType(this Type type)
		{
			if (type == null)
			{
				return false;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return true;
			case TypeCode.Object:
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return Nullable.GetUnderlyingType(type).IsNumericType();
				}
				return false;
			default:
				return false;
			}
		}
	}
}
