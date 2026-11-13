using System;

namespace DeviceDB
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	public class SerializedAttribute : Attribute
	{
		public byte PrimaryId
		{
			get;
			private set;
		}

		public byte[] AlternateIds
		{
			get;
			private set;
		}

		public SerializedAttribute(byte primaryId, params byte[] alternateIds)
		{
			PrimaryId = primaryId;
			AlternateIds = alternateIds;
		}
	}
}
