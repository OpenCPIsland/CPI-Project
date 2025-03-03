using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace HTTP
{
	public class Response
	{
		public Request request;

		public int status = 200;

		public string message = "OK";

		public byte[] bytes;

		private List<byte[]> chunks;

		private Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();

		public string Text
		{
			get
			{
				if (bytes == null)
				{
					return "";
				}
				return Encoding.UTF8.GetString(bytes);
			}
		}

		public string Asset
		{
			get
			{
				throw new NotSupportedException("This can't be done, yet.");
			}
		}

		public Hashtable Object
		{
			get
			{
				if (bytes == null)
				{
					return null;
				}
				bool success = false;
				Hashtable result = (Hashtable)JSON.JsonDecode(Text, ref success);
				if (!success)
				{
					result = null;
				}
				return result;
			}
		}

		public ArrayList Array
		{
			get
			{
				if (bytes == null)
				{
					return null;
				}
				bool success = false;
				ArrayList result = (ArrayList)JSON.JsonDecode(Text, ref success);
				if (!success)
				{
					result = null;
				}
				return result;
			}
		}

		private void AddHeader(string name, string value)
		{
			name = name.ToLower().Trim();
			value = value.Trim();
			if (!headers.ContainsKey(name))
			{
				headers[name] = new List<string>();
			}
			headers[name].Add(value);
		}

		public List<string> GetHeaders()
		{
			List<string> list = new List<string>();
			foreach (string key in headers.Keys)
			{
				foreach (string item in headers[key])
				{
					list.Add(key + ": " + item);
				}
			}
			return list;
		}

		public List<string> GetHeaders(string name)
		{
			name = name.ToLower().Trim();
			if (!headers.ContainsKey(name))
			{
				return new List<string>();
			}
			return headers[name];
		}

		public string GetHeader(string name)
		{
			name = name.ToLower().Trim();
			if (!headers.ContainsKey(name))
			{
				return string.Empty;
			}
			return headers[name][headers[name].Count - 1];
		}

		private string ReadLine(Stream stream)
		{
			List<byte> list = new List<byte>();
			while (true)
			{
				bool flag = true;
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new HTTPException("Unterminated Stream Encountered.");
				}
				if ((byte)num == Request.EOL[1])
				{
					break;
				}
				list.Add((byte)num);
			}
			return Encoding.ASCII.GetString(list.ToArray()).Trim();
		}

		private string[] ReadKeyValue(Stream stream)
		{
			string text = ReadLine(stream);
			if (text == "")
			{
				return null;
			}
			int num = text.IndexOf(':');
			if (num == -1)
			{
				return null;
			}
			return new string[2]
			{
				text.Substring(0, num).Trim(),
				text.Substring(num + 1).Trim()
			};
		}

		public byte[] TakeChunk()
		{
			byte[] result = null;
			lock (chunks)
			{
				if (chunks.Count > 0)
				{
					result = chunks[0];
					chunks.RemoveAt(0);
					return result;
				}
			}
			return result;
		}

		public void ReadFromStream(Stream inputStream)
		{
			string[] array = ReadLine(inputStream).Split(' ');
			if (!int.TryParse(array[1], out status))
			{
				throw new HTTPException("Bad Status Code");
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				message = string.Join(" ", array, 2, array.Length - 2);
				headers.Clear();
				while (true)
				{
					bool flag = true;
					string[] array2 = ReadKeyValue(inputStream);
					if (array2 == null)
					{
						break;
					}
					AddHeader(array2[0], array2[1]);
				}
				if (request.cookieJar != null)
				{
					List<string> list = GetHeaders("set-cookie");
					for (int i = 0; i < list.Count; i++)
					{
						string text = list[i];
						if (text.IndexOf("domain=", StringComparison.CurrentCultureIgnoreCase) == -1)
						{
							text = text + "; domain=" + request.uri.Host;
						}
						if (text.IndexOf("path=", StringComparison.CurrentCultureIgnoreCase) == -1)
						{
							text = text + "; path=" + request.uri.AbsolutePath;
						}
						request.cookieJar.SetCookie(new Cookie(text));
					}
				}
				if (GetHeader("transfer-encoding") == "chunked")
				{
					chunks = new List<byte[]>();
					while (true)
					{
						bool flag = true;
						string text2 = ReadLine(inputStream);
						if (text2 == "0")
						{
							break;
						}
						int num = int.Parse(text2, NumberStyles.AllowHexSpecifier);
						for (int j = 0; j < num; j++)
						{
							memoryStream.WriteByte((byte)inputStream.ReadByte());
						}
						lock (chunks)
						{
							if (GetHeader("content-encoding").Contains("gzip"))
							{
								throw new HTTPException("This build of HTTP.Response does not support content-encoding = gzip");
							}
							chunks.Add(memoryStream.ToArray());
						}
						memoryStream.SetLength(0L);
						inputStream.ReadByte();
						inputStream.ReadByte();
					}
					lock (chunks)
					{
						chunks.Add(new byte[0]);
					}
					while (true)
					{
						bool flag = true;
						string[] array2 = ReadKeyValue(inputStream);
						if (array2 == null)
						{
							break;
						}
						AddHeader(array2[0], array2[1]);
					}
					List<byte> list2 = new List<byte>();
					foreach (byte[] chunk in chunks)
					{
						list2.AddRange(chunk);
					}
					bytes = list2.ToArray();
				}
				else
				{
					int num2 = 0;
					try
					{
						num2 = int.Parse(GetHeader("content-length"));
					}
					catch
					{
						num2 = 0;
					}
					int num3;
					while ((num2 == 0 || memoryStream.Length < num2) && (num3 = inputStream.ReadByte()) != -1)
					{
						memoryStream.WriteByte((byte)num3);
					}
					if (num2 > 0 && memoryStream.Length != num2)
					{
						throw new HTTPException("Response length does not match content length");
					}
					if (GetHeader("content-encoding").Contains("gzip"))
					{
						throw new HTTPException("This build of HTTP.Response does not support content-encoding = gzip");
					}
					bytes = memoryStream.ToArray();
				}
			}
		}
	}
}
