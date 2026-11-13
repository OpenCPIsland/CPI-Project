namespace DeviceDB
{
	internal class ULongIndexValueType : AbstractFixedIndexValueType<ulong>
	{
		private readonly byte[] buffer;

		public ULongIndexValueType()
		{
			buffer = new byte[8];
		}

		protected override byte[] Serialize(ulong entryValue)
		{
			buffer[0] = (byte)(entryValue & 0xFF);
			buffer[1] = (byte)((entryValue & 0xFF00) >> 8);
			buffer[2] = (byte)((entryValue & 0xFF0000) >> 16);
			buffer[3] = (byte)((entryValue & 4278190080u) >> 24);
			buffer[4] = (byte)((entryValue & 0xFF00000000) >> 32);
			buffer[5] = (byte)((entryValue & 0xFF0000000000) >> 40);
			buffer[6] = (byte)((entryValue & 0xFF000000000000) >> 48);
			buffer[7] = (byte)((ulong)((long)entryValue & -72057594037927936L) >> 56);
			return buffer;
		}

		protected override ulong Deserialize(byte[] bytes)
		{
			return ((ulong)(uint)(bytes[4] | (bytes[5] << 8) | (bytes[6] << 16) | (bytes[7] << 24)) << 32) | (uint)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24));
		}
	}
}
