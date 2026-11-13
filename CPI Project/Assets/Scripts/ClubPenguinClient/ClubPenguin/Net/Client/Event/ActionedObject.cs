using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public class ActionedObject
	{
		public ObjectType type;

		public string id;

		public string tag;

		public ActionedObject()
		{
		}

		public ActionedObject(ObjectType type, string id, string tag)
		{
			this.type = type;
			this.id = id;
			this.tag = tag;
		}
	}
}
