using System.IO;

namespace NUnit.Framework.Constraints
{
	public abstract class PathConstraint : Constraint
	{
		private static readonly char[] DirectorySeparatorChars = new char[2]
		{
			'\\',
			'/'
		};

		protected string expected;

		protected bool caseInsensitive = Path.DirectorySeparatorChar == '\\';

		public PathConstraint IgnoreCase
		{
			get
			{
				caseInsensitive = true;
				return this;
			}
		}

		public PathConstraint RespectCase
		{
			get
			{
				caseInsensitive = false;
				return this;
			}
		}

		protected PathConstraint(string expected)
			: base(expected)
		{
			this.expected = expected;
		}

		protected override string GetStringRepresentation()
		{
			return string.Format("<{0} \"{1}\" {2}>", base.DisplayName, expected, caseInsensitive ? "ignorecase" : "respectcase");
		}

		protected string Canonicalize(string path)
		{
			string[] array = path.Split(DirectorySeparatorChars);
			int num = 0;
			bool flag = false;
			string[] array2 = array;
			foreach (string text in array2)
			{
				switch (text)
				{
				case ".":
				case "":
					flag = true;
					break;
				case "..":
					flag = true;
					if (num > 0)
					{
						num--;
					}
					break;
				default:
					if (flag)
					{
						array[num] = text;
					}
					num++;
					break;
				}
			}
			return string.Join(Path.DirectorySeparatorChar.ToString(), array, 0, num);
		}

		protected bool IsSamePath(string path1, string path2)
		{
			return string.Compare(Canonicalize(expected), Canonicalize((string)actual), caseInsensitive) == 0;
		}

		protected bool IsSamePathOrUnder(string path1, string path2)
		{
			path1 = Canonicalize(path1);
			path2 = Canonicalize(path2);
			int length = path1.Length;
			int length2 = path2.Length;
			if (length > length2)
			{
				return false;
			}
			if (length == length2)
			{
				return string.Compare(path1, path2, caseInsensitive) == 0;
			}
			if (string.Compare(path1, path2.Substring(0, length), caseInsensitive) != 0)
			{
				return false;
			}
			return path2[length - 1] == Path.DirectorySeparatorChar || path2[length] == Path.DirectorySeparatorChar;
		}
	}
}
