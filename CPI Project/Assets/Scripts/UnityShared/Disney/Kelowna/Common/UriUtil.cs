using System.IO;

namespace Disney.Kelowna.Common
{
	public static class UriUtil
	{
		public static string Combine(params string[] paths)
		{
			string text = paths[0];
			for (int i = 1; i < paths.Length; i++)
			{
				text = Path.Combine(text, paths[i]).Replace(Path.DirectorySeparatorChar, '/');
			}
			return text;
		}
	}
}
