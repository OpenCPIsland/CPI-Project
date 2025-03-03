namespace ICSharpCode.SharpZipLib.Core
{
	public abstract class WindowsPathUtils
	{
		internal WindowsPathUtils()
		{
		}

		public static string DropPathRoot(string path)
		{
			string text = path;
			if (path != null && path.Length > 0)
			{
				if (path[0] == '\\' || path[0] == '/')
				{
					if (path.Length > 1 && (path[1] == '\\' || path[1] == '/'))
					{
						int i = 2;
						int num = 2;
						for (; i <= path.Length; i++)
						{
							if ((path[i] == '\\' || path[i] == '/') && --num <= 0)
							{
								break;
							}
						}
						i++;
						text = ((i >= path.Length) ? "" : path.Substring(i));
					}
				}
				else if (path.Length > 1 && path[1] == ':')
				{
					int count = 2;
					if (path.Length > 2 && (path[2] == '\\' || path[2] == '/'))
					{
						count = 3;
					}
					text = text.Remove(0, count);
				}
			}
			return text;
		}
	}
}
