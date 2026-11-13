using System;
using System.IO;

namespace Disney.Manimal.Http
{
	public class SseHttpRequest : HttpRequest, IEventSource
	{
		private const byte DEFAULT_LINE_DELIMITER = 10;

		private const int DEFAULT_STREAM_READER_BUFFER_SIZE_BYTES = 64;

		private EventHandler<EventSourceMessage> onMessageInvoker = delegate
		{
		};

		private string eventTypeBuffer = "";

		private string dataBuffer = "";

		private string lastEventIdBuffer = "";

		public byte LineDelimiter
		{
			get;
			set;
		}

		public int StreamReaderBufferSizeBytes
		{
			get;
			set;
		}

		public event EventHandler<EventSourceMessage> OnMessage
		{
			add
			{
				onMessageInvoker = (EventHandler<EventSourceMessage>)Delegate.Combine(onMessageInvoker, value);
			}
			remove
			{
				onMessageInvoker = (EventHandler<EventSourceMessage>)Delegate.Remove(onMessageInvoker, value);
			}
		}

		public SseHttpRequest()
		{
			LineDelimiter = 10;
			StreamReaderBufferSizeBytes = 64;
			base.Headers.Add(new HttpHeader
			{
				Name = "Accept",
				Value = "text/event-stream"
			});
			base.Headers.Add(new HttpHeader
			{
				Name = "Cache-Control",
				Value = "no-cache"
			});
			base.ResponseWriter = SseResponseWriter;
		}

		private void SseResponseWriter(Stream stream)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] array = new byte[StreamReaderBufferSizeBytes];
				int num = 1;
				while (num != 0)
				{
					num = stream.Read(array, 0, array.Length);
					for (int i = 0; i < num; i++)
					{
						if (array[i].Equals(LineDelimiter))
						{
							HandleLine(base.Encoding.GetString(memoryStream.ToArray()));
							memoryStream.SetLength(0L);
						}
						else
						{
							memoryStream.WriteByte(array[i]);
						}
					}
				}
			}
		}

		private void HandleLine(string line)
		{
			if (line.Length == 0)
			{
				DispatchEvent();
				return;
			}
			int num = line.IndexOf(':');
			if (num == 0)
			{
				return;
			}
			if (num > 0)
			{
				string name = line.Substring(0, num);
				int num2 = num + 1;
				if (line.Substring(num2, 1) == " ")
				{
					num2++;
				}
				string value = line.Substring(num2);
				ProcessField(name, value);
			}
			else
			{
				ProcessField(line, "");
			}
		}

		private void ProcessField(string name, string value)
		{
			switch (name)
			{
			case "event":
				eventTypeBuffer = value;
				break;
			case "data":
				dataBuffer = dataBuffer + value + "\n";
				break;
			case "id":
				lastEventIdBuffer = value;
				break;
			case "retry":
				throw new NotImplementedException();
			}
		}

		private void DispatchEvent()
		{
			EventSourceMessage eventSourceMessage = new EventSourceMessage();
			eventSourceMessage.LastEventID = lastEventIdBuffer;
			if (string.IsNullOrEmpty(dataBuffer))
			{
				eventTypeBuffer = "";
				return;
			}
			if (dataBuffer.Substring(dataBuffer.Length - 1, 1) == "\n")
			{
				dataBuffer = dataBuffer.Substring(0, dataBuffer.Length - 1);
			}
			eventSourceMessage.Data = dataBuffer;
			if (!string.IsNullOrEmpty(eventTypeBuffer))
			{
				eventSourceMessage.Type = eventTypeBuffer;
			}
			dataBuffer = "";
			eventTypeBuffer = "";
			onMessageInvoker(this, eventSourceMessage);
		}
	}
}
