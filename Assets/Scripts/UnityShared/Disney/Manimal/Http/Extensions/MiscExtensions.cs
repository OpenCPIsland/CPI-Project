using System.IO;
using System.Text;

namespace Disney.Manimal.Http.Extensions
{
	public static class MiscExtensions
	{
		public static void SaveAs(this byte[] input, string path)
		{
			File.WriteAllBytes(path, input);
		}

		public static byte[] ReadAsBytes(this Stream input)
		{
			byte[] array = new byte[16384];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int count;
				while ((count = input.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
				return memoryStream.ToArray();
			}
		}

		public static void CopyTo(this Stream input, Stream output)
		{
			byte[] array = new byte[32768];
			while (true)
			{
				bool flag = true;
				int num = input.Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				output.Write(array, 0, num);
			}
		}

		public static string AsString(this byte[] buffer)
		{
			if (buffer == null)
			{
				return "";
			}
			Encoding uTF = Encoding.UTF8;
			return uTF.GetString(buffer, 0, buffer.Length);
		}
	}
}
