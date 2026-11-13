using System;
using System.Text;

namespace Tweaker.Core
{
	public class PrettyPrinter
	{
		public static string PrintObjectArray(object[] objects)
		{
			if (objects == null || objects.Length == 0)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder((objects[0] != null) ? objects[0].ToString() : "");
			for (int i = 1; i < objects.Length; i++)
			{
				stringBuilder.Append(",");
				stringBuilder.Append((objects[i] != null) ? objects[i].ToString() : "");
			}
			return stringBuilder.ToString();
		}

		public static string PrintTypeArray(Type[] types)
		{
			if (types == null || types.Length == 0)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder((types[0] != null) ? types[0].FullName : "");
			for (int i = 1; i < types.Length; i++)
			{
				stringBuilder.Append(",");
				stringBuilder.Append((types[i] != null) ? types[i].FullName : "");
			}
			return stringBuilder.ToString();
		}
	}
}
