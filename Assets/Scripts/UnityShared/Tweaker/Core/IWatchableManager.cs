using System.Reflection;

namespace Tweaker.Core
{
	public interface IWatchableManager
	{
		IWatchable RegisterWatchable(WatchableInfo info, PropertyInfo watchable);

		void RegisterTweakable(IWatchable watchable);

		void UnregisterWatchable(IWatchable watchable);

		void UnregisterWatchable(string name);

		TweakerDictionary<IWatchable> GetWatchables(SearchOptions options);

		IWatchable GetWatchable(SearchOptions options);

		IWatchable GetWatchable(string name);
	}
}
