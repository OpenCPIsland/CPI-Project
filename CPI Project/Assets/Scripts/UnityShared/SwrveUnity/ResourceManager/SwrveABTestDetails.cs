namespace SwrveUnity.ResourceManager
{
	public class SwrveABTestDetails
	{
		public readonly string Id;

		public readonly string Name;

		public readonly int CaseIndex;

		public SwrveABTestDetails(string id, string name, int caseIndex)
		{
			Id = id;
			Name = name;
			CaseIndex = caseIndex;
		}
	}
}
