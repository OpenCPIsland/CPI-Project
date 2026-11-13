using System.Collections.Specialized;

namespace Disney.Kelowna.Common
{
	public static class QueryParams
	{
		public static NameValueCollection ParseQueryString(string queryString)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (queryString.Contains("?"))
			{
				queryString = queryString.Substring(queryString.IndexOf('?') + 1);
			}
			string[] array = queryString.Split('&');
			foreach (string text in array)
			{
				string[] array2 = text.Split('=');
				if (array2.Length == 2)
				{
					nameValueCollection.Add(array2[0], array2[1]);
				}
				else
				{
					nameValueCollection.Add(array2[0], string.Empty);
				}
			}
			return nameValueCollection;
		}
	}
}
