using System;

namespace Disney.Manimal.Http
{
	public static class HttpUtility
	{
		public static HttpQueryParamCollection ParseQueryString(string query)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}
			if (query.Length > 0 && query[0] == '?')
			{
				query = query.Substring(1);
			}
			return new HttpQueryParamCollection(query);
		}
	}
}
