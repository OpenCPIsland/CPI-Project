using System;
using System.Collections.Generic;
using System.Text;

public class XmlHttpRequest
{
	private int requestId = 0;

	private Uri url;

	private int timeout = 0;

	private string method;

	private Dictionary<string, string> requestHeaders = new Dictionary<string, string>();

	private Dictionary<string, string> responseHeaders = null;

	public int RequestId
	{
		get
		{
			return requestId;
		}
	}

	public Uri Url
	{
		get
		{
			return url;
		}
	}

	public string ContentType
	{
		get
		{
			return requestHeaders["Content-Type"];
		}
		set
		{
			requestHeaders.Remove("Content-Type");
			if (value != null && value.Trim().Length > 0)
			{
				requestHeaders.Add("Content-Type", value);
			}
		}
	}

	public int Timeout
	{
		get
		{
			return timeout;
		}
		set
		{
			if (value < -1)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			timeout = value;
		}
	}

	public string UserAgent
	{
		get
		{
			return requestHeaders["User-Agent"];
		}
		set
		{
			requestHeaders.Remove("User-Agent");
			if (value != null && value.Trim().Length > 0)
			{
				requestHeaders.Add("User-Agent", value);
			}
		}
	}

	public string Method
	{
		get
		{
			return method;
		}
	}

	public bool IsComplete
	{
		get
		{
			return false;
		}
	}

	public int Status
	{
		get
		{
			return 0;
		}
	}

	public string StatusLine
	{
		get
		{
			return null;
		}
	}

	public byte[] Response
	{
		get
		{
			return null;
		}
	}

	public string ResponseText
	{
		get
		{
			byte[] response = Response;
			if (response == null)
			{
				return null;
			}
			return Encoding.UTF8.GetString(response);
		}
	}

	public Dictionary<string, string> RequestHeaders
	{
		get
		{
			return requestHeaders;
		}
	}

	public Dictionary<string, string> ResponseHeaders
	{
		get
		{
			if (responseHeaders != null)
			{
				return responseHeaders;
			}
			return null;
		}
	}

	public XmlHttpRequest(Uri url, string method)
	{
		this.url = url;
		this.method = method;
	}

	~XmlHttpRequest()
	{
	}

	public void Send(byte[] data = null)
	{
	}

	public void SendText(string text)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		Send(bytes);
	}

	public void Abort()
	{
	}

	private Dictionary<string, string> parseResponseHeaders(string headerStr)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (headerStr == null || headerStr.Length == 0)
		{
			return dictionary;
		}
		string[] array = headerStr.Split(new string[1]
		{
			"\r\n"
		}, StringSplitOptions.RemoveEmptyEntries);
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			string text = array[i];
			int num2 = text.IndexOf(": ");
			if (num2 > 0)
			{
				string key = text.Substring(0, num2);
				string text3 = dictionary[key] = text.Substring(num2 + 2);
			}
		}
		return dictionary;
	}
}
