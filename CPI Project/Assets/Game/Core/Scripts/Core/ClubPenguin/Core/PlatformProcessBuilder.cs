namespace ClubPenguin.Core
{
	public static class PlatformProcessBuilder
	{
		public static IPlatformProcess BuildClientProcess()
		{
			return WindowsProcess.BuildClientProcess();
		}

		public static IPlatformProcess BuildLauncherProcess()
		{
			return WindowsProcess.BuildLauncherProcess();
		}
	}
}
