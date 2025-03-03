namespace Disney.LaunchPadFramework.Utility
{
	public static class StringHelper
	{
		private static string numberToStringFormatCode = "N";

		private static string numberToStringFloatPrecisionCode = "2";

		private static string numberToStringIntPrecisionCode = "0";

		public static string GetFloatAsFormattedString(float amount)
		{
			string str = numberToStringFormatCode;
			str = ((!(amount % 1f > 0f)) ? (str + numberToStringIntPrecisionCode) : (str + numberToStringFloatPrecisionCode));
			return amount.ToString(str);
		}
	}
}
