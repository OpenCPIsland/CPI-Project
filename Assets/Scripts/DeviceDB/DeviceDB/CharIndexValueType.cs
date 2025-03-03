namespace DeviceDB
{
	internal class CharIndexValueType : AbstractFixedIndexValueType<char>
	{
		private readonly byte[] buffer;

		public CharIndexValueType()
		{
			buffer = new byte[2];
		}

		protected override byte[] Serialize(char entryValue)
		{
			buffer[0] = (byte)(entryValue & 0xFF);
			buffer[1] = (byte)((entryValue & 0xFF00) >> 8);
			return buffer;
		}

		protected override char Deserialize(byte[] bytes)
		{
			return (char)((bytes[1] << 8) | bytes[0]);
		}
	}
}
