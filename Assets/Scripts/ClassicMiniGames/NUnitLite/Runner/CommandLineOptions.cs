using NUnit;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NUnitLite.Runner
{
	public class CommandLineOptions
	{
		private class StringList : List<string>
		{
		}

		private string optionChars;

		private static string NL = Env.NewLine;

		private bool wait = false;

		private bool noheader = false;

		private bool help = false;

		private bool full = false;

		private bool explore = false;

		private string exploreFile;

		private string resultFile;

		private string outFile;

		private bool error = false;

		private StringList tests = new StringList();

		private StringList invalidOptions = new StringList();

		private StringList parameters = new StringList();

		public bool Wait
		{
			get
			{
				return wait;
			}
		}

		public bool NoHeader
		{
			get
			{
				return noheader;
			}
		}

		public bool ShowHelp
		{
			get
			{
				return help;
			}
		}

		public string[] Tests
		{
			get
			{
				return tests.ToArray();
			}
		}

		public bool Full
		{
			get
			{
				return full;
			}
		}

		public bool Explore
		{
			get
			{
				return explore;
			}
		}

		public string ExploreFile
		{
			get
			{
				return ExpandToFullPath(exploreFile);
			}
		}

		public string ResultFile
		{
			get
			{
				return ExpandToFullPath(resultFile);
			}
		}

		public string OutFile
		{
			get
			{
				return ExpandToFullPath(outFile);
			}
		}

		public int TestCount
		{
			get
			{
				return tests.Count;
			}
		}

		public string[] Parameters
		{
			get
			{
				return parameters.ToArray();
			}
		}

		public bool Error
		{
			get
			{
				return error;
			}
		}

		public string ErrorMessage
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string invalidOption in invalidOptions)
				{
					stringBuilder.Append("Invalid option: " + invalidOption + NL);
				}
				return stringBuilder.ToString();
			}
		}

		public string HelpText
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				string text = "NUnitLite";
				stringBuilder.Append(NL + text + " [assemblies] [options]" + NL + NL);
				stringBuilder.Append("Runs a set of NUnitLite tests from the console." + NL + NL);
				stringBuilder.Append("You may specify one or more test assemblies by name, without a path or" + NL);
				stringBuilder.Append("extension. They must be in the same in the same directory as the exe" + NL);
				stringBuilder.Append("or on the probing path. If no assemblies are provided, tests in the" + NL);
				stringBuilder.Append("executing assembly itself are run." + NL + NL);
				stringBuilder.Append("Options:" + NL);
				stringBuilder.Append("  -test:testname  Provides the name of a test to run. This option may be" + NL);
				stringBuilder.Append("                  repeated. If no test names are given, all tests are run." + NL + NL);
				stringBuilder.Append("  -out:PATH       Path to a file to which output is redirected. If this option" + NL);
				stringBuilder.Append("                  is not used, output is to the Console, which means it is lost" + NL);
				stringBuilder.Append("                  on devices without a Console." + NL + NL);
				stringBuilder.Append("  -full           Prints full report of all test results." + NL + NL);
				stringBuilder.Append("  -result:PATH    Path to a file to which the xml test result is written." + NL + NL);
				stringBuilder.Append("  -explore[:PATH] If provided, this option indicates that the tests in the" + NL);
				stringBuilder.Append("                  should be listed rather than executed. If a path is given" + NL);
				stringBuilder.Append("                  it represents the file to which the listing is directed." + NL);
				stringBuilder.Append("                  If no path is given, the listing displays on the Console." + NL + NL);
				stringBuilder.Append("  -help,-h        Displays this help" + NL + NL);
				stringBuilder.Append("  -noheader,-noh  Suppresses display of the initial message" + NL + NL);
				stringBuilder.Append("  -wait           Waits for a key press before exiting" + NL + NL);
				stringBuilder.Append("Notes:" + NL);
				stringBuilder.Append(" * Any relative path is based on the current directory or on the Documents" + NL);
				stringBuilder.Append("   folder if running on a device under the compact framework." + NL + NL);
				if (Path.DirectorySeparatorChar != '/')
				{
					stringBuilder.Append(" * On Windows, options may be prefixed by a '/' character if desired" + NL + NL);
				}
				stringBuilder.Append(" * Options that take values may use an equal sign or a colon" + NL);
				stringBuilder.Append("   to separate the option from its value." + NL + NL);
				return stringBuilder.ToString();
			}
		}

		private string ExpandToFullPath(string path)
		{
			if (path == null)
			{
				return null;
			}
			return Path.GetFullPath(path);
		}

		public CommandLineOptions()
		{
			optionChars = ((Path.DirectorySeparatorChar == '/') ? "-" : "/-");
		}

		public CommandLineOptions(string optionChars)
		{
			this.optionChars = optionChars;
		}

		public void Parse(params string[] args)
		{
			foreach (string text in args)
			{
				if (optionChars.IndexOf(text[0]) >= 0)
				{
					ProcessOption(text);
				}
				else
				{
					ProcessParameter(text);
				}
			}
		}

		private void ProcessOption(string opt)
		{
			int num = opt.IndexOfAny(new char[2]
			{
				':',
				'='
			});
			string item = string.Empty;
			if (num >= 0)
			{
				item = opt.Substring(num + 1);
				opt = opt.Substring(0, num);
			}
			switch (opt.Substring(1))
			{
			case "wait":
				wait = true;
				break;
			case "noheader":
			case "noh":
				noheader = true;
				break;
			case "help":
			case "h":
				help = true;
				break;
			case "test":
				tests.Add(item);
				break;
			case "full":
				full = true;
				break;
			case "explore":
				explore = true;
				exploreFile = item;
				break;
			case "result":
				resultFile = item;
				break;
			case "out":
				outFile = item;
				break;
			default:
				error = true;
				invalidOptions.Add(opt);
				break;
			}
		}

		private void ProcessParameter(string param)
		{
			parameters.Add(param);
		}
	}
}
