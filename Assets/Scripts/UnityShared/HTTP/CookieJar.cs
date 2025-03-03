using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HTTP
{
	public class CookieJar
	{
		private static string version = "v2";

		private object cookieJarLock = new object();

		private static CookieJar instance;

		public Dictionary<string, List<Cookie>> cookies;

		public ContentsChangedDelegate ContentsChanged;

		private static string cookiesStringPattern = "[:](?=\\s*[a-zA-Z0-9_\\-]+\\s*[=])";

		private static string boundary = "\n!!::!!\n";

		public static CookieJar Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new CookieJar();
				}
				return instance;
			}
		}

		public CookieJar()
		{
			Clear();
		}

		public void Clear()
		{
			lock (cookieJarLock)
			{
				cookies = new Dictionary<string, List<Cookie>>();
				if (ContentsChanged != null)
				{
					ContentsChanged();
				}
			}
		}

		public bool SetCookie(Cookie cookie)
		{
			lock (cookieJarLock)
			{
				bool flag = cookie.expirationDate < DateTime.Now;
				if (cookies.ContainsKey(cookie.name))
				{
					for (int i = 0; i < cookies[cookie.name].Count; i++)
					{
						Cookie cookie2 = cookies[cookie.name][i];
						if (cookie2.CollidesWith(new CookieAccessInfo(cookie)))
						{
							if (flag)
							{
								cookies[cookie.name].RemoveAt(i);
								if (cookies[cookie.name].Count == 0)
								{
									cookies.Remove(cookie.name);
									if (ContentsChanged != null)
									{
										ContentsChanged();
									}
								}
								return false;
							}
							cookies[cookie.name][i] = cookie;
							if (ContentsChanged != null)
							{
								ContentsChanged();
							}
							return true;
						}
					}
					if (flag)
					{
						return false;
					}
					cookies[cookie.name].Add(cookie);
					if (ContentsChanged != null)
					{
						ContentsChanged();
					}
					return true;
				}
				if (flag)
				{
					return false;
				}
				cookies[cookie.name] = new List<Cookie>();
				cookies[cookie.name].Add(cookie);
				if (ContentsChanged != null)
				{
					ContentsChanged();
				}
				return true;
			}
		}

		public Cookie GetCookie(string name, CookieAccessInfo accessInfo)
		{
			if (!cookies.ContainsKey(name))
			{
				return null;
			}
			for (int i = 0; i < cookies[name].Count; i++)
			{
				Cookie cookie = cookies[name][i];
				if (cookie.expirationDate > DateTime.Now && cookie.Matches(accessInfo))
				{
					return cookie;
				}
			}
			return null;
		}

		public List<Cookie> GetCookies(CookieAccessInfo accessInfo)
		{
			List<Cookie> list = new List<Cookie>();
			foreach (string key in cookies.Keys)
			{
				Cookie cookie = GetCookie(key, accessInfo);
				if (cookie != null)
				{
					list.Add(cookie);
				}
			}
			return list;
		}

		public void SetCookies(Cookie[] cookieObjects)
		{
			for (int i = 0; i < cookieObjects.Length; i++)
			{
				SetCookie(cookieObjects[i]);
			}
		}

		public void SetCookies(string cookiesString)
		{
			Match match = Regex.Match(cookiesString, cookiesStringPattern);
			if (!match.Success)
			{
				throw new Exception("Could not parse cookies string: " + cookiesString);
			}
			for (int i = 0; i < match.Groups.Count; i++)
			{
				SetCookie(new Cookie(match.Groups[i].Value));
			}
		}

		public string Serialize()
		{
			string text = version + boundary;
			lock (cookieJarLock)
			{
				foreach (string key in cookies.Keys)
				{
					for (int i = 0; i < cookies[key].Count; i++)
					{
						text = text + cookies[key][i].ToString() + boundary;
					}
				}
			}
			return text;
		}

		public void Deserialize(string cookieJarString, bool clear)
		{
			if (clear)
			{
				Clear();
			}
			Regex regex = new Regex(boundary);
			string[] array = regex.Split(cookieJarString);
			bool flag = false;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!flag)
				{
					if (text.IndexOf(version) != 0)
					{
						break;
					}
					flag = true;
				}
				else if (text.Length > 0)
				{
					SetCookie(new Cookie(text));
				}
			}
		}
	}
}
