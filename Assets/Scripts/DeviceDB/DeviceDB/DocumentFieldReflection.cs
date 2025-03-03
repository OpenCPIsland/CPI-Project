using System.Reflection;

namespace DeviceDB
{
	internal class DocumentFieldReflection
	{
		public FieldInfo FieldInfo
		{
			get;
			private set;
		}

		public DocumentFieldReflection(FieldInfo fieldInfo)
		{
			FieldInfo = fieldInfo;
		}
	}
}
