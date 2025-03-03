namespace UnityTest
{
	public enum TestResultState : byte
	{
		Inconclusive,
		NotRunnable,
		Skipped,
		Ignored,
		Success,
		Failure,
		Error,
		Cancelled
	}
}
