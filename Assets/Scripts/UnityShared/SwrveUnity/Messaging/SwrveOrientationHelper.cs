namespace SwrveUnity.Messaging
{
	public static class SwrveOrientationHelper
	{
		private const string PORTRAIT_KEY = "portrait";

		private const string LANDSCAPE_KEY = "landscape";

		private const string BOTH_KEY = "both";

		public static SwrveOrientation Parse(string orientation)
		{
			if (orientation.ToLower().Equals("portrait"))
			{
				return SwrveOrientation.Portrait;
			}
			if (orientation.ToLower().Equals("both"))
			{
				return SwrveOrientation.Both;
			}
			return SwrveOrientation.Landscape;
		}
	}
}
