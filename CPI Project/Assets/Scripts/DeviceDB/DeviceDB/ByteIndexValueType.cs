namespace DeviceDB
{
	internal class ByteIndexValueType : AbstractFixedIndexValueType<byte>
	{
		private readonly byte[] buffer;

		public ByteIndexValueType()
		{
			buffer = new byte[1];
		}

		protected override byte[] Serialize(byte entryValue)
		{
			buffer[0] = entryValue;
			return buffer;
		}

		protected override byte Deserialize(byte[] bytes)
		{
			return bytes[0];
		}
	}
}
