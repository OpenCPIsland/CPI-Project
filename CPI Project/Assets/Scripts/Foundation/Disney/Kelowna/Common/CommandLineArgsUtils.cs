using System;
using System.Collections;
using System.Text;

namespace Disney.Kelowna.Common
{
	public static class CommandLineArgsUtils
	{
		public static Hashtable GetCommandLineArgs()
		{
			return GetCommandLineArgs(Environment.GetCommandLineArgs());
		}

		public static Hashtable GetCommandLineArgs(string[] argsArray)
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < argsArray.Length; i++)
			{
				if (argsArray[i].StartsWith("-"))
				{
					if (argsArray[i].Contains("="))
					{
						string[] array = argsArray[i].Split('=');
						hashtable[array[0].Substring(1)] = array[1];
					}
					else if (i + 1 < argsArray.Length && !argsArray[i + 1].StartsWith("-"))
					{
						hashtable[argsArray[i].Substring(1)] = argsArray[i + 1];
					}
					else
					{
						hashtable[argsArray[i].Substring(1)] = true;
					}
				}
			}
			return hashtable;
		}

		public static StringBuilder AppendArg(StringBuilder stringBuilder, string key, object value)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException("stringBuilder");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			stringBuilder.AppendFormat(" -{0} {1}", EscapeSpaces(key), EscapeSpaces(value.ToString()));
			return stringBuilder;
		}

		public static StringBuilder AppendArg(StringBuilder stringBuilder, string key)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException("stringBuilder");
			}
			stringBuilder.AppendFormat(" -{0}", EscapeSpaces(key));
			return stringBuilder;
		}

		public static string EscapeSpaces(string arg)
		{
			if (arg.Contains(" "))
			{
				return string.Format("\"{0}\"", arg);
			}
			return arg;
		}
	}
}
