using System;

namespace ClubPenguin
{
	[Serializable]
	public class DHeldObject
	{
		public HeldObjectType ObjectType;

		public string ObjectId;

		public static bool operator ==(DHeldObject c1, DHeldObject c2)
		{
			if ((object)c1 == null && (object)c2 == null)
			{
				return true;
			}
			return (object)c1 != null && (object)c2 != null && c1.ObjectType == c2.ObjectType && c1.ObjectId == c2.ObjectId;
		}

		public static bool operator !=(DHeldObject c1, DHeldObject c2)
		{
			return !(c1 == c2);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is DHeldObject))
			{
				return false;
			}
			return this == (DHeldObject)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
