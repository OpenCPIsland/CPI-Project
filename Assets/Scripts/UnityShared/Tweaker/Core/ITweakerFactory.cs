namespace Tweaker.Core
{
	public interface ITweakerFactory
	{
		T Create<T>(params object[] constructorArgs);
	}
}
