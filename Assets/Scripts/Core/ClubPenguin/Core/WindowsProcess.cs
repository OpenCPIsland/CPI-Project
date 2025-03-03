using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace ClubPenguin.Core
{
	public class WindowsProcess : IPlatformProcess
	{
		private const string CLIENT_REGISTRY_KEY = "HKEY_LOCAL_MACHINE\\SOFTWARE\\OpenCPI\\Club Penguin Island";

		private const string CLIENT_WOW6432_REGISTRY_KEY = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\OpenCPI\\Club Penguin Island";

		private const string LAUNCHER_REGISTRY_KEY = "HKEY_LOCAL_MACHINE\\SOFTWARE\\OpenCPI\\Club Penguin Island Launcher";

		private const string LAUNCHER_WOW6432_REGISTRY_KEY = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\OpenCPI\\Club Penguin Island Launcher";

		private const string EXE_REGISTRY_VALUE = "InstallExePath";

		private const string PATH_REGISTRY_VALUE = "InstallPath";

		private Process process;

		private string workingDirectory;

		private string executableFilename;

		public static IPlatformProcess BuildClientProcess()
		{
			WindowsProcess windowsProcess = new WindowsProcess();
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\OpenCPI\\Club Penguin Island";
			string keyName2 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\OpenCPI\\Club Penguin Island";
			object value = Registry.GetValue(keyName, "InstallPath", null);
			if (value == null)
			{
				value = Registry.GetValue(keyName2, "InstallPath", null);
			}
			windowsProcess.workingDirectory = (string)value;
			value = Registry.GetValue(keyName, "InstallExePath", null);
			if (value == null)
			{
				value = Registry.GetValue(keyName2, "InstallExePath", null);
			}
			windowsProcess.executableFilename = (string)value;
			return windowsProcess;
		}

		public static IPlatformProcess BuildLauncherProcess()
		{
			WindowsProcess windowsProcess = new WindowsProcess();
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\OpenCPI\\Club Penguin Island Launcher";
			string keyName2 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\OpenCPI\\Club Penguin Island Launcher";
			object value = Registry.GetValue(keyName, "InstallPath", null);
			if (value == null)
			{
				value = Registry.GetValue(keyName2, "InstallPath", null);
			}
			windowsProcess.workingDirectory = (string)value;
			value = Registry.GetValue(keyName, "InstallExePath", null);
			if (value == null)
			{
				value = Registry.GetValue(keyName2, "InstallExePath", null);
			}
			windowsProcess.executableFilename = (string)value;
			return windowsProcess;
		}

		private WindowsProcess()
		{
		}

		public void Execute()
		{
			if (string.IsNullOrEmpty(executableFilename))
			{
				string message = "WindowsProcess.Execute(): executableFilename MUST be specified!";
				throw new Exception(message);
			}
			process = new Process();
			process.EnableRaisingEvents = true;
			process.StartInfo.UseShellExecute = false;
			if (!string.IsNullOrEmpty(workingDirectory))
			{
				process.StartInfo.WorkingDirectory = workingDirectory;
			}
			process.StartInfo.FileName = executableFilename;
			process.Start();
		}
	}
}
