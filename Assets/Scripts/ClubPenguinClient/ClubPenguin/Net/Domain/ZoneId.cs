using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class ZoneId
	{
		public string name;

		public string instanceId;

		public bool isEmpty()
		{
			return string.IsNullOrEmpty(name) && string.IsNullOrEmpty(instanceId);
		}

		public override string ToString()
		{
			return string.Format("[name:{0}, instanceId:{1}]", name, instanceId);
		}

		public override bool Equals(object obj)
		{
			ZoneId zoneId = obj as ZoneId;
			if (zoneId == null)
			{
				return false;
			}
			if (zoneId == this)
			{
				return true;
			}
			return string.Equals(name, zoneId.name) && string.Equals(instanceId, zoneId.instanceId);
		}
	}
}
