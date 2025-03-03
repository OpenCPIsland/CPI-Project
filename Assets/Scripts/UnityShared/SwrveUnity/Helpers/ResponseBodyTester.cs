using System;
using System.Text;

namespace SwrveUnity.Helpers
{
	public class ResponseBodyTester
	{
		public static bool TestUTF8(string data, out string decodedString)
		{
			return TestUTF8(Encoding.UTF8.GetBytes(data), out decodedString);
		}

		public static bool TestUTF8(byte[] bodyBytes, out string decodedString)
		{
			try
			{
				decodedString = Encoding.UTF8.GetString(bodyBytes, 0, bodyBytes.Length);
				return true;
			}
			catch (Exception)
			{
				decodedString = string.Empty;
			}
			return false;
		}
	}
}
