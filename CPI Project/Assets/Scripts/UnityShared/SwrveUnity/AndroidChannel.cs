namespace SwrveUnity
{
	public class AndroidChannel
	{
		public enum ImportanceLevel
		{
			Default,
			High,
			Low,
			Max,
			Min,
			None
		}

		public readonly string Id;

		public readonly string Name;

		public readonly ImportanceLevel Importance;

		public AndroidChannel(string id, string name, ImportanceLevel importance)
		{
			Id = id;
			Name = name;
			Importance = importance;
		}
	}
}
