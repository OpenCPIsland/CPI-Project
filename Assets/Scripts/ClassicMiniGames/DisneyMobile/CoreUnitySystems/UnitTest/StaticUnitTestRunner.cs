namespace DisneyMobile.CoreUnitySystems.UnitTest
{
	public static class StaticUnitTestRunner
	{
		public static void Run(bool failuesAreExceptions)
		{
			NUnitLiteUnityRunner.RunTests(failuesAreExceptions);
		}
	}
}
