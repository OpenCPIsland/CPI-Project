namespace DeviceDB
{
	internal class SByteIndexValueType : AbstractFixedIndexValueType<sbyte>
	{
		private readonly byte[] buffer;

		public SByteIndexValueType()
		{
			buffer = new byte[1];
		}

		protected override byte[] Serialize(sbyte entryValue)
		{
			buffer[0] = (byte)entryValue;
			return buffer;
		}

		protected override sbyte Deserialize(byte[] bytes)
		{
			return (sbyte)bytes[0];
		}
	}
}
