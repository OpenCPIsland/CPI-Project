using System;

namespace ClubPenguin.Data
{
	[Serializable]
	public class PlayerIdComponent
	{
		public string DisplayName
		{
			get;
			protected set;
		}

		public string Swid
		{
			get;
			protected set;
		}

		public long SessionId
		{
			get;
			protected set;
		}

		public void SetDisplayName(string displayName)
		{
			if (!string.IsNullOrEmpty(displayName))
			{
				DisplayName = displayName;
				return;
			}
			throw new Exception("Display name cannot be null or empty");
		}

		public void SetSwid(string swid)
		{
			if (!string.IsNullOrEmpty(swid))
			{
				Swid = swid;
				return;
			}
			throw new Exception("Swid cannot be null or empty");
		}

		public void SetSessionId(long sessionId)
		{
			if (sessionId > 0)
			{
				SessionId = sessionId;
				return;
			}
			throw new Exception("Session Id is not greater than zero");
		}
	}
}
