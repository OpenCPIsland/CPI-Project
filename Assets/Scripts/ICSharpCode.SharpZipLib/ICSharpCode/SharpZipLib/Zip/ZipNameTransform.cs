using ICSharpCode.SharpZipLib.Core;
using System;
using System.IO;
using System.Text;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class ZipNameTransform : INameTransform
	{
		private string trimPrefix_;

		private static readonly char[] InvalidEntryChars;

		private static readonly char[] InvalidEntryCharsRelaxed;

		public string TrimPrefix
		{
			get
			{
				return trimPrefix_;
			}
			set
			{
				trimPrefix_ = value;
				if (trimPrefix_ != null)
				{
					trimPrefix_ = trimPrefix_.ToLower();
				}
			}
		}

		public ZipNameTransform()
		{
		}

		public ZipNameTransform(string trimPrefix)
		{
			TrimPrefix = trimPrefix;
		}

		static ZipNameTransform()
		{
			char[] invalidPathChars = Path.GetInvalidPathChars();
			int num = invalidPathChars.Length + 2;
			InvalidEntryCharsRelaxed = new char[num];
			Array.Copy(invalidPathChars, 0, InvalidEntryCharsRelaxed, 0, invalidPathChars.Length);
			InvalidEntryCharsRelaxed[num - 1] = '*';
			InvalidEntryCharsRelaxed[num - 2] = '?';
			num = invalidPathChars.Length + 4;
			InvalidEntryChars = new char[num];
			Array.Copy(invalidPathChars, 0, InvalidEntryChars, 0, invalidPathChars.Length);
			InvalidEntryChars[num - 1] = ':';
			InvalidEntryChars[num - 2] = '\\';
			InvalidEntryChars[num - 3] = '*';
			InvalidEntryChars[num - 4] = '?';
		}

		public string TransformDirectory(string name)
		{
			name = TransformFile(name);
			if (name.Length > 0)
			{
				if (!name.EndsWith("/"))
				{
					name += "/";
				}
				return name;
			}
			throw new ZipException("Cannot have an empty directory name");
		}

		public string TransformFile(string name)
		{
			if (name != null)
			{
				string text = name.ToLower();
				if (trimPrefix_ != null && text.IndexOf(trimPrefix_) == 0)
				{
					name = name.Substring(trimPrefix_.Length);
				}
				name = name.Replace("\\", "/");
				name = WindowsPathUtils.DropPathRoot(name);
				while (name.Length > 0 && name[0] == '/')
				{
					name = name.Remove(0, 1);
				}
				while (name.Length > 0 && name[name.Length - 1] == '/')
				{
					name = name.Remove(name.Length - 1, 1);
				}
				for (int num = name.IndexOf("//"); num >= 0; num = name.IndexOf("//"))
				{
					name = name.Remove(num, 1);
				}
				name = MakeValidName(name, '_');
			}
			else
			{
				name = string.Empty;
			}
			return name;
		}

		private static string MakeValidName(string name, char replacement)
		{
			int num = name.IndexOfAny(InvalidEntryChars);
			if (num >= 0)
			{
				StringBuilder stringBuilder = new StringBuilder(name);
				while (num >= 0)
				{
					stringBuilder[num] = replacement;
					num = ((num < name.Length) ? name.IndexOfAny(InvalidEntryChars, num + 1) : (-1));
				}
				name = stringBuilder.ToString();
			}
			if (name.Length > 65535)
			{
				throw new PathTooLongException();
			}
			return name;
		}

		public static bool IsValidName(string name, bool relaxed)
		{
			bool flag = name != null;
			if (flag)
			{
				flag = ((!relaxed) ? (name.IndexOfAny(InvalidEntryChars) < 0 && name.IndexOf('/') != 0) : (name.IndexOfAny(InvalidEntryCharsRelaxed) < 0));
			}
			return flag;
		}

		public static bool IsValidName(string name)
		{
			return name != null && name.IndexOfAny(InvalidEntryChars) < 0 && name.IndexOf('/') != 0;
		}
	}
}
