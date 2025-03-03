using DevonLocalization.Core;
using Disney.MobileNetwork;

public static class LocalizationExtensions
{
	public static string Translate(this string token)
	{
		return Service.Get<Localizer>().GetTokenTranslation(token);
	}
}
