using System;
using System.Reflection;

namespace ClubPenguin.Core.StaticGameData
{
	public class StaticGameDataDefinitionIdAttribute : Attribute
	{
		public static FieldInfo GetAttributedField(Type type)
		{
			FieldInfo[] fields = type.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(StaticGameDataDefinitionIdAttribute), true);
				if (customAttributes.Length > 0)
				{
					return fieldInfo;
				}
			}
			return null;
		}
	}
}
