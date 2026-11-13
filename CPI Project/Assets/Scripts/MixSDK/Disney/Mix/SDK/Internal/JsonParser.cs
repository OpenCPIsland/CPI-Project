using LitJson;

namespace Disney.Mix.SDK.Internal
{
	public static class JsonParser
	{
		public static string ToJson<T>(T obj)
		{
			return (obj == null) ? "null" : JsonMapper.ToJson(obj);
		}

		public static T FromJson<T>(string json) where T : class
		{
			if (json == null || json == "null")
			{
				return null;
			}
			return JsonMapper.ToObject<T>(json);
		}

		public static T? NullableFromJson<T>(string json) where T : struct
		{
			if (json == null || json == "null")
			{
				return null;
			}
			return JsonMapper.ToObject<T>(json);
		}
	}
}
