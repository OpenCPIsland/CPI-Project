using LitJson;

namespace Disney.MobileNetwork
{
	public class LPFJson
	{
		public static object parse(string jsonInput)
		{
			try
			{
				return LPFJsonMapper.ToObjectSimple(jsonInput);
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
