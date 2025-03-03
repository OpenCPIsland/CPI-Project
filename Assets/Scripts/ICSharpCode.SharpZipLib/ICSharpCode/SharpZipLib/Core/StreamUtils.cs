using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Core
{
	public sealed class StreamUtils
	{
		public static void ReadFully(Stream stream, byte[] buffer)
		{
			ReadFully(stream, buffer, 0, buffer.Length);
		}

		public static void ReadFully(Stream stream, byte[] buffer, int offset, int count)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || offset + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			while (true)
			{
				if (count > 0)
				{
					int num = stream.Read(buffer, offset, count);
					if (num <= 0)
					{
						break;
					}
					offset += num;
					count -= num;
					continue;
				}
				return;
			}
			throw new EndOfStreamException();
		}

		public static void Copy(Stream source, Stream destination, byte[] buffer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length < 128)
			{
				throw new ArgumentException("Buffer is too small", "buffer");
			}
			bool flag = true;
			while (flag)
			{
				int num = source.Read(buffer, 0, buffer.Length);
				if (num > 0)
				{
					destination.Write(buffer, 0, num);
					continue;
				}
				destination.Flush();
				flag = false;
			}
		}

		public static void Copy(Stream source, Stream destination, byte[] buffer, ProgressHandler progressHandler, TimeSpan updateInterval, object sender, string name)
		{
			Copy(source, destination, buffer, progressHandler, updateInterval, sender, name, -1L);
		}

		public static void Copy(Stream source, Stream destination, byte[] buffer, ProgressHandler progressHandler, TimeSpan updateInterval, object sender, string name, long fixedTarget)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length < 128)
			{
				throw new ArgumentException("Buffer is too small", "buffer");
			}
			if (progressHandler == null)
			{
				throw new ArgumentNullException("progressHandler");
			}
			bool flag = true;
			DateTime now = DateTime.Now;
			long num = 0L;
			long target = 0L;
			if (fixedTarget >= 0)
			{
				target = fixedTarget;
			}
			else if (source.CanSeek)
			{
				target = source.Length - source.Position;
			}
			ProgressEventArgs e = new ProgressEventArgs(name, num, target);
			progressHandler(sender, e);
			bool flag2 = true;
			while (flag)
			{
				int num2 = source.Read(buffer, 0, buffer.Length);
				if (num2 > 0)
				{
					num += num2;
					flag2 = false;
					destination.Write(buffer, 0, num2);
				}
				else
				{
					destination.Flush();
					flag = false;
				}
				if (DateTime.Now - now > updateInterval)
				{
					flag2 = true;
					now = DateTime.Now;
					e = new ProgressEventArgs(name, num, target);
					progressHandler(sender, e);
					flag = e.ContinueRunning;
				}
			}
			if (!flag2)
			{
				e = new ProgressEventArgs(name, num, target);
				progressHandler(sender, e);
			}
		}

		private StreamUtils()
		{
		}
	}
}
