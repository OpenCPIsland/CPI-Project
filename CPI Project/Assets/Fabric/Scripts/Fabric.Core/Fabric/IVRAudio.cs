namespace Fabric
{
	public interface IVRAudio
	{
		void Initialise();

		void Shutdown();

		void Set(Component component);

		void Unset();

		void Update();
	}
}
