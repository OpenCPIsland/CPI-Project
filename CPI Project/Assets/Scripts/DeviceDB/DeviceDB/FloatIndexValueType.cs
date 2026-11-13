namespace DeviceDB
{
	internal class FloatIndexValueType : AbstractFixedIndexValueType<float>
	{
		private readonly byte[] buffer;

		public FloatIndexValueType()
		{
			buffer = new byte[4];
		}

		protected unsafe override byte[] Serialize(float entryValue)
		{
			byte* ptr = (byte*)(&entryValue);
			buffer[0] = *ptr;
			buffer[1] = ptr[1];
			buffer[2] = ptr[2];
			buffer[3] = ptr[3];
			return buffer;
		}

		protected unsafe override float Deserialize(byte[] bytes)
		{
			float result = default(float);
			byte* ptr = (byte*)(&result);
			*ptr = bytes[0];
			ptr[1] = bytes[1];
			ptr[2] = bytes[2];
			ptr[3] = bytes[3];
			return result;
		}
	}
}
