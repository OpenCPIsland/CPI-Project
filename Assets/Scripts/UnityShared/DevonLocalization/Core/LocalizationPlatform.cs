namespace DevonLocalization.Core
{
	public class LocalizationPlatform
	{
		private static string[] PLATFORMS = new string[4]
		{
			"Global",
			"android",
			"ios",
			"none"
		};

		public static string GetPlatformString(Platform platformEnum)
		{
			return PLATFORMS[(int)platformEnum];
		}
	}
}
