using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HTTP
{
	public class Cookie
	{
		public string name = null;

		public string value = null;

		public DateTime expirationDate = DateTime.MaxValue;

		public string path = null;

		public string domain = null;

		public bool secure = false;

		public bool scriptAccessible = true;

		private static string cookiePattern = "\\s*([^=]+)(?:=((?:.|\\n)*))?";

		public Cookie(string cookieString)
		{
			string[] array = cookieString.Split(';');
			string[] array2 = array;
			int num = 0;
			while (true)
			{
				if (num >= array2.Length)
				{
					return;
				}
				string input = array2[num];
				Match match = Regex.Match(input, cookiePattern);
				if (!match.Success)
				{
					break;
				}
				if (name == null)
				{
					name = match.Groups[1].Value;
					value = match.Groups[2].Value;
				}
				else
				{
					switch (match.Groups[1].Value.ToLower())
					{
					case "httponly":
						scriptAccessible = false;
						break;
					case "expires":
						expirationDate = DateTime.Parse(match.Groups[2].Value);
						break;
					case "path":
						path = match.Groups[2].Value;
						break;
					case "domain":
						domain = match.Groups[2].Value;
						break;
					case "secure":
						secure = true;
						break;
					}
				}
				num++;
			}
			throw new Exception("Could not parse cookie string: " + cookieString);
		}

		public bool Matches(CookieAccessInfo accessInfo)
		{
			if (secure != accessInfo.secure || !CollidesWith(accessInfo))
			{
				return false;
			}
			return true;
		}

		public bool CollidesWith(CookieAccessInfo accessInfo)
		{
			if ((path != null && accessInfo.path == null) || (domain != null && accessInfo.domain == null))
			{
				return false;
			}
			if (path != null && accessInfo.path != null && accessInfo.path.IndexOf(path) != 0)
			{
				return false;
			}
			if (domain == accessInfo.domain)
			{
				return true;
			}
			if (domain != null && domain.Length >= 1 && domain[0] == '.')
			{
				int num = accessInfo.domain.IndexOf(domain.Substring(1));
				if (num == -1 || num != accessInfo.domain.Length - domain.Length + 1)
				{
					return false;
				}
			}
			else if (domain != null)
			{
				return false;
			}
			return true;
		}

		public string ToValueString()
		{
			return name + "=" + value;
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			list.Add(name + "=" + value);
			if (expirationDate != DateTime.MaxValue)
			{
				list.Add("expires=" + expirationDate.ToString());
			}
			if (domain != null)
			{
				list.Add("domain=" + domain);
			}
			if (path != null)
			{
				list.Add("path=" + path);
			}
			if (secure)
			{
				list.Add("secure");
			}
			if (!scriptAccessible)
			{
				list.Add("httponly");
			}
			return string.Join("; ", list.ToArray());
		}
	}
}
