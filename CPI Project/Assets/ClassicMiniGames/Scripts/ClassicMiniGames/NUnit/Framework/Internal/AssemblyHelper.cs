using System;
using System.IO;
using System.Reflection;

namespace NUnit.Framework.Internal
{
	public class AssemblyHelper
	{
		public static string GetAssemblyPath(Assembly assembly)
		{
			string codeBase = assembly.CodeBase;
			Uri uri = new Uri(codeBase);
			if (!uri.IsFile)
			{
				return assembly.Location;
			}
			if (uri.IsUnc)
			{
				return codeBase.Substring(Uri.UriSchemeFile.Length + 1);
			}
			int num = Uri.UriSchemeFile.Length + Uri.SchemeDelimiter.Length;
			if (codeBase[num] == '/' && codeBase[num + 2] == ':')
			{
				num++;
			}
			return codeBase.Substring(num);
		}

		public static string GetDirectoryName(Assembly assembly)
		{
			return Path.GetDirectoryName(GetAssemblyPath(assembly));
		}
	}
}
