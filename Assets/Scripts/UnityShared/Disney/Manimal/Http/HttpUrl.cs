using System;
using System.Collections.Generic;

namespace Disney.Manimal.Http
{
	public class HttpUrl
	{
		public string Path
		{
			get;
			private set;
		}

		public HttpQueryParamCollection QueryParams
		{
			get;
			private set;
		}

		public HttpUrl(string baseUrl)
		{
			if (baseUrl == null)
			{
				throw new ArgumentNullException("baseUrl");
			}
			string[] array = baseUrl.Split('?');
			Path = array[0];
			QueryParams = HttpUtility.ParseQueryString((array.Length > 1) ? array[1] : string.Empty);
		}

		public HttpUrl AppendPathSegment(string segment)
		{
			if (segment == null)
			{
				throw new ArgumentNullException("segment");
			}
			if (!Path.EndsWith("/"))
			{
				Path += "/";
			}
			Path += CleanSegment(segment.TrimStart('/').TrimEnd('/'));
			return this;
		}

		public HttpUrl AppendPathSegments(params string[] segments)
		{
			foreach (string segment in segments)
			{
				AppendPathSegment(segment);
			}
			return this;
		}

		public HttpUrl AppendPathSegments(IEnumerable<string> segments)
		{
			foreach (string segment in segments)
			{
				AppendPathSegment(segment);
			}
			return this;
		}

		public HttpUrl AddQueryParam(HttpParameter parameter)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException("parameter");
			}
			QueryParams.Add(parameter);
			return this;
		}

		public HttpUrl AddQueryParam(string name, string value)
		{
			QueryParams.Add(name, value);
			return this;
		}

		public HttpUrl AddQueryParams(params HttpParameter[] parameters)
		{
			foreach (HttpParameter parameter in parameters)
			{
				AddQueryParam(parameter);
			}
			return this;
		}

		public HttpUrl AddQueryParams(string name, params string[] values)
		{
			foreach (string value in values)
			{
				AddQueryParam(name, value);
			}
			return this;
		}

		public HttpUrl AddQueryParams(IEnumerable<HttpParameter> parameters)
		{
			foreach (HttpParameter parameter in parameters)
			{
				AddQueryParam(parameter);
			}
			return this;
		}

		public HttpUrl AddQueryParams(string name, IEnumerable<string> values)
		{
			foreach (string value in values)
			{
				AddQueryParam(name, value);
			}
			return this;
		}

		public HttpUrl RemoveQueryParam(string name)
		{
			return RemoveQueryParam(name, false);
		}

		public HttpUrl RemoveQueryParam(string name, bool ignoreCase)
		{
			QueryParams.Remove(name, ignoreCase);
			return this;
		}

		public HttpUrl RemoveQueryParams(params string[] names)
		{
			return RemoveQueryParams(false, names);
		}

		public HttpUrl RemoveQueryParams(bool ignoreCase, params string[] names)
		{
			foreach (string name in names)
			{
				RemoveQueryParam(name, ignoreCase);
			}
			return this;
		}

		public HttpUrl RemoveQueryParams(IEnumerable<string> names)
		{
			return RemoveQueryParams(names, false);
		}

		public HttpUrl RemoveQueryParams(IEnumerable<string> names, bool ignoreCase)
		{
			foreach (string name in names)
			{
				RemoveQueryParam(name, ignoreCase);
			}
			return this;
		}

		public HttpUrl ResetToRoot()
		{
			Path = GetRoot(Path);
			QueryParams.Clear();
			return this;
		}

		public override string ToString()
		{
			string text = Path;
			string text2 = QueryParams.ToString();
			if (text2.Length > 0)
			{
				text = text + "?" + text2;
			}
			return text;
		}

		public static implicit operator string(HttpUrl url)
		{
			return url.ToString();
		}

		public static string GetRoot(string url)
		{
			Uri uri = new Uri(url);
			return uri.GetComponents(UriComponents.Scheme | UriComponents.UserInfo | UriComponents.Host | UriComponents.Port, UriFormat.Unescaped);
		}

		public static string Combine(string url, params string[] segments)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			return new HttpUrl(url).AppendPathSegments(segments).ToString();
		}

		private static string CleanSegment(string segment)
		{
			string stringToEscape = Uri.UnescapeDataString(segment);
			return Uri.EscapeUriString(stringToEscape).Replace("?", "%3F");
		}
	}
}
