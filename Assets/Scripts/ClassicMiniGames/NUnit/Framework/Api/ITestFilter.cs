namespace NUnit.Framework.Api
{
	public interface ITestFilter
	{
		bool Pass(ITest test);
	}
}
