namespace DeviceDB
{
	internal class ShortIndexValueType : AbstractFixedIndexValueType<short>
	{
		private readonly byte[] buffer;

		public ShortIndexValueType()
		{
			buffer = new byte[2];
		}

		protected override byte[] Serialize(short entryValue)
		{
			buffer[0] = (byte)(entryValue & 0xFF);
			buffer[1] = (byte)((entryValue & 0xFF00) >> 8);
			return buffer;
		}

		protected override short Deserialize(byte[] bytes)
		{
			return (short)((bytes[1] << 8) | bytes[0]);
		}
	}
}
