namespace ClubPenguin.Tests
{
	public struct FieldTestData
	{
		public string InputValue;

		public bool ShouldTriggerError;

		public string Message;

		public FieldTestData(string value, bool shouldTriggerError, string message)
		{
			InputValue = value;
			ShouldTriggerError = shouldTriggerError;
			Message = message;
		}
	}
}
