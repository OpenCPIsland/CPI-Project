using System.IO;

namespace Disney.Mix.SDK.Internal
{
	public static class StreamExtensions
	{
		private static readonly byte[] copyBuffer = new byte[16384];

		public static void CopyTo(this Stream inStream, Stream outStream)
		{
			lock (copyBuffer)
			{
				int count;
				while ((count = inStream.Read(copyBuffer, 0, copyBuffer.Length)) > 0)
				{
					outStream.Write(copyBuffer, 0, count);
				}
			}
		}
	}
}
