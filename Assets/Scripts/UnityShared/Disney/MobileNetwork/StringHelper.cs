namespace Disney.MobileNetwork
{
	public static class StringHelper
	{
		public static string ToHexString(byte[] bytes)
		{
			char[] array = new char[bytes.Length * 2];
			for (int i = 0; i < bytes.Length; i++)
			{
				int num = bytes[i] >> 4;
				array[i * 2] = (char)(55 + num + ((num - 10 >> 31) & -7));
				num = (bytes[i] & 0xF);
				array[i * 2 + 1] = (char)(55 + num + ((num - 10 >> 31) & -7));
			}
			return new string(array);
		}
	}
}
