using System;
using System.Collections.Generic;

namespace DI.HTTP.Security.Pinning
{
	public class HostInfo : IHostInfo
	{
		private string commonName;

		public HostInfo()
		{
		}

		public HostInfo(IDictionary<string, object> blob)
		{
			try
			{
				setCommonName((string)blob["cn"]);
			}
			catch (Exception ex)
			{
				throw new HTTPException("Unable to parse Host Info. " + ex.Message);
			}
		}

		public void setCommonName(string commonName)
		{
			this.commonName = commonName;
		}

		public string getCommonName()
		{
			return commonName;
		}
	}
}
