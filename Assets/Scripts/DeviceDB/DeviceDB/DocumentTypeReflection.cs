namespace DeviceDB
{
	internal class DocumentTypeReflection
	{
		public DocumentFieldReflection[] FieldReflections
		{
			get;
			private set;
		}

		public DocumentTypeReflection(DocumentFieldReflection[] fieldReflections)
		{
			FieldReflections = fieldReflections;
		}
	}
}
