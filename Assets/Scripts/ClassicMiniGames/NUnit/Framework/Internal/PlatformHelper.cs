using System;

namespace NUnit.Framework.Internal
{
	public class PlatformHelper
	{
		private OSPlatform os;

		private RuntimeFramework rt;

		private string reason = string.Empty;

		public static readonly string OSPlatforms = "Win,Win32,Win32S,Win32NT,Win32Windows,WinCE,Win95,Win98,WinMe,NT3,NT4,NT5,NT6,Win2K,WinXP,Win2003Server,Vista,Win2008Server,Win2008ServerR2,Win2012Server,Windows7,Windows8,Unix,Linux";

		public static readonly string RuntimePlatforms = "Net,NetCF,SSCLI,Rotor,Mono";

		public string Reason
		{
			get
			{
				return reason;
			}
		}

		public PlatformHelper()
		{
			os = OSPlatform.CurrentPlatform;
			rt = RuntimeFramework.CurrentFramework;
		}

		public PlatformHelper(OSPlatform os, RuntimeFramework rt)
		{
			this.os = os;
			this.rt = rt;
		}

		public bool IsPlatformSupported(string[] platforms)
		{
			foreach (string platform in platforms)
			{
				if (IsPlatformSupported(platform))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsPlatformSupported(PlatformAttribute platformAttribute)
		{
			string include = platformAttribute.Include;
			string exclude = platformAttribute.Exclude;
			try
			{
				if (include != null && !IsPlatformSupported(include))
				{
					reason = string.Format("Only supported on {0}", include);
					return false;
				}
				if (exclude != null && IsPlatformSupported(exclude))
				{
					reason = string.Format("Not supported on {0}", exclude);
					return false;
				}
			}
			catch (Exception ex)
			{
				reason = ex.Message;
				return false;
			}
			return true;
		}

		public bool IsPlatformSupported(string platform)
		{
			if (platform.IndexOf(',') >= 0)
			{
				return IsPlatformSupported(platform.Split(','));
			}
			string text = platform.Trim();
			bool flag = false;
			switch (text.ToUpper())
			{
			case "WIN":
			case "WIN32":
				flag = os.IsWindows;
				break;
			case "WIN32S":
				flag = os.IsWin32S;
				break;
			case "WIN32WINDOWS":
				flag = os.IsWin32Windows;
				break;
			case "WIN32NT":
				flag = os.IsWin32NT;
				break;
			case "WINCE":
				flag = os.IsWinCE;
				break;
			case "WIN95":
				flag = os.IsWin95;
				break;
			case "WIN98":
				flag = os.IsWin98;
				break;
			case "WINME":
				flag = os.IsWinME;
				break;
			case "NT3":
				flag = os.IsNT3;
				break;
			case "NT4":
				flag = os.IsNT4;
				break;
			case "NT5":
				flag = os.IsNT5;
				break;
			case "WIN2K":
				flag = os.IsWin2K;
				break;
			case "WINXP":
				flag = os.IsWinXP;
				break;
			case "WIN2003SERVER":
				flag = os.IsWin2003Server;
				break;
			case "NT6":
				flag = os.IsNT6;
				break;
			case "VISTA":
				flag = os.IsVista;
				break;
			case "WIN2008SERVER":
				flag = os.IsWin2008Server;
				break;
			case "WIN2008SERVERR2":
				flag = os.IsWin2008ServerR2;
				break;
			case "WIN2012SERVER":
				flag = os.IsWin2012Server;
				break;
			case "WINDOWS7":
				flag = os.IsWindows7;
				break;
			case "WINDOWS8":
				flag = os.IsWindows8;
				break;
			case "UNIX":
			case "LINUX":
				flag = os.IsUnix;
				break;
			default:
				flag = IsRuntimeSupported(text);
				break;
			}
			if (!flag)
			{
				reason = "Only supported on " + platform;
			}
			return flag;
		}

		private bool IsRuntimeSupported(string platformName)
		{
			string versionSpecification = null;
			string[] array = platformName.Split('-');
			if (array.Length == 2)
			{
				platformName = array[0];
				versionSpecification = array[1];
			}
			switch (platformName.ToUpper())
			{
			case "NET":
				return IsRuntimeSupported(RuntimeType.Net, versionSpecification);
			case "NETCF":
				return IsRuntimeSupported(RuntimeType.NetCF, versionSpecification);
			case "SSCLI":
			case "ROTOR":
				return IsRuntimeSupported(RuntimeType.SSCLI, versionSpecification);
			case "MONO":
				return IsRuntimeSupported(RuntimeType.Mono, versionSpecification);
			default:
				throw new ArgumentException("Invalid platform name", platformName);
			}
		}

		private bool IsRuntimeSupported(RuntimeType runtime, string versionSpecification)
		{
			Version version = (versionSpecification == null) ? RuntimeFramework.DefaultVersion : new Version(versionSpecification);
			RuntimeFramework target = new RuntimeFramework(runtime, version);
			return rt.Supports(target);
		}
	}
}
