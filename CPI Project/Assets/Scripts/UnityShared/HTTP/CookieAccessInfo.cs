namespace HTTP
{
	public class CookieAccessInfo
	{
		public string domain = null;

		public string path = null;

		public bool secure = false;

		public bool scriptAccessible = true;

		public CookieAccessInfo(string domain, string path)
		{
			this.domain = domain;
			this.path = path;
		}

		public CookieAccessInfo(string domain, string path, bool secure)
		{
			this.domain = domain;
			this.path = path;
			this.secure = secure;
		}

		public CookieAccessInfo(string domain, string path, bool secure, bool scriptAccessible)
		{
			this.domain = domain;
			this.path = path;
			this.secure = secure;
			this.scriptAccessible = scriptAccessible;
		}

		public CookieAccessInfo(Cookie cookie)
		{
			domain = cookie.domain;
			path = cookie.path;
			secure = cookie.secure;
			scriptAccessible = cookie.scriptAccessible;
		}
	}
}
