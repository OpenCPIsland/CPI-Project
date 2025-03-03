using System.Reflection;

namespace DeviceDB
{
	internal class SerializedFieldReflection
	{
		public FieldInfo FieldInfo
		{
			get;
			private set;
		}

		public byte Id
		{
			get;
			private set;
		}

		public SerializedFieldReflection(FieldInfo fieldInfo, byte id)
		{
			FieldInfo = fieldInfo;
			Id = id;
		}
	}
}
