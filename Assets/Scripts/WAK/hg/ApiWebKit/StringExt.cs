namespace hg.ApiWebKit
{
	public static class StringExt
	{
		public static string Truncate(this string value, int maxLength, bool appendSuffix = false)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			string text = (value.Length > maxLength) ? value.Substring(0, maxLength) : value;
			if (value.Length > maxLength && appendSuffix)
			{
				text += "...";
			}
			return text;
		}
	}
}
