namespace ClubPenguin
{
	public static class InWorldExportValue
	{
		private const int DECIMAL_PLACES = 4;

		public static bool TryConvert(float floatValue, out decimal decimalValue)
		{
			try
			{
				decimalValue = decimal.Round((decimal)floatValue, 4);
			}
			catch
			{
				decimalValue = 0m;
				return false;
			}
			return true;
		}
	}
}
