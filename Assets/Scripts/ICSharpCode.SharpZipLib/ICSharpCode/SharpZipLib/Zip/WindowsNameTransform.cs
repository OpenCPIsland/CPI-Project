using ICSharpCode.SharpZipLib.Core;
using System;
using System.IO;
using System.Text;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class WindowsNameTransform : INameTransform
	{
		private const int MaxPath = 260;

		private string _baseDirectory;

		private bool _trimIncomingPaths;

		private char _replacementChar = '_';

		private static readonly char[] InvalidEntryChars;

		public string BaseDirectory
		{
			get
			{
				return _baseDirectory;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_baseDirectory = Path.GetFullPath(value);
			}
		}

		public bool TrimIncomingPaths
		{
			get
			{
				return _trimIncomingPaths;
			}
			set
			{
				_trimIncomingPaths = value;
			}
		}

		public char Replacement
		{
			get
			{
				return _replacementChar;
			}
			set
			{
				for (int i = 0; i < InvalidEntryChars.Length; i++)
				{
					if (InvalidEntryChars[i] == value)
					{
						throw new ArgumentException("invalid path character");
					}
				}
				if (value == '\\' || value == '/')
				{
					throw new ArgumentException("invalid replacement character");
				}
				_replacementChar = value;
			}
		}

		public WindowsNameTransform(string baseDirectory)
		{
			if (baseDirectory == null)
			{
				throw new ArgumentNullException("baseDirectory", "Directory name is invalid");
			}
			BaseDirectory = baseDirectory;
		}

		public WindowsNameTransform()
		{
		}

		public string TransformDirectory(string name)
		{
			name = TransformFile(name);
			if (name.Length > 0)
			{
				while (name.EndsWith("\\"))
				{
					name = name.Remove(name.Length - 1, 1);
				}
				return name;
			}
			throw new ZipException("Cannot have an empty directory name");
		}

		public string TransformFile(string name)
		{
			if (name != null)
			{
				name = MakeValidName(name, _replacementChar);
				if (_trimIncomingPaths)
				{
					name = Path.GetFileName(name);
				}
				if (_baseDirectory != null)
				{
					name = Path.Combine(_baseDirectory, name);
				}
			}
			else
			{
				name = string.Empty;
			}
			return name;
		}

		public static bool IsValidName(string name)
		{
			return name != null && name.Length <= 260 && string.Compare(name, MakeValidName(name, '_')) == 0;
		}

		static WindowsNameTransform()
		{
			char[] invalidPathChars = Path.GetInvalidPathChars();
			int num = invalidPathChars.Length + 3;
			InvalidEntryChars = new char[num];
			Array.Copy(invalidPathChars, 0, InvalidEntryChars, 0, invalidPathChars.Length);
			InvalidEntryChars[num - 1] = '*';
			InvalidEntryChars[num - 2] = '?';
			InvalidEntryChars[num - 3] = ':';
		}

		public static string MakeValidName(string name, char replacement)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			name = WindowsPathUtils.DropPathRoot(name.Replace("/", "\\"));
			while (name.Length > 0 && name[0] == '\\')
			{
				name = name.Remove(0, 1);
			}
			while (name.Length > 0 && name[name.Length - 1] == '\\')
			{
				name = name.Remove(name.Length - 1, 1);
			}
			int num;
			for (num = name.IndexOf("\\\\"); num >= 0; num = name.IndexOf("\\\\"))
			{
				name = name.Remove(num, 1);
			}
			num = name.IndexOfAny(InvalidEntryChars);
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
			if (name.Length > 260)
			{
				throw new PathTooLongException();
			}
			return name;
		}
	}
}
