namespace DeviceDB
{
	[Serialized(byte.MaxValue, new byte[]
	{

	})]
	internal class MetadataDocument : AbstractDocument
	{
		[Serialized(0, new byte[]
		{

		})]
		public byte[] InitializationVector;
	}
}
