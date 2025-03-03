using System;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpPathAttribute : Attribute
	{
		public string BaseUriName;

		public string Path;

		public string Uri
		{
			get
			{
				return Configuration.GetBaseUri(BaseUriName) + Path;
			}
		}

		public HttpPathAttribute(string path)
		{
			BaseUriName = "default";
			Path = validatePath(path);
		}

		public HttpPathAttribute(string baseUriName, string path)
		{
			BaseUriName = baseUriName;
			Path = validatePath(path);
		}

		private string validatePath(string path)
		{
			if (!path.StartsWith("/") && BaseUriName != null)
			{
				Configuration.LogInternal(path + " path must begin with '/'", LogSeverity.WARNING);
				return "/" + path;
			}
			return path;
		}
	}
}
