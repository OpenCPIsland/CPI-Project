namespace DeviceDB
{
	internal class IntIndexValueType : AbstractFixedIndexValueType<int>
	{
		private readonly byte[] buffer;

		public IntIndexValueType()
		{
			buffer = new byte[4];
		}

		protected override byte[] Serialize(int entryValue)
		{
			buffer[0] = (byte)(entryValue & 0xFF);
			buffer[1] = (byte)((entryValue & 0xFF00) >> 8);
			buffer[2] = (byte)((entryValue & 0xFF0000) >> 16);
			buffer[3] = (byte)((entryValue & 4278190080u) >> 24);
			return buffer;
		}

		protected override int Deserialize(byte[] bytes)
		{
			return (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
		}
	}
}
