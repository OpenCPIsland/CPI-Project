using System;
using System.Runtime.InteropServices;

namespace NUnit.Framework.Internal
{
	public class OSPlatform
	{
		public enum ProductType
		{
			Unknown,
			WorkStation,
			DomainController,
			Server
		}

		private struct OSVERSIONINFOEX
		{
			public uint dwOSVersionInfoSize;

			public uint dwMajorVersion;

			public uint dwMinorVersion;

			public uint dwBuildNumber;

			public uint dwPlatformId;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;

			public short wServicePackMajor;

			public short wServicePackMinor;

			public short wSuiteMask;

			public byte ProductType;

			public byte Reserved;
		}

		private PlatformID platform;

		private Version version;

		private ProductType product;

		private static OSPlatform currentPlatform;

		public static readonly PlatformID UnixPlatformID_Microsoft = PlatformID.Unix;

		public static readonly PlatformID UnixPlatformID_Mono = (PlatformID)128;

		public static OSPlatform CurrentPlatform
		{
			get
			{
				if (currentPlatform == null)
				{
					OperatingSystem oSVersion = Environment.OSVersion;
					if (oSVersion.Platform == PlatformID.Win32NT && oSVersion.Version.Major >= 5)
					{
						OSVERSIONINFOEX osvi = default(OSVERSIONINFOEX);
						osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
						GetVersionEx(ref osvi);
						currentPlatform = new OSPlatform(oSVersion.Platform, oSVersion.Version, (ProductType)osvi.ProductType);
					}
					else
					{
						currentPlatform = new OSPlatform(oSVersion.Platform, oSVersion.Version);
					}
				}
				return currentPlatform;
			}
		}

		public PlatformID Platform
		{
			get
			{
				return platform;
			}
		}

		public Version Version
		{
			get
			{
				return version;
			}
		}

		public ProductType Product
		{
			get
			{
				return product;
			}
		}

		public bool IsWindows
		{
			get
			{
				return platform == PlatformID.Win32NT || platform == PlatformID.Win32Windows || platform == PlatformID.Win32S || platform == PlatformID.WinCE;
			}
		}

		public bool IsUnix
		{
			get
			{
				return platform == UnixPlatformID_Microsoft || platform == UnixPlatformID_Mono;
			}
		}

		public bool IsWin32S
		{
			get
			{
				return platform == PlatformID.Win32S;
			}
		}

		public bool IsWin32Windows
		{
			get
			{
				return platform == PlatformID.Win32Windows;
			}
		}

		public bool IsWin32NT
		{
			get
			{
				return platform == PlatformID.Win32NT;
			}
		}

		public bool IsWinCE
		{
			get
			{
				return platform == PlatformID.WinCE;
			}
		}

		public bool IsWin95
		{
			get
			{
				return platform == PlatformID.Win32Windows && version.Major == 4 && version.Minor == 0;
			}
		}

		public bool IsWin98
		{
			get
			{
				return platform == PlatformID.Win32Windows && version.Major == 4 && version.Minor == 10;
			}
		}

		public bool IsWinME
		{
			get
			{
				return platform == PlatformID.Win32Windows && version.Major == 4 && version.Minor == 90;
			}
		}

		public bool IsNT3
		{
			get
			{
				return platform == PlatformID.Win32NT && version.Major == 3;
			}
		}

		public bool IsNT4
		{
			get
			{
				return platform == PlatformID.Win32NT && version.Major == 4;
			}
		}

		public bool IsNT5
		{
			get
			{
				return platform == PlatformID.Win32NT && version.Major == 5;
			}
		}

		public bool IsWin2K
		{
			get
			{
				return IsNT5 && version.Minor == 0;
			}
		}

		public bool IsWinXP
		{
			get
			{
				return IsNT5 && (version.Minor == 1 || (version.Minor == 2 && Product == ProductType.WorkStation));
			}
		}

		public bool IsWin2003Server
		{
			get
			{
				return IsNT5 && version.Minor == 2 && Product == ProductType.Server;
			}
		}

		public bool IsNT6
		{
			get
			{
				return platform == PlatformID.Win32NT && version.Major == 6;
			}
		}

		public bool IsVista
		{
			get
			{
				return IsNT6 && version.Minor == 0 && Product == ProductType.WorkStation;
			}
		}

		public bool IsWin2008Server
		{
			get
			{
				return IsNT6 && Product == ProductType.Server;
			}
		}

		public bool IsWin2008ServerR1
		{
			get
			{
				return IsNT6 && version.Minor == 0 && Product == ProductType.Server;
			}
		}

		public bool IsWin2008ServerR2
		{
			get
			{
				return IsNT6 && version.Minor == 1 && Product == ProductType.Server;
			}
		}

		public bool IsWin2012Server
		{
			get
			{
				return IsNT6 && version.Minor == 2 && Product == ProductType.Server;
			}
		}

		public bool IsWindows7
		{
			get
			{
				return IsNT6 && version.Minor == 1 && Product == ProductType.WorkStation;
			}
		}

		public bool IsWindows8
		{
			get
			{
				return IsNT6 && version.Minor == 8 && Product == ProductType.WorkStation;
			}
		}

		[DllImport("Kernel32.dll")]
		private static extern bool GetVersionEx(ref OSVERSIONINFOEX osvi);

		public OSPlatform(PlatformID platform, Version version)
		{
			this.platform = platform;
			this.version = version;
		}

		public OSPlatform(PlatformID platform, Version version, ProductType product)
			: this(platform, version)
		{
			this.product = product;
		}
	}
}
