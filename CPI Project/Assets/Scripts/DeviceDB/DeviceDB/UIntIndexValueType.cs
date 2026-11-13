namespace DeviceDB
{
	internal class UIntIndexValueType : AbstractFixedIndexValueType<uint>
	{
		private readonly byte[] buffer;

		public UIntIndexValueType()
		{
			buffer = new byte[4];
		}

		protected override byte[] Serialize(uint entryValue)
		{
			buffer[0] = (byte)(entryValue & 0xFF);
			buffer[1] = (byte)((entryValue & 0xFF00) >> 8);
			buffer[2] = (byte)((entryValue & 0xFF0000) >> 16);
			buffer[3] = (byte)((uint)((int)entryValue & -16777216) >> 24);
			return buffer;
		}

		protected override uint Deserialize(byte[] bytes)
		{
			return (uint)((bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0]);
		}
	}
}
