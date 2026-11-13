using System;

namespace ClubPenguin.Tests
{
	public class Token : EventArgs
	{
		public string access_token
		{
			get;
			set;
		}

		public string refresh_token
		{
			get;
			set;
		}

		public uint? refresh_ttl
		{
			get;
			set;
		}

		public uint ttl
		{
			get;
			set;
		}

		public string swid
		{
			get;
			set;
		}
	}
}
