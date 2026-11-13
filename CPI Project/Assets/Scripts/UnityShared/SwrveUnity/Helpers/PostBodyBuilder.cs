using System.Text;
using System.Text.RegularExpressions;

namespace SwrveUnity.Helpers
{
	public class PostBodyBuilder
	{
		private const int ApiVersion = 2;

		private static readonly string Format = Regex.Replace("\r\n{{\r\n \"user\":\"{0}\",\r\n \"version\":{1},\r\n \"app_version\":\"{2}\",\r\n \"session_token\":\"{3}\",\r\n \"device_id\":\"{4}\",\r\n \"data\":[{5}]\r\n}}", "\\s", "");

		public static byte[] Build(string apiKey, int appId, string userId, string deviceId, string appVersion, long time, string events)
		{
			string text = CreateSessionToken(apiKey, appId, userId, time);
			string s = string.Format(Format, userId, 2, appVersion, text, deviceId, events);
			return Encoding.UTF8.GetBytes(s);
		}

		private static string CreateSessionToken(string apiKey, int appId, string userId, long time)
		{
			string text = SwrveHelper.ApplyMD5(string.Format("{0}{1}{2}", userId, time, apiKey));
			return string.Format("{0}={1}={2}={3}", appId, userId, time, text);
		}
	}
}
