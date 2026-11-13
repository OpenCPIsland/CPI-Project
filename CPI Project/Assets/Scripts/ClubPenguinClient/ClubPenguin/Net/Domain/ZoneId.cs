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

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (name != null) ? name.GetHashCode() : 0;
				return (hashCode * 397) ^ ((instanceId != null) ? instanceId.GetHashCode() : 0);
			}
		}
	}
}
