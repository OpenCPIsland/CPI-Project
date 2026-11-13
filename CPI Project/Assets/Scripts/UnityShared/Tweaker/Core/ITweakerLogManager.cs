namespace Tweaker.Core
{
	public interface ITweakerLogManager
	{
		ITweakerLogger GetLogger(string name);
	}
}
