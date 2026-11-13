using NUnit.Framework;

namespace JetpackReboot
{
	public static class mg_jr_EnvironmentExtensions
	{
		public static string FileNameFragment(this EnvironmentType _environment, EnvironmentVariant _variant)
		{
			string result = "";
			switch (_environment)
			{
			case EnvironmentType.CAVE:
				result = "cave";
				break;
			case EnvironmentType.FOREST:
				result = ((_variant != EnvironmentVariant.NIGHT) ? "forest_day" : "forest_night");
				break;
			case EnvironmentType.TOWN:
				result = ((_variant != EnvironmentVariant.NIGHT) ? "town_day" : "town_night");
				break;
			case EnvironmentType.WATER:
				result = ((_variant != EnvironmentVariant.NIGHT) ? "water_day" : "water_night");
				break;
			default:
				Assert.Fail(string.Concat("No case for environment '", _environment, "'"));
				break;
			}
			return result;
		}
	}
}
