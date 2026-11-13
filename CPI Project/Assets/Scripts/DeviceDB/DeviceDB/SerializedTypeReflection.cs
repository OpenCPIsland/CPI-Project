using System;

namespace DeviceDB
{
	internal class SerializedTypeReflection
	{
		public Type Type
		{
			get;
			private set;
		}

		public byte Id
		{
			get;
			private set;
		}

		public SerializedFieldReflection[] FieldReflections
		{
			get;
			private set;
		}

		public SerializedTypeReflection(Type type, byte id, SerializedFieldReflection[] fieldReflections)
		{
			Type = type;
			Id = id;
			FieldReflections = fieldReflections;
		}
	}
}
