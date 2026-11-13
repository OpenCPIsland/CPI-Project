using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NUnit.Framework.Internal
{
	[Serializable]
	public sealed class RuntimeFramework
	{
		private class FrameworkList : List<RuntimeFramework>
		{
		}

		public static readonly Version DefaultVersion = new Version(0, 0);

		private static RuntimeFramework currentFramework;

		private static RuntimeFramework[] availableFrameworks;

		private static Version[] knownVersions = new Version[4]
		{
			new Version(1, 0, 3705),
			new Version(1, 1, 4322),
			new Version(2, 0, 50727),
			new Version(4, 0, 30319)
		};

		private RuntimeType runtime;

		private Version frameworkVersion;

		private Version clrVersion;

		private string displayName;

		public static RuntimeFramework CurrentFramework
		{
			get
			{
				if (currentFramework == null)
				{
					Type type = Type.GetType("Mono.Runtime", false);
					bool flag = type != null;
					RuntimeType runtimeType = flag ? RuntimeType.Mono : ((Environment.OSVersion.Platform != PlatformID.WinCE) ? RuntimeType.Net : RuntimeType.NetCF);
					int num = Environment.Version.Major;
					int minor = Environment.Version.Minor;
					if (flag)
					{
						switch (num)
						{
						case 1:
							minor = 0;
							break;
						case 2:
							num = 3;
							minor = 5;
							break;
						}
					}
					currentFramework = new RuntimeFramework(runtimeType, new Version(num, minor));
					currentFramework.clrVersion = Environment.Version;
					if (flag)
					{
						MethodInfo method = type.GetMethod("GetDisplayName", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.ExactBinding);
						if (method != null)
						{
							currentFramework.displayName = (string)method.Invoke(null, new object[0]);
						}
					}
				}
				return currentFramework;
			}
		}

		public static RuntimeFramework[] AvailableFrameworks
		{
			get
			{
				if (availableFrameworks == null)
				{
					FrameworkList frameworkList = new FrameworkList();
					AppendDotNetFrameworks(frameworkList);
					AppendDefaultMonoFramework(frameworkList);
					availableFrameworks = frameworkList.ToArray();
				}
				return availableFrameworks;
			}
		}

		public bool IsAvailable
		{
			get
			{
				RuntimeFramework[] array = AvailableFrameworks;
				foreach (RuntimeFramework target in array)
				{
					if (Supports(target))
					{
						return true;
					}
				}
				return false;
			}
		}

		public RuntimeType Runtime
		{
			get
			{
				return runtime;
			}
		}

		public Version FrameworkVersion
		{
			get
			{
				return frameworkVersion;
			}
		}

		public Version ClrVersion
		{
			get
			{
				return clrVersion;
			}
		}

		public bool AllowAnyVersion
		{
			get
			{
				return clrVersion == DefaultVersion;
			}
		}

		public string DisplayName
		{
			get
			{
				return displayName;
			}
		}

		public RuntimeFramework(RuntimeType runtime, Version version)
		{
			this.runtime = runtime;
			if (version.Build < 0)
			{
				InitFromFrameworkVersion(version);
			}
			else
			{
				InitFromClrVersion(version);
			}
			if (version.Major == 3)
			{
				clrVersion = new Version(2, 0, 50727);
			}
			displayName = GetDefaultDisplayName(runtime, version);
		}

		private void InitFromFrameworkVersion(Version version)
		{
			frameworkVersion = (clrVersion = version);
			Version[] array = knownVersions;
			foreach (Version version2 in array)
			{
				if (version2.Major == version.Major && version2.Minor == version.Minor)
				{
					clrVersion = version2;
					break;
				}
			}
			if (runtime == RuntimeType.Mono && version.Major == 1)
			{
				frameworkVersion = new Version(1, 0);
				clrVersion = new Version(1, 1, 4322);
			}
		}

		private void InitFromClrVersion(Version version)
		{
			frameworkVersion = new Version(version.Major, version.Minor);
			clrVersion = version;
			if (runtime == RuntimeType.Mono && version.Major == 1)
			{
				frameworkVersion = new Version(1, 0);
			}
		}

		public static RuntimeFramework Parse(string s)
		{
			RuntimeType runtimeType = RuntimeType.Any;
			Version version = DefaultVersion;
			string[] array = s.Split('-');
			if (array.Length == 2)
			{
				runtimeType = (RuntimeType)Enum.Parse(typeof(RuntimeType), array[0], true);
				string text = array[1];
				if (text != "")
				{
					version = new Version(text);
				}
			}
			else if (char.ToLower(s[0]) == 'v')
			{
				version = new Version(s.Substring(1));
			}
			else if (IsRuntimeTypeName(s))
			{
				runtimeType = (RuntimeType)Enum.Parse(typeof(RuntimeType), s, true);
			}
			else
			{
				version = new Version(s);
			}
			return new RuntimeFramework(runtimeType, version);
		}

		public static RuntimeFramework GetBestAvailableFramework(RuntimeFramework target)
		{
			RuntimeFramework runtimeFramework = target;
			if (target.ClrVersion.Build < 0)
			{
				RuntimeFramework[] array = AvailableFrameworks;
				foreach (RuntimeFramework runtimeFramework2 in array)
				{
					if (runtimeFramework2.Supports(target) && runtimeFramework2.ClrVersion.Build > runtimeFramework.ClrVersion.Build)
					{
						runtimeFramework = runtimeFramework2;
					}
				}
			}
			return runtimeFramework;
		}

		public override string ToString()
		{
			if (AllowAnyVersion)
			{
				return runtime.ToString().ToLower();
			}
			string text = frameworkVersion.ToString();
			if (runtime == RuntimeType.Any)
			{
				return "v" + text;
			}
			return runtime.ToString().ToLower() + "-" + text;
		}

		public bool Supports(RuntimeFramework target)
		{
			if (Runtime != 0 && target.Runtime != 0 && Runtime != target.Runtime)
			{
				return false;
			}
			if (AllowAnyVersion || target.AllowAnyVersion)
			{
				return true;
			}
			return VersionsMatch(ClrVersion, target.ClrVersion) && FrameworkVersion.Major >= target.FrameworkVersion.Major && FrameworkVersion.Minor >= target.FrameworkVersion.Minor;
		}

		private static bool IsRuntimeTypeName(string name)
		{
			string[] names = Enum.GetNames(typeof(RuntimeType));
			foreach (string text in names)
			{
				if (text.ToLower() == name.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		private static string GetDefaultDisplayName(RuntimeType runtime, Version version)
		{
			if (version == DefaultVersion)
			{
				return runtime.ToString();
			}
			if (runtime == RuntimeType.Any)
			{
				return "v" + version.ToString();
			}
			return runtime.ToString() + " " + version.ToString();
		}

		private static bool VersionsMatch(Version v1, Version v2)
		{
			return v1.Major == v2.Major && v1.Minor == v2.Minor && (v1.Build < 0 || v2.Build < 0 || v1.Build == v2.Build) && (v1.Revision < 0 || v2.Revision < 0 || v1.Revision == v2.Revision);
		}

		private static void AppendMonoFrameworks(FrameworkList frameworks)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				AppendAllMonoFrameworks(frameworks);
			}
			else
			{
				AppendDefaultMonoFramework(frameworks);
			}
		}

		private static void AppendAllMonoFrameworks(FrameworkList frameworks)
		{
			AppendDefaultMonoFramework(frameworks);
		}

		private static void AppendDefaultMonoFramework(FrameworkList frameworks)
		{
			string text = null;
			string version = null;
			string directoryName = Path.GetDirectoryName(typeof(object).Assembly.Location);
			text = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(directoryName)));
			AppendMonoFramework(frameworks, text, version);
		}

		private static void AppendMonoFramework(FrameworkList frameworks, string monoPrefix, string version)
		{
			if (monoPrefix != null)
			{
				string format = (version != null) ? ("Mono " + version + " - {0} Profile") : "Mono {0} Profile";
				if (File.Exists(Path.Combine(monoPrefix, "lib/mono/1.0/mscorlib.dll")))
				{
					RuntimeFramework runtimeFramework = new RuntimeFramework(RuntimeType.Mono, new Version(1, 1, 4322));
					runtimeFramework.displayName = string.Format(format, "1.0");
					frameworks.Add(runtimeFramework);
				}
				if (File.Exists(Path.Combine(monoPrefix, "lib/mono/2.0/mscorlib.dll")))
				{
					RuntimeFramework runtimeFramework = new RuntimeFramework(RuntimeType.Mono, new Version(2, 0, 50727));
					runtimeFramework.displayName = string.Format(format, "2.0");
					frameworks.Add(runtimeFramework);
				}
				if (File.Exists(Path.Combine(monoPrefix, "lib/mono/4.0/mscorlib.dll")))
				{
					RuntimeFramework runtimeFramework = new RuntimeFramework(RuntimeType.Mono, new Version(4, 0, 30319));
					runtimeFramework.displayName = string.Format(format, "4.0");
					frameworks.Add(runtimeFramework);
				}
			}
		}

		private static void AppendDotNetFrameworks(FrameworkList frameworks)
		{
		}
	}
}
