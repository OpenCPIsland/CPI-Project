using System.Reflection;

namespace Tweaker.Core
{
	public interface ITweakableManager
	{
		ITweakable RegisterTweakable<T>(TweakableInfo<T> info, PropertyInfo propertyInfo, object instance = null);

		ITweakable RegisterTweakable<T>(TweakableInfo<T> info, FieldInfo fieldInfo, object instance = null);

		void RegisterTweakable(ITweakable tweakable);

		void UnregisterTweakable(ITweakable tweakable);

		void UnregisterTweakable(string name);

		TweakerDictionary<ITweakable> GetTweakables(SearchOptions options = null);

		ITweakable GetTweakable(SearchOptions options = null);

		ITweakable GetTweakable(string name);

		void SetTweakableValue<T>(ITweakable tweakable, T value);

		void SetTweakableValue<T>(string name, T value);

		T GetTweakableValue<T>(ITweakable tweakable);

		T GetTweakableValue<T>(string name);
	}
}
