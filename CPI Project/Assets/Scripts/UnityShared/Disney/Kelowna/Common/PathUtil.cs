using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Disney.Kelowna.Common
{
	public static class PathUtil
	{
		public static string MakeRelative(string basePath, string targetPath)
		{
			if (string.IsNullOrEmpty(basePath))
			{
				throw new ArgumentNullException("basePath");
			}
			if (string.IsNullOrEmpty(targetPath))
			{
				throw new ArgumentNullException("targetPath");
			}
			Uri uri = new Uri(basePath);
			Uri uri2 = new Uri(targetPath);
			if (uri.Scheme != uri2.Scheme)
			{
				return targetPath;
			}
			Uri uri3 = uri.MakeRelativeUri(uri2);
			string text = Uri.UnescapeDataString(uri3.ToString());
			if (uri2.Scheme.ToUpperInvariant() == "FILE")
			{
				text = text.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}
			return text;
		}

		public static IEnumerable<FileInfo> GetFiles(string rootDirectory, IEnumerable<string> includes, IEnumerable<string> excludes, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(rootDirectory);
			if (!directoryInfo.Exists)
			{
				throw new ArgumentException("Directory does not exist: " + rootDirectory, "rootDirectory");
			}
			FileInfo[] files = directoryInfo.GetFiles(searchPattern, searchOption);
			return from file in files
				where includes == null || includes.Any(file.FullName.Contains)
				where excludes == null || !excludes.Any(file.FullName.Contains)
				select file;
		}

		public static string Combine(params string[] paths)
		{
			string text = paths[0];
			for (int i = 1; i < paths.Length; i++)
			{
				text = Path.Combine(text, paths[i].TrimStart(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
			}
			return text;
		}

		public static string EnsureTrailingSlash(string path)
		{
			string text = Path.DirectorySeparatorChar.ToString();
			string text2 = Path.AltDirectorySeparatorChar.ToString();
			path = path.Trim();
			if (path.EndsWith(text) || path.EndsWith(text2))
			{
				return path;
			}
			if (path.Contains(text2))
			{
				return path + text2;
			}
			return path + text;
		}

		public static string GetParentPath(string path)
		{
			return Directory.GetParent(path).ToString();
		}

		public static List<string> SplitPathName(string pathName)
		{
			List<string> list = new List<string>();
			while (!string.IsNullOrEmpty(pathName))
			{
				string fileName = Path.GetFileName(pathName);
				pathName = Path.GetDirectoryName(pathName);
				list.Insert(0, fileName);
			}
			return list;
		}

		public static void HardDeleteFilesAndSubdirectories(string parentDirectory)
		{
			if (Directory.Exists(parentDirectory))
			{
				string[] files = Directory.GetFiles(parentDirectory, "*", SearchOption.AllDirectories);
				foreach (string text in files)
				{
					new FileInfo(text).IsReadOnly = false;
					File.Delete(text);
				}
				files = Directory.GetDirectories(parentDirectory);
				foreach (string path in files)
				{
					Directory.Delete(path, true);
				}
			}
		}
	}
}
