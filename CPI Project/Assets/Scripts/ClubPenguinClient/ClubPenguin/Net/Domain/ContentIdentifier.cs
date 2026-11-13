using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class ContentIdentifier
	{
		public string clientVersion;

		public string contentVersion;

		public string subContentVersion;

		public string abTestGroup;

		public override string ToString()
		{
			return clientVersion + ";" + contentVersion + ";" + subContentVersion + ";" + abTestGroup;
		}

		public ContentIdentifier()
		{
			clientVersion = "";
			contentVersion = "";
			subContentVersion = "";
			abTestGroup = "";
		}

		public ContentIdentifier(string clientVersion, string contentVersion, string subContentVersion, string abTestGroup)
		{
			this.clientVersion = clientVersion;
			this.contentVersion = contentVersion;
			this.subContentVersion = subContentVersion;
			this.abTestGroup = abTestGroup;
		}

		public ContentIdentifier(string contentIdentifierString)
		{
			string[] array = contentIdentifierString.Split(';');
			clientVersion = array[0];
			contentVersion = array[1];
			subContentVersion = array[2];
			abTestGroup = array[3];
		}
	}
}
