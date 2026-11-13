namespace DeviceDB
{
	internal class UShortIndexValueType : AbstractFixedIndexValueType<ushort>
	{
		private readonly byte[] buffer;

		public UShortIndexValueType()
		{
			buffer = new byte[2];
		}

		protected override byte[] Serialize(ushort entryValue)
		{
			buffer[0] = (byte)(entryValue & 0xFF);
			buffer[1] = (byte)((entryValue & 0xFF00) >> 8);
			return buffer;
		}

		protected override ushort Deserialize(byte[] bytes)
		{
			return (ushort)((bytes[1] << 8) | bytes[0]);
		}
	}
}
