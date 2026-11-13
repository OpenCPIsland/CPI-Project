using System;

namespace DI.HTTP.Security.Pinning
{
	public class URLParser
	{
		private string host;

		public URLParser(string url)
		{
			bool flag = false;
			try
			{
				Uri uri = new Uri(url);
				setHost(uri.Host);
				flag = true;
			}
			catch (Exception)
			{
			}
			if (!flag)
			{
				Uri uri = new Uri("https://" + url);
				setHost(uri.Host);
			}
		}

		public string getHost()
		{
			return host;
		}

		public void setHost(string host)
		{
			this.host = host;
		}
	}
}
