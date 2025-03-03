namespace DeviceDB
{
	internal class BoolIndexValueType : AbstractFixedIndexValueType<bool>
	{
		private readonly byte[] buffer;

		public BoolIndexValueType()
		{
			buffer = new byte[1];
		}

		protected override byte[] Serialize(bool entryValue)
		{
			buffer[0] = (byte)(entryValue ? 1 : 0);
			return buffer;
		}

		protected override bool Deserialize(byte[] bytes)
		{
			return bytes[0] != 0;
		}
	}
}
