using System;

namespace ClubPenguin.Net.Domain.Igloo
{
	[Serializable]
	public struct CustomItemProperty
	{
		public string customName;

		public string customType;

		public object customValue;

		public override string ToString()
		{
			return string.Format("Name: {0}, Type: {1}, Value: {2}", customName, customType, customValue);
		}
	}
}
