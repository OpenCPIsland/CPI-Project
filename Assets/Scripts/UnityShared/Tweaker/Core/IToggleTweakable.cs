namespace Tweaker.Core
{
	public interface IToggleTweakable : IStepTweakable
	{
		int CurrentIndex
		{
			get;
		}

		int ToggleCount
		{
			get;
		}

		int GetIndexOfValue(object value);

		string GetNameByIndex(int index);

		string GetNameByValue(object value);

		object SetValueByName(string valueName);

		string GetValueName();
	}
}
