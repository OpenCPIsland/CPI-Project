using System.Reflection;
using UnityEngine;

namespace hg.ApiWebKit.converters
{
	public class Escape : IValueConverter
	{
		public object Convert(object input, FieldInfo targetField, out bool successful, params object[] parameters)
		{
			successful = false;
			if (input == null)
			{
				return null;
			}
			try
			{
				string result = WWW.EscapeURL((string)input);
				successful = true;
				return result;
			}
			catch
			{
				return null;
			}
		}
	}
}
