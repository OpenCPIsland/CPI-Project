using ClubPenguin.Core.StaticGameData;

namespace ClubPenguin.Core
{
	public interface IGameData
	{
		T Get<T>();

		TDefinition GetDefinitionById<TDefinition, TId>(TypedStaticGameDataKey<TDefinition, TId> staticGameDataKey) where TDefinition : StaticGameDataDefinition;

		bool IsAvailable<T>();
	}
}
