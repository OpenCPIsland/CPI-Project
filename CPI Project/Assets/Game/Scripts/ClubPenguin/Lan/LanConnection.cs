#if !UNITY_WEBGL
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ClubPenguin.Net.Lan
{
	internal sealed class LanBuffer
	{
		private byte[] data = new byte[4096];

		private int start;

		private int length;

		public int Length
		{
			get
			{
				return length;
			}
		}

		public byte[] Data
		{
			get
			{
				return data;
			}
		}

		public int Start
		{
			get
			{
				return start;
			}
		}

		public void Append(byte[] src, int offset, int count)
		{
			if (start + length + count > data.Length)
			{
				if (length + count > data.Length)
				{
					byte[] bigger = new byte[Math.Max(data.Length * 2, length + count)];
					Buffer.BlockCopy(data, start, bigger, 0, length);
					data = bigger;
				}
				else
				{
					Buffer.BlockCopy(data, start, data, 0, length);
				}
				start = 0;
			}
			Buffer.BlockCopy(src, offset, data, start + length, count);
			length += count;
		}

		public void Consume(int count)
		{
			start += count;
			length -= count;
			if (length == 0)
			{
				start = 0;
			}
		}

		public void Clear()
		{
			start = 0;
			length = 0;
		}
	}

	public sealed class LanConnection
	{
		private const string WebSocketMagic = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		private const int MaxMessageSize = 1000000;

		private readonly Socket socket;

		private readonly LanServer server;

		private readonly BlockingCollection<byte[]> sendQueue = new BlockingCollection<byte[]>();

		private readonly LanBuffer handshake = new LanBuffer();

		private readonly LanBuffer wsBuffer = new LanBuffer();

		private readonly LanBuffer sfsBuffer = new LanBuffer();

		private readonly LanBuffer fragment = new LanBuffer();

		private int mode;

		private int closed;

		private long windowStart;

		private int packetsInWindow;

		public LanPlayer Player
		{
			get;
			private set;
		}

		public string Address
		{
			get;
			private set;
		}

		public long LastInboundMs
		{
			get;
			private set;
		}

		public bool IsClosed
		{
			get
			{
				return closed != 0;
			}
		}

		public LanConnection(LanServer server, Socket socket)
		{
			this.server = server;
			this.socket = socket;
			IPEndPoint ep = socket.RemoteEndPoint as IPEndPoint;
			Address = (ep != null) ? ep.Address.ToString() : "";
			Player = new LanPlayer(this);
			LastInboundMs = server.Loop.NowMs;
		}

		public void Start()
		{
			socket.NoDelay = true;
			Thread reader = new Thread(ReadLoop);
			reader.IsBackground = true;
			reader.Name = "CPI LAN Reader";
			reader.Start();
			Thread writer = new Thread(WriteLoop);
			writer.IsBackground = true;
			writer.Name = "CPI LAN Writer";
			writer.Start();
		}

		private void ReadLoop()
		{
			byte[] buf = new byte[16384];
			try
			{
				while (closed == 0)
				{
					int n = socket.Receive(buf);
					if (n <= 0)
					{
						break;
					}
					byte[] chunk = new byte[n];
					Buffer.BlockCopy(buf, 0, chunk, 0, n);
					server.Loop.Post(delegate
					{
						OnBytes(chunk);
					});
				}
			}
			catch (Exception)
			{
			}
			server.Loop.Post(Close);
		}

		private void WriteLoop()
		{
			try
			{
				foreach (byte[] data in sendQueue.GetConsumingEnumerable())
				{
					int sent = 0;
					while (sent < data.Length)
					{
						sent += socket.Send(data, sent, data.Length - sent, SocketFlags.None);
					}
				}
			}
			catch (Exception)
			{
			}
			try
			{
				socket.Close();
			}
			catch (Exception)
			{
			}
		}

		public void SendRaw(byte[] bytes)
		{
			if (closed != 0)
			{
				return;
			}
			try
			{
				sendQueue.Add(bytes);
			}
			catch (InvalidOperationException)
			{
			}
		}

		public void Send(byte[] packet)
		{
			SendRaw((mode == 2) ? EncodeFrame(packet, 0x82) : packet);
		}

		public void Close()
		{
			if (Interlocked.Exchange(ref closed, 1) != 0)
			{
				return;
			}
			try
			{
				socket.Shutdown(SocketShutdown.Both);
			}
			catch (Exception)
			{
			}
			sendQueue.CompleteAdding();
			server.OnConnectionClosed(this);
		}

		public void CloseAfter(double ms)
		{
			server.Loop.After(ms, Close);
		}

		private void OnBytes(byte[] chunk)
		{
			if (closed != 0)
			{
				return;
			}
			LastInboundMs = server.Loop.NowMs;
			if (mode == 0)
			{
				handshake.Append(chunk, 0, chunk.Length);
				if (handshake.Data[handshake.Start] != (byte)'G')
				{
					mode = 1;
					sfsBuffer.Append(handshake.Data, handshake.Start, handshake.Length);
					handshake.Clear();
					ParsePackets();
					return;
				}
				string text = Encoding.ASCII.GetString(handshake.Data, handshake.Start, handshake.Length);
				int headerEnd = text.IndexOf("\r\n\r\n", StringComparison.Ordinal);
				if (headerEnd < 0)
				{
					if (handshake.Length > 8192)
					{
						Close();
					}
					return;
				}
				Match m = Regex.Match(text.Substring(0, headerEnd), "Sec-WebSocket-Key:\\s*(\\S+)", RegexOptions.IgnoreCase);
				if (!m.Success)
				{
					Close();
					return;
				}
				string accept;
				using (SHA1 sha = SHA1.Create())
				{
					accept = Convert.ToBase64String(sha.ComputeHash(Encoding.ASCII.GetBytes(m.Groups[1].Value + WebSocketMagic)));
				}
				string response = "HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Accept: " + accept + "\r\n\r\n";
				SendRaw(Encoding.ASCII.GetBytes(response));
				mode = 2;
				int consumed = Encoding.ASCII.GetByteCount(text.Substring(0, headerEnd + 4));
				handshake.Consume(consumed);
				if (handshake.Length > 0)
				{
					wsBuffer.Append(handshake.Data, handshake.Start, handshake.Length);
				}
				handshake.Clear();
				ParseFrames();
				return;
			}
			if (mode == 2)
			{
				wsBuffer.Append(chunk, 0, chunk.Length);
				if (wsBuffer.Length > MaxMessageSize + 14)
				{
					Close();
					return;
				}
				ParseFrames();
			}
			else
			{
				sfsBuffer.Append(chunk, 0, chunk.Length);
				if (sfsBuffer.Length > MaxMessageSize + 5)
				{
					Close();
					return;
				}
				ParsePackets();
			}
		}

		private void ParseFrames()
		{
			while (closed == 0)
			{
				byte[] d = wsBuffer.Data;
				int s = wsBuffer.Start;
				int len = wsBuffer.Length;
				if (len < 2)
				{
					return;
				}
				bool fin = (d[s] & 0x80) != 0;
				int opcode = d[s] & 0x0F;
				bool masked = (d[s + 1] & 0x80) != 0;
				long payloadLen = d[s + 1] & 0x7F;
				int offset = 2;
				if (payloadLen == 126)
				{
					if (len < offset + 2)
					{
						return;
					}
					payloadLen = (d[s + 2] << 8) | d[s + 3];
					offset += 2;
				}
				else if (payloadLen == 127)
				{
					if (len < offset + 8)
					{
						return;
					}
					payloadLen = 0;
					for (int i = 0; i < 8; i++)
					{
						payloadLen = (payloadLen << 8) | d[s + 2 + i];
					}
					offset += 8;
				}
				if (payloadLen > MaxMessageSize)
				{
					Close();
					return;
				}
				int maskOffset = offset;
				if (masked)
				{
					offset += 4;
				}
				if (len < offset + payloadLen)
				{
					return;
				}
				byte[] payload = new byte[payloadLen];
				Buffer.BlockCopy(d, s + offset, payload, 0, (int)payloadLen);
				if (masked)
				{
					for (int i = 0; i < payload.Length; i++)
					{
						payload[i] ^= d[s + maskOffset + (i % 4)];
					}
				}
				wsBuffer.Consume(offset + (int)payloadLen);
				if (opcode == 8)
				{
					Close();
					return;
				}
				if (opcode == 9)
				{
					SendRaw(EncodeFrame(payload, 0x8A));
					continue;
				}
				if (opcode == 10)
				{
					continue;
				}
				fragment.Append(payload, 0, payload.Length);
				if (fragment.Length > MaxMessageSize + 5)
				{
					Close();
					return;
				}
				if (fin)
				{
					sfsBuffer.Append(fragment.Data, fragment.Start, fragment.Length);
					fragment.Clear();
					ParsePackets();
				}
			}
		}

		private void ParsePackets()
		{
			while (closed == 0)
			{
				SfsHeader h = SfsCodec.TryReadHeader(sfsBuffer.Data, sfsBuffer.Start, sfsBuffer.Length);
				if (h == null)
				{
					return;
				}
				if (h.BodyLength > MaxMessageSize)
				{
					Close();
					return;
				}
				if (sfsBuffer.Length < h.TotalLength)
				{
					return;
				}
				long now = server.Loop.NowMs;
				if (now - windowStart >= 1000)
				{
					windowStart = now;
					packetsInWindow = 0;
				}
				if (++packetsInWindow > 200)
				{
					Close();
					return;
				}
				int bodyStart = sfsBuffer.Start + h.HeaderLength;
				int bodyLength = (int)h.BodyLength;
				int controllerId = -1;
				int actionId = -1;
				SfsObject parameters = null;
				bool decoded = false;
				if (!h.Compressed && !h.Encrypted)
				{
					try
					{
						SfsCodec.DecodeBody(sfsBuffer.Data, bodyStart, bodyLength, out controllerId, out actionId, out parameters);
						decoded = true;
					}
					catch (Exception)
					{
					}
				}
				sfsBuffer.Consume((int)h.TotalLength);
				if (decoded)
				{
					try
					{
						server.Dispatch(this, controllerId, actionId, parameters);
					}
					catch (Exception)
					{
					}
				}
			}
		}

		private static byte[] EncodeFrame(byte[] payload, byte first)
		{
			byte[] header;
			if (payload.Length < 126)
			{
				header = new byte[2]
				{
					first,
					(byte)payload.Length
				};
			}
			else if (payload.Length < 65536)
			{
				header = new byte[4]
				{
					first,
					126,
					(byte)(payload.Length >> 8),
					(byte)payload.Length
				};
			}
			else
			{
				header = new byte[10];
				header[0] = first;
				header[1] = 127;
				long l = payload.Length;
				for (int i = 0; i < 8; i++)
				{
					header[9 - i] = (byte)(l >> (8 * i));
				}
			}
			byte[] frame = new byte[header.Length + payload.Length];
			Buffer.BlockCopy(header, 0, frame, 0, header.Length);
			Buffer.BlockCopy(payload, 0, frame, header.Length, payload.Length);
			return frame;
		}
	}
}
#endif
