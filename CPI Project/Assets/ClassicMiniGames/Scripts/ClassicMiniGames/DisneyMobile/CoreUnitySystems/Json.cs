using LitJson;

namespace DisneyMobile.CoreUnitySystems
{
	public class Json
	{
		public static object parse(string jsonInput)
		{
			try
			{
				return JsonMapper.ToObjectSimple(jsonInput);
			}
			catch (JsonException inner_exception)
			{
				throw new JsonParseException("Internal JSON parse error:", inner_exception);
			}
		}

		public static string serialize(object input)
		{
			try
			{
				return JsonMapper.ToJson(input);
			}
			catch (JsonException)
			{
				return null;
			}
		}
	}
}
