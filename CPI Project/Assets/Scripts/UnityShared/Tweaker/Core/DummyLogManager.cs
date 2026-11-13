namespace Tweaker.Core
{
	internal class DummyLogManager : ITweakerLogManager
	{
		public ITweakerLogger GetLogger(string name)
		{
			return new DummyLogger();
		}
	}
}
