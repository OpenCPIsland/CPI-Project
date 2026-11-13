namespace DeviceDB
{
	internal class DoubleIndexValueType : AbstractFixedIndexValueType<double>
	{
		private readonly byte[] buffer;

		public DoubleIndexValueType()
		{
			buffer = new byte[8];
		}

		protected unsafe override byte[] Serialize(double entryValue)
		{
			byte* ptr = (byte*)(&entryValue);
			buffer[0] = *ptr;
			buffer[1] = ptr[1];
			buffer[2] = ptr[2];
			buffer[3] = ptr[3];
			buffer[4] = ptr[4];
			buffer[5] = ptr[5];
			buffer[6] = ptr[6];
			buffer[7] = ptr[7];
			return buffer;
		}

		protected unsafe override double Deserialize(byte[] bytes)
		{
			double result = default(double);
			byte* ptr = (byte*)(&result);
			*ptr = bytes[0];
			ptr[1] = bytes[1];
			ptr[2] = bytes[2];
			ptr[3] = bytes[3];
			ptr[4] = bytes[4];
			ptr[5] = bytes[5];
			ptr[6] = bytes[6];
			ptr[7] = bytes[7];
			return result;
		}
	}
}
