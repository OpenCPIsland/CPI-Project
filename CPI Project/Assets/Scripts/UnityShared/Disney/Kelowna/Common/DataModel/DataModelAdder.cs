namespace Disney.Kelowna.Common.DataModel
{
	public class DataModelAdder<T> : AbstractDataModelAdder where T : BaseData
	{
		private readonly string entityName;

		public DataModelAdder(string entityName)
		{
			this.entityName = entityName;
		}

		public override void AddComponent(DataEntityCollection dataEntityCollection)
		{
			DataEntityHandle handle = dataEntityCollection.AddEntity(entityName);
			dataEntityCollection.AddComponent<T>(handle);
		}
	}
}
